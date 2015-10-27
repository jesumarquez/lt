using System;
using System.Collections.Generic;
using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.Estadistica.ResumenVehicular
{
    public partial class ReportesEstadisticaResumenVehicularConductoresPorDia : SecuredGridReportPage<MobileDriversByDayVo>
    {
        #region Protected Properties

        /// <summary>
        /// Report grid.
        /// </summary>
        public override C1GridView Grid { get { return grid; } }

        protected override UpdatePanel UpdatePanelGrid{get{return updGrid;}}

        /// <summary>
        /// Not found label message.
        /// </summary>
        protected override InfoLabel NotFound { get { return infoLabel1; } }

        /// <summary>
        /// C1 web tool bar.
        /// </summary>
        protected override ToolBar ToolBar { get { return ToolBar1; } }

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        protected override string VariableName { get { return "DETALLE_CHOFERES_DIA"; } }

        protected override ResourceButton BtnSearch{get{return null;}}

        protected override UpdatePanel UpdatePanelPrint { get { return upPrint; } }

        protected override C1GridView GridPrint { get { return gridPrint; } }

        /// <summary>
        /// Gets the report data.
        /// </summary>
        /// <returns></returns>
        protected override List<MobileDriversByDayVo> GetResults() { return ReportData; }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            lblInterno.Text = string.Format(CultureManager.GetLabel("INTERNO_PATENTE_RESPONSABLE"), Movil, Patente, Responsable, Desde.ToDisplayDateTime().ToShortDateString(), Hasta.ToDisplayDateTime().ToShortDateString());
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetLabel("INTERNO"), Movil},
                           {CultureManager.GetLabel("PATENTE"), Patente},
                           {CultureManager.GetLabel("RESPONSABLE"), Responsable},
                           {CultureManager.GetLabel("DESDE"), Desde.ToDisplayDateTime().ToShortDateString()},
                           {CultureManager.GetLabel("HASTA"), Hasta.ToDisplayDateTime().ToShortDateString()}
                       };
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "RESUMEN_VEHICULAR"; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) Bind();
        }

        protected override void SelectedIndexChanged()
        {
            ShowRoute();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobileDriversByDayVo dataItem)
        {
            GridUtils.GetCell(e.Row, MobileDriversByDayVo.IndexFecha).Text = string.Format("{0:dd-MM-yyyy}", dataItem.Fecha);
        }
 
        ///// <summary>
        ///// Adds mobile stadistics as header parameters.
        ///// </summary>
        //protected override Dictionary<string, string> GetHeaderParameters()
        //{
        //    if (ReportData == null) return null;

        //    var parameters = new Dictionary<string, string>
        //                             {
        //                                 {"Movil", Movil},
        //                                 {"Patente", Patente},
        //                                 {"Desde", Desde.ToDisplayDateTime().ToString()},
        //                                 {"Hasta", Hasta.ToDisplayDateTime().ToString()},
        //                                 {"Responsable", Responsable}
        //                             };
        //    return parameters;
        //}

        #endregion

        #region Private Methods

        /// <summary>
        /// Property for getting the data associated to the report.
        /// </summary>
        private List<MobileDriversByDayVo> ReportData
        {
            get
            {
                if (Session["ChoferesPorDia"] != null)
                {
                    ViewState["ChoferesPorDia"] = Session["ChoferesPorDia"];

                    Session["ChoferesPorDia"] = null;
                }

                return ViewState["ChoferesPorDia"] as List<MobileDriversByDayVo> ?? new List<MobileDriversByDayVo>();
            }
        }

        private string Responsable
        {
            get
            {
                if (Session["ConductoresResponsable"] != null)
                {
                    ViewState["ConductoresResponsable"] = Session["ConductoresResponsable"];

                    Session["ConductoresResponsable"] = null;
                }

                return ViewState["ConductoresResponsable"] as string;
            }
        }

        private string Movil
        {
            get
            {
                if (Session["ConductoresInterno"] != null)
                {
                    ViewState["ConductoresInterno"] = Session["ConductoresInterno"];

                    Session["ConductoresInterno"] = null;
                }

                return ViewState["ConductoresInterno"] as string;
            }
        }

        private string Patente
        {
            get
            {
                if (Session["ConductoresPatente"] != null)
                {
                    ViewState["ConductoresPatente"] = Session["ConductoresPatente"];

                    Session["ConductoresPatente"] = null;
                }

                return ViewState["ConductoresPatente"] as string;
            }
        }

        private DateTime Desde
        {
            get
            {
                if (Session["ConductoresDesde"] != null)
                {
                    ViewState["ConductoresDesde"] = Session["ConductoresDesde"];

                    Session["ConductoresDesde"] = null;
                }
                return (DateTime)ViewState["ConductoresDesde"];
            }
        }

        private DateTime Hasta
        {
            get
            {
                if (Session["ConductoresHasta"] != null)
                {
                    ViewState["ConductoresHasta"] = Session["ConductoresHasta"];

                    Session["ConductoresHasta"] = null;
                }
                return (DateTime)ViewState["ConductoresHasta"];
            }
        }

        /// <summary>
        /// Add session parameters for details reports.
        /// </summary>
        private void AddSessionParameters()
        {
            var mobileId = Convert.ToInt32(grid.SelectedDataKey[0]);
            var desde = Convert.ToDateTime(GridUtils.GetCell(grid.SelectedRow, MobileDriversByDayVo.IndexFecha).Text);
            var mobile = DAOFactory.CocheDAO.FindById(mobileId);

            Session.Add("TypeMobile", mobile.TipoCoche.Id);
            Session.Add("Mobile", mobile.Id);
            Session.Add("Distrito", mobile.Empresa != null ? mobile.Empresa.Id : mobile.Linea != null ? mobile.Linea.Empresa.Id : -1);
            Session.Add("Location", mobile.Linea != null ? mobile.Linea.Id : -1);
            Session.Add("InitialDate", desde);
            Session.Add("FinalDate", desde.AddDays(1));
            Session.Add("ShowMessages", 1);
            Session.Add("ShowPOIS", 1);
        }

        /// <summary>
        /// Shows the reported route of the selected item in the historic monitor.
        /// </summary>
        private void ShowRoute()
        {
            AddSessionParameters();

            OpenWin(String.Concat(ApplicationPath, "Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        #endregion
    }
}