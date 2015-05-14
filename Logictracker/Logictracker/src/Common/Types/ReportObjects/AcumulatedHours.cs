using System;

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Represents the total worked time for a employee
    /// </summary>
    [Serializable]
    public class AcumulatedHours
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new MobilesTime using the provided vehicle and time information.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="empleado"></param>
        public AcumulatedHours(double time, int id, string empleado)
        {
            IdEmpleado = id;
            Empleado = empleado;
            ElapsedTime = time;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The mobile code.
        /// </summary>
        public int IdEmpleado { get; set; }

        /// <summary>
        /// The employee name
        /// </summary>
        public string Empleado { get; set; }

        /// <summary>
        /// The total amount of movement time associated to the mobile.
        /// </summary>
        public double ElapsedTime { get; set; }

        #endregion
    }
}
