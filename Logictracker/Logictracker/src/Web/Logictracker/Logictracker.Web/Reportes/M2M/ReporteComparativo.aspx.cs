using InfoSoftGlobal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.M2M
{
    public partial class ReporteComparativo : SecuredGraphReportPage<double>
    {
        protected override string GetRefference() { return "REP_COMPARATIVO"; }
        protected override string VariableName { get { return "REP_COMPARATIVO"; } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("DIAS"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("CONSUMO"); } }

        private Dictionary<string, double[]> _meses = new Dictionary<string, double[]>();
        private Dictionary<string, double[]> _dias = new Dictionary<string, double[]>();

        protected string[] IndicesMeses
        {
            get { return Session["IndicesMeses"] as string[] ?? new string[0]; }
            set { Session.Add("IndicesMeses", value); }
        }
        protected string[] IndicesDias
        {
            get { return Session["IndicesDias"] as string[] ?? new string[0]; }
            set { Session.Add("IndicesDias", value); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dtFechaDesde.SetDate();
                dtFechaHasta.SetDate();
            }
        }

        protected override void OnUnload(EventArgs e)
        {
        	 base.OnUnload(e);
             if (_meses.Count > 0)
                 IndicesMeses = _meses.Select(m => m.Key).ToArray();
             
             if (_dias.Count > 0) 
                 IndicesDias = _dias.Select(m => m.Key).ToArray();
        }

        protected override List<double> GetResults()
        {
            if (dtFechaDesde.SelectedDate == null || dtFechaHasta.SelectedDate == null)
                return null;

            var desde = SecurityExtensions.ToDataBaseDateTime(dtFechaDesde.SelectedDate.Value);
            var hasta = SecurityExtensions.ToDataBaseDateTime(dtFechaHasta.SelectedDate.Value);

            var mediciones = GetValorMedicion(desde, hasta, cbVehiculo.Selected) + GetValorConsumo(desde, hasta, cbVehiculo.Selected) + GetValorConsumoCalculado(desde, hasta, cbVehiculo.Selected);

            return mediciones > 0.0 ? new List<double>{mediciones} : new List<double>();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                var desde = dtFechaDesde.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFechaDesde.SelectedDate.Value) : new DateTime();
                var hasta = dtFechaHasta.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFechaHasta.SelectedDate.Value) : new DateTime();
                
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("DESDE_HASTA"), desde.ToDisplayDateTime(), hasta.ToDisplayDateTime()));

                helper.AddConfigEntry("xAxisName", "MESES");
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", " - ");

                divChartDias.InnerHtml = "";
                _meses.Clear();

                if (desde < hasta)
                {
                    while (desde.ToDisplayDateTime().Month < hasta.ToDisplayDateTime().Month || desde.ToDisplayDateTime().Year < hasta.ToDisplayDateTime().Year)
                    {
                        var fin = new DateTime(desde.Year, desde.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1).ToDataBaseDateTime();

                        var valor = GetValorMedicion(desde, fin, cbVehiculo.Selected);
                        var consumo = GetValorConsumo(desde, fin, cbVehiculo.Selected);
                        var consumoCalculado = GetValorConsumoCalculado(desde, fin, cbVehiculo.Selected);

                        _meses.Add(desde.ToDisplayDateTime().ToString("MM/yyyy"), new[] { valor, consumo, consumoCalculado });

                        desde = fin.AddSeconds(1);
                    }

                    if (desde.ToDisplayDateTime().Month == hasta.ToDisplayDateTime().Month && desde.ToDisplayDateTime().Year == hasta.ToDisplayDateTime().Year)
                    {
                        var valor = GetValorMedicion(desde, hasta, cbVehiculo.Selected);
                        var consumo = GetValorConsumo(desde, hasta, cbVehiculo.Selected);
                        var consumoCalculado = GetValorConsumoCalculado(desde, hasta, cbVehiculo.Selected);

                        _meses.Add(desde.ToDisplayDateTime().ToString("MM/yyyy"), new[] { valor, consumo, consumoCalculado });
                    }

                    var pb = new List<string>();

                    foreach (var pair in _meses)
                    {
                        var item = new FusionChartsItem();
                        item.AddPropertyValue("link", "javascript:a(" + pb.Count + ")");
                        
                        item.AddPropertyValue("color", "AFD8F8");
                        item.AddPropertyValue("name", pair.Key.Split('/')[0]);
                        item.AddPropertyValue("value", pair.Value[1].ToString(CultureInfo.InvariantCulture));

                        helper.AddItem(item);

                        item = new FusionChartsItem();
                        item.AddPropertyValue("link", "javascript:a(" + pb.Count + ")");
                        
                        item.AddPropertyValue("color", "FF5904");
                        item.AddPropertyValue("value", pair.Value[0].ToString(CultureInfo.InvariantCulture));

                        helper.AddItem(item);

                        item = new FusionChartsItem();
                        item.AddPropertyValue("link", "javascript:a(" + pb.Count + ")");
                        pb.Add(ClientScript.GetPostBackEventReference(btnMes, ""));

                        item.AddPropertyValue("color", "FAB802");
                        item.AddPropertyValue("value", pair.Value[2].ToString(CultureInfo.InvariantCulture));

                        helper.AddItem(item);
                    }

                    var script = "var pb =[" + string.Join(",", pb.Select(p => "\"" + p + "\"").ToArray())
                                    + "]; function a(n) { document.getElementById('" + hidMes.ClientID + "').value = n; eval(pb[n]); }";

                    System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "a", script, true);
                }

                lblLitrosCargados.Text = CultureManager.GetLabel("LITROS_CARGADOS");
                lblLitrosConsumidos.Text = CultureManager.GetLabel("LITROS_CONS");
                lblLitrosCalculados.Text = CultureManager.GetLabel("LITROS_CALCULADOS");
                pnlInferior.Visible = true;
                
                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), cbEmpresa.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), cbLinea.SelectedItem.Text}
                            };
        }

        protected void ClickMes(object sender, EventArgs e)
        {
            try
            {
                var index = Convert.ToInt32((string) hidMes.Value);
                var fecha = DateTime.Parse("01/" + IndicesMeses[index]);
                divChartDias.InnerHtml = FusionCharts.RenderChartHTML(ChartXmlDefinition, "", GetGraphXmlDias(fecha), "Report", Size.Width.ToString(), Size.Heigth.ToString(), false);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private string GetGraphXmlDias(DateTime fecha)
        {
            var fechaSel = dtFechaDesde.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtFechaDesde.SelectedDate.Value) : DateTime.MinValue;
            var desde = fecha.ToDataBaseDateTime() < fechaSel ? fechaSel : fecha.ToDataBaseDateTime();
            var fin = new DateTime(desde.Year, desde.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1).ToDataBaseDateTime();
            var hasta = dtFechaHasta.SelectedDate.HasValue && fin > SecurityExtensions.ToDataBaseDateTime(dtFechaHasta.SelectedDate.Value)
                            ? SecurityExtensions.ToDataBaseDateTime(dtFechaHasta.SelectedDate.Value)
                            : fin;

            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("DESDE_HASTA"), desde.ToDisplayDateTime(), hasta.ToDisplayDateTime()));

                helper.AddConfigEntry("xAxisName", "DIAS");
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", " - ");

                _dias.Clear();

                while (desde.ToDisplayDateTime().Day < hasta.ToDisplayDateTime().Day)
                {
                    fin = new DateTime(desde.Year, desde.Month, desde.Day, 23, 59, 59).ToDataBaseDateTime();

                    var valor = GetValorMedicion(desde, fin, cbVehiculo.Selected);
                    var consumo = GetValorConsumo(desde, fin, cbVehiculo.Selected);
                    var consumoCalculado = GetValorConsumoCalculado(desde, fin, cbVehiculo.Selected);

                    _dias.Add(desde.ToDisplayDateTime().ToString("dd/MM/yyyy"), new[] { valor, consumo, consumoCalculado });

                    desde = fin.AddSeconds(1);
                }

                if (desde.ToDisplayDateTime().Day == hasta.ToDisplayDateTime().Day)
                {
                    var valor = GetValorMedicion(desde, hasta, cbVehiculo.Selected);
                    var consumo = GetValorConsumo(desde, hasta, cbVehiculo.Selected);
                    var consumoCalculado = GetValorConsumoCalculado(desde, hasta, cbVehiculo.Selected);

                    _dias.Add(desde.ToDisplayDateTime().ToString("dd/MM/yyyy"), new[] { valor, consumo, consumoCalculado });
                }

                foreach (var pair in _dias)
                {
                    var item = new FusionChartsItem();
                    item.AddPropertyValue("color", "AFD8F8");
                    item.AddPropertyValue("name", pair.Key.Split('/')[0]);
                    item.AddPropertyValue("value", pair.Value[1].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);

                    item = new FusionChartsItem();
                    item.AddPropertyValue("color", "FF5904");
                    item.AddPropertyValue("value", pair.Value[0].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);

                    item = new FusionChartsItem();
                    item.AddPropertyValue("color", "FAB802");
                    item.AddPropertyValue("value", pair.Value[2].ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);
                }

                lblLitrosCargados.Text = CultureManager.GetLabel("LITROS_CARGADOS");
                lblLitrosConsumidos.Text = CultureManager.GetLabel("LITROS_CONS");
                lblLitrosCalculados.Text = CultureManager.GetLabel("LITROS_CALCULADOS");
                pnlInferior.Visible = true;

                return helper.BuildXml();
            }
        }

        private double GetValorMedicion(DateTime desde, DateTime hasta, int idVehiculo)
        {
            return DAOFactory.DatamartDAO.GetSummarizedDatamart(desde, hasta, idVehiculo).Consumo;
        }

        private double GetValorConsumo(DateTime desde, DateTime hasta, int idVehiculo)
        {
            return DAOFactory.ConsumoDetalleDAO.GetList(new[] { cbEmpresa.Selected }, 
                                                        new[] { cbLinea.Selected }, 
                                                        new[] { -1 },
                                                        new[] { -1 }, 
                                                        new[] { -1 }, 
                                                        new[] { -1 }, 
                                                        new[] { idVehiculo }, 
                                                        new[] { -1 }, 
                                                        new[] { -1 }, 
                                                        new[] { -1 }, 
                                                        new[] { -1 },
                                                        new[] { -1 },
                                                        new[] { -1 },
                                                        desde, 
                                                        hasta, 
                                                        new[] { -1 })
                                               .Where(c => c.Insumo.TipoInsumo.DeCombustible 
                                                        && c.ConsumoCabecera.Estado != ConsumoCabecera.Estados.Eliminado)
                                               .Sum(c => c.Cantidad);
        }

        private double GetValorConsumoCalculado(DateTime desde, DateTime hasta, int idVehiculo)
        {
            var valor = 0.0;
            double rendimiento;

            var vehiculo = DAOFactory.CocheDAO.FindById(idVehiculo);
            
            if (vehiculo.CocheOperacion != null && vehiculo.CocheOperacion.Rendimiento > 0.0)
                rendimiento = vehiculo.CocheOperacion.Rendimiento;
            else
                rendimiento = vehiculo.Modelo != null ? vehiculo.Modelo.Rendimiento : 0.0;

            var kmRecorridos = DAOFactory.DatamartDAO.GetSummarizedDatamart(desde, hasta, idVehiculo).Kilometros;

            if (rendimiento > 0.0 && kmRecorridos > 0.0)
                valor += (kmRecorridos / 100.0) * rendimiento;

            return valor;
        }

        private string ChartXmlDefinition
        {
            get
            {
                var template = String.Empty;

                switch (GraphType)
                {
                    case GraphTypes.Barrs: template = "FCF_Column3D.swf"; break;
                    case GraphTypes.Lines: template = "FCF_Line.swf"; break;
                    case GraphTypes.MultiLine: template = "FCF_MSLine.swf"; break;
                    case GraphTypes.MultiColumn: template = "FCF_MSColumn2D.swf"; break;
                    case GraphTypes.StackedColumn: template = "FCF_StackedColumn3D.swf"; break;
                }

                return String.Concat(FusionChartDir, template);
            }
        }
    }
}