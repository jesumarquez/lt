using System;
using System.Collections.Generic;
using System.Drawing;
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
    public partial class ReportesTomasGps : SecuredGridReportPage<MobilePositionVo> 
    {
        #region Protected Properties

        public override int PageSize { get { return 10000; } }
        public override C1GridView Grid { get { return grid; } }
        protected override string VariableName { get { return "DOP_VERI_DISPOSITIVO"; } }
        protected override string GetRefference() { return "REP_TOMAS"; }
        protected override bool ExcelButton { get { return true; } }

        protected override InfoLabel NotFound { get { return infoLabel1; } }
        protected override ToolBar ToolBar { get { return ToolBar1; } }
        protected override InfoLabel LblInfo { get { return infoLabel1; } }
        protected override ResourceButton BtnSearch { get { return btnActualizar; } }
        protected override UpdatePanel UpdatePanelGrid { get { return updGrid; } }
        protected override C1GridView GridPrint { get { return gridPrint; } }
        protected override UpdatePanel UpdatePanelPrint { get { return upPrint; } }
        protected override Repeater PrintFilters { get { return FiltrosPrint; } }
        public override string SearchString { get { return txtBuscar.Text; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack) Bind();
        }

        protected override void OnRowDataBound(C1GridView grid, C1GridViewRowEventArgs e, MobilePositionVo dataItem)
        {
            SetBackgroundColor(e);

            FormatDisplayData(e);
        }

        /// <summary>
        /// Gets the object list to be bounded to the report.
        /// </summary>
        /// <returns></returns>
        protected override List<MobilePositionVo> GetResults()
        {
            var linea = cbLinea.Selected;
            var empresa = ddlDistrito.Selected > 0 ? ddlDistrito.Selected : cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected).Empresa.Id : -1;

            var devices = DAOFactory.DispositivoDAO.GetList(new[]{empresa}, new[]{linea}, new[]{ddlTipoDispositivo.Selected});

            var coches = devices.Select(d => DAOFactory.CocheDAO.FindMobileByDevice(d.Id)).Where(c => c != null);
            return ReportFactory.MobilePositionDAO.GetMobilesLastPosition(coches).Select(mp => new MobilePositionVo(mp)).ToList();
        }

        protected override void SelectedIndexChanged()
        {
            var fechaUltimoReporte = GridUtils.GetCell(Grid.SelectedRow, MobilePositionVo.IndexFecha).Text;
            if (!string.IsNullOrEmpty(fechaUltimoReporte)) ShowPositionsDetails();
            else AddNoDetailsScript();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets row background color depending on the report state of each mobile.
        /// </summary>
        /// <param name="e"></param>
        private void SetBackgroundColor(C1GridViewRowEventArgs e)
        {
            var estado = Convert.ToInt32(GridUtils.GetCell(e.Row, MobilePositionVo.IndexEstadoReporte).Text);

            switch (estado)
            {
                case 0: e.Row.BackColor = Color.LightGreen; break;
                case 1: e.Row.BackColor = Color.Yellow; break;
                case 2: e.Row.BackColor = Color.LightCoral; break;
                default: e.Row.BackColor = Color.LightGray; break;
            }
        }

        /// <summary>
        /// Formats the givenn row invalid data.
        /// </summary>
        /// <param name="e"></param>
        private void FormatDisplayData(C1GridViewRowEventArgs e)
        {
            var posicion = e.Row.DataItem as MobilePositionVo;

            if (posicion == null) return;

            GridUtils.GetCell(e.Row, MobilePositionVo.IndexEstadoReporte).Text = GetReportStatusDescirtion(posicion.EstadoReporte);

            if (!posicion.Fecha.HasValue)
                GridUtils.GetCell(e.Row, MobilePositionVo.IndexFecha).Text = string.Empty;
        }

        /// <summary>
        /// Gets the mobile report status code description.
        /// </summary>
        /// <param name="estadoReporte"></param>
        /// <returns></returns>
        private static string GetReportStatusDescirtion(int estadoReporte)
        {
            switch (estadoReporte)
            {
                case 0: return CultureManager.GetLabel("REPORTANDO");
                case 1: return CultureManager.GetLabel("ACTIVO");
                case 2: return CultureManager.GetLabel("INACTIVO");
            }

            return CultureManager.GetLabel("SIN_REPORTAR");
        }

        /// <summary>
        /// Add parameter to session.
        /// </summary>
        private void AddSessionParameters()
        {
            Session.Add("Dispositivo", grid.SelectedDataKey.Values[0]);
            Session.Add("Vehiculo", grid.SelectedDataKey.Values[1]);
        }

        /// <summary>
        /// Adds no details script to alert user that the selected mobile does not have any reported position.
        /// </summary>
        private void AddNoDetailsScript()
        {
            var dispositivo = GridUtils.GetCell(Grid.SelectedRow, MobilePositionVo.IndexDispositivo).Text;

            var noDetailsScript = string.Format(string.Concat("alert('", CultureManager.GetSystemMessage("NO_POSITIONS_DEVICE"), "');"), dispositivo);

            ScriptManager.RegisterStartupScript(this, typeof(string), "NoDetails", noDetailsScript, true);
        }

        /// <summary>
        /// Shows the reporting details for the selected mobile.
        /// </summary>
        private void ShowPositionsDetails()
        {
            AddSessionParameters();

            OpenWin("TomasDetalle.aspx", "Detalle posiciones");
        }

        protected override Dictionary<string, string> GetFilterValues()
        {
            return new Dictionary<string, string>
                       {  
                           {CultureManager.GetEntity("PARENTI01"), ddlDistrito.SelectedItem.Text }, 
                           {CultureManager.GetEntity("PARENTI02"), cbLinea.SelectedItem.Text },
                           {CultureManager.GetEntity("PARENTI32"), ddlTipoDispositivo.SelectedItem.Text }
                       };
        }

        #endregion
    }
}
