
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.Estadistica
{
    public partial class EstadisticaMobilesKilometers : SecuredGraphReportPage<MobilesKilometers>
    {
        private const string DesdeHasta = "DESDE_HASTA";
        private const string Kilometros = "KILOMETROS_SIN_ACENTO";
        private const string Parenti03 = "PARENTI03";

        protected override string VariableName { get { return "STAT_REP_KM_ACUM"; } }
        protected override string GetRefference() { return "MOBILES_KILOMETERS"; }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetEntity(Parenti03); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Kilometros); } }

        protected override bool ExcelButton { get { return true; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool SendReportButton { get { return true; } }

        protected override List<MobilesKilometers> GetResults()
        {
            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            ToogleItems(lbMobiles);

            return ReportFactory.MobilesKilometersDAO.GetMobilesKilometers(desde, hasta, lbMobiles.SelectedValues, chkEnCiclo.Checked);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel(DesdeHasta), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString()));

                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("numberSuffix", "km");
                helper.AddConfigEntry("hoverCapSepChar", " - ");
                helper.AddConfigEntry("rotateNames", "1");

                foreach (var kilometer in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("link", Server.UrlEncode(string.Format(
                        "n-{0}Reportes/Estadistica/MonthKilometers.aspx?Movil={1}&InitialDate={2}&FinalDate={3}&SoloEnRuta={4}",
                        ApplicationPath,
                        kilometer.Movil, dpDesde.SelectedDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture),
                        dpHasta.SelectedDate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture),
                        chkEnCiclo.Checked)));

                    item.AddPropertyValue("color", "AFD8F8");

                    item.AddPropertyValue("name", kilometer.Interno.Replace('&', 'y'));
                    item.AddPropertyValue("value", kilometer.Kilometers.ToString(CultureInfo.InvariantCulture));
                    item.AddPropertyValue("hoverText", kilometer.Interno.Replace('&', 'y'));

                    helper.AddItem(item);
                }
                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var datasets = new List<FusionChartsDataset>();
            var data = new FusionChartsDataset { Name = YAxisLabel };
            var categories = new List<string>();

            foreach (var kilometer in ReportObjectsList)
            {
                categories.Add(kilometer.Interno);
                data.addValue(kilometer.Kilometers.ToString(CultureInfo.InvariantCulture));
            }

            datasets.Add(data);
            GraphCategories = categories;
            GraphDataSet = datasets;
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => t.Interno,
                                                  t => t.Kilometers.ToString(CultureInfo.InvariantCulture));
        }

        protected override List<string> GetExcelExtraItemList()
        {
            var items = new List<string>();
            foreach (var movil in ReportObjectsList)
            {
                var coche = DAOFactory.CocheDAO.FindById(movil.Movil);

                if (coche != null)
                {
                    var tipo = coche.TipoCoche != null ? coche.TipoCoche.Descripcion : string.Empty;
                    var dominio = coche.Patente;
                    var resp = coche.Chofer != null && coche.Chofer.Entidad != null ? coche.Chofer.Entidad.Descripcion : string.Empty;

                    items.Add(tipo + "|" + dominio + "|" + resp);
                }
            }
            return items;
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI17"), ddlTipoVehiculo.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().AddDays(-1).ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        protected override Empresa GetEmpresa()
        {
            return (ddlLocacion.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (ddlPlanta != null && ddlPlanta.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
        }

        protected override string GetSelectedVehicles()
        {
            var sVehiculos = new StringBuilder();

            if (lbMobiles.SelectedValues.Contains(0)) lbMobiles.ToogleItems();

            foreach (var vehiculo in lbMobiles.SelectedValues)
            {
                if (!sVehiculos.ToString().Equals(""))
                    sVehiculos.Append(",");

                sVehiculos.Append(vehiculo.ToString());
            }

            return sVehiculos.ToString();
        }

        protected override string GetDescription()
        {
            var linea = GetLinea();
            if (lbMobiles.SelectedValues.Contains(0)) lbMobiles.ToogleItems();

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base {0} - ", linea.Descripcion);
            sDescription.AppendFormat("Tipo de Vehiculo: {0} - ", ddlTipoVehiculo.SelectedItem.Text);
            sDescription.AppendFormat("Cantidad Vehiculos: {0} ", lbMobiles.SelectedStringValues.Count);

            return sDescription.ToString();
        }

        protected override List<int> GetSelectedListByField(string field)
        {
            if (lbMobiles.SelectedValues.Contains(0)) lbMobiles.ToogleItems();
            return lbMobiles.SelectedValues;
        }

        protected override DateTime GetSinceDateTime()
        {
            return dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override DateTime GetToDateTime()
        {
            return dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override int GetCompanyId()
        {
            return GetEmpresa().Id;
        }

        protected override bool GetCicleCheck()
        {
            return chkEnCiclo.Checked;
        }
    }
}
