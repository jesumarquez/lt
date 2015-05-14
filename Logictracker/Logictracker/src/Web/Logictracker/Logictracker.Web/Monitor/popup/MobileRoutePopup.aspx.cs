#region Usings

using System;
using System.Globalization;
using Logictracker.Services.Helpers;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Monitor.popup
{
    public partial class MonitorPopupMobileRoutePopup : ApplicationSecuredPage
    {
        #region Protected Properties

        /// <summary>
        /// Error message label.
        /// </summary>
        protected override InfoLabel LblInfo { get { return null; } }

        #endregion

        #region ProtectedMethods

        /// <summary>
        /// Initial data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!string.IsNullOrEmpty(Request.QueryString["latitude"]) && !string.IsNullOrEmpty(Request.QueryString["longitude"]))
            {
                var latitude = Convert.ToDouble(Request.QueryString["latitude"], CultureInfo.InvariantCulture);
                var longitude = Convert.ToDouble(Request.QueryString["longitude"], CultureInfo.InvariantCulture);

                lblEsquina.Text = GeocoderHelper.GetDescripcionEsquinaMasCercana(latitude, longitude);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["inicio"])) lblInicio.Text = Request.QueryString["inicio"];
            if (!string.IsNullOrEmpty(Request.QueryString["duracion"])) lblDuracion.Text = Request.QueryString["duracion"];
            if (!string.IsNullOrEmpty(Request.QueryString["distancia"])) lblDistancia.Text = string.Concat(Request.QueryString["distancia"], "km");

            if (!string.IsNullOrEmpty(Request.QueryString["velocidadMinima"])) lblVelocidadMinima.Text = string.Concat(Request.QueryString["velocidadMinima"], "km/h");
            if (!string.IsNullOrEmpty(Request.QueryString["velocidadMaxima"])) lblVelocidadMaxima.Text = string.Concat(Request.QueryString["velocidadMaxima"], "km/h");
            if (!string.IsNullOrEmpty(Request.QueryString["velocidadPromedio"])) lblVelocidadPromedio.Text = string.Concat(Convert.ToInt32(Request.QueryString["velocidadPromedio"]), " km/h");
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "MONITOR_HISTORICO"; }

        #endregion
    }
}
