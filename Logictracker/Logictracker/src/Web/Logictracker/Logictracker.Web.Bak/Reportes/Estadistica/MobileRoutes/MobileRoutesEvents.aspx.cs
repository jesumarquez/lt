using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.Estadistica.MobileRoutes
{
    public partial class EstadisticaMobileRoutesMobileRoutesEvents : SecuredGridReportPage<MobileRouteEventVo>
    {
        #region Protected Properties

        protected override InfoLabel LblInfo { get { return infoLabel1; } }
        protected override ResourceButton BtnSearch { get { return null; } }
        protected override string VariableName { get { return ""; } }
        protected override System.Web.UI.UpdatePanel UpdatePanelGrid { get { return updGrid; } }
        protected override System.Web.UI.UpdatePanel UpdatePanelPrint { get { return upPrint; } }
        protected override C1GridView GridPrint { get { return gridPrint; } }
        public override C1GridView Grid { get { return grid; } }
        protected override InfoLabel NotFound { get { return infoLabel1; } }
        protected override ToolBar ToolBar { get { return null; } }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets from sessio or viewstate the data to show.
        /// </summary>
        private List<MobileEvent> ReportData
        {
            get
            {
                if (Session["RouteFragmentEvents"] != null)
                {
                    ViewState["RouteFragmentEvents"] = Session["RouteFragmentEvents"];

                    if (Session["KeepInSession"] == null) Session["RouteFragmentEvents"] = null;
                    Session["KeepInSession"] = null;
                }

                return (List<MobileEvent>) ViewState["RouteFragmentEvents"] ?? new List<MobileEvent>();
            }
        }

        /// <summary>
        /// Gets the district to wich the mobile is asigned.
        /// </summary>
        private int District
        {
            get
            {
                if (ViewState["District"] == null)
                {
                    ViewState["District"] = Session["RouteFragmentDistrict"];
                    Session["RouteFragmentDistrict"] = null;
                }

                return ViewState["District"] == null ? 0 : Convert.ToInt32(ViewState["District"]);
            }
        }

        /// <summary>
        /// Gets the location to wich the mobile is asigned.
        /// </summary>
        private int Location
        {
            get
            {
                if (ViewState["Location"] == null)
                {
                    ViewState["Location"] = Session["RouteFragmentLocation"];
                    Session["RouteFragmentLocation"] = null;
                }

                return ViewState["Location"] == null ? 0 : Convert.ToInt32(ViewState["Location"]);
            }
        }

        /// <summary>
        /// Gets the route fragment mobile.
        /// </summary>
        private int Mobile
        {
            get
            {
                if (ViewState["Mobile"] == null)
                {
                    ViewState["Mobile"] = Session["RouteFragmentMobile"];
                    Session["RouteFragmentMobile"] = null;
                }

                return ViewState["Mobile"] == null ? 0 : Convert.ToInt32(ViewState["Mobile"]);
            }
        }

        /// <summary>
        /// Gets the vehicle T associated to the mobile.
        /// </summary>
        private int MobileType
        {
            get
            {
                if (ViewState["MobileType"] == null)
                {
                    ViewState["MobileType"] = Session["RouteFragmentMobileType"];
                    Session["RouteFragmentMobileType"] = null;
                }

                return ViewState["MobileType"] == null ? 0 : Convert.ToInt32(ViewState["MobileType"]);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Displays report results.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Bind();
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            var isPrinting = !String.IsNullOrEmpty(Request.QueryString["IsPrinting"]) ? Convert.ToBoolean(Request.QueryString["IsPrinting"]) : false;

            if (isPrinting) grid.SkinID = "PrintGrid";
        }

        /// <summary>
        /// Gets report data.
        /// </summary>
        /// <returns></returns>
        protected override List<MobileRouteEventVo> GetResults() { return ReportData.Select(rd => new MobileRouteEventVo(rd)).ToList(); }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "MOBILE_ROUTES"; }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileRouteEventVo dataItem)
        {
            GridUtils.GetCell(e.Row, MobileRouteEventVo.IndexIconUrl).Text = string.Format("<img src='{0}' />", IconDir + dataItem.IconUrl);
        }

        protected override void SelectedIndexChanged()
        {
            DisplayEvent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calls the historic monitor with the appropiate filter values for displaying the current event.
        /// </summary>
        private void DisplayEvent()
        {
            var id = Convert.ToInt32(Grid.SelectedDataKey[0]);

            var eventTime = Convert.ToDateTime(GridUtils.GetCell(Grid.SelectedRow, MobileRouteEventVo.IndexEventTime).Text);
            var duration = Convert.ToDateTime(GridUtils.GetCell(Grid.SelectedRow, MobileRouteEventVo.IndexDuration).Text).TimeOfDay.TotalMinutes;

            Session.Add("Distrito", District);
            Session.Add("Location", Location);
            Session.Add("TypeMobile", MobileType);
            Session.Add("Mobile", Mobile);
            Session.Add("InitialDate", eventTime.AddMinutes(-5));
            Session.Add("FinalDate", eventTime.AddMinutes(5 + duration));
            Session.Add("MessageCenterIndex", id);
            Session.Add("ShowMessages", 0);
            Session.Add("ShowPOIS", 0);

            OpenWin(String.Concat(ApplicationPath,"Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        #endregion
    }
}