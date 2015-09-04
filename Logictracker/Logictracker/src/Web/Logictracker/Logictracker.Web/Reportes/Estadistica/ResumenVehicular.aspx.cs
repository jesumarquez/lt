using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.ValueObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.Estadistica
{
    public partial class AccidentologiaResumenVehicular : SecuredMixedReportPage<MaxSpeeds, MobileDriversVo>
    {
        #region Private Properties

        /// <summary>
        /// Operator stadiatics for the givenn period.
        /// </summary>
        private MobileStadistics Stadistics
        {
            get { return ViewState["Stadistics"] == null ? new MobileStadistics() : (MobileStadistics)ViewState["Stadistics"]; }
            set { ViewState["Stadistics"] = value; }
        }

        /// <summary>
        /// List of mobile drivers for the givenn time span.
        /// </summary>
        private List<MobileDriversVo> MobileDrivers
        {
            get { return ViewState["MobileDrivers"] == null ? new List<MobileDriversVo>() : (List<MobileDriversVo>)ViewState["MobileDrivers"]; }
            set { ViewState["MobileDrivers"] = value; }
        }

        /// <summary>
        /// Initial data filter datetime.
        /// </summary>
        private DateTime Desde
        {
            get
            {
                if (Session["Desde"] != null)
                {
                    ViewState["Desde"] = Session["Desde"];

                    Session["Desde"] = null;

                    FromSession = true;
                }
                return (DateTime)(ViewState["Desde"] ?? DateTime.MinValue);
            }
        }

        /// <summary>
        /// Final datetime filter value.
        /// </summary>
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
        }

        /// <summary>
        /// Mobile id coming from session.
        /// </summary>
        private int Mobile
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
        }

        /// <summary>
        /// Base id comin from session.
        /// </summary>
        private int Base
        {
            get { if (Session["Base"] != null)
                    {
                        ViewState["Base"] = Session["Base"];
                        Session["Base"] = null;
                    }
            return (int)(ViewState["Base"] ?? 0);
            }
        }

        private bool FromSession { get; set; }

        /// <summary>
        /// Vehicle Type id coming from session.
        /// </summary>
        private int VehicleType
        {
            get
            {
                if (Session["VehicleType"] != null)
                {
                    ViewState["VehicleType"] = Session["VehicleType"];

                    Session["VehicleType"] = null;
                }
                return (int)(ViewState["VehicleType"] ?? 0);
            }
        }

        /// <summary>
        /// Day format variable name.
        /// </summary>
        private const string Dia = "DIA";

        /// <summary>
        /// Report caption variable name.
        /// </summary>
        private const string MaximasDiarias = "MAXIMAS_DIARIAS";

        /// <summary>
        /// Speed format variable name.
        /// </summary>
        private const string Velocidad = "VELOCIDAD";

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "STAT_RESUMEN_VEHICULAR"; } }
        protected override string GetRefference() { return "RESUMEN_VEHICULAR"; }
        protected override string XAxisLabel { get { return CultureManager.GetLabel(Dia); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel(Velocidad); } }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override int? DefaultWidth { get { return 1200; } }
        protected override int? DefaultHeight { get { return 400; } }
        protected override bool ScheduleButton { get { return true; } }
        protected override bool ExcelButton { get { return true; } }

        protected override Empresa GetEmpresa()
        {
            return (ddlDistrito.Selected > 0) ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : null;
        }

        protected override Linea GetLinea()
        {
            return (ddlBase != null && ddlBase.Selected > 0) ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds report search results to session.
        /// </summary>
        private void AddResultsToSession()
        {
            Session.Add("Stadistics", Stadistics);
            Session.Add("Drivers", MobileDrivers);
            Session.Add("FechaDesde", SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()));
            Session.Add("FechaHasta", SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()));
            Session.Add("ResumenMobile", ddlVehiculo.Selected);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Custom search method implementation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void BtnSearchClick(object sender, EventArgs e)
        {
            base.BtnSearchClick(sender, e);

            RealizarBusqueda();
        }

        /// <summary>
        /// Gets graph xml definition based on the report data.
        /// </summary>
        /// <returns></returns>
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
                    item.AddPropertyValue("hoverText", string.Concat(CultureManager.GetLabel(Dia), string.Format(" {0:dd/MM/yyyy}", speed.Dia),
                        " - ", CultureManager.GetLabel("ALCANZADA_POR"), " ", speed.CometidoPor));

                    helper.AddItem(item);
                }

                return helper.BuildXml();
            }
        }

        /// <summary>
        /// Gets the data to be displayed at the printed report.
        /// </summary>
        protected override void GetGraphCategoriesAndDatasets()
        {
            var dataset = new FusionChartsDataset { Name = ddlVehiculo.SelectedItem.Text };
            var categories = new List<string>();

            foreach (var t in ReportObjectsList)
            {
                categories.Add(String.Format("{0:dd/MM}", t.Dia));
                dataset.addValue(t.Velocidad.ToString("#0"));
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        /// <summary>
        /// Initial filter values setup and data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }

            if (IsPostBack || (DateTime.MinValue.Equals(Desde))) return;

            SetInitialFilterValues();

            if (FromSession) RealizarBusqueda();
        }

        /// <summary>
        /// Gets data associated to the specified filter values.
        /// </summary>
        /// <returns></returns>
        protected override List<MaxSpeeds> GetResults()
        {
            var inicio = DateTime.UtcNow;
            try
            {
                var results = ReportFactory.MaxSpeedDAO.GetMobileMaxSpeeds(ddlVehiculo.Selected, SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault()), SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault()));
                
                var duracion = (DateTime.UtcNow - inicio).TotalSeconds.ToString("##0.00");

				STrace.Trace("Resumen Vehicular", String.Format("Duración de la consulta: {0} segundos", duracion));

				return results;
            }
            catch (Exception ex)
            {
				STrace.Exception("Resumen Vehicular", ex, String.Format("Reporte: Resumen Vehicular. Duración de la consulta: {0:##0.00} segundos", (DateTime.UtcNow - inicio).TotalSeconds));

                throw;
            }
        }

        protected override List<MobileDriversVo> GetMixedReportResults()
        {
            return MobileDrivers.ToList();
        }
        
        /// <summary>
        /// Sets up hidden size control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Size.Tick = OnTick;

            if (!IsPostBack) Size.EnableTick = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs initial serach when called from a link.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e)
        {
            if (Mobile <= 0) return;

            BtnSearchClick(sender, e);

            UpdatePanelGraph.Update();
        }

        #endregion

        #region CSV And Print Methods

        /// <summary>
        /// Exports the report into a csv file.
        /// </summary>
        protected override void ExportToCsv()
        {
			var builder = new BaseCsvBuilder(Usuario.CsvSeparator);

            GenerateCsvHeader(builder);
            GenerateStadisticsColumns(builder);
            GenerateMaximumSpeedColumns(builder);
            GenerateOperatorsColumns(builder);

            Session["CSV_EXPORT"] = builder.Build();
            Session["CSV_FILE_NAME"] = "report";

            OpenWin(string.Concat(ApplicationPath, "Common/exportCSV.aspx"), CultureManager.GetSystemMessage("EXPORT_CSV_DATA"));
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => String.Format("{0:dd/MM}", t.Dia),
                                                  t => t.Velocidad.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the filter values for the exported report version.
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, string> GetFilterValues()
        {
            var hsInfraccion = TimeSpan.FromHours(Stadistics.HsInfraccion);

            return new Dictionary<string, string>
                        {
                            {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                            {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                            {CultureManager.GetEntity("PARENTI03"), ddlVehiculo.SelectedItem.Text}, 
                            {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                            {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                            {CultureManager.GetLabel("TOTAL"), Stadistics.KilometrosTotales.ToString("#0.00") + " km"},
                            {CultureManager.GetLabel("PROMEDIO_DIARIO"), Stadistics.KilometrosPromedio.ToString("#0.00") + " km"},
                            {CultureManager.GetLabel("TOTALES"), Stadistics.Dias.ToString("#0")},
                            {CultureManager.GetLabel("CON_ACTIVIDAD"), Stadistics.DiasActivo.ToString("#0")},
                            {CultureManager.GetLabel("SIN_ACTIVIDAD"), (Stadistics.Dias - Stadistics.DiasActivo).ToString("#0")},
                            {CultureManager.GetLabel("MAXIMA_ALCANZADA"), Stadistics.VelocidadMaxima + " km/h"},
                            {CultureManager.GetLabel("PROMEDIO"), Stadistics.VelocidadPromedio + " km/h"},
                            {CultureManager.GetLabel("MOVIMIENTO"), CultureManager.GetLabel("DIAS") + ": " + Stadistics.HorasMovimiento.Days + " - " + 
                                                                    CultureManager.GetLabel("HORAS") + ": " + Stadistics.HorasMovimiento.Hours.ToString("00") + ":" + Stadistics.HorasMovimiento.Minutes.ToString("00") + ":" + Stadistics.HorasMovimiento.Seconds.ToString("00")},
                            {CultureManager.GetLabel("DETENCION"), CultureManager.GetLabel("DIAS") + ": " + Stadistics.HorasDetenido.Days + " - " + 
                                                                   CultureManager.GetLabel("HORAS") + ": " + Stadistics.HorasDetenido.Hours.ToString("00") + ":" + Stadistics.HorasDetenido.Minutes.ToString("00") + ":" + Stadistics.HorasDetenido.Seconds.ToString("00")},
                            {CultureManager.GetLabel("SIN_REPORTAR"), CultureManager.GetLabel("DIAS") + ": " + Stadistics.HorasSinReportar.Days + " - " + 
                                                                      CultureManager.GetLabel("HORAS") + ": " + Stadistics.HorasSinReportar.Hours.ToString("00") + ":" + Stadistics.HorasSinReportar.Minutes.ToString("00") + ":" + Stadistics.HorasSinReportar.Seconds.ToString("00")},
                            {CultureManager.GetLabel("CANTIDAD"), Stadistics.Infracciones.ToString("#0")},
                            {CultureManager.GetLabel("TIEMPO"), hsInfraccion.Hours.ToString("00") + ":" + hsInfraccion.Minutes.ToString("00") + ":" + hsInfraccion.Seconds.ToString("00")}
                        };
        }

        protected override Dictionary<string, string> GetFilterValuesProgramados()
        {
            return new Dictionary<string, string> { { "VEHICULO", ddlVehiculo.SelectedValue } };
        }

        /// <summary>
        /// Gets info of the maximun daily speed for the exported report.
        /// </summary>
        /// <param name="builder"></param>
		private void GenerateMaximumSpeedColumns(BaseCsvBuilder builder)
        {
            if (ReportObjectsList == null) return;

            var separator = Usuario.CsvSeparator;

            builder.GenerateRow(CultureManager.GetLabel("MAXIMAS_DIARIAS"));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("DIA"), separator, CultureManager.GetLabel("VELOCIDAD"), separator, CultureManager.GetLabel("ALCANZADA_POR"), separator));

            foreach (var o in ReportObjectsList) builder.GenerateRow(String.Concat(o.Dia, separator, o.Velocidad, separator, o.CometidoPor, separator));

            builder.GenerateRow(string.Empty);
        }

        /// <summary>
        /// Generates the CSV data of the Stadistic Iframe.
        /// </summary>
        /// <param name="builder"></param>
		private void GenerateStadisticsColumns(BaseCsvBuilder builder)
        {
            if (Stadistics == null) return;

            var separator = Usuario.CsvSeparator;

            builder.GenerateRow(CultureManager.GetLabel("DETALLE_VEHICULO"));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("RECORRIDO"), separator, CultureManager.GetLabel("TOTAL"), ":", Stadistics.KilometrosTotales, separator,
                CultureManager.GetLabel("PROMEDIO_DIARIO"), ":", Stadistics.KilometrosPromedio, separator));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("DIAS"), separator, CultureManager.GetLabel("TOTALES"), ":", Stadistics.Dias, separator,
                CultureManager.GetLabel("CON_ACTIVIDAD"), ":", Stadistics.DiasActivo, separator, CultureManager.GetLabel("SIN_ACTIVIDAD"), ":", Stadistics.Dias - Stadistics.DiasActivo, separator));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("VELOCIDAD"), separator, CultureManager.GetLabel("MAXIMA_ALCANZADA"), ":", Stadistics.VelocidadMaxima, separator,
                CultureManager.GetLabel("PROMEDIO"), ":", Stadistics.VelocidadPromedio, separator));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("TIEMPOS"), separator, CultureManager.GetLabel("MOVIMIENTO"), ":", Stadistics.HorasMovimiento, separator,
                CultureManager.GetLabel("DETENCION"), ":", Stadistics.HorasDetenido, separator, CultureManager.GetLabel("SIN_REPORTAR"), ":", Stadistics.HorasSinReportar, separator));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("INFRACCIONES"), separator, CultureManager.GetLabel("CANTIDAD"), ":", Stadistics.Infracciones, separator,
                CultureManager.GetLabel("TIEMPO"), ":", Stadistics.HsInfraccion, separator));

            builder.GenerateRow("");
        }

        /// <summary>
        /// Generates the Header With Report Filters.
        /// </summary>
        /// <param name="builder"></param>
		private void GenerateCsvHeader(BaseCsvBuilder builder)
        {
            var param = new Dictionary<string, string>
                        {
                            {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                            {CultureManager.GetEntity("PARENTI02"), ddlBase.SelectedItem.Text},
                            {CultureManager.GetEntity("PARENTI03"), ddlVehiculo.SelectedItem.Text}, 
                            {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.ToString()},
                            {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.ToString()}
                        };
            builder.GenerateHeader(CultureManager.GetMenu("STAT_RESUMEN_VEHICULAR"), param);
        }

        /// <summary>
        /// Generates the CSV data of the Mobiles Iframe.
        /// </summary>
        /// <param name="builder"></param>
		private void GenerateOperatorsColumns(BaseCsvBuilder builder)
        {
            if (MobileDrivers == null) return;

            var separator = Usuario.CsvSeparator;

            builder.GenerateRow(CultureManager.GetEntity("PARENTI09"));

            builder.GenerateRow(String.Concat(CultureManager.GetLabel("LEGAJO"), separator, CultureManager.GetLabel("NAME"), separator, CultureManager.GetEntity("TARJETA"), separator,
                CultureManager.GetLabel("INFRACCIONES"), separator, CultureManager.GetLabel("KILOMETROS"), separator, CultureManager.GetLabel("MOVIMIENTO"), separator));

            foreach (var o in MobileDrivers)
                builder.GenerateRow(String.Concat(o.Legajo, separator, o.Nombre, separator, o.Tarjeta, separator, o.Infracciones, separator, o.Kilometros, separator, o.TiempoConduccion, separator));
        }

        /// <summary>
        /// Sets the initial filter values based on query string parameters.
        /// </summary>
        private void SetInitialFilterValues()
        {
            dpDesde.SelectedDate = Desde;
            dpHasta.SelectedDate = Hasta;
            ddlDistrito.SetSelectedValue(-1);
            ddlBase.SetSelectedValue(Base);
            ddlTipoVehiculo.SetSelectedValue(VehicleType);
            ddlVehiculo.SetSelectedValue(Mobile);
        }

        /// <summary>
        /// Perfmorms custom search actions.
        /// </summary>
        private void RealizarBusqueda()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            Stadistics = ReportFactory.MobileStadisticsDAO.GetMobileStadistics(ddlVehiculo.Selected, desde, hasta);

            var showResults = Stadistics.HasActiveDays();

            if (showResults)
            {
               MobileDrivers = ReportFactory.MobileDriversDAO.GetMobileDrivers(ddlVehiculo.Selected, desde, hasta).Select(md => new MobileDriversVo(md)).ToList();

               AddResultsToSession();
            }
           
            ifDetalleVehiculo.Visible = ifConductores.Visible = showResults;

            if (NotFound != null) NotFound.Text = !showResults ? CultureManager.GetSystemMessage("NO_RESULT_FOR_CURRENT_FILTERS") : null;
        }

        /// <summary>
        /// Remembers current page data and layout before printing the report.
        /// </summary>
        protected override void OnPrePrint()
        {
            AddResultsToSession();

            Session["KeepInSession"] = true;

            ifConductoresPrint.Visible = ifDetalleVehiculoPrint.Visible = Stadistics.HasActiveDays();
        }

        #endregion
    }
}