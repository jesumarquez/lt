using System;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Helpers.FussionChartHelpers;
using System.Collections.Generic;
using Logictracker.Culture;
using InfoSoftGlobal;
using System.Globalization;

namespace Logictracker.Reportes.CicloLogistico
{
    public partial class PedidosProgramados : ApplicationSecuredPage
    {
        protected override string GetRefference() { return "EST_PEDIDOS_PROGRAMADOS"; }
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        const double TiempoReferencia = 120.0;

        private double CantidadTotal { get; set; }
        private double Carga { get; set; }
        private int CantidadMixers { get; set; }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                dtDia.SelectedDate = DateTime.UtcNow.ToDisplayDateTime().Date;

                litTabProg.Text = CultureManager.GetLabel("PROGRAMACION_VIEW_PROGRAMACION");
                litTabGraph.Text = CultureManager.GetLabel("PROGRAMACION_VIEW_GRAFICO");
            }
        }

        protected void RepPedidosItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
                BoundItem(new RepeaterItemPedido(e.Item));

            if(e.Item.ItemType == ListItemType.Footer)
                BoundFooter(new RepeaterItemPedido(e.Item));

            if(e.Item.ItemType == ListItemType.Header)
                BoundHeader(new RepeaterItemPedido(e.Item));
        }

        protected void BtBuscarClick(object sender, EventArgs e) { Search(); }

        protected void LblPedido_OnClick(object sender, EventArgs e)
        {
            Session.Add("id", ((LinkButton)sender).Attributes["id"]);
            Response.Redirect("../../CicloLogistico/PedidoAlta.aspx");
        }
        
        #region Data Bound

        protected void BoundItem(RepeaterItemPedido item)
        {
            const string cell = "<td class=\"number\">{0}</td>";
            
            var tickets = DAOFactory.TicketDAO.GetByPedido(new[] { cbEmpresa.Selected },
                                                           new[] { cbLinea.Selected },
                                                           new[] { item.Pedido.Id })
                                              .OrderBy(p => p.Vehiculo);

            Carga = Convert.ToDouble(tickets.First().CantidadCarga);
            var cantidad = item.Pedido.Cantidad + item.Pedido.CantidadAjuste;
            
            CantidadMixers = 0;
            var ultimoVehiculo = 0;
            foreach (var ticket in tickets)
            {
                if (ticket.Vehiculo != null && ticket.Vehiculo.Id != ultimoVehiculo)
                {    
                    CantidadMixers++;
                    ultimoVehiculo = ticket.Vehiculo.Id;
                }
            }
            
            var movilesNecesarios = Math.Ceiling(Math.Min(cantidad / Carga, item.Pedido.TiempoCiclo * 1.0 / item.Pedido.Frecuencia));
            
            item.lblPedido.Text = item.Pedido.Codigo;
            item.lblPedido.Attributes.Add("id", item.Pedido.Id.ToString("#0"));
            item.lblHoraCarga.Text = item.Pedido.HoraCarga.ToDisplayDateTime().ToString("HH:mm");
            item.lblEnObra.Text = item.Pedido.FechaEnObra.ToDisplayDateTime().ToString("HH:mm");
            item.lblEnObra.Attributes.Add("estado", item.Pedido.Estado == Pedido.Estados.Pendiente ? "0" : "1");
            item.lblCliente.Text = item.Pedido.Cliente.Descripcion;
            item.lblPuntoEntrega.Text = item.Pedido.PuntoEntrega.Descripcion;
            item.LblContacto.Text = item.Pedido.Contacto;
            item.LblObservacion.Text = item.Pedido.Observacion;
            item.lblProducto.Text = item.Pedido.Producto != null ? item.Pedido.Producto.Descripcion : "";
            item.lblCantidad.Text = cantidad.ToString("0.00");
            item.lblBomba.Text = item.Pedido.NumeroBomba;
            item.LblEsMinimixer.Text = item.Pedido.EsMinimixer ? CultureManager.GetLabel("SI") : CultureManager.GetLabel("NO");
            item.lblTiempoCiclo.Text = FormatTimeSpan(TimeSpan.FromMinutes(item.Pedido.TiempoCiclo));
            item.lblFrecuencia.Text = FormatTimeSpan(TimeSpan.FromMinutes(item.Pedido.Frecuencia));
            item.lblMovilesNecesarios.Text = movilesNecesarios.ToString("0");
            item.hidId.Value = item.Pedido.Id.ToString("#0");
            item.litStyle.Text = item.Pedido.Estado == Pedido.Estados.Pendiente ? "pendiente" : "generado";

            var cargaPorHora = GetCargaPorHora(item);
            foreach (var cargaHora in cargaPorHora)
            {
                var val = cargaHora.Value;
                item.litHoras.Text += string.Format(cell, val == 0 ? string.Empty : val.ToString());

                if (val > 0)
                {
                    var hora = cargaHora.Key;

                    if (_volumenPorHora.ContainsKey(hora)) _volumenPorHora[hora] += val;
                    else _volumenPorHora.Add(hora, val);
                }
            }

            _tiempoCiclos.Add(cantidad * item.Pedido.TiempoCiclo);
            CantidadTotal += cantidad;
        }
        
        protected void BoundHeader(RepeaterItemPedido item)
        {
            const string cell = "<th class=\"hora\">{0}</th>";

            var boca = DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected);
            var inicio = GetInicioActividad(boca);
            var fin = GetFinActividad(boca);

            for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1)))
            {
                item.litHoras.Text += string.Format(cell, FormatTimeSpan(i));
            }
        }
        
        protected void BoundFooter(RepeaterItemPedido item)
        {
            const string cell = "<td class=\"number {1}\">{0}</td>";

            var boca = DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected);
            var inicio = GetInicioActividad(boca);
            var fin = GetFinActividad(boca);
            var tiempoPromedio = Convert.ToInt32(_tiempoCiclos.Sum()/CantidadTotal);
            var prev = 0;

            item.lblCantidad.Text = CantidadTotal.ToString("0.00");
            item.lblTiempoCiclo.Text = FormatTimeSpan(TimeSpan.FromMinutes(tiempoPromedio));
                        
            for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1)))
            {
                var hora = Convert.ToInt32(i.Hours);
                var val = _volumenPorHora.ContainsKey(hora) ? _volumenPorHora[hora] : 0;
                var mix = (int)Math.Ceiling(1.0 * val / Carga);
                var equi = Math.Round((prev + mix) * tiempoPromedio / TiempoReferencia);

                item.litHoras.Text += string.Format(cell, val > 0 ? val.ToString() : "", val > boca.Rendimiento ? "total_mal" : "total_ok");
                item.litHoras2.Text += string.Format(cell, mix > 0 ? mix.ToString() : "", "");
                item.litHoras3.Text += string.Format(cell, equi > 0 ? equi.ToString() : "", equi > CantidadMixers ? "total_mal" : "total_ok");

                prev = mix;
            }
        } 
        
        #endregion

        private void Search()
        {
            try
            {
                ValidateEntity(cbEmpresa.Selected, "PARENTI01");
                ValidateEntity(cbLinea.Selected, "PARENTI02");
                ValidateEntity(cbBocaDeCarga.Selected, "BOCADECARGA");
                ValidateEmpty((DateTime?) dtDia.SelectedDate, "DIA");

                var desde = SecurityExtensions.ToDataBaseDateTime(dtDia.SelectedDate.Value).Date;
                var hasta = desde.AddDays(1);
                var list = DAOFactory.PedidoDAO.GetList(new[] { cbEmpresa.Selected },
                                                        new[] { cbLinea.Selected }, 
                                                        new[] { -1 }, 
                                                        new[] { -1 },
                                                        new[] { cbBocaDeCarga.Selected },
                                                        new[] { Pedido.Estados.Pendiente, 
                                                                Pedido.Estados.EnCurso,
                                                                Pedido.Estados.Entregado},
                                                        new[] { cbProducto.Selected },
                                                        desde, 
                                                        hasta);

                if (repPedidos.Visible == list.Count > 0)
                {
                    repPedidos.DataSource = list;
                    repPedidos.DataBind();
                    CreateGraphic(list);
                    panelResultado.Visible = true;
                }
                if (!repPedidos.Visible)
                {
                    ShowResourceError("NO_MATCHES_FOUND");
                    panelResultado.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        } 

        private Dictionary<int, double> GetCargaPorHora(RepeaterItemPedido item)
        {
            var total = item.Pedido.Cantidad + item.Pedido.CantidadAjuste;
            var carga = item.Pedido.CargaViaje;

            var inicio = GetInicioActividad(item.Pedido.BocaDeCarga);
            var fin = GetFinActividad(item.Pedido.BocaDeCarga);
            var enObra = item.Pedido.FechaEnObra.ToDisplayDateTime().TimeOfDay;

            var entregado = 0.0;
            var values = new Dictionary<int, double>();

            for (var i = inicio; i <= fin; i = i.Add(TimeSpan.FromHours(1)))
            {
                var val = 0.0;

                if (i.Hours < enObra.Hours) val = 0;
                else if (entregado == total) val = 0;
                else if (total < carga) val = total;
                else if (entregado < total)
                {
                    var val1 = Math.Ceiling(60.0 / item.Pedido.Frecuencia) * carga;
                    var val2 = total - entregado;
                    val = Math.Min(val1, val2);
                }

                values.Add(Convert.ToInt32(i.Hours), val);
                entregado += val;
            }
            return values;
        } 

        protected void CreateGraphic(List<Pedido> list)
        {
            try
            {
                using (var helper = new FusionChartsMultiSeriesHelper())
                {
                    var boca = DAOFactory.BocaDeCargaDAO.FindById(cbBocaDeCarga.Selected);
                    var inicio = GetInicioActividad(boca);
                    var fin = GetFinActividad(boca);

                    var tiempoPromedio = Convert.ToInt32(_tiempoCiclos.Sum() / CantidadTotal);
                    var prev = 0;
                    double maxValue = boca.Rendimiento;
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
                        var mix = (int)Math.Ceiling(1.0 * val / Carga);
                        var equi = (int)Math.Round((prev + mix) * tiempoPromedio / TiempoReferencia);


                        helper.AddCategory(FormatTimeSpan(i));
                        dataSetVolumen.addValue(val.ToString());
                        dataSetMixers.addValue(equi.ToString());

                        if (val > maxValue) maxValue = val;
                        if (equi > maxMixers) maxMixers = equi;

                        prev = mix;
                    } 
                    #endregion

                    #region Calculo proporcional de maximos y trendlines
                    maxMixers++;
                    maxValue++;
                    var divlines = 1;
                    if (maxValue > maxMixers)
                    {
                        while (maxValue % maxMixers != 0) maxValue++;
                        divlines = Convert.ToInt32(maxValue/maxMixers);
                    }
                    else
                    {
                        while (maxMixers % maxValue != 0) maxMixers++;
                        divlines = Convert.ToInt32(maxMixers / maxValue);
                    }

                    var rend = boca.Rendimiento * maxMixers * 1.0 / maxValue;

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
                    helper.AddConfigEntry("PyAxisMaxValue", maxValue.ToString());
                    #endregion
                                       
                    litGraph.Text = FusionCharts.RenderChartHTML("../../FusionCharts/FCF_MSColumn2DLineDY.swf", "", helper.BuildXml(), "Report", (sizeField.Width).ToString(), (sizeField.Heigth).ToString(), false);
                }
            }
            catch (Exception ex) { ShowError(ex); }
        } 

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            return string.Concat(timeSpan.Hours.ToString("0").PadLeft(2, '0'), ':',
                                 timeSpan.Minutes.ToString("0").PadLeft(2, '0'));
        }

        protected class RepeaterItemPedido
        {
            public RepeaterItem Item { get; set; }
            public Pedido Pedido { get; private set; }
            public int Id { get; private set; }
            public LinkButton lblPedido { get; private set; }
            public Label lblHoraCarga { get; private set; }
            public Label lblTiempoCiclo { get; private set; }
            public Label lblFrecuencia { get; private set; }
            public Label lblMovilesNecesarios { get; private set; }
            public Label lblBomba { get; private set; }
            public Label LblEsMinimixer { get; private set; }
            public Label lblProducto { get; private set; }
            public Label lblCantidad { get; private set; }
            public Literal litHoras { get; private set; }
            public Literal litHoras2 { get; private set; }
            public Literal litHoras3 { get; private set; }
            public Label lblEnObra { get; private set; }
            public Label lblCliente { get; private set; }
            public Label lblPuntoEntrega { get; private set; }
            public Label LblContacto { get; private set; }
            public Label LblObservacion { get; private set; }
            public HiddenField hidId { get; private set; }
            public Literal litStyle { get; private set; }
                
            public RepeaterItemPedido(RepeaterItem item)
            {
                Item = item;
                Pedido = item.DataItem as Pedido;
                lblPedido = item.FindControl("lblPedido") as LinkButton;
                lblHoraCarga = item.FindControl("lblHoraCarga") as Label;
                lblTiempoCiclo = item.FindControl("lblTiempoCiclo") as Label;
                lblFrecuencia = item.FindControl("lblFrecuencia") as Label;
                lblMovilesNecesarios = item.FindControl("lblMovilesNecesarios") as Label;
                lblBomba = item.FindControl("lblBomba") as Label;
                LblEsMinimixer = item.FindControl("lblEsMinimixer") as Label;
                lblProducto = item.FindControl("lblProducto") as Label;
                lblCantidad = item.FindControl("lblCantidad") as Label;
                litHoras = item.FindControl("litHoras") as Literal;
                litHoras2 = item.FindControl("litHoras2") as Literal;
                litHoras3 = item.FindControl("litHoras3") as Literal;
                lblEnObra = item.FindControl("lblEnObra") as Label;
                lblCliente = item.FindControl("lblCliente") as Label;
                lblPuntoEntrega = item.FindControl("lblPuntoEntrega") as Label;
                LblContacto = item.FindControl("lblContacto") as Label;
                LblObservacion = item.FindControl("lblObservacion") as Label;
                hidId = item.FindControl("hidId") as HiddenField;
                litStyle = item.FindControl("litStyle") as Literal;
                Id = hidId != null && !string.IsNullOrEmpty(hidId.Value) ? Convert.ToInt32(hidId.Value) : 0;
            }
        } 
    }
}