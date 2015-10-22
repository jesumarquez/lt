#region Usings

using System;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.ReportObjects;
using Logictracker.Web.CustomWebControls.Labels;

#endregion

namespace Logictracker.Reportes.Estadistica.ResumenOperador
{
    public partial class ReportesEstadisticaResumenOperadorDetalleOperador : ApplicationSecuredPage
    {
        #region Private Properties

        /// <summary>
        /// Operator and file format string variable name.
        /// </summary>
        private const string OperadorYLegajo = "OPERADOR_Y_LEGAJO";

        /// <summary>
        /// Movement and events format string variable name.
        /// </summary>
        private const string MovimientoYEventos = "MOVIMIENTO_Y_EVENTOS";

        /// <summary>
        /// Gets report data.
        /// </summary>
        private OperatorStadistics ReportData
        {
            get
            {
                var reportData = Session["Stadistics"] == null ? null : (OperatorStadistics)Session["Stadistics"];

                if (Session["KeepInSes"] == null) Session["Stadistics"] = null;

                return reportData;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Error label message.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initial data binding.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var stadistics = ReportData;

            if (stadistics == null) return;

            var employeeType = String.IsNullOrEmpty(stadistics.TipoEmpleado) ? CultureManager.GetControl("DDL_NO_TIPOEMPLEADO") : stadistics.TipoEmpleado;

            lblOperador.Text = string.Format(CultureManager.GetLabel(OperadorYLegajo), stadistics.Nombre, employeeType, stadistics.Legajo);

            lblTotal.Text = string.Format("{0:0.00}km", stadistics.KilometrosTotales);
            lblDistanciaPromedio.Text = string.Format("{0:0.00}km", stadistics.KilometrosPromedio);
            lblDias.Text = stadistics.Dias.ToString();
            lblActivo.Text = stadistics.DiasActivo.ToString();
            lblInactivo.Text = (stadistics.Dias - stadistics.DiasActivo).ToString();
            lblAlcanzada.Text = string.Format("{0:0.00}km/h", stadistics.VelocidadMaxima);
            lblVelocidadPromedio.Text = string.Format("{0:0.00}km/h", stadistics.VelocidadPromedio);

            lblMovimiento.Text = string.Format(CultureManager.GetLabel(MovimientoYEventos), stadistics.HorasMovimiento.Days, stadistics.HorasMovimiento.Hours,
                stadistics.HorasMovimiento.Minutes, stadistics.HorasMovimiento.Seconds, stadistics.MovementEvents);

            lblDetencion.Text = string.Format(CultureManager.GetLabel(MovimientoYEventos), stadistics.HorasDetenido.Days, stadistics.HorasDetenido.Hours,
                stadistics.HorasDetenido.Minutes, stadistics.HorasDetenido.Seconds, stadistics.StoppedEvents);

            lblSinReportar.Text = string.Format(CultureManager.GetLabel(MovimientoYEventos), stadistics.HorasSinReportar.Days, stadistics.HorasSinReportar.Hours,
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
