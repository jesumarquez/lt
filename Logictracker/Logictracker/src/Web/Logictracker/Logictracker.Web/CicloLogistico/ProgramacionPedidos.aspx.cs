using System;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Culture;
using Logictracker.DAL.NHibernate;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Helpers;
using Logictracker.Web.CustomWebControls.Input;
using Logictracker.Web.Helpers.FussionChartHelpers;
using System.Collections.Generic;
using InfoSoftGlobal;
using System.Globalization;
using System.Text;

namespace Logictracker.CicloLogistico
{
    public partial class ProgramacionPedidos : SecuredBaseReportPage<Pedido>
    {
        protected override string VariableName { get { return "CLOG_PROGRAMACION"; } }
        protected override string GetRefference() { return "PROGRAMACION"; }

        private const double TiempoReferencia = 120.0;

        private string[] CsvHeader = new[]
                                         {
                                             CultureManager.GetLabel("CODE"),
                                             CultureManager.GetLabel("HORA_CARGA"),
                                             CultureManager.GetLabel("FECHA_EN_OBRA"),
                                             CultureManager.GetEntity("BOCADECARGA"),
                                             CultureManager.GetEntity("CLIENT"),
                                             CultureManager.GetEntity("PARENTI44"),
                                             CultureManager.GetLabel("CONTACTO"),
                                             CultureManager.GetLabel("OBSERVACION"),
                                             CultureManager.GetLabel("NUMERO_BOMBA"),
                                             CultureManager.GetLabel("MULTIPLES_REMITOS"),
                                             CultureManager.GetLabel("PRODUCTO"),
                                             CultureManager.GetLabel("CANTIDAD_PEDIDO"),
                                             CultureManager.GetLabel("PEDIDO_TIEMPO_CICLO"),
                                             CultureManager.GetLabel("PEDIDO_FRECUENCIA_ENTREGA"),
                                             CultureManager.GetLabel("PROGRAMACION_MOVILES_NECESARIOS")
                                         };

    
        #region Temp Variables

        private bool _algunoPendiente;
        private double Carga { get { return (double) (ViewState["Carga"] ?? 8); } set { ViewState["Carga"] = value; } }
        private int CantidadMixers { get { return (int)(ViewState["CantidadMixers"] ?? 8); } set { ViewState["CantidadMixers"] = value; } }
        private double CantidadTotal { get; set; }
        private readonly Dictionary<int, double> _volumenPorHora = new Dictionary<int, double>();
        private readonly List<double> _tiempoCiclos = new List<double>();

        private TimeSpan _inicioActividad = TimeSpan.MaxValue;
        private TimeSpan _finActividad = TimeSpan.MaxValue;

        private TimeSpan GetInicioActividad(BocaDeCarga boca)
        {
            if (_inicioActividad == TimeSpan.MaxValue)
            {
                var inicio = TimeSpan.FromMinutes(boca.HoraInicioActividad);
                _inicioActividad = TimeSpan.FromHours(inicio.Hours);
            }
            return _inicioActividad;
        }
        private TimeSpan GetFinActividad(BocaDeCarga boca)
        {
            if (_finActividad == TimeSpan.MaxValue)
            {
                var inicio = GetInicioActividad(boca);
                _finActividad = inicio.Add(TimeSpan.FromHours(boca.HorasLaborales));
            }
            return _finActividad;
        } 

        private double GetCarga(Pedido pedido)
        {
            return pedido == null || pedido.CargaViaje == 0.00 ? Carga : pedido.CargaViaje;
        }

        #endregion

        #region Events
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                dtDia.SelectedDate = DateTime.UtcNow.ToDisplayDateTime().Date;
                SetCantidadDeMixers();

