#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects.Datamart
{
    /// <summary>
    /// Class for getting regeneration periods for the datamart.
    /// </summary>
    public class RegenerateDatamart
    {
        #region Constructors

        /// <summary>
        /// Generates a new datamart regeneration period based on the givenn timespan.
        /// </summary>
        /// <param name="day"></param>
        public RegenerateDatamart(DateTime day)
        {
            Desde = new DateTime(day.Year, day.Month, day.Day, 0, 0, 0);
            Hasta = new DateTime(day.Year, day.Month, day.Day, 23, 59, 59, 990);
        }

        #endregion

        #region Public Properties

        public DateTime Desde { get; private set; }
        public DateTime Hasta { get; private set; }

        #endregion
    }
}