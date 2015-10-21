using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.CicloLogistico.Distribucion;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class FlujoDeTareas : SecuredGridReportPage<FlujoDeTareasVo>
    {
        protected override string VariableName { get { return "REP_FLUJO_TAREAS"; } }
        protected override string GetRefference() { return "REP_FLUJO_TAREAS"; }
        public override OutlineMode GridOutlineMode { get { return OutlineMode.StartCollapsed; } }
        public override bool HasTotalRow { get { return true; } }
        protected override bool ExcelButton { get { return true; } }
        public override int PageSize { get { return 500; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }
        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        private TimeSpan _totalTraslados, _totalParadas, _totalTareas;
        private int _cantTraslados, _cantParadas, _cantTareas, _cantViajes;
        private double _totalKms;
        private int _totalCompletados, _totalNoCompletados, _totalEnSitio,
                    _totalVisitados, _totalNoVisitados, _totalEnZona;

        private readonly TimeSpan _minimoDetencion = new TimeSpan(0,2,0);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                dtFecha.SetDate();
        }

        protected override List<FlujoDeTareasVo> GetResults()
        {
            var ini = DateTime.UtcNow;
            try
            {
                var results = new List<FlujoDeTareasVo>();
                var desde = dtFecha.SelectedDate.Value;
                var hasta = desde.AddDays(1);
                
                _totalCompletados = _totalNoCompletados = 
                _totalVisitados = _totalNoVisitados = 
                _totalEnSitio = _totalEnZona = 0;

                var viajes = DAOFactory.ViajeDistribucionDAO.GetList(new[] { ddlLocacion.Selected },
                                                                     new[] { ddlPlanta.Selected },
                                                                     lbTransportista.SelectedValues,
                                                                     lbDepartamento.SelectedValues,
                                                                     lbCentroDeCosto.SelectedValues,
                                                                     lbSubCentroDeCosto.SelectedValues,
                                                                     lbVehiculo.SelectedValues,
                                                                     SecurityExtensions.ToDataBaseDateTime(desde),
                                                                     SecurityExtensions.ToDataBaseDateTime(hasta))
                                                            .Where(v => v.InicioReal.HasValue);
                var vehicles = viajes.Select(v => v.Vehiculo.Id).Distinct().ToList();
                var codes = new List<string> 
                            {
                                "6000", "6001", "6002", "6003", "6050", "6051", 
                                "6052", "6053", "6054", "6055", "6056", "6057", 
                                "6058", "6059", "6060", "6061", "6062", "6063"
                            };
                var eventos = DAOFactory.LogMensajeDAO.GetByMobilesAndTypes(vehicles, codes, SecurityExtensions.ToDataBaseDateTime(desde), SecurityExtensions.ToDataBaseDateTime(hasta), 3);

                foreach (var viajeDistribucion in viajes)
                {
                    _cantViajes++;
                    var dets = viajeDistribucion.Detalles.Where(d => d.Linea == null);
                    _totalCompletados += dets.Count(e => e.Estado == EntregaDistribucion.Estados.Completado);
                    _totalNoCompletados += dets.Count(e => e.Estado == EntregaDistribucion.Estados.NoCompletado);
                    _totalVisitados += dets.Count(e => e.Estado == EntregaDistribucion.Estados.Visitado);
                    _totalNoVisitados += dets.Count(e => e.Estado == EntregaDistribucion.Estados.SinVisitar
                                                    || e.Estado== EntregaDistribucion.Estados.Pendiente);
                    _totalEnSitio += dets.Count(e => e.Estado == EntregaDistribucion.Estados.EnSitio);
                    _totalEnZona += dets.Count(e => e.Estado == EntregaDistribucion.Estados.EnZona);

                    var empleado = viajeDistribucion.Empleado != null && viajeDistribucion.Empleado.Entidad != null
                                       ? " (Empleado: " + viajeDistribucion.Empleado.Entidad.Descripcion + ")"
                                       : string.Empty;
                    var flujos = new List<FlujoDeTareasVo>();

                    var comienzo = viajeDistribucion.Detalles[0].Salida.HasValue
                                       ? viajeDistribucion.Detalles[0].Salida.Value
                                       : viajeDistribucion.InicioReal.Value;

                    var inicio = new FlujoDeTareasVo
                                     {
                                         Vehiculo = viajeDistribucion.Vehiculo.Interno,
                                         Distribucion = viajeDistribucion.Codigo + empleado,
                                         Flujo = "Inicio",
                                         Inicio = viajeDistribucion.InicioReal.Value.ToDisplayDateTime().AddMilliseconds(-1),
                                         Fin = comienzo.ToDisplayDateTime(),
                                         Inactividad = comienzo.Subtract(viajeDistribucion.InicioReal.Value),
                                         Km = 0.0
                                     };
                    if (viajeDistribucion.Vehiculo.Empresa.ControlaInicioDistribucion && inicio.Inactividad.TotalMinutes > viajeDistribucion.Umbral)
                        inicio.Flujo = "Inicio Retrasado";

                    _totalParadas = _totalParadas.Add(inicio.Inactividad);
                    if (inicio.Inactividad.TotalMinutes > 0) _cantParadas++;

                    flujos.Add(inicio);
                    
                    var entregas = viajeDistribucion.Detalles.Where(ent => ent.Entrada.HasValue
                                                                        && ent.Salida.HasValue)
                                                             .OrderBy(ent => ent.Entrada);
                    
                    foreach (var entrega in entregas)
                    {
                        if (!flujos.Last().Fin.HasValue || flujos.Last().Fin.Value.ToDataBaseDateTime() < entrega.Entrada.Value)
                        {
                            var final = entrega.Entrada.Value;

                            // Busco las detenciones anteriores a la entrega
                            var maxMonths = viajeDistribucion.Vehiculo.Empresa != null ? viajeDistribucion.Vehiculo.Empresa.MesesConsultaPosiciones : 3;
                            var detenciones = DAOFactory.LogMensajeDAO.GetEventos(new[] {viajeDistribucion.Vehiculo.Id},
                                                                                  new[] {MessageCode.StoppedEvent.GetMessageCode()},
                                                                                  comienzo,
                                                                                  final,
                                                                                  maxMonths)
                                                        .Where(det => det.FechaFin.HasValue &&
                                                                      det.FechaFin.Value < final &&
                                                                      det.FechaFin.Value.Subtract(det.Fecha) > _minimoDetencion)
                                                        .OrderBy(ev => ev.Fecha);

                            foreach (var detencion in detenciones)
                            {
                                // Genero el traslado anterior a la detención
                                var traslado = new FlujoDeTareasVo
                                                   {
                                                       Vehiculo = viajeDistribucion.Vehiculo.Interno,
                                                       Distribucion = viajeDistribucion.Codigo + empleado,
                                                       Flujo = "Traslado",
                                                       Inicio = comienzo.ToDisplayDateTime(),
                                                       Fin = detencion.Fecha.ToDisplayDateTime(),
                                                       Traslados = detencion.Fecha.Subtract(comienzo),
                                                       Km = DAOFactory.CocheDAO.GetDistance(viajeDistribucion.Vehiculo.Id, comienzo, detencion.Fecha)
                                                   };
                                _totalTraslados = _totalTraslados.Add(traslado.Traslados);
                                _cantTraslados++;
                                
                                if (traslado.Traslados.TotalSeconds > 0)
                                {
                                    if (chkVerEventos.Checked)
                                    {
                                        var evs = eventos.Where(e => e.Coche.Interno == traslado.Vehiculo
                                                                     && e.Fecha.ToDisplayDateTime() > traslado.Inicio
                                                                     && e.Fecha.ToDisplayDateTime() < traslado.Fin);
                                        foreach (var ev in evs) traslado.Flujo += " - " + ev.Texto;
                                    }

                                    flujos.Add(traslado);
                                    if (traslado.Km > 0) _totalKms += traslado.Km;
                                }

                                var det = new FlujoDeTareasVo
                                              {
                                                  Vehiculo = viajeDistribucion.Vehiculo.Interno,
                                                  Distribucion = viajeDistribucion.Codigo + empleado,
                                                  Flujo = "Detención",
                                                  Inicio = detencion.Fecha.ToDisplayDateTime(),
                                                  Fin = detencion.FechaFin.Value.ToDisplayDateTime(),
                                                  Inactividad = detencion.FechaFin.Value.Subtract(detencion.Fecha),
                                                  Km = 0.0
                                              };

                                _totalParadas = _totalParadas.Add(det.Inactividad);
                                _cantParadas++;

                                if (chkVerEventos.Checked)
                                {
                                    var eevs = eventos.Where(e => e.Coche.Interno == det.Vehiculo
                                                                  && e.Fecha.ToDisplayDateTime() > det.Inicio
                                                                  && e.Fecha.ToDisplayDateTime() < det.Fin);
                                    foreach (var ev in eevs) det.Flujo += " - " + ev.Texto;
                                }
                                flujos.Add(det);
                                comienzo = detencion.FechaFin.Value;
                            }

                            var tras = new FlujoDeTareasVo
                                           {
                                               Vehiculo = viajeDistribucion.Vehiculo.Interno,
                                               Distribucion = viajeDistribucion.Codigo + empleado,
                                               Flujo = "Traslado",
                                               Inicio = comienzo.ToDisplayDateTime(),
                                               Fin = entrega.Entrada.Value.ToDisplayDateTime(),
                                               Traslados = entrega.Entrada.Value.Subtract(comienzo),
                                               Km = DAOFactory.CocheDAO.GetDistance(viajeDistribucion.Vehiculo.Id, comienzo, entrega.Entrada.Value)
                                           };
                            _totalTraslados = _totalTraslados.Add(tras.Traslados);
                            _cantTraslados++;
                            
                            if (tras.Traslados.TotalSeconds > 0)
                            {
                                if (chkVerEventos.Checked)
                                {
                                    var evss = eventos.Where(e => e.Coche.Interno == tras.Vehiculo
                                                                  && e.Fecha.ToDisplayDateTime() > tras.Inicio
                                                                  && e.Fecha.ToDisplayDateTime() < tras.Fin);
                                    foreach (var ev in evss) tras.Flujo += " - " + ev.Texto;
                                }
                                flujos.Add(tras);
                                if (tras.Km > 0) _totalKms += tras.Km;
                            }

                            var despacho = new FlujoDeTareasVo
                                               {
                                                   Vehiculo = entrega.Viaje.Vehiculo.Interno,
                                                   Distribucion = entrega.Viaje.Codigo + empleado,
                                                   Flujo = entrega.Descripcion + " (" + CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado)) + ")",
                                                   Inicio = entrega.Entrada.Value.ToDisplayDateTime(),
                                                   Fin = entrega.Salida.Value.ToDisplayDateTime(),
                                                   Tarea = entrega.Salida.Value.Subtract(entrega.Entrada.Value),
                                                   Km = 0.0
                                               };

                            if (chkVerEventos.Checked)
                            {
                                var evvs = eventos.Where(e => e.Coche.Interno == despacho.Vehiculo
                                                              && e.Fecha.ToDisplayDateTime() > despacho.Inicio
                                                              && e.Fecha.ToDisplayDateTime() < despacho.Fin);
                                foreach (var ev in evvs) despacho.Flujo += " - " + ev.Texto;
                            }
                            flujos.Add(despacho);
                            _totalTareas = _totalTareas.Add(despacho.Tarea);
                            _cantTareas++;

                            comienzo = entrega.Salida.Value;
                        }
                        else if (flujos.Last().Fin.HasValue && flujos.Last().Fin.Value.ToDataBaseDateTime() >= entrega.Entrada.Value)
                        {
                            flujos.Last().Flujo += " - " + entrega.Descripcion + " (" + CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entrega.Estado)) + ") ";
                            if (flujos.Last().Fin.Value.ToDataBaseDateTime() < entrega.Salida.Value)
                            {
                                flujos.Last().Fin = entrega.Salida.Value.ToDisplayDateTime();
                                var extraTarea = entrega.Salida.Value.Subtract(flujos.Last().Fin.Value.ToDataBaseDateTime());
                                flujos.Last().Tarea = flujos.Last().Tarea.Add(extraTarea);
                                _totalTareas = _totalTareas.Add(extraTarea);
                            }
                        }
                    }

                    var ultimoFlujo = flujos.Last();
                    if (viajeDistribucion.Estado == ViajeDistribucion.Estados.Cerrado)
                    {
                        if (ultimoFlujo.Fin.HasValue && ultimoFlujo.Fin.Value < viajeDistribucion.Fin)
                        {
                            var ultimoTraslado = new FlujoDeTareasVo
                                                     {
                                                         Vehiculo = ultimoFlujo.Vehiculo,
                                                         Distribucion = ultimoFlujo.Distribucion,
                                                         Flujo = "Traslado",
                                                         Inicio = ultimoFlujo.Fin,
                                                         Fin = viajeDistribucion.Fin.ToDisplayDateTime(),
                                                         Traslados = viajeDistribucion.Fin.ToDisplayDateTime().Subtract(ultimoFlujo.Fin.Value),
                                                         Km = DAOFactory.CocheDAO.GetDistance(viajeDistribucion.Vehiculo.Id, ultimoFlujo.Fin.Value.ToDataBaseDateTime(), viajeDistribucion.Fin)
                                                     };

                            if (ultimoTraslado.Traslados.TotalSeconds > 0)
                            {
                                if (chkVerEventos.Checked)
                                {
                                    var evs = eventos.Where(e => e.Coche.Interno == ultimoTraslado.Vehiculo
                                                                 && e.Fecha.ToDisplayDateTime() > ultimoTraslado.Inicio
                                                                 && e.Fecha.ToDisplayDateTime() < ultimoTraslado.Fin);
                                    foreach (var ev in evs) ultimoTraslado.Flujo += " - " + ev.Texto;
                                }
                                flujos.Add(ultimoTraslado);
                                _totalTraslados = _totalTraslados.Add(ultimoTraslado.Traslados);
                                _cantTraslados++;
                                if (ultimoTraslado.Km > 0) _totalKms += ultimoTraslado.Km;
                            }
                        }

                        var fin = new FlujoDeTareasVo
                                      {
                                          Vehiculo = ultimoFlujo.Vehiculo,
                                          Distribucion = ultimoFlujo.Distribucion,
                                          Flujo = "Fin",
                                          Inicio = viajeDistribucion.Fin.AddSeconds(1).ToDisplayDateTime(),
                                          Km = 0.0
                                      };
                        flujos.Add(fin);
                    }
                    else if (viajeDistribucion.Estado == ViajeDistribucion.Estados.EnCurso)
                    {
                        if (ultimoFlujo.Fin.HasValue && ultimoFlujo.Fin.Value < DateTime.UtcNow.ToDisplayDateTime())
                        {
                            var now = DateTime.UtcNow;
                            var dtNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                            var t = new FlujoDeTareasVo
                            {
                                Vehiculo = ultimoFlujo.Vehiculo,
                                Distribucion = ultimoFlujo.Distribucion,
                                Flujo = "Traslado",
                                Inicio = ultimoFlujo.Fin,
                                Fin = dtNow.ToDisplayDateTime(),
                                Traslados = dtNow.ToDisplayDateTime().Subtract(ultimoFlujo.Fin.Value),
                                Km = DAOFactory.CocheDAO.GetDistance(viajeDistribucion.Vehiculo.Id, ultimoFlujo.Fin.Value.ToDataBaseDateTime(), dtNow)
                            };

                            if (t.Traslados.TotalSeconds > 0)
                            {
                                if (chkVerEventos.Checked)
                                {
                                    var evs = eventos.Where(e => e.Coche.Interno == t.Vehiculo
                                                                 && e.Fecha.ToDisplayDateTime() > t.Inicio
                                                                 && e.Fecha.ToDisplayDateTime() < t.Fin);
                                    foreach (var ev in evs) t.Flujo += " - " + ev.Texto;
                                }
                                flujos.Add(t);
                                _totalTraslados = _totalTraslados.Add(t.Traslados);
                                _cantTraslados++;
                                if (t.Km > 0) _totalKms += t.Km;
                            }
                        }
                    }

                    results.AddRange(flujos);
                }

                ShowTotales();

                var duracion = (DateTime.UtcNow - ini).TotalSeconds.ToString("##0.00");

                STrace.Trace("Flujo de Tareas", String.Format("Duración de la consulta: {0} segundos", duracion));
				return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Flujo de Tareas", e, String.Format("Reporte: Flujo de Tareas. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - ini).TotalSeconds));
                throw;
            }
        }

        private void ShowTotales()
        {
            tbl_totales.Visible = true;
            tbl2.Visible = true;

            var total = _totalTareas.Add(_totalParadas).Add(_totalTraslados);
            var porcDespachos = 0.0;
            var porcParadas = 0.0;
            var porcTraslados = 0.0;
            var promDespachos = new TimeSpan();
            var promParadas = new TimeSpan();
            var promTraslados = new TimeSpan();
            var promKms = 0.0;
            var promTiempo = new TimeSpan();

            lblTotalKm.Text = _totalKms.ToString("#0.00");
            lblTotalTiempo.Text = total.ToString();
            lblTotalDespachos.Text = _totalTareas.ToString();
            lblTotalParadas.Text = _totalParadas.ToString();
            lblTotalTraslados.Text = _totalTraslados.ToString();

            if (total.TotalMinutes > 0)
            {
                porcDespachos = _totalTareas.TotalMinutes/total.TotalMinutes;
                porcParadas = _totalParadas.TotalMinutes/total.TotalMinutes;
                porcTraslados = _totalTraslados.TotalMinutes/total.TotalMinutes;
            }
            lblPorcDespachos.Text = porcDespachos.ToString("#0%");
            lblPorcParadas.Text = porcParadas.ToString("#0%");
            lblPorcTraslados.Text = porcTraslados.ToString("#0%");

            lblCantDespachos.Text = _cantTareas.ToString("#0");
            lblCantParadas.Text = _cantParadas.ToString("#0");
            lblCantTraslados.Text = _cantTraslados.ToString("#0");
            lblCantKm.Text = _cantViajes.ToString("#0");
            lblCantTiempo.Text = _cantViajes.ToString("#0");

            if (_cantTareas > 0) promDespachos = new TimeSpan(_totalTareas.Ticks/_cantTareas);
            if (_cantParadas > 0) promParadas = new TimeSpan(_totalParadas.Ticks/_cantParadas);
            if (_cantTraslados > 0) promTraslados = new TimeSpan(_totalTraslados.Ticks/_cantTraslados);
            if (_cantViajes > 0)
            {
                promKms = _totalKms / _cantViajes;
                promTiempo = new TimeSpan(total.Ticks/_cantViajes);
            }

            promDespachos = new TimeSpan(0, 0, (int)promDespachos.TotalSeconds);
            promParadas = new TimeSpan(0, 0, (int)promParadas.TotalSeconds);
            promTraslados = new TimeSpan(0, 0, (int)promTraslados.TotalSeconds);
            promTiempo = new TimeSpan(0, 0, (int)promTiempo.TotalSeconds);

            lblPromDespachos.Text = promDespachos.ToString(); 
            lblPromParadas.Text = promParadas.ToString();
            lblPromTraslados.Text = promTraslados.ToString();
            lblPromKm.Text = promKms.ToString("#0.00");
            lblPromTiempo.Text = promTiempo.ToString();

            var tot = _totalCompletados + _totalNoCompletados + _totalVisitados + _totalNoVisitados + _totalEnSitio + _totalEnZona;
            var totalRealizados = _totalCompletados + _totalVisitados + _totalEnSitio + _totalEnZona;
            var porcCompletados = (tot != 0 ? (double)_totalCompletados / (double)tot * 100 : 0);
            var porcNoCompletados = (tot != 0 ? (double)_totalNoCompletados / (double)tot * 100 : 0);
            var porcVisitados = (tot != 0 ? (double)_totalVisitados / (double)tot * 100 : 0);
            var porcNoVisitados = (tot != 0 ? (double)_totalNoVisitados / (double)tot * 100 : 0);
            var porcEnSitio = (tot != 0 ? (double)_totalEnSitio / (double)tot * 100 : 0);
            var porcEnZona = (tot != 0 ? (double)_totalEnZona / (double)tot * 100 : 0);
            var porcRealizados = (tot != 0 ? (double)totalRealizados / (double)tot * 100 : 0);

            lblTotal.Text = tot.ToString("#0");
            lblCantRealizadas.Text = totalRealizados.ToString("#0");
            lblPorcRealizadas.Text = porcRealizados.ToString("#0.00");
            lblCantCompletadas.Text = _totalCompletados.ToString("#0");
            lblPorcCompletadas.Text = porcCompletados.ToString("#0.00");
            lblCantNoCompletadas.Text = _totalNoCompletados.ToString("#0");
            lblPorcNoCompletadas.Text = porcNoCompletados.ToString("#0.00");
            lblCantVisitadas.Text = _totalVisitados.ToString("#0");
            lblPorcVisitadas.Text = porcVisitados.ToString("#0.00");
            lblCantNoVisitadas.Text = _totalNoVisitados.ToString("#0");
            lblPorcNoVisitadas.Text = porcNoVisitados.ToString("#0.00");
            lblCantEnSitio.Text = _totalEnSitio.ToString("#0");
            lblPorcEnSitio.Text = porcEnSitio.ToString("#0.00");
            lblCantEnZona.Text = _totalEnZona.ToString("#0");
            lblPorcEnZona.Text = porcEnZona.ToString("#0.00");
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, FlujoDeTareasVo dataItem)
        {
            e.Row.Attributes.Remove("onclick");
            e.Row.Style.Add("cursor","default");
            e.Row.Cells[FlujoDeTareasVo.IndexTotal].Text = string.Empty;

            switch (dataItem.Flujo)
            {
                case "Inicio":
                case "Fin":
                    e.Row.BackColor = System.Drawing.Color.LightBlue; break;
                case "Inicio Retrasado":
                    e.Row.BackColor = System.Drawing.Color.Red; break;
                default:
                    if (dataItem.Flujo.Contains("Detención")) e.Row.BackColor = System.Drawing.Color.OrangeRed;
                    else if (dataItem.Flujo.Contains("Traslado")) e.Row.BackColor = System.Drawing.Color.Yellow;
                    else e.Row.BackColor = System.Drawing.Color.LightGreen;
                    break;
            }
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            var porcTareas = 0.0;
            var porcParadas = 0.0;
            var porcTraslados = 0.0;
            var promDespachos = new TimeSpan();
            var promParadas = new TimeSpan();
            var promTraslados = new TimeSpan();
            var promKms = 0.0;
            var promTiempo = new TimeSpan();

            var total = _totalTareas.Add(_totalParadas).Add(_totalTraslados);
            if (total.TotalMinutes > 0)
            {
                porcTareas = _totalTareas.TotalMinutes/total.TotalMinutes;
                porcParadas = _totalParadas.TotalMinutes/total.TotalMinutes;
                porcTraslados = _totalTraslados.TotalMinutes/total.TotalMinutes;
            }

            if (_cantTareas > 0) promDespachos = new TimeSpan(_totalTareas.Ticks/_cantTareas);
            if (_cantParadas > 0) promParadas = new TimeSpan(_totalParadas.Ticks/_cantParadas);
            if (_cantTraslados > 0) promTraslados = new TimeSpan(_totalTraslados.Ticks/_cantTraslados);
            if (_cantViajes > 0)
            {
                promKms = _totalKms / _cantViajes;
                promTiempo = new TimeSpan(total.Ticks/_cantViajes);
            }

            promDespachos = new TimeSpan(0, 0, (int)promDespachos.TotalSeconds);
            promParadas = new TimeSpan(0, 0, (int)promParadas.TotalSeconds);
            promTraslados = new TimeSpan(0, 0, (int)promTraslados.TotalSeconds);
            promTiempo = new TimeSpan(0, 0, (int)promTiempo.TotalSeconds);

            var totalRealizados = _totalCompletados + _totalVisitados + _totalEnSitio + _totalEnZona;
            var totalEntregas = totalRealizados + _totalNoCompletados + _totalNoVisitados;
            var porcRealizados = 0.0;
            var porcCompletados = 0.0;
            var porcNoCompletados = 0.0;
            var porcVisitados = 0.0;
            var porcNoVisitados = 0.0;
            var porcEnSitio = 0.0; 
            var porcEnZona = 0.0;
            if (totalEntregas > 0)
            {
                porcRealizados = (double)totalRealizados / (double)totalEntregas * 100;
                porcCompletados = (double)_totalCompletados / (double)totalEntregas * 100;
                porcNoCompletados = (double)_totalNoCompletados / (double)totalEntregas * 100;
                porcVisitados = (double)_totalVisitados / (double)totalEntregas * 100;
                porcNoVisitados = (double)_totalNoVisitados / (double)totalEntregas * 100;
                porcEnSitio = (double)_totalEnSitio / (double)totalEntregas * 100;
                porcEnZona = (double)_totalEnZona / (double)totalEntregas * 100;
            }

            return new Dictionary<string, string>
                       {
                           {"KM_TOTAL", _totalKms.ToString("#0.00")},
                           {"TIEMPO_TOTAL", total.ToString()},
                           {"TOTAL_TRASLADOS", _totalTraslados.ToString()},
                           {"TOTAL_INACTIVIDAD", _totalParadas.ToString()},
                           {"TOTAL_TAREAS", _totalTareas.ToString()},
                           {"PORC_TRASLADOS", porcTraslados.ToString("#0%")},
                           {"PORC_INACTIVIDAD", porcParadas.ToString("#0%")},
                           {"PORC_TAREAS", porcTareas.ToString("#0%")},
                           {"CANT_KM", _cantViajes.ToString("#0")},
                           {"CANT_TIEMPO", _cantViajes.ToString("#0")},
                           {"CANT_TRASLADOS", _cantTraslados.ToString("#0")},
                           {"CANT_INACTIVIDAD", _cantParadas.ToString("#0")},
                           {"CANT_TAREAS", _cantTareas.ToString("#0")},
                           {"PROM_KM", promKms.ToString("#0.00")},
                           {"PROM_TIEMPO", promTiempo.ToString()},
                           {"PROM_TRASLADOS", promTraslados.ToString()},
                           {"PROM_INACTIVIDAD", promParadas.ToString()},
                           {"PROM_TAREAS", promDespachos.ToString()},
                           {"ENTREGAS", totalEntregas.ToString("#0")},
                           {"REALIZADOS_CANT", totalRealizados.ToString("#0")},
                           {"REALIZADOS_PORC", porcRealizados.ToString("#0.00")},
                           {"COMPLETADOS_CANT", _totalCompletados.ToString("#0")},
                           {"COMPLETADOS_PORC", porcCompletados.ToString("#0.00")},
                           {"NO_COMPLETADOS_CANT", _totalNoCompletados.ToString("#0")},
                           {"NO_COMPLETADOS_PORC", porcNoCompletados.ToString("#0.00")},
                           {"VISITADOS_CANT", _totalVisitados.ToString("#0")},
                           {"VISITADOS_PORC", porcVisitados.ToString("#0.00")},
                           {"NO_VISITADOS_CANT", _totalNoVisitados.ToString("#0")},
                           {"NO_VISITADOS_PORC", porcNoVisitados.ToString("#0.00")},
                           {"EN_SITIO_CANT", _totalEnSitio.ToString("#0")},
                           {"EN_SITIO_PORC", porcEnSitio.ToString("#0.00")},
                           {"EN_ZONA_CANT", _totalEnZona.ToString("#0")},
                           {"EN_ZONA_PORC", porcEnZona.ToString("#0.00")}
                       };
        }
    }
}
