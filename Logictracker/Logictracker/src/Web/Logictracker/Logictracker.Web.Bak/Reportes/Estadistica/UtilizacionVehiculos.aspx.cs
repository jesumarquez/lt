using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Logictracker.Utils;
using Logictracker.Web.Helpers.ColorHelpers;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.Estadistica
{
    public partial class UtilizacionVehiculos : SecuredGraphReportPage<VehicleUtilization>
    {
        protected override string VariableName { get { return "MOBILE_UTILIZATION"; } }
        protected override string GetRefference() { return "UTILIZACION_VEHICULOS"; }
        protected override GraphTypes GraphType { get { return GraphTypes.StackedColumn; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("DATE"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("HORAS"); } }
        protected override bool CsvButton { get { return false; } }
        protected override bool PrintButton { get { return false; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Size.Tick = OnTick;

            if (!IsPostBack && Movil > 0) Size.EnableTick = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            dpDesde.SetDate();
            dpHasta.SetDate();

            SetInitialFilterValues();
        }

        protected override List<VehicleUtilization> GetResults()
        {
            return ReportFactory.VehicleUtilizationDAO.GetMobileUtilizations(Convert.ToInt32((string) ddlMovil.SelectedValue), SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.Value),
                                                                             SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.Value));
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsMultiSeriesHelper())
            {
                if (ReportObjectsList.Count <= 0) throw new Exception("No se encontraron datos asociados a los filtros seleccionados!");

                SetGraphProperties(helper);

                AddDates(helper);

                var maxTurnos = ReportObjectsList.Select(res => res.HsTurnos.Count).Max();

                var colorGenerator = new ColorGenerator();
                FusionChartsDataset niveles;

                for (var i = 0; i < maxTurnos; i++)
                {
                    niveles = new FusionChartsDataset();
                    niveles.SetPropertyValue("color", HexColorUtil.ColorToHex(colorGenerator.GetNextColor()));

                    foreach (var dia in ReportObjectsList)
                    {
                        if (dia.HsTurnos.Count > 1)
                        {
                            niveles.addValue(dia.HsTurnos.First().ToString(CultureInfo.InvariantCulture));
                            dia.HsTurnos.Remove(dia.HsTurnos.First());
                            niveles.addValue(dia.HsReales.First().ToString(CultureInfo.InvariantCulture));
                            dia.HsReales.Remove(dia.HsReales.First());
                            niveles.addValue("0");
                        }
                        else
                            if (dia.HsTurnos.Count == 1)
                            {
                                niveles.addValue("0");
                                niveles.addValue("0");
                                niveles.addValue("0");
                            }
                    }
                    helper.AddDataSet(niveles);
                }

                niveles = new FusionChartsDataset();
                niveles.SetPropertyValue("color", HexColorUtil.ColorToHex(Color.Gray));
                niveles.SetPropertyValue("seriesName", "Fuera de Turno");
                foreach (var dia in ReportObjectsList)
                {
                    niveles.addValue(dia.HsTurnos.First().ToString(CultureInfo.InvariantCulture));
                    niveles.addValue(dia.HsReales.First().ToString(CultureInfo.InvariantCulture));
                    niveles.addValue("0");
                }
                helper.AddDataSet(niveles);

                return helper.BuildXml();
            }
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI03"), ddlMovil.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private int Movil
        {
            get
            {
                if (Session["Movil"] != null)
                {
                    ViewState["Movil"] = Session["Movil"];

                    Session["Movil"] = null;
                }

                return (int)(ViewState["Movil"] ?? 0);
            }

            set { ViewState["Movil"] = value; }
        }

        private DateTime Desde
        {
            get
            {
                if (Session["Desde"] != null)
                {
                    ViewState["Desde"] = Session["Desde"];

                    Session["Desde"] = null;
                }
                return (DateTime)(ViewState["Desde"] ?? DateTime.MinValue);
            }

            set { ViewState["Desde"] = value; }
        }

        private DateTime Hasta
        {
            get
            {
                if (Session["Hasta"] != null)
                {
                    ViewState["Hasta"] = Session["Hasta"];

                    Session["Hasta"] = null;
                }
                return (DateTime)(ViewState["Hasta"] ?? DateTime.MinValue);
            }

            set { ViewState["Hasta"] = value; }
        }

        private void SetGraphProperties(FusionChartsMultiSeriesHelper helper)
        {
            helper.AddConfigEntry("caption", string.Format("Utilización del Vehículo {2}: {0} Hasta: {1}",
                                                           dpDesde.SelectedDate.Value.ToShortDateString(), dpHasta.SelectedDate.Value.ToShortDateString(), ddlMovil.SelectedItem.Text));

            helper.AddConfigEntry("xAxisName", XAxisLabel);
            helper.AddConfigEntry("yAxisName", YAxisLabel);
            helper.AddConfigEntry("decimalPrecision", "2");
            helper.AddConfigEntry("thousandSeparator", ",");
            helper.AddConfigEntry("formatNumberScale", "0");
            helper.AddConfigEntry("numberSuffix", "hs");
            helper.AddConfigEntry("showValues", "0");
            helper.AddConfigEntry("showNames", "1");
            helper.AddConfigEntry("rotateNames", "1");
        }

        private void AddDates(FusionChartsMultiSeriesHelper helper)
        {
            foreach (var nivel in ReportObjectsList)
            {
                helper.AddCategory(String.Format("{0} + Horas Teoricas", nivel.Fecha.ToShortDateString()));
                helper.AddCategory(String.Format("{0} + Horas Reales", nivel.Fecha.ToShortDateString()));
                helper.AddCategory(String.Format(""));
            }
        }

        private void SetInitialFilterValues()
        {
            if (IsPostBack) return;

            if (Desde != DateTime.MinValue) dpDesde.SelectedDate = Desde.ToDisplayDateTime();

            if (Hasta != DateTime.MinValue) dpHasta.SelectedDate = Hasta.ToDisplayDateTime();

            if (Movil != 0) ddlMovil.SelectedValue = Movil.ToString();
        }

        private void OnTick(object sender, EventArgs e)
        {
            BtnSearchClick(sender, e);

            UpdatePanelGraph.Update();
        }

        #endregion
    }
}
