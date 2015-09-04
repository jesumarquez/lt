#region Usings

using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Reportes.Estadistica.ResumenVehicular
{
    public partial class AccidentologiaResumenVehicularDetalleVehiculo : ApplicationSecuredPage
    {
        #region Private Properties

        /// <summary>
        /// Intern and device format string variable name.
        /// </summary>
        private const string INTERNO_Y_DISPOSITIVO = "INTERNO_Y_DISPOSITIVO";

        /// <summary>
        /// Movement and events format string variable name.
        /// </summary>
        private const string MOVIMIENTO_Y_EVENTOS = "MOVIMIENTO_Y_EVENTOS";

        #endregion

        #region Protected Properties

        /// <summary>
        /// Error label message.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        /// <summary>
        /// Gets report data.
        /// </summary>
        protected MobileStadistics ReportData
        {
            get
            {
                var reportData = Session["Stadistics"] == null ? null : (MobileStadistics) Session["Stadistics"];

                if (Session["KeepInSession"] == null) Session["Stadistics"] = null;

                return reportData;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            var stadistics = ReportData;

            base.OnLoad(e);

            if (stadistics == null) return;

            lblInterno.Text = string.Format(CultureManager.GetLabel(INTERNO_Y_DISPOSITIVO), stadistics.Interno, stadistics.Patente, stadistics.TipoVehiculo, stadistics.Dispositivo);

            lblTotal.Text = string.Format("{0:0.00}km", stadistics.KilometrosTotales);
            lblDistanciaPromedio.Text = string.Format("{0:0.00}km", stadistics.KilometrosPromedio);
            lblDias.Text = stadistics.Dias.ToString();
            lblActivo.Text = stadistics.DiasActivo.ToString();
            lblInactivo.Text = (stadistics.Dias - stadistics.DiasActivo).ToString();
            lblAlcanzada.Text = string.Format("{0}km/h", stadistics.VelocidadMaxima);
            lblVelocidadPromedio.Text = string.Format("{0}km/h", stadistics.VelocidadPromedio);

            lblMovimiento.Text = string.Format(CultureManager.GetLabel(MOVIMIENTO_Y_EVENTOS), stadistics.HorasMovimiento.Days, stadistics.HorasMovimiento.Hours,
                stadistics.HorasMovimiento.Minutes, stadistics.HorasMovimiento.Seconds, stadistics.MovementEvents);

            lblDetencion.Text = string.Format(CultureManager.GetLabel(MOVIMIENTO_Y_EVENTOS), stadistics.HorasDetenido.Days, stadistics.HorasDetenido.Hours,
                stadistics.HorasDetenido.Minutes, stadistics.HorasDetenido.Seconds, stadistics.StoppedEvents);

            lblSinReportar.Text = string.Format(CultureManager.GetLabel(MOVIMIENTO_Y_EVENTOS), stadistics.HorasSinReportar.Days, stadistics.HorasSinReportar.Hours,
                stadistics.HorasSinReportar.Minutes, stadistics.HorasSinReportar.Seconds, stadistics.NoReportEvents);
        
            lblInfraccion.Text = stadistics.HorasInfraccion.ToString();
            lblInfracciones.Text = stadistics.Infracciones.ToString();
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "RESUMEN_VEHICULAR"; }

        #endregion
    }
}