using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Types.ValueObjects.ReportObjects;
using Logictracker.Web.CustomWebControls.Buttons;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.CustomWebControls.ToolBar;

namespace Logictracker.Reportes.DatosOperativos
{
    public partial class EstadisticaTomasDetalle : SecuredGridReportPage<MobilePositionDetailVo>
    {
        #region Private Properties
        /// <summary>
        /// Selected mobile id.
        /// </summary>
        private int Vehiculo
        {
            get
            {
                if (ViewState["Vehiculo"] == null)
                {
                    ViewState["Vehiculo"] = Session["Vehiculo"];
                    Session["Vehiculo"] = null;
                }

                return ViewState["Vehiculo"] != null ? Convert.ToInt32(ViewState["Vehiculo"]) : -1;
            }
        }

        /// <summary>
        /// Selected device id.
        /// </summary>
        private int Dispositivo
        {
            get
            {
                if (ViewState["Dispositivo"] == null)
                {
                    ViewState["Dispositivo"] = Session["Dispositivo"];
                    Session["Dispositivo"] = null;
                }

                return ViewState["Dispositivo"] != null ? Convert.ToInt32(ViewState["Dispositivo"]) : -1;
            }
        }

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "DOP_VERI_DETAILS"; } }

        protected override string GetRefference() { return "REP_TOMAS,TOMAS_MOVILES"; }

        /// <summary>
        /// Report grid.
        /// </summary>
        public override C1GridView Grid { get { return grid;} }

        /// <summary>
        /// Not found message label.
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

        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }

        protected override ResourceButton BtnSearch { get { return butonSearch; } }

        protected override C1GridView GridPrint { get { return gridPrint; } }

        protected override UpdatePanel UpdatePanelPrint { get { return upPrint; } }

        protected override Repeater PrintFilters { get { return FiltrosPrint; } }

        #endregion

        #region Protected Method

        /// <summary>
        /// Gets the object list to be bounded to the report.
        /// </summary>
        /// <returns></returns>
        protected override List<MobilePositionDetailVo> GetResults()
        {
            return ReportFactory.MobilePositionDAO.GetLastNPosition(npCount.Number, Dispositivo, Vehiculo).Select(mp => new MobilePositionDetailVo(mp)).ToList();
        }

        /// <summary>
        /// Initial report binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) Bind();

            Title = CultureManager.GetMenu("DOP_VERI_DETAILS");
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobilePositionDetailVo dataItem)
        {
            if (dataItem == null) return;

            FormatDisplayData(e, dataItem);
        }

        protected override void SelectedIndexChanged()
        {
            ShowRoute();
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Formats the givenn row invalid data.
        /// </summary>
        /// <param name="e"></param>
        private void FormatDisplayData(C1GridViewRowEventArgs e, MobilePositionDetailVo posicion)
        {
            if (posicion.Fecha.Equals(DateTime.MinValue)) 
                GridUtils.GetCell(e.Row, MobilePositionDetailVo.IndexFecha).Text = string.Empty;

            if (posicion.UltimoLogin.Equals(DateTime.MinValue)) 
                GridUtils.GetCell(e.Row, MobilePositionDetailVo.IndexUltimoLogin).Text = string.Empty;

            if (posicion.TiempoDesdeUltimoLogin.Equals(TimeSpan.Zero)) 
                GridUtils.GetCell(e.Row, MobilePositionDetailVo.IndexTiempoAUltimoLogin).Text = string.Empty;
            else
                GridUtils.GetCell(e.Row, MobilePositionDetailVo.IndexTiempoAUltimoLogin).Text = string.Format(CultureManager.GetLabel("MOVIMIENTO_SIN_EVENTOS"), posicion.TiempoDesdeUltimoLogin.Days,
                posicion.TiempoDesdeUltimoLogin.Hours, posicion.TiempoDesdeUltimoLogin.Minutes, posicion.TiempoDesdeUltimoLogin.Seconds);

            if (!posicion.UltimoLogin.Equals(DateTime.MinValue) && posicion.Chofer.Equals(string.Empty)) 
                GridUtils.GetCell(e.Row, MobilePositionDetailVo.IndexChofer).Text = CultureManager.GetLabel("REVISAR_TARJETA");
        }

        /// <summary>
        /// Add session parameters for details reports.
        /// </summary>
        private void AddSessionParameters()
        {
            var mobileId = Convert.ToInt32(Grid.SelectedDataKey[MobilePositionDetailVo.KeyIndexIdMovil]);
            var mobile = DAOFactory.CocheDAO.FindById(mobileId);

            Session.Add("PosCenterIndex", Grid.SelectedDataKey[MobilePositionDetailVo.KeyIndexIdPosicion]);
            Session.Add("TypeMobile", mobile.TipoCoche.Id);
            Session.Add("Mobile", mobile.Id);
            Session.Add("Distrito", mobile.Empresa != null ? mobile.Empresa.Id : mobile.Linea != null ? mobile.Linea.Empresa.Id : -1);
            Session.Add("Location", mobile.Linea != null ? mobile.Linea.Id : -1);
            Session.Add("InitialDate", ReportObjectsList[ReportObjectsList.Count - 1].Fecha);
            Session.Add("FinalDate", ReportObjectsList[0].Fecha);
            Session.Add("ShowMessages", 1);
            Session.Add("ShowPOIS", 1);
        }

        /// <summary>
        /// Shows the reported route of the selected item in the historic monitor.
        /// </summary>
        private void ShowRoute()
        {
            AddSessionParameters();

            OpenWin(String.Concat(ApplicationPath,"Monitor/MonitorHistorico/monitorHistorico.aspx"), "Monitor Historico");
        }

        #endregion
    }
}