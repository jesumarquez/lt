#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents operator activities stadistics.
    /// </summary>
    [Serializable]
    public class OperatorStadistics
    {
        #region Public Properties

        /// <summary>
        /// The operator.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// The mobile intern.
        /// </summary>
        public string TipoEmpleado { get; set; }

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
        public double VelocidadPromedio { get; set; }

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
        public TimeSpan HorasInfraccion { get { return TimeSpan.FromMinutes(MinsInfraccion); } }

        /// <summary>
        /// Total days with data within the period.
        /// </summary>
        public int Dias { get; set; }

        /// <summary>
        /// Days with movement within the period.
        /// </summary>
        public int DiasActivo { get; set; }

        /// <summary>
        /// The operator file.
        /// </summary>
        public string Legajo { get; set; }

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

        /// <summary>
        /// Aux time variables.
        /// </summary>
        public double HsMovimiento { get; set; }
        public double HsDetenido { get; set; }
        public double MinsInfraccion { get; set; }
        public double HsSinReportar { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the givenn operator has any activity during the givenn time span.
        /// </summary>
        /// <returns></returns>
        public bool HasActiveDays() { return !DiasActivo.Equals(0); }

        #endregion
    }
}
