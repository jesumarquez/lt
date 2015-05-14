#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class MobileActivity
    {
        #region Public Properties


        public int Id { get; set; }

        /// <summary>
        /// The mobile identifier code.
        /// </summary>
        public string Movil { get; set; }

        public string Patente { get; set; }

        /// <summary>
        /// The mobile brand.
        /// </summary>
        //public string Marca { get; set; }

        /// <summary>
        /// The total kilometers traveled.
        /// </summary>
        public double Recorrido { get; set; }

        /// <summary>
        /// The vehicle type.
        /// </summary>
        public string TipoVehiculo { get; set; }

        /// <summary>
        /// The total number of infractions.
        /// </summary>
        public int Infracciones { get; set; }

        /// <summary>
        /// The maximum speed from the selected period
        /// </summary>
        public int VelocidadMaxima { get; set; }

        /// <summary>
        /// The average speed from the selected period
        /// </summary>
        public int VelocidadPromedio { get; set; }

        /// <summary>
        /// Total movement time.
        /// </summary>
        public TimeSpan HorasActivo { get { return TimeSpan.FromHours(HsActivos); } }

        /// <summary>
        /// Total stopped time.
        /// </summary>
        public TimeSpan HorasDetenido { get { return TimeSpan.FromHours(HsInactivos); } }

        /// <summary>
        /// The total time elapsed within infractions.
        /// </summary>
        public TimeSpan HorasInfraccion { get { return TimeSpan.FromHours(HsInfraccion); } }

        public TimeSpan HorasSinReportar { get { return TimeSpan.FromHours(HsSinReportar); } }

        /// <summary>
        /// The current cost center to wich the vehicle is assigned.
        /// </summary>
        public String CentroDeCostos { get; set; }

        /// <summary>
        /// Aux time variables.
        /// </summary>
        public double HsActivos { get; set; }
        public double HsInactivos { get; set; }
        public double HsInfraccion { get; set; }
        public double HsSinReportar { get; set; }

        #endregion
    }
}
