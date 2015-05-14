using InfoSoftGlobal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.M2M
{
    public partial class Medidores : SecuredGraphReportPage<Medicion>
    {
        protected override string GetRefference() { return "REP_MEDIDORES"; }
        protected override string VariableName { get { return "REP_MEDIDORES"; } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("MESES"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("CONSUMO"); } }

        private Dictionary<string, double> _meses = new Dictionary<string, double>();
        private Dictionary<string, double> _dias = new Dictionary<string, double>();
        private Dictionary<string, double> _horas = new Dictionary<string, double>();
        
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
                dtFecha.SetDate();

            if (ctrlFiltros.Empresa != cbEmpresa.Selected || ctrlFiltros.Linea != cbLinea.Selected
                || ListasDistintas(ctrlFiltros.TipoEntidad, cbTipoEntidad.SelectedValues))
            {
                ctrlFiltros.Empresa = cbEmpresa.Selected > 0 ? cbEmpresa.Selected : -1;
                ctrlFiltros.Linea = cbLinea.Selected > 0 ? cbLinea.Selected : -1;
                ctrlFiltros.TipoEntidad = cbTipoEntidad.SelectedValues ?? new List<int> {-1};

                ctrlFiltros.Filtros = null;
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

        protected void CbEntidadSelectedIndexChanged(object sender, EventArgs e)
        {
            cbSubEntidad.ClearSelection();
            cbSubEntidad.Enabled = cbEntidad.SelectedValues.Count <= 1;
        }

        protected override List<Medicion> GetResults()
        {
            var desde = dtFecha.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.Value) : new DateTime();
            var hasta = desde.AddMonths(1).AddSeconds(-1);

            return MedicionesPorFecha(hasta);
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("DESDE_HASTA"),
                                                               dtFecha.SelectedDate.HasValue ? dtFecha.SelectedDate.Value.ToShortDateString() : DateTime.MinValue.ToShortDateString(),
                                                               dtFecha.SelectedDate.HasValue ? dtFecha.SelectedDate.Value.AddMonths(1).AddSeconds(-1).ToShortDateString() : DateTime.MinValue.ToShortDateString()));

                helper.AddConfigEntry("xAxisName", "MESES");
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", " - ");

                divChartDias.InnerHtml = "";
                divChartHoras.InnerHtml = "";

                _meses.Clear();

                CargarAñoAnterior();
                CargarMesAnterior();
                CargarMesActual();
                
                var pb = new List<string>();

                foreach (var pair in _meses)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("link", "javascript:a(" + pb.Count + ")");
                    pb.Add(ClientScript.GetPostBackEventReference(btnMes, ""));

                    var split = pair.Key.Split('/');
                    var key = GetMes(Convert.ToInt32(split[0])) + " " + split[1];

                    item.AddPropertyValue("color", "FF5904");
                    item.AddPropertyValue("name", key);
                    item.AddPropertyValue("value", pair.Value.ToString(CultureInfo.InvariantCulture));
                    
                    helper.AddItem(item);
                }

                var hasta = dtFecha.SelectedDate.HasValue ? dtFecha.SelectedDate.Value.AddMonths(1).AddSeconds(-1) : DateTime.MinValue;
                var now = DateTime.UtcNow;
                if (hasta.Year == now.Year && hasta.Month == now.Month && hasta > now)
                {
                    var promedio = _meses[hasta.ToString("MM/yyyy")] / now.Day;
                    var proyectado = promedio * GetDiasMes(hasta.Month, hasta.Year);
                    var key = CultureManager.GetLabel("PROYECTADO") + " " + GetMes(Convert.ToInt32(hasta.Month)) + " " + hasta.Year;

                    var item = new FusionChartsItem();

                    item.AddPropertyValue("color", "FAB802");
                    item.AddPropertyValue("name", key);
                    item.AddPropertyValue("value", proyectado.ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);
                }

                var script = "var pb =[" + string.Join(",", pb.Select(p => "\"" + p + "\"").ToArray())
                    + "]; function a(n) { document.getElementById('" + hidMes.ClientID + "').value = n; eval(pb[n]); }";

                System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "a", script, true);
                return helper.BuildXml();
            }
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

        protected void ClickDia(object sender, EventArgs e)
        {
            try
            {
                var index = Convert.ToInt32((string) hidMes.Value);
                var fecha = DateTime.Parse("01/" + IndicesMeses[index]);
                divChartDias.InnerHtml = FusionCharts.RenderChartHTML(ChartXmlDefinition, "", GetGraphXmlDias(fecha), "Report", Size.Width.ToString(), Size.Heigth.ToString(), false);

                index = Convert.ToInt32((string) hidDia.Value);
                fecha = DateTime.Parse(IndicesDias[index]);
                divChartHoras.InnerHtml = FusionCharts.RenderChartHTML(ChartXmlDefinition, "", GetGraphXmlHoras(fecha), "Report", Size.Width.ToString(), Size.Heigth.ToString(), false);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            foreach (var pair in _meses)
            {
                categories.Add(pair.Key);
                dataset.addValue(pair.Value.ToString());
            }

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

        private List<Medicion> MedicionesPorFecha(DateTime hasta)
        {
            var ret = new List<Medicion>();

            if (cbEntidad.SelectedValues.Count > 1)
            {
                foreach (ListItem item in cbSubEntidad.Items) item.Selected = true;
            }

            var entidadesFiltradas = GetEntidadesFiltradas();

            foreach (var idSubEntidad in cbSubEntidad.SelectedValues)
            {
                var subEntidad = DAOFactory.SubEntidadDAO.FindById(idSubEntidad);

                if (subEntidad != null && subEntidad.Id > 0 && subEntidad.Sensor != null && entidadesFiltradas.Contains(subEntidad.Entidad.Id))
                {
                    var medicionFinal = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subEntidad.Sensor.Dispositivo.Id, subEntidad.Sensor.Id, hasta);

                    if (medicionFinal != null)
                        ret.Add(medicionFinal);                        
                }
            }
            return ret;
        }
        
        private static string GetMes(int mes)
        {
            var sMes = string.Empty;

            switch (mes)
            {
                case 1: sMes = CultureManager.GetLabel("ENERO"); break;
                case 2: sMes = CultureManager.GetLabel("FEBRERO"); break;
                case 3: sMes = CultureManager.GetLabel("MARZO"); break;
                case 4: sMes = CultureManager.GetLabel("ABRIL"); break;
                case 5: sMes = CultureManager.GetLabel("MAYO"); break;
                case 6: sMes = CultureManager.GetLabel("JUNIO"); break;
                case 7: sMes = CultureManager.GetLabel("JULIO"); break;
                case 8: sMes = CultureManager.GetLabel("AGOSTO"); break;
                case 9: sMes = CultureManager.GetLabel("SEPTIEMBRE"); break;
                case 10: sMes = CultureManager.GetLabel("OCTUBRE"); break;
                case 11: sMes = CultureManager.GetLabel("NOVIEMBRE"); break;
                case 12: sMes = CultureManager.GetLabel("DICIEMBRE"); break;
            }

            return sMes;
        }

        private static int GetDiasMes(int mes, int año)
        {
            var dias = 0;

            switch (mes)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    dias = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    dias = 30;
                    break;
                case 2:
                    var a = (decimal)año/4;
                    var b = Math.Round(a, 0);
                    dias = a == b ? 29 : 28;
                    break;
            }

            return dias;
        }
        
        private string GetGraphXmlDias(DateTime fecha)
        {
            var desde = fecha.ToDataBaseDateTime();
            var hasta = desde.AddMonths(1).AddSeconds(-1);

            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("DESDE_HASTA"), desde.ToDisplayDateTime(), hasta.ToDisplayDateTime()));

                helper.AddConfigEntry("xAxisName", "DIAS");
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", " - ");

                _dias.Clear();
                var diasMes = GetDiasMes(desde.ToDisplayDateTime().Month, desde.ToDisplayDateTime().Year);

                for (var i = 0; i < diasMes; i++)
                {
                    var fechaDesde = desde.AddDays(i).AddSeconds(-1);
                    var fechaHasta = fechaDesde.AddDays(1);

                    var valor = GetValorMedicion(fechaDesde, fechaHasta);

                    _dias.Add((i+1) + "/" + desde.ToDisplayDateTime().ToString("MM/yyyy"), valor);
                }

                var pb = new List<string>();

                foreach (var pair in _dias)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("link", "javascript:a(" + pb.Count + ")");
                    pb.Add(ClientScript.GetPostBackEventReference(btnDia, ""));

                    item.AddPropertyValue("color", "FF5904");
                    item.AddPropertyValue("name", pair.Key.Split('/')[0]);
                    item.AddPropertyValue("value", pair.Value.ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);
                }

                var script = "var pb =[" + string.Join(",", pb.Select(p => "\"" + p + "\"").ToArray())
                    + "]; function a(n) { document.getElementById('" + hidDia.ClientID + "').value = n; eval(pb[n]); }";

                System.Web.UI.ScriptManager.RegisterStartupScript(this, typeof(string), "a", script, true);
                return helper.BuildXml();
            }
        }

        private string GetGraphXmlHoras(DateTime fecha)
        {
            var desde = fecha.ToDataBaseDateTime();
            var hasta = desde.AddDays(1).AddSeconds(-1);

            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("DESDE_HASTA"), desde.ToDisplayDateTime(), hasta.ToDisplayDateTime()));

                helper.AddConfigEntry("xAxisName", "HORAS");
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", " - ");

                _horas.Clear();

                for (var i = 0; i < 24; i++)
                {
                    var fechaDesde = desde.AddHours(i).AddSeconds(-1);
                    var fechaHasta = fechaDesde.AddHours(1);

                    var valor = GetValorMedicion(fechaDesde, fechaHasta);

                    _horas.Add(i.ToString("#0"), valor);
                }

                foreach (var pair in _horas)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("color", "FF5904");
                    item.AddPropertyValue("name", pair.Key);
                    item.AddPropertyValue("value", pair.Value.ToString(CultureInfo.InvariantCulture));

                    helper.AddItem(item);
                }

                return helper.BuildXml();
            }
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

        private void CargarAñoAnterior()
        {
            var desde = dtFecha.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.Value.AddYears(-1)) : new DateTime();
            var hasta = desde.AddMonths(1).AddSeconds(-1);

            var valor = GetValorMedicion(desde, hasta);

            var key = desde.ToDisplayDateTime().ToString("MM/yyyy");
            _meses.Add(key, valor);
        }

        private void CargarMesAnterior()
        {
            var desde = dtFecha.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.Value.AddMonths(-1)) : new DateTime();
            var hasta = desde.AddMonths(1).AddSeconds(-1);

            var valor = GetValorMedicion(desde, hasta);

            var key = desde.ToDisplayDateTime().ToString("MM/yyyy");
            _meses.Add(key, valor);
        }

        private void CargarMesActual()
        {
            var desde = dtFecha.SelectedDate != null ? SecurityExtensions.ToDataBaseDateTime(dtFecha.SelectedDate.Value) : new DateTime();
            var hasta = desde.AddMonths(1).AddSeconds(-1);

            var valor = GetValorMedicion(desde, hasta);

            var key = desde.ToDisplayDateTime().ToString("MM/yyyy");
            _meses.Add(key, valor);
        }

        private double GetValorMedicion(DateTime desde, DateTime hasta)
        {
            var valor = 0.0;

            var entidadesFiltradas = GetEntidadesFiltradas();

            foreach (var idSubEntidad in cbSubEntidad.SelectedValues)
            {
                var subEntidad = DAOFactory.SubEntidadDAO.FindById(idSubEntidad);

                if (subEntidad.Sensor != null && entidadesFiltradas.Contains(subEntidad.Entidad.Id))
                {
                    var medicionInicial = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subEntidad.Sensor.Dispositivo.Id, subEntidad.Sensor.Id, desde);
                    var medicionFinal = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subEntidad.Sensor.Dispositivo.Id, subEntidad.Sensor.Id, hasta);

                    if (medicionFinal != null && medicionInicial != null)
                        valor += medicionFinal.ValorDouble - medicionInicial.ValorDouble;
                    else if (medicionFinal != null)
                        valor += medicionFinal.ValorDouble;
                }
            }   

            return valor;
        }

        private List<int> GetEntidadesFiltradas()
        {
            var entidadesId = cbEntidad.SelectedValues;

            if (QueryExtensions.IncludesAll((IEnumerable<int>) entidadesId))
                entidadesId = DAOFactory.EntidadDAO.GetList(new[] {cbEmpresa.Selected},
                                                            new[] {cbLinea.Selected},
                                                            new[] {-1},
                                                            new[] {cbTipoEntidad.Selected})
                                                   .Select(e => e.Id).ToList();

            if (ctrlFiltros.Filtros.Count > 0)
                entidadesId = DAOFactory.EntidadDAO.GetEntidadesByFiltros(entidadesId, ctrlFiltros.Filtros)
                                                   .Cast<EntidadPadre>()
                                                   .Select(e => e.Id).ToList();

            return entidadesId;
        }

        private static bool ListasDistintas(IList<int> a, IList<int> b)
        {
            return a.Count != b.Count || a.Where((t, i) => t != b[i]).Any();
        }
    }
}