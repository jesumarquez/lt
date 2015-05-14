#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents all the elapsed movement time
    /// for a mobile in a givenn day.
    /// </summary>
    [Serializable]
    public class MobileTime
    {
        #region Public Properties

        /// <summary>
        /// The day of month.
        /// </summary>
        public DateTime Fecha{ get; set; }

        /// <summary>
        /// The elapsed time.
        /// </summary>
        public int ElapsedTime { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that recieves the day and associated time span.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="elapsedTime"></param>
        public MobileTime(DateTime date, double elapsedTime)
        {
            Fecha = date;
            ElapsedTime = (int)(elapsedTime*3600);
        }

        #endregion
    }
}
