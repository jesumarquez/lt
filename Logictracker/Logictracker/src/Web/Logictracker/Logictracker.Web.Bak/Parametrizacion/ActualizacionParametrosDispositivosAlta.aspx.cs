#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.Messages.Sender;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Utils;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionActualizacionParametrosDispositivosAlta : SecuredAbmPage<TipoParametroDispositivo>
    {
        #region Private Constants

        /// <summary>
        /// Columns constant values.
        /// </summary>
        private const int Device = 0;

        #endregion

        #region Protected Properties

        /// <summary>
        /// Report Name.
        /// </summary>
        protected override string VariableName  { get { return "PAR_ACTUALIZACION_PARAMETROS"; } }

        /// <summary>
        /// Associated list page.
        /// </summary>
        protected override string RedirectUrl { get { return "ActualizacionParametrosDispositivosLista.aspx"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            BindGrid();
        }

        /// <summary>
        /// Toggles selected devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkDispositivosActualizar_Click(object sender, EventArgs e)
        {
            lbDispositivos.ToogleItems();

            BindGrid();
        }

        /// <summary>
        /// Formats data for display.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridParametros_RowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            var details = e.Row.DataItem as DetalleDispositivo;

            if (details == null) return;

            e.Row.Cells[Device].Text = details.Dispositivo.Codigo;
        }

        /// <summary>
        /// Rebinds grid for the new sort expression.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gridParametros_SortingCommand(object sender, C1GridViewSortEventArgs e)
        {
            var preview = GetPreviewData();

            preview.Sort(new ObjectComparer<DetalleDispositivo>(e.SortExpression, e.SortDirection == C1SortDirection.Descending));

            gridParametros.DataSource = preview;
            gridParametros.DataBind();
        }

        /// <summary>
        /// Rebinds devices current data preview for the newly selected devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbDispositivos_SelectedIndexChanged(object sender, EventArgs e) { BindGrid(); }

        /// <summary>
        /// Rebinds preview data for the newly selected device type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void filters_SelectedIndexChanged(object sender, EventArgs e) { tblPreview.Visible = false; }

        /// <summary>
        /// Adds only the buttons needed for this page functionality.
        /// </summary>
        protected override void AddToolBarIcons()
        {
            var module = Module;

            if (module.Edit) ToolBar.AddSaveToolbarButton();

            ToolBar.AddListToolbarButton();
        }

        /// <summary>
        /// Displays data to be edited.
        /// </summary>
        protected override void Bind()
        {
            lblParametro.Text = EditObject.Nombre;
            lblTipoDato.Text = EditObject.TipoDato;
            lblValor.Text = EditObject.ValorInicial;
        }

        /// <summary>
        /// Sends the new parameter value to all selected devices.
        /// </summary>
        protected override void OnSave()
        {
            var value = txtValorEnviar.Text.Trim();
            var name = EditObject.Nombre;

            var dispositivoDao = DAOFactory.DispositivoDAO;

            var devices = lbDispositivos.SelectedValues.Select(id => dispositivoDao.FindById(id)).ToList();

            foreach (var device in devices)
            {
                var parametro = dispositivoDao.UpdateDeviceParameter(device, name, value);
                if (parametro != null && parametro.TipoParametro.RequiereReset)
                    MessageSender.CreateReboot(device, null).Send();
            }
        }

        /// <summary>
        /// This method is not used for this page.
        /// </summary>
        protected override void OnDelete() { }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "PAR_ACTUALIZACION_PARAMETROS"; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the current value of the parameter for the selected devices.
        /// </summary>
        /// <returns></returns>
        private List<DetalleDispositivo> GetPreviewData()
        {
            return (from DetalleDispositivo detalle in DAOFactory.DetalleDispositivoDAO.GetDevicesDetail(lbDispositivos.SelectedValues, new List<int> {EditObject.Id})
                    orderby detalle.Dispositivo.Codigo
                    select detalle).ToList();
        }

        /// <summary>
        /// Displays the current value of the selected devices to use as a refference.
        /// </summary>
        private void BindGrid()
        {
            tblPreview.Visible = !lbDispositivos.SelectedValues.Contains(0);

            if (!tblPreview.Visible) return;

            gridParametros.DataSource = GetPreviewData();
            gridParametros.DataBind();
        }

        #endregion
    }
}