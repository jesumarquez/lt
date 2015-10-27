using System;
using System.Collections.Generic;
using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.Estadistica.ResumenOperador
{
    public partial class Reportes_Estadistica_ResumenOperador_ListaMobiles : SecuredGridReportPage<OperatorMobilesVo>
    {
        #region Private Properties

        private DateTime Desde
        {
            get
            {
                if (Session["Desde"] != null)
                {
                    ViewState["Desde"] = Session["Desde"];

                    Session["Desde"] = null;
                }
                return (DateTime)ViewState["Desde"];
            }
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
                return (DateTime)ViewState["Hasta"];
            }
        }

        private int Empleado
        {
            get
            {
                if (Session["Empleado"] != null)
                {
                    ViewState["Empleado"] = Session["Empleado"];

                    Session["Empleado"] = null;
                }
                return (int)ViewState["Empleado"];
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

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        protected override ResourceButton BtnSearch { get { return null; } }

        protected override string VariableName { get { return ""; } }

        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }

        protected override UpdatePanel UpdatePanelPrint { get { return null; } }

        protected override C1GridView GridPrint { get { return null; } }

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

            Bind();
        }

        /// <summary>
        /// Gets the report data.
        /// </summary>
        /// <returns></returns>
        protected override List<OperatorMobilesVo> GetResults() { return ReportData; }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "RESUMEN_OPERADOR"; }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, OperatorMobilesVo dataItem)
        {
            GridUtils.GetCell(e.Row, OperatorMobilesVo.IndexKilometros).Text = string.Format("{0:0.00}km", dataItem.Kilometros);

            GridUtils.GetCell(e.Row, OperatorMobilesVo.IndexMovimiento).Text = string.Format(CultureManager.GetLabel("MOVIMIENTO_SIN_EVENTOS"), dataItem.Movimiento.Days,
                                                                                             dataItem.Movimiento.Hours, dataItem.Movimiento.Minutes, dataItem.Movimiento.Seconds);
        }

        protected override void SelectedIndexChanged()
        {
            AddSessionParameters();
            OpenWin(String.Concat(ApplicationPath, "Reportes/Estadistica/ResumenOperador/VehiculosPorDia.aspx"), "Vehiculos");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Property for getting the data associated to the report.
        /// </summary>
        private List<OperatorMobilesVo> ReportData
        {
            get
            {
                if (Session["Mobiles"] != null)
                {
                    ViewState["Mobiles"] = Session["Mobiles"];

                    if (Session["KeepInSes"] == null) Session["Mobiles"] = null;
                    Session["KeepInSes"] = null;
                }

                return ViewState["Mobiles"] as List<OperatorMobilesVo> ?? new List<OperatorMobilesVo>();
            }
        }

        /// <summary>
        /// Add parameters to session.
        /// </summary>
        private void AddSessionParameters()
        {
            var empleado = DAOFactory.EmpleadoDAO.FindById(Empleado);
            Session.Add("Nombre", empleado.Entidad.Descripcion);
            Session.Add("Legajo", empleado.Legajo);
            Session.Add("FechaDesde",Desde);
            Session.Add("FechaHasta",Hasta);
            var moviles = ReportFactory.OperatorMobilesByDayDAO.GetOperatorMobilesByDay(Empleado, Desde, Hasta);
            Session.Add("MovilesPorDia", moviles ?? new List<OperatorMobilesByDay>());
        }

        #endregion

    }
}
