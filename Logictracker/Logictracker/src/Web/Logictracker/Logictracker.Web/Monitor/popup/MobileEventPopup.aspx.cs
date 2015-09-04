#region Usings

using System;
using Logictracker.Services.Helpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Security;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Monitor.popup
{
    public partial class Monitor_MonitorHistorico_popup_MobileEventPopup : ApplicationSecuredPage
    {
        #region Protected Properties

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return null; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Message data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var id = !string.IsNullOrEmpty(Request.QueryString["id"]) ? Convert.ToInt32(Request.QueryString["id"]) : 0;

            var message = DAOFactory.LogMensajeDAO.FindById(id);

            if (message == null) return;

            lblCoche.Text = message.Coche.Interno;
            lblMensaje.Text = message.Texto;

            lblCorner.Text = GeocoderHelper.GetDescripcionEsquinaMasCercana(message.Latitud, message.Longitud);
        
            lblFecha.Text = string.Concat(message.Fecha.ToDisplayDateTime().ToShortDateString(), " ", message.Fecha.ToDisplayDateTime().TimeOfDay.ToString());

            lblLink.Visible = message.TieneFoto;
            if (lblLink.Visible)
            {
                lblLink.Text = @"<div class=""withPhoto""></div>";

                const string link = "window.open('../../Common/Pictures?e={0}', 'Fotos_{0}', 'width=345,height=408,directories=no,location=no,menubar=no,resizable=no,scrollbars=no,status=no,toolbar=no');";

                lblLink.Attributes.Add("onclick", string.Format(link, message.Id));
            }
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "MONITOR_HISTORICO,MONITOR_CALIDAD"; }

        #endregion
    }
}
