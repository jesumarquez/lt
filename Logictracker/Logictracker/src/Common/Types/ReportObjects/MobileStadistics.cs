#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents mobile activities stadistics.
    /// </summary>
    [Serializable]
    public class MobileStadistics
    {
        #region Public Properties

        /// <summary>
        /// The mobile.
        /// </summary>
        public string Interno { get; set; }

        /// <summary>
        /// The mobile intern.
        /// </summary>
        public string TipoVehiculo { get; set; }

        /// <summary>
        /// Total kilometers for the period.
        /// </summary>
        public double KilometrosTotales { get; set; }

        /// <summary>
        /// Daily kilometers average.
        /// </summary>
        public double KilometrosPromedio { get; set; }

        /// <summary>
        /// Max speed reached.
        /// </summary>
        public int VelocidadMaxima { get; set; }

        /// <summary>
        /// Average speed.
        /// </summary>
        public int VelocidadPromedio { get; set; }

        /// <summary>
        /// Number of commited infractions.
        /// </summary>
        public int Infracciones { get; set; }

        /// <summary>
        /// Movement hours.
        /// </summary>
        public TimeSpan HorasMovimiento { get { return TimeSpan.FromHours(HsMovimiento); } }

        /// <summary>
        /// Stopped hours.
        /// </summary>
        public TimeSpan HorasDetenido { get { return TimeSpan.FromHours(HsDetenido); } }

        /// <summary>
        /// Non report hours.
        /// </summary>
        public TimeSpan HorasSinReportar { get { return TimeSpan.FromHours(HsSinReportar); } }

        /// <summary>
        /// Infraction hours.
        /// </summary>
        public TimeSpan HorasInfraccion { get { return TimeSpan.FromMinutes(HsInfraccion); } }

        /// <summary>
        /// Total days with data within the period.
        /// </summary>
        public int Dias { get; set; }

        /// <summary>
        /// Days with movement within the period.
        /// </summary>
        public int DiasActivo { get; set; }

        /// <summary>
        /// The mobile liscense plate.
        /// </summary>
        public string Patente { get; set; }

        /// <summary>
        /// The device assigned to the mobile.
        /// </summary>
        public string Dispositivo { get; set; }

        /// <summary>
        /// The total amount of stopped events.
        /// </summary>
        public int StoppedEvents { get; set; }

        /// <summary>
        /// The total amount of movement events.
        /// </summary>
        public int MovementEvents { get; set; }

        /// <summary>
        /// The total amount of no report events.
        /// </summary>
        public int NoReportEvents { get; set; }

        public double HsMovimiento { get; set; }
        public double HsDetenido { get; set; }
        public double HsInfraccion { get; set; }
        public double HsSinReportar { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the givenn mobile has any activity during the givenn time span.
        /// </summary>
        /// <returns></returns>
        public bool HasActiveDays() { return !DiasActivo.Equals(0); }

        #endregion
    }
}
