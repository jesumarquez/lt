using System;

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Representa las horas trabajadas en un día
    /// </summary>
    [Serializable]
    public class ACWorkedHours
    {
        #region Public Properties

        /// <summary>
        /// The day of month.
        /// </summary>
        public DateTime Fecha{ get; set; }

        /// <summary>
        /// The elapsed time.
        /// </summary>
        public double ElapsedTime { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that recieves the day and associated time span.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="elapsedTime"></param>
        public ACWorkedHours(DateTime date, double elapsedTime)
        {
            Fecha = date;
            ElapsedTime = elapsedTime;
        }

        #endregion
    }
}
