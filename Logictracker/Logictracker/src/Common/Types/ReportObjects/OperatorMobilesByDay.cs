#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class OperatorMobilesByDay
    {
        #region Public Properties

        /// <summary>
        /// The mobile id
        /// </summary>
        public int IdMovil { get; set; }

        /// <summary>
        /// The mobile identifier code.
        /// </summary>
        public string Movil { get; set; }

        /// <summary>
        /// The mobile brand.
        /// </summary>
        public string Marca { get; set; }

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
        /// The mobile responsable
        /// </summary>
        public string Responsable { get; set; }

        /// <summary>
        /// Total movement time.
        /// </summary>
        public TimeSpan HorasActivo { get;set; }

        public DateTime Fecha { get; set; }

        #endregion
    }
}