                litTabProg.Text = CultureManager.GetLabel("PROGRAMACION_VIEW_PROGRAMACION");
                litTabGraph.Text = CultureManager.GetLabel("PROGRAMACION_VIEW_GRAFICO");
            }
        }

        protected void CbLinea_SelectedIndexChanged(object sender, EventArgs e) { SetCantidadDeMixers(); }

        protected void RepPedidos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
                BoundItem(new RepeaterItemPedido(e.Item));

            if(e.Item.ItemType == ListItemType.Footer)
                BoundFooter(new RepeaterItemPedido(e.Item));

            if(e.Item.ItemType == ListItemType.Header)
                BoundHeader(new RepeaterItemPedido(e.Item));
        }

        protected void RepPedidos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var item = new RepeaterItemPedido(e.Item);
            switch (e.CommandName)
            {
                case "CambiarEnObra": CambiarEnObra(item); break;
                case "CambiarHoraCarga": CambiarHoraCarga(item); break;
                case "GenerarTickets": GenerarPedido(item); break;
                case "GenerarTodo": GenerarTodos(); break;
            }
        }

        protected void LblPedido_OnClick(object sender, EventArgs e)
        {
            Session.Add("id", ((LinkButton)sender).Attributes["id"]);
            OpenWin("../CicloLogistico/PedidoAlta.aspx", "_blank");
        }

        #endregion

        #region Csv
        protected override void ExportToCsv()
        {
            var separator = Usuario.CsvSeparator;

            var sb = new StringBuilder();
            sb.Append(GetLine(CultureManager.GetMenu(ToolBar.VariableName)));
            sb.Append(GetLine(DateTime.UtcNow.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm")));
            sb.Append(GetLine());
            sb.Append(GetLine(CultureManager.GetEntity("PARENTI01"), cbEmpresa.SelectedItem.Text));
            sb.Append(GetLine(CultureManager.GetEntity("PARENTI02"), cbLinea.SelectedItem.Text));
            sb.Append(GetLine(CultureManager.GetEntity("BOCADECARGA"), cbBocaDeCarga.SelectedItem.Text));
            sb.Append(GetLine(CultureManager.GetEntity("PARENTI63"), cbProducto.SelectedItem.Text));
            sb.Append(GetLine(CultureManager.GetLabel("DIA"), dtDia.SelectedDate.Value.ToString("dd/MM/yyyy")));
            sb.Append(GetLine(CultureManager.GetLabel("PROGRAMACION_CARGA_MAXIMA"), txtCargaMaxima.Text));
            sb.Append(GetLine(CultureManager.GetLabel("PROGRAMACION_CANTIDAD_MIXERS"), txtCantidadMixers.Text));
            sb.Append(GetLine());

            var bocas = cbBocaDeCarga.Selected > 0
                ? new List<BocaDeCarga> { DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected) }
                : DAOFactory.BocaDeCargaDAO.GetList(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected });

            var inicio = bocas.Min(boca => GetInicioActividad(boca));
            var fin = bocas.Max(boca => GetFinActividad(boca));

            sb.Append(string.Join(separator.ToString(), CsvHeader));
            for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1)))
            {
                sb.Append(string.Concat(separator, FormatTimeSpan(i)));
            }
            sb.Append(GetLine());

            foreach (RepeaterItem r in repPedidos.Items)
            {
                var item = new RepeaterItemPedido(r);

                if (r.ItemType != ListItemType.Item && r.ItemType != ListItemType.AlternatingItem) continue;
                sb.Append(GetLine(string.Join(separator.ToString(), item.GetCsvLine())));
            }
            sb.Append((string) hidCsvFooter.Value);
            sb.Append(GetLine());

            Session["CSV_EXPORT"] = sb.ToString();
            Session["CSV_FILE_NAME"] = "programacion";

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));

        }
        private string GetLine() { return GetLine(string.Empty); }
        private string GetLine(params string[] vals) { return GetLine(vals as IEnumerable<string>); }
        private string GetLine(IEnumerable<string> vals)
        {
            var v = vals.Select(s => EscapeChars(s)).ToArray();
            return GetLine(string.Join(Usuario.CsvSeparator.ToString(), v));
        }
        private string GetLine(string vals) { return vals + "\n"; }
        private string EscapeChars(string val)
        {
            return string.IsNullOrEmpty(val) ? string.Empty : val.Replace('\r',' ').Replace('\n',' ').Replace(Usuario.CsvSeparator, Usuario.CsvSeparator == ';' ? ',' : ';');
        }

        #endregion

        #region Data Bound

        protected void BoundItem(RepeaterItemPedido item)
        {
            const string cell = "<td class=\"number\">{0}</td>";

            var carga = GetCarga(item.Pedido);
            var cantidad = item.Pedido.Cantidad + item.Pedido.CantidadAjuste;
            var movilesNecesarios = Math.Ceiling(Math.Min(cantidad / carga, item.Pedido.TiempoCiclo * 1.0 / item.Pedido.Frecuencia));

            item.LblPedido.Text = item.Pedido.Codigo;
            item.LblPedido.Attributes.Add("id", item.Pedido.Id.ToString());
            item.LblHoraCarga.Text = item.Pedido.HoraCarga.ToDisplayDateTime().ToString("HH:mm");
            item.LblHoraCarga.Attributes.Add("estado", item.Pedido.Estado == Pedido.Estados.Pendiente ? "0" : "1");
            item.LblEnObra.Text = item.Pedido.FechaEnObra.ToDisplayDateTime().ToString("HH:mm");
            item.LblEnObra.Attributes.Add("estado", item.Pedido.Estado == Pedido.Estados.Pendiente ? "0" : "1");
            item.DtEnObra.SelectedDate = item.Pedido.FechaEnObra.ToDisplayDateTime();
            item.DtHoraCarga.SelectedDate = item.Pedido.HoraCarga.ToDisplayDateTime();
            item.LblBocaDeCarga.Text = EscapeChars(item.Pedido.BocaDeCarga.Descripcion);
            item.LblCliente.Text = EscapeChars(item.Pedido.Cliente.Descripcion);
            item.LblPuntoEntrega.Text = EscapeChars(item.Pedido.PuntoEntrega.Descripcion);
            item.LblContacto.Text = EscapeChars(item.Pedido.Contacto);
            item.LblObservacion.Text = EscapeChars(item.Pedido.Observacion);
            item.LblProducto.Text = EscapeChars(item.Pedido.Producto != null ? item.Pedido.Producto.Descripcion : "");
            item.LblCantidad.Text = cantidad.ToString("0.00");
            item.LblBomba.Text = item.Pedido.NumeroBomba;
            item.LblEsMinimixer.Text = item.Pedido.EsMinimixer ? CultureManager.GetLabel("SI") : CultureManager.GetLabel("NO");
            item.LblTiempoCiclo.Text = FormatTimeSpan(TimeSpan.FromMinutes(item.Pedido.TiempoCiclo));
            item.LblFrecuencia.Text = FormatTimeSpan(TimeSpan.FromMinutes(item.Pedido.Frecuencia));
            item.LblMovilesNecesarios.Text = movilesNecesarios.ToString("0");
            item.HidId.Value = item.Pedido.Id.ToString();
            item.LitStyle.Text = item.Pedido.Estado == Pedido.Estados.Pendiente ? "pendiente" : "generado";
            item.BtCambiarEnObra.OnClientClick = string.Format("return confirm('{0}');", CultureManager.GetSystemMessage("CONFIRM_OPERATION"));
            item.BtCambiarHoraCarga.OnClientClick = string.Format("return confirm('{0}');", CultureManager.GetSystemMessage("CONFIRM_OPERATION"));
            item.BtGenerarTickets.OnClientClick = string.Format("return confirm('{0}');", CultureManager.GetSystemMessage("CONFIRM_OPERATION"));
            item.BtGenerarTickets.Visible = item.Pedido.Estado == Pedido.Estados.Pendiente;

            if (item.Pedido.Estado == Pedido.Estados.Pendiente) _algunoPendiente = true;

            var csv = new List<string>();
            var cargaPorHora = GetCargaPorHora(item);
            foreach (var cargaHora in cargaPorHora)
            {
                var val = cargaHora.Value;
                var toShow = val == 0 ? string.Empty : val.ToString();
                item.LitHoras.Text += string.Format(cell,toShow);
                csv.Add(toShow);

                if (val > 0)
                {
                    var hora = cargaHora.Key;

                    if (_volumenPorHora.ContainsKey(hora)) _volumenPorHora[hora] += val;
                    else _volumenPorHora.Add(hora, val);
                }
            }

            _tiempoCiclos.Add(cantidad * item.Pedido.TiempoCiclo);
            CantidadTotal += cantidad;

            item.HidCsvLine.Value = string.Join(";", csv.ToArray());
        }
        protected void BoundHeader(RepeaterItemPedido item)
        {
            const string cell = "<th class=\"hora\">{0}</th>";

            var bocas = cbBocaDeCarga.Selected > 0 
                ? new List<BocaDeCarga> {DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected)}
                : DAOFactory.BocaDeCargaDAO.GetList(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected});

            var inicio = bocas.Min(boca => GetInicioActividad(boca));
            var fin = bocas.Max(boca => GetFinActividad(boca));

            for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1)))
            {
                item.LitHoras.Text += string.Format(cell, FormatTimeSpan(i));
            }
        }
        protected void BoundFooter(RepeaterItemPedido item)
        {
            const string cell = "<td class=\"number {1}\">{0}</td>";
            
            var rendimientoExcedido = false;
            var mixersExcedidos = false;

            var carga = GetCarga(item.Pedido);
            var bocas = cbBocaDeCarga.Selected > 0
                ? new List<BocaDeCarga> { DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected) }
                : DAOFactory.BocaDeCargaDAO.GetList(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected });

            var inicio = bocas.Min(boca => GetInicioActividad(boca));
            var fin = bocas.Max(boca => GetFinActividad(boca));
            var rendimiento = bocas.Sum(boca => boca.Rendimiento);

            var tiempoPromedio = Convert.ToInt32(_tiempoCiclos.Sum()/CantidadTotal);
            var prev = 0;

            item.LblCantidad.Text = CantidadTotal.ToString("0.00");
            item.LblTiempoCiclo.Text = FormatTimeSpan(TimeSpan.FromMinutes(tiempoPromedio));
            item.BtGenerarTickets.OnClientClick = string.Format("return confirm('{0}');", CultureManager.GetSystemMessage("CONFIRM_OPERATION"));
            

            var csv1 = new List<string>();
            var csv2 = new List<string>();
            var csv3 = new List<string>();

            for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1)))
            {
                var hora = Convert.ToInt32(i.Hours);
                var val = _volumenPorHora.ContainsKey(hora) ? _volumenPorHora[hora] : 0;
                var mix = (int)Math.Ceiling(1.0 * val / carga);
                var equi = Math.Round((prev + mix) * tiempoPromedio / TiempoReferencia);

                var toShow1 = val > 0 ? val.ToString() : "";
                var toShow2 =  mix > 0 ? mix.ToString() : "";
                var toShow3 = equi > 0 ? equi.ToString() : "";
                item.LitHoras.Text += string.Format(cell, toShow1, val > rendimiento ? "total_mal" : "total_ok");
                item.LitHoras2.Text += string.Format(cell, toShow2, "");
                item.LitHoras3.Text += string.Format(cell, toShow3, equi > CantidadMixers ? "total_mal" : "total_ok");

                csv1.Add(toShow1);
                csv2.Add(toShow2);
                csv3.Add(toShow3);

                rendimientoExcedido |= val > rendimiento;
                mixersExcedidos |= equi > CantidadMixers;
                prev = mix;
            }

            if (rendimientoExcedido || mixersExcedidos)
            {
                foreach (RepeaterItem itm in repPedidos.Items)
                {
                    var it = new RepeaterItemPedido(itm)
                                 {BtGenerarTickets = {Enabled = false, OnClientClick = string.Empty}};
                }
                item.BtGenerarTickets.Enabled = false;
                item.BtGenerarTickets.OnClientClick = string.Empty;
            }
            if (rendimientoExcedido || mixersExcedidos || !_algunoPendiente)
            {
                item.BtGenerarTickets.Enabled = false;
                item.BtGenerarTickets.OnClientClick = string.Empty;
            }

            var row1 = new[] {
                                  string.Empty, //codigo
                                  string.Empty, // hora de carga
                                  string.Empty, // hora en obra
                                  string.Empty, // boca de carga
                                  string.Empty, // cliente
                                  string.Empty, // punto de entrega
                                  string.Empty, // contacto
                                  string.Empty, // observacion
                                  string.Empty, // nro bomba
                                  string.Empty, // is minimixer
                                  CultureManager.GetLabel("TOTAL"), //producto
                                  item.LblCantidad.Text, // cantidad pedido
                                  item.LblTiempoCiclo.Text, // Tiempo de ciclo
                                  string.Empty, // frecuencia
                                  CultureManager.GetLabel("PROGRAMACION_VOLUMEN_HORA"), // moviles necesarios
                                  string.Join(";", csv1.ToArray()) // valores dinamicos
                              };
            var row2 = new[] {
                                  string.Empty, //codigo
                                  string.Empty, // hora de carga
                                  string.Empty, // hora en obra
                                  string.Empty, // boca de carga
                                  string.Empty, // cliente
                                  string.Empty, // punto de entrega
                                  string.Empty, // contacto
                                  string.Empty, // observacion
                                  string.Empty, // nro bomba
                                  string.Empty, // is minimixer
                                  string.Empty, //producto
                                  string.Empty, // cantidad pedido
                                  string.Empty, // Tiempo de ciclo
                                  string.Empty, // frecuencia
                                  CultureManager.GetLabel("PROGRAMACION_MIXERS_HORA"), // moviles necesarios
                                  string.Join(";", csv2.ToArray()) // valores dinamicos
                              };
            var row3 = new[] {
                                  string.Empty, //codigo
                                  string.Empty, // hora de carga
                                  string.Empty, // hora en obra
                                  string.Empty, // boca de carga
                                  string.Empty, // cliente
                                  string.Empty, // punto de entrega
                                  string.Empty, // contacto
                                  string.Empty, // observacion
                                  string.Empty, // nro bomba
                                  string.Empty, // is minimixer
                                  string.Empty, //producto
                                  string.Empty, // cantidad pedido
                                  string.Empty, // Tiempo de ciclo
                                  string.Empty, // frecuencia
                                  CultureManager.GetLabel("PROGRAMACION_MIXERS_EQUIVALENTES"), // moviles necesarios
                                  string.Join(";", csv3.ToArray()) // valores dinamicos
                              };

            hidCsvFooter.Value = string.Join(";", row1) + "\n" + string.Join(";", row3) + "\n" + string.Join(";", row3) + "\n";
        } 
        #endregion

        #region Search
        private void Search()
        {
            try
            {
                ValidateEntity(cbEmpresa.Selected, "PARENTI01");
                ValidateEntity(cbLinea.Selected, "PARENTI02");
                //ValidateEntity(cbBocaDeCarga.Selected, "BOCADECARGA");
                ValidateEmpty((DateTime?) dtDia.SelectedDate, (string) "DIA");
                if(!WebSecurity.IsSecuredAllowed(Securables.HormigonProgramarFuturo))
                {
                    ValidateHigher(DateTime.UtcNow.ToDisplayDateTime(), dtDia.SelectedDate.Value.Date, "DIA");
                }
                var cargaPromedio = ValidateEmpty((string) txtCargaMaxima.Text, (string) "PROGRAMACION_CARGA_MAXIMA");
                ValidateInt32(cargaPromedio, "PROGRAMACION_CARGA_MAXIMA");
                var mixers = ValidateEmpty((string) txtCantidadMixers.Text, (string) "PROGRAMACION_CANTIDAD_MIXERS");
                ValidateInt32(mixers, "PROGRAMACION_CANTIDAD_MIXERS");

                Carga = Convert.ToInt32((string) txtCargaMaxima.Text.Trim());
                CantidadMixers = Convert.ToInt32((string) txtCantidadMixers.Text);
                var desde = dtDia.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtDia.SelectedDate.Value).Date : DateTime.MinValue;
                var hasta = desde.AddDays(1);
                var list = DAOFactory.PedidoDAO.GetList(new[] { cbEmpresa.Selected },
                                                        new[] { cbLinea.Selected }, 
                                                        new[] { cbBocaDeCarga.Selected },
                                                        new[] { Pedido.Estados.Pendiente, 
                                                                Pedido.Estados.EnCurso,
                                                                Pedido.Estados.Entregado},
                                                        desde, 
                                                        hasta)
                                               .OrderBy(p => p.FechaEnObra)
                                               .ToList();

                

                if (list.Count > 0)
                {
                    repPedidos.Visible = true;
                    repPedidos.DataSource = list;
                    repPedidos.DataBind();
                    CreateGraphic(list);
                    panelResultado.Visible = true;
                }
                else
                {
                    repPedidos.Visible = false;
                    ShowResourceError("NO_MATCHES_FOUND");
                    panelResultado.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        } 
        #endregion

        #region Cambiar En Obra
        private void CambiarEnObra(RepeaterItemPedido item)
        {
            try
            {
                ValidateEmpty(item.DtEnObra.SelectedDate, "FECHA_EN_OBRA");

                var pedido = DAOFactory.PedidoDAO.FindById(item.Id);
                var dif = pedido.FechaEnObra - pedido.HoraCarga;

                if (pedido.Estado != Pedido.Estados.Pendiente) return;
                pedido.FechaEnObra = item.DtEnObra.SelectedDate != null ? item.DtEnObra.SelectedDate.Value.ToDataBaseDateTime() : DateTime.MinValue;
                pedido.HoraCarga = pedido.FechaEnObra.Add(-dif);
                DAOFactory.PedidoDAO.SaveOrUpdate(pedido);
                Search();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        } 
        #endregion

        #region Cambiar Hora de Carga
        private void CambiarHoraCarga(RepeaterItemPedido item)
        {
            try
            {
                ValidateEmpty(item.DtHoraCarga.SelectedDate, "HORA_CARGA");

                var pedido = DAOFactory.PedidoDAO.FindById(item.Id);
                if (pedido.Estado != Pedido.Estados.Pendiente) return;
                pedido.HoraCarga = item.DtHoraCarga.SelectedDate != null ? item.DtHoraCarga.SelectedDate.Value.ToDataBaseDateTime() : DateTime.MinValue;

                if (pedido.HoraCarga > pedido.FechaEnObra)
                    pedido.FechaEnObra = pedido.HoraCarga;

                DAOFactory.PedidoDAO.SaveOrUpdate(pedido);
                Search();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
        #endregion

        #region Generar Tickets

        private void GenerarPedido(RepeaterItemPedido item)
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    GenerarTickets(item);

                    transaction.Commit();

                    Search();
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        ShowError(ex);
                    }
                    ShowError(ex);
                }
            }
        }
        private void GenerarTodos()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    foreach (var item in Enumerable.OfType<RepeaterItem>(repPedidos.Items).Select(i => new RepeaterItemPedido(i)))
                    {
                        GenerarTickets(item);
                    }

                    transaction.Commit();

                    Search();
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                        ShowError(ex);
                    }
                    ShowError(ex);
                }
            }
        }

        private void GenerarTickets(RepeaterItemPedido item)
        {
            var pedido = DAOFactory.PedidoDAO.FindById(item.Id);
            if (pedido.Estado != Pedido.Estados.Pendiente) return;

            var cantidad = pedido.Cantidad + pedido.CantidadAjuste;
            double carga = GetCarga(pedido);

            var estados = DAOFactory.EstadoDAO.GetByPlanta(pedido.Linea.Id);
            var total = estados.Sum(e => Convert.ToInt32(e.Deltatime));
            if (total == 0) total = pedido.TiempoCiclo;

            if (!estados.Any(e => e.EsPuntoDeControl == Estado.Evento.LlegaAObra)) ThrowError("PROGRAMACION_NO_OBRA");

            var indexEnObra =
                estados.IndexOf(estados.First(e => e.EsPuntoDeControl == Estado.Evento.LlegaAObra));

            var deltas =
                estados.Select(
                    e =>
                    Convert.ToInt32(((e.Deltatime == 0 ? total/estados.Count : e.Deltatime)*pedido.TiempoCiclo*1.0)/
                                    total)).ToList();
            var minutos = Convert.ToInt32(deltas.Where((e, i) => i < indexEnObra).Sum());
            var inicioServicio = pedido.FechaEnObra.AddMinutes(-minutos);

            var ticketCount = 1;
            for (var entregado = 0.0; entregado < cantidad; entregado += carga)
            {
                if (entregado + carga > cantidad)
                    carga = cantidad - entregado;

                var ticket = new Ticket
                                 {
                                     CantidadCarga = carga.ToString(),
                                     CantidadCargaReal = "0",
                                     CantidadPedido = cantidad.ToString(),
                                     Cliente = pedido.Cliente,
                                     Codigo = pedido.Codigo + "-" + ticketCount.ToString().PadLeft(2, '0'),
                                     CodigoProducto = pedido.Producto != null ? pedido.Producto.Codigo : "",
                                     CumulativeQty = (entregado + carga).ToString(),
                                     DescripcionProducto = pedido.Producto != null ? pedido.Producto.Descripcion : "",
                                     Empresa = pedido.BocaDeCarga.Linea != null ? pedido.BocaDeCarga.Linea.Empresa : null,
                                     Linea = pedido.BocaDeCarga.Linea,
                                     BaseDestino = pedido.BocaDeCarga.Linea,
                                     Estado = Ticket.Estados.Pendiente,
                                     FechaTicket = inicioServicio,
                                     Pedido = pedido,
                                     PuntoEntrega = pedido.PuntoEntrega,
                                     SourceFile = "Programacion",
                                     SourceStation = "WEB",
                                     Unidad = "m3",
                                     UserField1 = string.Empty,
                                     UserField2 = string.Empty,
                                     UserField3 = string.Empty,
                                     OrdenDiario = DAOFactory.TicketDAO.FindNextOrdenDiario(pedido.BocaDeCarga.Linea != null ? pedido.BocaDeCarga.Linea.Empresa.Id : -1, pedido.BocaDeCarga.Linea != null ? pedido.BocaDeCarga.Linea.Id : -1, inicioServicio),
                                     ASincronizar = true
                                 };

                var minutosTranscurridos = 0;

                for (int i = 0; i < estados.Count; i++)
                {
                    var estado = estados[i];
                    var detalle = new DetalleTicket
                                      {
                                          Ticket = ticket,
                                          EstadoLogistico = estado,
                                          Programado = inicioServicio.AddMinutes(minutosTranscurridos)
                                      };
                    ticket.Detalles.Add(detalle);
                    minutosTranscurridos += deltas[i];
                }

                inicioServicio = inicioServicio.Add(TimeSpan.FromMinutes(pedido.Frecuencia));
                ticketCount++;

                DAOFactory.TicketDAO.SaveOrUpdate(ticket);
            }

            pedido.Estado = Pedido.Estados.EnCurso;
            DAOFactory.PedidoDAO.SaveOrUpdate(pedido);
        }

        #endregion

        #region Calculo de Carga Por Hora
        private Dictionary<int, double> GetCargaPorHora(RepeaterItemPedido item)
        {
            var total = item.Pedido.Cantidad + item.Pedido.CantidadAjuste;
            var carga = GetCarga(item.Pedido);

            var inicio = GetInicioActividad(item.Pedido.BocaDeCarga);
            var fin = GetFinActividad(item.Pedido.BocaDeCarga);
            var enObra = item.Pedido.FechaEnObra.ToDisplayDateTime().TimeOfDay;

            var values = new Dictionary<int, double>();
            for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1))) values.Add(Convert.ToInt32(i.Hours), 0);

            var proxima = enObra;
            for (var i = proxima; total > 0; i = i.Add(TimeSpan.FromMinutes(item.Pedido.Frecuencia)))
            {
                var cantidad = Math.Min(total, carga);
                total -= cantidad;
                if(values.ContainsKey(Convert.ToInt32(i.Hours)))
                    values[Convert.ToInt32(i.Hours)] += cantidad;
            }

            return values;
        } 
        #endregion

        #region SetCantidadDeMixers
        private void SetCantidadDeMixers()
        {
            try
            {
                ValidateEntity(cbEmpresa.Selected, "PARENTI01");
                ValidateEntity(cbLinea.Selected, "PARENTI02");

                var coches = DAOFactory.CocheDAO.GetList(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected })
                    .Where(c => c.Estado == Coche.Estados.Activo);

                txtCantidadMixers.Text = coches.Count().ToString();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        } 
        #endregion

        #region Crear Grafico
        protected void CreateGraphic(List<Pedido> list)
        {
            try
            {
                using (var helper = new FusionChartsMultiSeriesHelper())
                {
                    var carga = GetCarga(null);
                    var bocas = cbBocaDeCarga.Selected > 0
                        ? new List<BocaDeCarga> { DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected) }
                        : DAOFactory.BocaDeCargaDAO.GetList(new[] { cbEmpresa.Selected }, new[] { cbLinea.Selected });

                    var inicio = bocas.Min(boca => GetInicioActividad(boca));
                    var fin = bocas.Max(boca => GetFinActividad(boca));
                    var rendimiento = bocas.Sum(boca => boca.Rendimiento);

                    var tiempoPromedio = Convert.ToInt32(_tiempoCiclos.Sum() / CantidadTotal);
                    var prev = 0;
                    double maxValue = rendimiento;
                    var maxMixers = CantidadMixers;

                    #region DataSet Rendimiento (Trendline)
                    var dataSetTrendLineRendimiento = new FusionChartsDataset();
                    dataSetTrendLineRendimiento.SetPropertyValue("seriesName", CultureManager.GetLabel("RENDIMIENTO"));
                    dataSetTrendLineRendimiento.SetPropertyValue("color", "c30000");
                    dataSetTrendLineRendimiento.SetPropertyValue("parentYAxis", "S");
                    dataSetTrendLineRendimiento.SetPropertyValue("showAnchors", "0");
                    dataSetTrendLineRendimiento.SetPropertyValue("alpha", "60");
                    dataSetTrendLineRendimiento.SetPropertyValue("lineThickness", "2");
                    helper.AddDataSet(dataSetTrendLineRendimiento);
                    #endregion

                    #region DataSet Volumen por Hora (Barras)
                    var dataSetVolumen = new FusionChartsDataset();
                    dataSetVolumen.SetPropertyValue("seriesName", CultureManager.GetLabel("PROGRAMACION_VOLUMEN_HORA"));
                    dataSetVolumen.SetPropertyValue("color", "6868c3");
                    dataSetVolumen.SetPropertyValue("parentYAxis", "P"); 
                    helper.AddDataSet(dataSetVolumen);
                    #endregion

                    #region DataSet Mixers Disponibles (Trendline)
                    var dataSetTrendLineMixers = new FusionChartsDataset();
                    dataSetTrendLineMixers.SetPropertyValue("seriesName", CultureManager.GetLabel("AVAILABLE_VEHICLES"));
                    dataSetTrendLineMixers.SetPropertyValue("color", "c39600");
                    dataSetTrendLineMixers.SetPropertyValue("parentYAxis", "S");
                    dataSetTrendLineMixers.SetPropertyValue("showAnchors", "0");
                    dataSetTrendLineMixers.SetPropertyValue("alpha", "60");
                    dataSetTrendLineMixers.SetPropertyValue("lineThickness", "2");
                    helper.AddDataSet(dataSetTrendLineMixers);
                    #endregion  

                    #region DataSet Mixers Equivalentes (Linea)
                    var dataSetMixers = new FusionChartsDataset();
                    dataSetMixers.SetPropertyValue("seriesName", CultureManager.GetLabel("PROGRAMACION_MIXERS_EQUIVALENTES"));
                    dataSetMixers.SetPropertyValue("color", "009900");
                    dataSetMixers.SetPropertyValue("parentYAxis", "S");
                    dataSetMixers.SetPropertyValue("anchorSides", "10");
                    dataSetMixers.SetPropertyValue("anchorRadius", "3");
                    dataSetMixers.SetPropertyValue("anchorBorderColor", "009900");
                    helper.AddDataSet(dataSetMixers);
                    #endregion

                    #region Llenado de Categorias y Valores
                    for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1)))
                    {
                        var hora = Convert.ToInt32(i.Hours);
                        var val = _volumenPorHora.ContainsKey(hora) ? _volumenPorHora[hora] : 0;
                        var mix = (int)Math.Ceiling(1.0 * val / carga);
                        var equi = (int)Math.Round((prev + mix) * tiempoPromedio / TiempoReferencia);


                        helper.AddCategory(FormatTimeSpan(i));
                        dataSetVolumen.addValue(val.ToString().Replace(',','.'));
                        dataSetMixers.addValue(equi.ToString());

                        if (val > maxValue) maxValue = val;
                        if (equi > maxMixers) maxMixers = equi;

                        prev = mix;
                    } 
                    #endregion

                    #region Calculo proporcional de maximos y trendlines
                    maxMixers++;
                    maxValue++;
                    int divlines;
                    if (maxValue > maxMixers)
                    {
                        while (Math.Round(maxValue) % maxMixers != 0) maxValue++;
                        divlines = Convert.ToInt32(maxValue/maxMixers);
                    }
                    else
                    {
                        while (maxMixers % Math.Round(maxValue) != 0) maxMixers++;
                        divlines = Convert.ToInt32(maxMixers / maxValue);
                    }

                    var rend = rendimiento * maxMixers * 1.0 / maxValue;

                    for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1)))
                    {
                        dataSetTrendLineRendimiento.addValue(rend.ToString(CultureInfo.InvariantCulture));
                        dataSetTrendLineMixers.addValue(CantidadMixers.ToString());
                    } 
                    #endregion
                    
                    #region Valores de Cabecera
                    helper.AddConfigEntry("caption", "");
                    helper.AddConfigEntry("xAxisName", CultureManager.GetLabel("HORA"));
                    helper.AddConfigEntry("PyAxisName", CultureManager.GetLabel("VOLUMEN"));
                    helper.AddConfigEntry("SyAxisName", CultureManager.GetLabel("VEHICULOS"));
                    helper.AddConfigEntry("decimalPrecision", "0");
                    helper.AddConfigEntry("showValues", "0");
                    helper.AddConfigEntry("numberSuffix", "");
                    helper.AddConfigEntry("rotateNames", "1");
                    helper.AddConfigEntry("limitsDecimalPrecision", "0");
                    helper.AddConfigEntry("hoverCapSepChar", "-");
                    helper.AddConfigEntry("divLineAlpha", "60");
                    helper.AddConfigEntry("showAlternateHGridColor", "1");
                    helper.AddConfigEntry("alternateHGridColor", "d8d8d8");
                    helper.AddConfigEntry("alternateHGridAlpha", "60");
                    helper.AddConfigEntry("zeroPlaneThickness", "20");

                    helper.AddConfigEntry("numDivLines", (divlines-1).ToString());
                    helper.AddConfigEntry("SyAxisMaxValue", maxMixers.ToString());
                    helper.AddConfigEntry("PyAxisMaxValue", Math.Round(maxValue).ToString());
                    #endregion

                                       
                    litGraph.Text = FusionCharts.RenderChartHTML("../FusionCharts/FCF_MSColumn2DLineDY.swf", "", helper.BuildXml(), "Report", (sizeField.Width).ToString(), (sizeField.Heigth).ToString(), false);

                }
            }
            catch (Exception ex) { ShowError(ex); }
        } 
        #endregion

        #region Format Functions

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return string.Concat(timeSpan.Hours.ToString("0").PadLeft(2, '0'), ':',
                                 timeSpan.Minutes.ToString("0").PadLeft(2, '0'));
        }
        #endregion

        #region Class RepeaterItemPedido
        protected class RepeaterItemPedido
        {
            public RepeaterItem Item { get; set; }
            public Pedido Pedido { get; private set; }
            public int Id { get; private set; }
            public LinkButton LblPedido { get; private set; }
            public Label LblTiempoCiclo { get; private set; }
            public Label LblFrecuencia { get; private set; }
            public Label LblMovilesNecesarios { get; private set; }
            public Label LblBomba { get; private set; }
            public Label LblEsMinimixer { get; private set; }
            public Label LblProducto { get; private set; }
            public Label LblCantidad { get; private set; }
            public Literal LitHoras { get; private set; }
            public Literal LitHoras2 { get; private set; }
            public Literal LitHoras3 { get; private set; }
            public Label LblEnObra { get; private set; }
            public Label LblHoraCarga { get; private set; }
            public Label LblBocaDeCarga { get; private set; }
            public Label LblCliente { get; private set; }
            public Label LblPuntoEntrega { get; private set; }
            public Label LblContacto { get; private set; }
            public Label LblObservacion { get; private set; }
            public DateTimePicker DtEnObra { get; private set; }
            public DateTimePicker DtHoraCarga { get; private set; }
            public HiddenField HidId { get; private set; }
            public ImageButton BtCambiarEnObra { get; private set; }
            public ImageButton BtCambiarHoraCarga { get; private set; }
            public LinkButton BtGenerarTickets { get; private set; }
            public Literal LitStyle { get; private set; }
            public HiddenField HidCsvLine { get; private set; }
                
            public RepeaterItemPedido(RepeaterItem item)
            {
                Item = item;
                Pedido = item.DataItem as Pedido;
                LblPedido = item.FindControl("lblPedido") as LinkButton;
                LblTiempoCiclo = item.FindControl("lblTiempoCiclo") as Label;
                LblFrecuencia = item.FindControl("lblFrecuencia") as Label;
                LblMovilesNecesarios = item.FindControl("lblMovilesNecesarios") as Label;
                LblBomba = item.FindControl("lblBomba") as Label;
                LblEsMinimixer = item.FindControl("lblEsMinimixer") as Label;
                LblProducto = item.FindControl("lblProducto") as Label;
                LblCantidad = item.FindControl("lblCantidad") as Label;
                LitHoras = item.FindControl("litHoras") as Literal;
                LitHoras2 = item.FindControl("litHoras2") as Literal;
                LitHoras3 = item.FindControl("litHoras3") as Literal;
                LblEnObra = item.FindControl("lblEnObra") as Label;
                LblHoraCarga = item.FindControl("lblHoraCarga") as Label;
                LblBocaDeCarga = item.FindControl("lblBocaDeCarga") as Label;
                LblCliente = item.FindControl("lblCliente") as Label;
                LblPuntoEntrega = item.FindControl("lblPuntoEntrega") as Label;
                LblContacto = item.FindControl("lblContacto") as Label;
                LblObservacion = item.FindControl("lblObservacion") as Label;
                DtEnObra = item.FindControl("dtEnObra") as DateTimePicker;
                DtHoraCarga = item.FindControl("dtHoraCarga") as DateTimePicker;
                HidId = item.FindControl("hidId") as HiddenField;
                BtCambiarEnObra = item.FindControl("btCambiarEnObra") as ImageButton;
                BtCambiarHoraCarga = item.FindControl("btCambiarHoraCarga") as ImageButton;
                BtGenerarTickets = item.FindControl("btGenerarTickets") as LinkButton;
                LitStyle = item.FindControl("litStyle") as Literal;
                HidCsvLine = item.FindControl("hidCsvLine") as HiddenField;
                Id = HidId != null && !string.IsNullOrEmpty(HidId.Value) ? Convert.ToInt32(HidId.Value) : 0;
            }

            public string[] GetCsvLine()
            {
                return new []
                              {
                                  LblPedido.Text, //codigo
                                  LblHoraCarga.Text, // hora de carga
                                  LblEnObra.Text, // hora en obra
                                  LblBocaDeCarga.Text, // boca de carga
                                  LblCliente.Text, // cliente
                                  LblPuntoEntrega.Text, // punto de entrega
                                  LblContacto.Text, // contacto
                                  LblObservacion.Text, // observacion
                                  LblBomba.Text, // nro bomba
                                  LblEsMinimixer.Text, // es minimixer
                                  LblProducto.Text, //producto
                                  LblCantidad.Text, // cantidad pedido
                                  LblTiempoCiclo.Text, // Tiempo de ciclo
                                  LblFrecuencia.Text, // frecuencia
                                  LblMovilesNecesarios.Text, // moviles necesarios
                                  HidCsvLine.Value // valores dinamicos
                              };
            }
        } 
        #endregion


        protected override Dictionary<string, string> GetFilterValues()
        {
            var dic = new Dictionary<string, string>
                          {
                              {CultureManager.GetEntity("PARENTI01"), cbEmpresa.SelectedItem.Text},
                              {CultureManager.GetEntity("PARENTI02"), cbLinea.SelectedItem.Text},
                              {CultureManager.GetEntity("BOCADECARGA"), cbBocaDeCarga.SelectedItem.Text},
                              {CultureManager.GetEntity("PARENTI63"), cbProducto.SelectedItem.Text},
                              {CultureManager.GetLabel("DIA"), dtDia.SelectedDate.Value.ToString("dd/MM/yyyy")},
                              {CultureManager.GetLabel("PROGRAMACION_CARGA_MAXIMA"), txtCargaMaxima.Text},
                              {CultureManager.GetLabel("PROGRAMACION_CANTIDAD_MIXERS"), txtCantidadMixers.Text}
                          };
            return dic;
        }

        protected override void Print()
        {
            try
            {
                base.Print();

                var sh = new ScriptHelper(this);

                sh.RegisterStartupScript("print", "CloneAndPrintReport();");
            }
            catch (Exception ex) { ShowError(ex); }
        }

        protected override void Schedule() {}

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            Search();
        }

        protected override List<Pedido> GetResults() { return null; }

        protected override void ExportToExcel()
        {
            throw new NotImplementedException();
        }
    }
}