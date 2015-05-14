using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Types.ValueObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Reportes.Estadistica
{
    public partial class ReportesEstadisticaResumenOperador : SecuredGraphReportPage<MaxSpeeds>
    {
        private OperatorStadistics Stadistics
        {
            get { return ViewState["Stadistics"] == null ? new OperatorStadistics() : (OperatorStadistics)ViewState["Stadistics"]; }
            set { ViewState["Stadistics"] = value; } 
        }
        private List<OperatorMobilesVo> OperatorMobiles
        {
            get { return ViewState["OperatorMobiles"] == null ? new List<OperatorMobilesVo>() : (List<OperatorMobilesVo>)ViewState["OperatorMobiles"]; }
            set { ViewState["OperatorMobiles"] = value; }
        }

        private const string Dia = "DIA";
        private const string MaximasDiarias = "MAXIMAS_DIARIAS";
        private const string Velocidad = "VELOCIDAD";

        protected override int? DefaultWidth { get { return 1200; } }
        protected override int? DefaultHeight { get { return 400; } }
        protected override string VariableName { get { return "STAT_RESUMEN_OPERADOR"; } }
        protected override string GetRefference() { return "RESUMEN_OPERADOR"; }
        protected override string XAxisLabel { get { return CultureManager.GetLabel(Dia); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Velocidad); } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override bool ExcelButton { get { return true; } }

        protected override void OnPrePrint()
        {
            AddResultsToSession();

            Session["KeepInSes"] = true;

            ifMovilesPrint.Visible = ifDetalleVehiculoPrint.Visible = Stadistics.HasActiveDays();
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
                helper.AddConfigEntry("caption", CultureManager.GetLabel(MaximasDiarias));
                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("numberSuffix", "km/h");
                helper.AddConfigEntry("hoverCapSepChar", " -");
                helper.AddConfigEntry("rotateNames", "1");

                foreach (var speed in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    if (!speed.Dia.DayOfWeek.Equals(DayOfWeek.Saturday) & !speed.Dia.DayOfWeek.Equals(DayOfWeek.Sunday)) item.AddPropertyValue("color", "008ED6");

                    item.AddPropertyValue("name", speed.Dia.ToString("dd/MM"));
                    item.AddPropertyValue("value", speed.Velocidad.ToString("#0"));
                    item.AddPropertyValue("hoverText", string.Concat(CultureManager.GetLabel(Dia), string.Format(" {0:dd/MM/yyyy}", speed.Dia), " - ", CultureManager.GetLabel("ALCANZADA_POR"), " ", speed.CometidoPor));

                    helper.AddItem(item);
                }

                return helper.BuildXml();
            }
        }

        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = ddlEmpleado.SelectedItem.Text };
            var categories = new List<string>();

            foreach (var t in ReportObjectsList)
            {
                categories.Add(String.Format("{0:dd/MM}", t.Dia));
                dataset.addValue(t.Velocidad.ToString("#0"));
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            base.BtnSearchClick(sender, e);

            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            Stadistics = ReportFactory.OperatorStadisticsDAO.GetOperatorStadistics(ddlEmpleado.Selected, desde, hasta);

            var showResults = Stadistics != null && Stadistics.HasActiveDays();

            if (showResults)
            {
                OperatorMobiles = ReportFactory.OperatorMobilesDAO.GetOperatorMobiles(ddlEmpleado.Selected, desde, hasta).Select(om => new OperatorMobilesVo(om)).ToList();

                AddResultsToSession();
            }

            ifDetalleVehiculo.Visible = ifMoviles.Visible = showResults;

            if (NotFound != null) NotFound.Text = !showResults ? CultureManager.GetSystemMessage("NO_RESULT_FOR_CURRENT_FILTERS") : null;
        }

        private void AddResultsToSession()
        {
            Session.Add("Stadistics", Stadistics);
            Session.Add("Mobiles", OperatorMobiles);
            Session.Add("Empleado", ddlEmpleado.Selected);
            Session.Add("Desde", SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()));
            Session.Add("Hasta", SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()));
        }

        protected override void ExportToCsv()
        {
			var builder = new BaseCsvBuilder(Usuario.CsvSeparator);

            GenerateCsvHeader(builder);
            GenerateStadisticsColumns(builder);
            GenerateMaximumSpeedColumns(builder);
            GenerateMobilesColumns(builder);

            Session["CSV_EXPORT"] = builder.Build();
            Session["CSV_FILE_NAME"] = "report";

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

		private void GenerateStadisticsColumns(BaseCsvBuilder builder)
        {
            if (Stadistics == null) return;
            
            var separator = Usuario.CsvSeparator;

            builder.GenerateRow(CultureManager.GetLabel("DETALLE_OPERADOR"));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("RECORRIDO"), separator, CultureManager.GetLabel("TOTAL"), ":", Stadistics.KilometrosTotales, separator,
                CultureManager.GetLabel("PROMEDIO_DIARIO"), ":", Stadistics.KilometrosPromedio, separator));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("DIAS"), separator, CultureManager.GetLabel("TOTALES"), ":", Stadistics.Dias, separator,
                CultureManager.GetLabel("CON_ACTIVIDAD"), ":", Stadistics.DiasActivo, separator, CultureManager.GetLabel("SIN_ACTIVIDAD"), ":", Stadistics.Dias - Stadistics.DiasActivo, separator));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("VELOCIDAD"), separator, CultureManager.GetLabel("MAXIMA_ALCANZADA"), ":", Stadistics.VelocidadMaxima, separator,
                CultureManager.GetLabel("PROMEDIO"), ":", Stadistics.VelocidadPromedio, separator));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("TIEMPOS"), separator, CultureManager.GetLabel("MOVIMIENTO"), ":", Stadistics.HorasMovimiento, separator,
                CultureManager.GetLabel("DETENCION"), ":", Stadistics.HorasDetenido, separator, CultureManager.GetLabel("SIN_REPORTAR"), ":", Stadistics.HorasSinReportar, separator));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("INFRACCIONES"), separator, CultureManager.GetLabel("CANTIDAD"), ":", Stadistics.Infracciones, separator,
                CultureManager.GetLabel("TIEMPO"), ":", Stadistics.MinsInfraccion, separator));

            builder.GenerateRow("");
        }

		private void GenerateCsvHeader(BaseCsvBuilder builder)
        {
            var param = new Dictionary<string, string>
                            {
                                {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                                {CultureManager.GetEntity("PARENTI09"), ddlEmpleado.SelectedItem.Text},
                                {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.ToString()},
                                {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.ToString()}
                            };

            builder.GenerateHeader(CultureManager.GetMenu("STAT_RESUMEN_OPERADOR"), param);
        }

		private void GenerateMaximumSpeedColumns(BaseCsvBuilder builder)
        {
            if (ReportObjectsList == null) return;
            var separator = Usuario.CsvSeparator;
            builder.GenerateRow(CultureManager.GetLabel("MAXIMAS_DIARIAS"));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("DIA"), separator, CultureManager.GetLabel("VELOCIDAD"), separator, CultureManager.GetLabel("ALCANZADA_POR"), separator));

            foreach (var o in ReportObjectsList) builder.GenerateRow(String.Concat(o.Dia, separator, o.Velocidad, separator, o.CometidoPor, separator));

            builder.GenerateRow(string.Empty);
        }

		private void GenerateMobilesColumns(BaseCsvBuilder builder)
        {
            if (OperatorMobiles == null) return;

            var separator = Usuario.CsvSeparator;

            builder.GenerateRow(CultureManager.GetEntity("PARENTI03"));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("PATENTE"), separator, CultureManager.GetLabel("INTERNO"), separator, CultureManager.GetLabel("INFRACCIONES"), separator,
                CultureManager.GetLabel("KILOMETROS"), separator, CultureManager.GetLabel("TIEMPO_CONDUCCION"), separator));

            foreach (var o in OperatorMobiles) builder.GenerateRow(String.Concat(o.Patente, separator, o.Interno, separator, o.Infracciones, separator, o.Kilometros, separator, o.Movimiento, separator));
        }

        protected override List<MaxSpeeds> GetResults()
        {
            return ReportFactory.MaxSpeedDAO.GetOperatorMaxSpeeds(ddlEmpleado.Selected, 
                                                                  SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()),
                                                                  SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()));
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => String.Format("{0:dd/MM}", t.Dia),
                                                  t => t.Velocidad.ToString(CultureInfo.InvariantCulture));
        }

        protected override Dictionary<String, String> GetFilterValues()
        {
            return new Dictionary<String, String>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI09"), ddlEmpleado.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }
    }
}
