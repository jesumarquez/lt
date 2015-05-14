#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents the total kilometers
    /// for a mobile on a givenn date.
    /// </summary>
    [Serializable]
    public class MobileKilometer
    {
        #region Public Properties

        /// <summary>
        /// the date
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// The route kilometers.
        /// </summary>
        public double Kilometers { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that recieves the day end its associated kilometers.
        /// </summary>  
        /// <param name="date"></param>
        /// <param name="kilometers"></param>
        public MobileKilometer(DateTime date, double kilometers)
        {
            Fecha = date;
            Kilometers = kilometers;
        }

        #endregion
    }
}
