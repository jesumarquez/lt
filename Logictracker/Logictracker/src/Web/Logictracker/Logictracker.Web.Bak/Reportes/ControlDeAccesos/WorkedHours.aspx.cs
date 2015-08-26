using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Web.Helpers.FussionChartHelpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Reportes.ControlDeAccesos
{
    public partial class WorkedHours : SecuredGraphReportPage<ACWorkedHours>
    {
        private int Empleado
        {
            get
            {
                if (ViewState["RepHEmpleado"] == null)
                {
                    ViewState["RepHEmpleado"] = Session["RepHEmpleado"];
                    Session["RepHEmpleado"] = null;
                }
                return (ViewState["RepHEmpleado"] != null) ? Convert.ToInt32(ViewState["RepHEmpleado"]) : 0;
            }
            set { ViewState["RepHEmpleado"] = value; }
        }
        private DateTime InitialDate
        {
            get
            {
                if (ViewState["RepHInitialDate"] == null)
                {
                    ViewState["RepHInitialDate"] = Session["RepHInitialDate"];
                    Session["RepHInitialDate"] = null;
                }
                return (ViewState["RepHInitialDate"] != null) ? Convert.ToDateTime(ViewState["RepHInitialDate"]) : DateTime.Today;
            }
            set { ViewState["RepHInitialDate"] = value; }
        }
        private DateTime FinalDate
        {
            get
            {
                if (ViewState["RepHFinalDate"] == null)
                {
                    ViewState["RepHFinalDate"] = Session["RepHFinalDate"];
                    Session["RepHFinalDate"] = null;
                }
                return (ViewState["RepHFinalDate"] != null) ? Convert.ToDateTime(ViewState["RepHFinalDate"]) : DateTime.Today.Add(new TimeSpan(23, 59, 59));
            }
            set { ViewState["RepHFinalDate"] = value; }
        }

        protected override string VariableName { get { return "AC_REP_HORAS"; } }
        protected override string GetRefference() { return "CA_MONTH_TIMERS"; }
        protected override GraphTypes GraphType { get { return GraphTypes.Barrs; } }
        protected override string XAxisLabel { get { return CultureManager.GetLabel("DIA"); } }
        protected override string YAxisLabel { get { return CultureManager.GetLabel("HORAS"); } }
        protected override bool ExcelButton { get { return true; } }

        #region Protected Methods

        protected override List<ACWorkedHours> GetResults()
        {
            var desde = SecurityExtensions.ToDataBaseDateTime(dpDesde.SelectedDate.GetValueOrDefault());
            var hasta = SecurityExtensions.ToDataBaseDateTime(dpHasta.SelectedDate.GetValueOrDefault());

            return ReportFactory.WorkedHoursDAO.GetWorkedHours(ddlEmpleado.Selected, desde, hasta).ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dpDesde.SetDate();
                dpHasta.SetDate();
            }

            SetInitialFilterValues();
        }

        protected override string GetGraphXml()
        {
            using (var helper = new FusionChartsHelper())
            {
                helper.AddConfigEntry("caption", string.Format(CultureManager.GetLabel("EMPLEADO_DESDE_HASTA"), ddlEmpleado.SelectedItem.Text, dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString(),
                                                               dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString()));

                helper.AddConfigEntry("xAxisName", XAxisLabel);
                helper.AddConfigEntry("yAxisName", YAxisLabel);
                helper.AddConfigEntry("decimalPrecision", "2");
                helper.AddConfigEntry("numberSuffix", "hs");
                helper.AddConfigEntry("limitsDecimalPrecision", "2");
                helper.AddConfigEntry("hoverCapSepChar", "-");
                helper.AddConfigEntry("rotateNames", "1");
                helper.AddConfigEntry("yAxisMaxValue", "24");
            
                foreach (var timer in ReportObjectsList)
                {
                    var item = new FusionChartsItem();

                    if (!timer.Fecha.DayOfWeek.Equals(DayOfWeek.Saturday) & !timer.Fecha.DayOfWeek.Equals(DayOfWeek.Sunday))
                        item.AddPropertyValue("color", "008ED6");

                    item.AddPropertyValue("name", String.Format("{0:dd/MM}", timer.Fecha));
                    item.AddPropertyValue("value", timer.ElapsedTime > 0 ? timer.ElapsedTime.ToString(CultureInfo.InvariantCulture) : "0.000001");
                    item.AddPropertyValue("hoverText", string.Concat(TimeSpan.FromHours(timer.ElapsedTime).ToString(), "hs "));

                    item.AddPropertyValue("link", Server.UrlEncode(string.Format(
                        "n-{0}Reportes/ControlDeAccesos/AccessControl.aspx?Emp={1}&InitialDate={2}&FinalDate={3}",
                        ApplicationPath,
                        ddlEmpleado.Selected, timer.Fecha.ToString(CultureInfo.InvariantCulture),
                        timer.Fecha.AddDays(1).ToString(CultureInfo.InvariantCulture))));

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
                categories.Add(String.Format("{0:dd/MM}", t.Fecha));
                dataset.addValue(t.ElapsedTime.ToString());
            }

            GraphCategories = categories;
            GraphDataSet = new List<FusionChartsDataset> { dataset };
        }

        protected override Dictionary<string, string> GetExcelItemList()
        {
            return ReportObjectsList.ToDictionary(t => string.Format("{0:dd/MM}", t.Fecha),
                                                  t => t.ElapsedTime.ToString());
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI02"), ddlPlanta.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI43"), ddlTipoEmpleado.SelectedItem.Text},
                           {CultureManager.GetEntity("PARENTI09"), ddlEmpleado.SelectedItem.Text},
                           {CultureManager.GetLabel("DESDE"), dpDesde.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpDesde.SelectedDate.GetValueOrDefault().ToShortTimeString()},
                           {CultureManager.GetLabel("HASTA"), dpHasta.SelectedDate.GetValueOrDefault().ToShortDateString() + " " + dpHasta.SelectedDate.GetValueOrDefault().ToShortTimeString()}
                       };
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Size.Tick = OnTick;

            if (!IsPostBack) Size.EnableTick = true;
        }

        #endregion

        #region Private Methods

        private void OnTick(object sender, EventArgs e)
        {
            if (Empleado <= 0) return;

            BtnSearchClick(sender, e);

            UpdatePanelGraph.Update();
        }

        /// <summary>
        /// Sets up initial filter values according to how the page was called.
        /// </summary>
        private void SetInitialFilterValues()
        {
            if (IsPostBack) return;

            GetQueryStringParameters();

            if (Empleado <= 0) return;

            var emp = DAOFactory.EmpleadoDAO.FindById(Empleado);

            dpDesde.SelectedDate = InitialDate;
            dpHasta.SelectedDate = FinalDate;
            ddlDistrito.SetSelectedValue(emp.Empresa != null ? emp.Empresa.Id : -1);
            ddlPlanta.SetSelectedValue(emp.Linea != null ? emp.Linea.Id : -1);
            ddlTipoEmpleado.SetSelectedValue(emp.TipoEmpleado != null ? emp.TipoEmpleado.Id : -1);
            ddlEmpleado.SetSelectedValue(Empleado);
        }

        /// <summary>
        /// Get filter initial values from query string.
        /// </summary>
        private void GetQueryStringParameters()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["Emp"])) Empleado = Convert.ToInt32(Request.QueryString["Emp"]);

            if (!string.IsNullOrEmpty(Request.QueryString["InitialDate"]))
                InitialDate = Convert.ToDateTime(Request.QueryString["InitialDate"], CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(Request.QueryString["FinalDate"]))
                FinalDate = Convert.ToDateTime(Request.QueryString["FinalDate"], CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
