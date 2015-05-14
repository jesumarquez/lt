#region Usings

using System;
using Logictracker.Services.Helpers;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents the significative events that ocurred
    /// in dtermined route for a mobile.
    /// </summary>
    [Serializable]
    public class RouteEvent : IComparable<RouteEvent>
    {
        #region Private Properties

        private string _finalCorner;
        private string _initialCorner;
        private string _state = string.Empty;

        #endregion

        #region Public Properties

        /// <summary>
        /// Initial date for the event.
        /// </summary>
        public DateTime InitialDate { get; set; }

        /// <summary>
        /// Final date for the event.
        /// </summary>
        public DateTime FinalDate { get; set; }

        /// <summary>
        /// The duration of the event.
        /// </summary>
        public TimeSpan ElapsedTime { get { return FinalDate.Subtract(InitialDate); } }

        /// <summary>
        /// The minimum speed registered whitin the event.
        /// </summary>
        public int MinimumSpeed { get; set; }

        /// <summary>
        /// The maximum speed registered within the event.
        /// </summary>
        public int MaximumSpeed { get; set; }

        /// <summary>
        /// Average speed for the event.
        /// </summary>
        public double AverageSpeed { get; set; }

        /// <summary>
        /// The cardinal direction of the mobile´s route whitin the event.
        /// </summary>
        public double Direction { get; set; }

        /// <summary>
        /// The distance crossed in km by the mobile within the event.
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// The initial streets for the event.
        /// </summary>
        public string InitialCorner { get { return _initialCorner ?? (_initialCorner = GeocoderHelper.GetDescripcionEsquinaMasCercana(InitialLatitude, InitialLongitude)); } }

        /// <summary>
        /// The final streets for the event.
        /// </summary>
        public string FinalCorner
        {
            get
            {
                if (Distance != 0)
                {
                    return _finalCorner ?? (_finalCorner = GeocoderHelper.GetDescripcionEsquinaMasCercana(FinalLatitude, FinalLongitude));
                }

                return _initialCorner;
            }
        }

        /// <summary>
        /// The initial latitude.
        /// </summary>
        public double InitialLatitude { get; set; }

        /// <summary>
        /// The initial longitude.
        /// </summary>
        public double InitialLongitude { get; set; }

        /// <summary>
        /// The final latitude
        /// </summary>
        public double FinalLatitude { get; set; }

        /// <summary>
        /// The final longitude
        /// </summary>
        public double FinalLongitude { get; set; }

        /// <summary>
        /// The mobile route section state.
        /// </summary>
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method for comparing two events by initial date
        /// </summary>
        /// <param name="aEvent">A event to compare</param>
        /// <returns>A indication of its relative values.</returns>
        public int CompareTo(RouteEvent aEvent) { return InitialDate.CompareTo(aEvent.InitialDate); }

        #endregion
    }
}
