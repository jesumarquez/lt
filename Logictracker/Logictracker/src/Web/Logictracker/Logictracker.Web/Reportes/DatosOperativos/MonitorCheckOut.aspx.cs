
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
    public partial class MonitorCheckOut : SecuredGraphReportPage<CheckOut>
    {
        private const string DesdeHasta = "DESDE_HASTA";
        private const string Hora = "HORA";
        private const string Cantidad = "CANTIDAD";

        protected override string VariableName { get { return "MON_CHECK_OUT"; } }
        protected override string GetRefference() { return "MON_CHECK_OUT"; }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel(Hora); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Cantidad); } }

        protected override bool ExcelButton { get { return true; } }

        protected override List<CheckOut> GetResults()
        {
            var desde = dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
            var hasta = dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();

            var periodo = 15;
            if (rbtn30.Checked) periodo = 30;
            else if (rbtn60.Checked) periodo = 60;

            return ReportFactory.CheckOutDAO.GetCheckOuts(ddlLocacion.Selected, ddlPlanta.Selected, ddlTransportista.Selected, desde, hasta, periodo);
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
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel(DesdeHasta), dpDesde.SelectedDate.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm"), dpHasta.SelectedDate.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm")));

                helper.AddConfigEntry("decimalPrecision", "1");
                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("hoverCapSepChar", " - ");
                helper.AddConfigEntry("rotateNames", "1");

                foreach (var checkout in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    item.AddPropertyValue("color", "AFD8F8");
                    item.AddPropertyValue("name", checkout.Fecha.ToDisplayDateTime().ToString("HH:mm"));
                    item.AddPropertyValue("value", checkout.Cantidad.ToString(CultureInfo.InvariantCulture));
                    
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

            foreach (var checkout in ReportObjectsList)
            {
                categories.Add(checkout.Fecha.ToString("HH:mm"));
                data.addValue(checkout.Cantidad.ToString(CultureInfo.InvariantCulture));
            }

            datasets.Add(data);
            GraphCategories = categories;
            GraphDataSet = datasets;
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => t.Fecha.ToDisplayDateTime().ToString("HH:mm"),
                                                  t => t.Cantidad.ToString(CultureInfo.InvariantCulture));
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlLocacion.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI07"), ddlTransportista.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm")},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToString("dd/MM/yyyy HH:mm")}
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

        protected override string GetDescription(string s)
        {
            var linea = GetLinea();

            var sDescription = new StringBuilder(GetEmpresa().RazonSocial + " - ");
            if (linea != null) sDescription.AppendFormat("Base {0} - ", linea.Descripcion);

            return sDescription.ToString();
        }

        protected override DateTime GetSinceDateTime()
        {
            return dpDesde.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }

        protected override DateTime GetToDateTime()
        {
            return dpHasta.SelectedDate.GetValueOrDefault().ToDataBaseDateTime();
        }
    }
}
