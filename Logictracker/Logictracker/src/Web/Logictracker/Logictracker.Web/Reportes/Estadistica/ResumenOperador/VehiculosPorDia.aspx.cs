using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.Estadistica.ResumenOperador
{
    public partial class Reportes_Estadistica_ResumenOperador_VehiculosPorDia : SecuredGridReportPage<OperatorMobilesByDayVo>
    {
        #region Protected Properties

        /// <summary>
        /// Report grid.
        /// </summary>
        public override C1GridView Grid { get { return grid; } }

        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }

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

        protected override string VariableName { get { return "DETALLE_VEHICULO_DIA"; } }

        protected override ResourceButton BtnSearch { get { return null; } }

        protected override UpdatePanel UpdatePanelPrint { get { return upPrint; } }

        protected override C1GridView GridPrint { get { return gridPrint; } }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            lblEmpleado.Text = string.Format(CultureManager.GetLabel("NOMBRE_LEGAJO"), Nombre, Legajo, Desde.ToDisplayDateTime().ToShortDateString(), Hasta.ToDisplayDateTime().ToShortDateString());
        }

        /// <summary>
        /// Gets the report data.
        /// </summary>
        /// <returns></returns>
        protected override List<OperatorMobilesByDayVo> GetResults()
        {
            return ReportData.Select(rd => new OperatorMobilesByDayVo(rd)).ToList();
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "RESUMEN_OPERADOR"; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) Bind();
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {
                           {CultureManager.GetEntity("PARENTI09"), Nombre},
                           {CultureManager.GetLabel("LEGAJO"), Legajo},
                           {CultureManager.GetLabel("DESDE"), Desde.ToDisplayDateTime().ToShortDateString()},
                           {CultureManager.GetLabel("HASTA"), Hasta.ToDisplayDateTime().ToShortDateString()}
                       };
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, OperatorMobilesByDayVo dataItem)
        {
            GridUtils.GetCell(e.Row, OperatorMobilesByDayVo.IndexFecha).Text = string.Format("{0:dd-MM-yyyy}", dataItem.Fecha);
            GridUtils.GetCell(e.Row, OperatorMobilesByDayVo.IndexRecorrido).Text = string.Format("{0:0.00} km", dataItem.Recorrido);
            GridUtils.GetCell(e.Row, OperatorMobilesByDayVo.IndexVelocidadMaxima).Text = string.Format("{0:0.00} km/h", dataItem.VelocidadMaxima);
            GridUtils.GetCell(e.Row, OperatorMobilesByDayVo.IndexVelocidadPromedio).Text = string.Format("{0:0.00} km/h", dataItem.VelocidadPromedio);
            GridUtils.GetCell(e.Row, OperatorMobilesByDayVo.IndexHorasActivo).Text = string.Format("{0} hs", dataItem.HorasActivo);
        }

        protected override void SelectedIndexChanged()
        {
            ShowRoute();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Property for getting the data associated to the report.
        /// </summary>
        private List<OperatorMobilesByDay> ReportData
        {
            get
            {
                if (Session["MovilesPorDia"] != null)
                {
                    ViewState["MovilesPorDia"] = Session["MovilesPorDia"];

                    Session["MovilesPorDia"] = null;
                }

                return ViewState["MovilesPorDia"] as List<OperatorMobilesByDay> ?? new List<OperatorMobilesByDay>();
            }
        }

        private string Nombre
        {
            get
            {
                if (Session["Nombre"] != null)
                {
                    ViewState["Nombre"] = Session["Nombre"];

                    Session["Nombre"] = null;
                }

                return ViewState["Nombre"] as string;
            }
        }

        private string Legajo
        {
            get
            {
                if (Session["Legajo"] != null)
                {
                    ViewState["Legajo"] = Session["Legajo"];

                    Session["Legajo"] = null;
                }

                return ViewState["Legajo"] as string;
            }
        }

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

        /// <summary>
        /// Add session parameters for details reports.
        /// </summary>
        private void AddSessionParameters()
        {
            var mobileId = Convert.ToInt32(grid.SelectedDataKey[0]);
            var desde = Convert.ToDateTime(GridUtils.GetCell(grid.SelectedRow, OperatorMobilesByDayVo.IndexFecha).Text);
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
