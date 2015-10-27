#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Messages.Sender;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.ValueObjects.Dispositivos;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionParametrosDispositivoLista : SecuredListPage<TipoParametroDispositivoVo>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_PARAM_DISPOSITIVOS"; } }
        protected override string RedirectUrl { get { return "ParametrosDispositivoAlta.aspx"; } }
        protected override string GetRefference() { return "PARAMETROS_DISPOSITIVO"; }

        #endregion

        #region Protected Methods

        protected override List<TipoParametroDispositivoVo> GetListData()
        {
            return DAOFactory.TipoParametroDispositivoDAO.FindByTipoDispositivo(ddlTipoDispositivo.Selected)
                .OfType<TipoParametroDispositivo>().Select(p => new TipoParametroDispositivoVo(p)).ToList();
        }

        /// <summary>
        /// Resset all devices configuration to its type parameters values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnResetDevices_Click(object sender, EventArgs e)
        {
            try
            {
                var deviceType = DAOFactory.TipoDispositivoDAO.FindById(ddlTipoDispositivo.Selected);
                var devices = DAOFactory.DispositivoDAO.GetByTipo(deviceType);

                foreach (var device in devices)
                {
                    DAOFactory.DispositivoDAO.ResetConfiguration(device);
                    if (device.DetallesDispositivo.Cast<DetalleDispositivo>().Any(detail => detail.TipoParametro.RequiereReset))
                        MessageSender.CreateReboot(device, null);
                }

                ShowInfo(CultureManager.GetLabel("DEVICE_RESET_OK"));
            }
            catch { ShowError(new Exception(CultureManager.GetError("DEVICE_RESET_ERROR"))); }
        }

        #endregion

        protected override void OnLoadFilters(FilterData data)
        {
            var tipo = data[FilterData.StaticTipoDispositivo];
            if (tipo != null) ddlTipoDispositivo.SetSelectedValue((int)tipo);
        }

        protected override FilterData GetFilters(FilterData data)
        {
            data.AddStatic(FilterData.StaticTipoDispositivo, ddlTipoDispositivo.Selected);
            return data;
        }
    }
}
