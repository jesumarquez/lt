using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.BaseClasses.BasePages;
using C1.Web.UI.Controls.C1GridView;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReporteDistribucion : SecuredGridReportPage<ReporteDistribucionVo>
    {
        protected override string VariableName { get { return "REP_DISTRIBUCION"; } }
        protected override string GetRefference() { return "REP_DISTRIBUCION"; }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
                ddlEstados.SelectedIndex = -1;
                LoadQueryString();
            }
        }

        private void LoadQueryString()
        {
            if (Request.QueryString["Movil"] != null)
            {
                int movil;
                if (int.TryParse(Request.QueryString["Movil"], out movil) && movil > 0)
                    ddlVehiculo.SelectedValue = movil.ToString("#0");
            }
            if (Request.QueryString["Fecha"] != null && Request.QueryString["Fecha"].Length == 8)
            {
                int anio, mes, dia;
                var fecha = Request.QueryString["fecha"];
                if (int.TryParse(fecha.Substring(0, 4), out anio) &&
                    int.TryParse(fecha.Substring(4, 2), out mes) &&
                    int.TryParse(fecha.Substring(6, 2), out dia))
                {
                    var desde = new DateTime(anio, mes, dia);
                    var hasta = desde.AddHours(23).AddMinutes(59);
                    dpDesde.SelectedDate = desde;
                    dpHasta.SelectedDate = hasta;
                }
            }
        }

        protected override List<ReporteDistribucionVo> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var results = new List<ReporteDistribucionVo>();

                var viajes = DAOFactory.ViajeDistribucionDAO.GetList(ddlLocacion.SelectedValues,
                                                                     ddlPlanta.SelectedValues,
                                                                     ddlTransportista.SelectedValues,
                                                                     new[] { -1 }, // DEPARTAMENTOS
                                                                     new[] { -1 }, // CENTROS DE COSTO
                                                                     new[] { -1 }, // SUB CENTROS DE COSTO
                                                                     ddlVehiculo.SelectedValues,
                                                                     new[] { -1 }, // EMPLEADOS
                                                                     new[] { -1 }, // ESTADOS
                                                                     dpDesde.SelectedDate.Value.ToDataBaseDateTime(),
                                                                     dpHasta.SelectedDate.Value.ToDataBaseDateTime())
                                                            .Where(e => e.Id == ddlRuta.Selected || ddlRuta.Selected == 0);
                
                foreach (var viaje in viajes)
                {
                    EntregaDistribucion anterior = null;

                    var estados = ddlEstados.SelectedValues;
                    if (ddlEstados.SelectedStringValues.Count == 0)
                    {
                        estados.Clear();
                        estados.Add(EntregaDistribucion.Estados.Cancelado);
                        estados.Add(EntregaDistribucion.Estados.Completado);
                        estados.Add(EntregaDistribucion.Estados.NoCompletado);
                        estados.Add(EntregaDistribucion.Estados.EnSitio); 
                        estados.Add(EntregaDistribucion.Estados.Pendiente);
                        estados.Add(EntregaDistribucion.Estados.Visitado); 
                        estados.Add(EntregaDistribucion.Estados.SinVisitar);
                    }

                    var detalles = viaje.Detalles;

                    if (chkVerOrdenManual.Checked)
                        detalles = viaje.GetEntregasPorOrdenManual();
                    else if (viaje.Tipo == ViajeDistribucion.Tipos.Desordenado)
                        detalles = viaje.GetEntregasPorOrdenReal();
                    
                    detalles = detalles.Where(e => ddlPuntoEntrega.Selected == 0 ||
                                                   (e.PuntoEntrega != null && e.PuntoEntrega.Id == ddlPuntoEntrega.Selected))
                                       .Where(e => estados.Contains(e.Estado))
                                       .ToList();
                    
                    var orden = 0;
                    foreach (var entrega in detalles)
                    {
                        var kms = 0.0;


                        //CAMBIAR LAS CONDICIONES DE ESTE IF .. MATIAS

                        if (anterior != null && !entrega.Estado.Equals(EntregaDistribucion.Estados.Cancelado)
                            && !entrega.Estado.Equals(EntregaDistribucion.Estados.NoCompletado)
                            && !entrega.Estado.Equals(EntregaDistribucion.Estados.SinVisitar)
                         && entrega.Viaje.Vehiculo != null && anterior.FechaMin < entrega.FechaMin && entrega.FechaMin < DateTime.MaxValue)
                                kms = DAOFactory.CocheDAO.GetDistance(entrega.Viaje.Vehiculo.Id, anterior.FechaMin, entrega.FechaMin);
                        
                        results.Add(new ReporteDistribucionVo(entrega, anterior, orden, kms, chkVerConfirmacion.Checked));
                        orden++;

                        if (!entrega.Estado.Equals(EntregaDistribucion.Estados.Cancelado)
                         && !entrega.Estado.Equals(EntregaDistribucion.Estados.NoCompletado)
                         && !entrega.Estado.Equals(EntregaDistribucion.Estados.SinVisitar))
                            anterior = entrega;
                    }
                }

                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Estado de Entregas", String.Format("Duración de la consulta: {0} segundos", duracion));
				return results;
            }
            catch (Exception e)
            {
                STrace.Exception("Estado de Entregas", e);
                throw;
            }
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, ReporteDistribucionVo dataItem)
        {
            base.OnRowDataBound(grid, e, dataItem);
            e.Row.Attributes.Add("id", dataItem.Id.ToString("#0"));
            
            var clickScript = ClientScript.GetPostBackEventReference(Grid, string.Format("Select:{0}", e.Row.RowIndex));
            e.Row.Attributes.Remove("onclick");

            var checkIndex = GridUtils.GetColumnIndex(ReporteDistribucionVo.IndexTieneFoto);
            for (var i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i != checkIndex)
                {
                    GridUtils.GetCell(e.Row, i).Attributes.Add("onclick", clickScript);
                }
                else if (dataItem.TieneFoto)
                {
                    GridUtils.GetCell(e.Row, ReporteDistribucionVo.IndexTieneFoto).Text = @"<div class=""withPhoto""></div>";

                    const string link = "window.open('../../Common/Pictures/Default.aspx?d={0}&f={1}&t={2}', 'Fotos_{0}', 'width=345,height=408,directories=no,location=no,menubar=no,resizable=no,scrollbars=no,status=no,toolbar=no');";
                    GridUtils.GetCell(e.Row, i).Attributes.Add("onclick", string.Format(link, dataItem.IdDispositivo, dataItem.Desde.ToString("dd/MM/yyyy HH:mm:ss"), dataItem.Hasta.ToString("dd/MM/yyyy HH:mm:ss")));
                }
            }

            grid.Columns[ReporteDistribucionVo.IndexConfirmacion].Visible = chkVerConfirmacion.Checked;
            grid.Columns[ReporteDistribucionVo.IndexHorario].Visible = chkVerConfirmacion.Checked;

            grid.Columns[ReporteDistribucionVo.IndexReadInactive].Visible = chkInteraccionGarmin.Checked;
            grid.Columns[ReporteDistribucionVo.IndexUnreadInactive].Visible = chkInteraccionGarmin.Checked;

            grid.Columns[ReporteDistribucionVo.IndexDescripcion].Visible = chkVerDescripcion.Checked;
        }

        protected override void SelectedIndexChanged()
        {
            OpenWin(String.Concat(ApplicationPath, "Ciclologistico/Distribucion/ViajeDistribucionAlta.aspx?id=" + Grid.SelectedRow.Attributes["id"]), "Distribución");
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           { CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text },
                           { CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text },
                           { CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm") }, 
                           { CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.Value.ToString("dd/MM/yyyy HH:mm") }
                       };
        }
    }
}