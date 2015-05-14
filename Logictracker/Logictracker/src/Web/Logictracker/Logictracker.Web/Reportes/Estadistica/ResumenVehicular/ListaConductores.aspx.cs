using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.Estadistica.ResumenVehicular
{
    public partial class AccidentologiaResumenVehicularListaConductores : SecuredGridReportPage<MobileDriversVo>
    {
        #region Private Properties

        private DateTime Desde
        {
            get
            {
                if (Session["FechaDesde"] != null)
                {
                    ViewState["FechaDesde"] = Session["FechaDesde"];

                    Session["FechaDesde"] = null;
                }
                return (DateTime)ViewState["FechaDesde"];
            }
        }

        private DateTime Hasta
        {
            get
            {
                if (Session["FechaHasta"] != null)
                {
                    ViewState["FechaHasta"] = Session["FechaHasta"];

                    Session["FechaHasta"] = null;
                }
                return (DateTime)ViewState["FechaHasta"];
            }
        }

        private int Mobile
        {
            get
            {
                if (Session["ResumenMobile"] != null)
                {
                    ViewState["ResumenMobile"] = Session["ResumenMobile"];

                    Session["ResumenMobile"] = null;
                }
                return ViewState["ResumenMobile"] != null ? (int)ViewState["ResumenMobile"] : 0;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Report grid.
        /// </summary>
        public override C1GridView Grid { get { return grid; } }

        /// <summary>
        /// Not found label message.
        /// </summary>
        protected override InfoLabel NotFound { get { return null; } }

        /// <summary>
        /// C1 web tool bar.
        /// </summary>
        protected override ToolBar ToolBar { get { return null; } }

        protected override ResourceButton BtnSearch { get { return null; } }

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        protected override UpdatePanel UpdatePanelGrid{get{return updGrid;}}

        protected override UpdatePanel UpdatePanelPrint { get { return null; } }

        protected override C1GridView GridPrint{get{return null;}}

        protected override string VariableName{get { return ""; }}

        #endregion

        #region Protected Methods

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            var isPrinting = !String.IsNullOrEmpty(Request.QueryString["IsPrinting"]) && Convert.ToBoolean(Request.QueryString["IsPrinting"]);

            if (isPrinting) grid.SkinID = "PrintGrid";
        }


        /// <summary>
        /// Report initial data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(IsPostBack) return;

            Bind();
        }

        /// <summary>
        /// Gets the report data.
        /// </summary>
        /// <returns></returns>
        protected override List<MobileDriversVo> GetResults() { return ReportData; }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "RESUMEN_VEHICULAR"; }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileDriversVo dataItem)
        {
            GridUtils.GetCell(e.Row,MobileDriversVo.IndexKilometros).Text = string.Format("{0:0.00}km", dataItem.Kilometros);

            GridUtils.GetCell(e.Row,MobileDriversVo.IndexTiempoConduccion).Text = string.Format(CultureManager.GetLabel("MOVIMIENTO_SIN_EVENTOS"), dataItem.TiempoConduccion.Days,
                dataItem.TiempoConduccion.Hours, dataItem.TiempoConduccion.Minutes, dataItem.TiempoConduccion.Seconds); 
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParameters();
            OpenWin(String.Concat(ApplicationPath, "Reportes/Estadistica/ResumenVehicular/ConductoresPorDia.aspx"), "Conductores");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add parameters to session.
        /// </summary>
        private void AddSessionParameters()
        {
            if (Mobile <= 0) return;

            var coche = DAOFactory.CocheDAO.FindById(Mobile);

            Session.Add("ConductoresInterno", coche.Interno);
            Session.Add("ConductoresPatente", coche.Patente);
            Session.Add("ConductoresResponsable", coche.Chofer != null ? coche.Chofer.Entidad.Descripcion : "Sin Responsable");
            Session.Add("ConductoresDesde",Desde);
            Session.Add("ConductoresHasta",Hasta);

            var conductores = ReportFactory.MobileDriversByDayDAO.GetMobileDriversByDay(Mobile, Desde, Hasta).Select(m => new MobileDriversByDayVo(m)).ToList();

            Session.Add("ChoferesPorDia", conductores);
        }

        /// <summary>
        /// Property for getting the data associated to the report.
        /// </summary>
        private List<MobileDriversVo> ReportData
        {
            get
            {
                if (Session["Drivers"] != null)
                {
                    ViewState["Drivers"] = Session["Drivers"];

                    if (Session["KeepInSession"] == null) Session["Drivers"] = null;
                    Session["KeepInSession"] = null;
                }

                return ViewState["Drivers"] as List<MobileDriversVo> ?? new List<MobileDriversVo>();
            }
        }

        #endregion
    }
}