#region Usings

using System;
using Logictracker.Services.Helpers;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Multiple mobile stopped events data access class.
    /// </summary>
    [Serializable]
    public class MobileStoppedEvent
    {
        #region Private Properties

        private string _corner;

        #endregion

        #region Public Properties

        /// <summary>
        /// Refference latitude for the stopped events.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Refference longitude for the stopped events.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Amount of stopped events that toke place within a predefined radius of the refference latitude and longitude.
        /// </summary>
        public int StoppedEvents { get; set; }

        /// <summary>
        /// The description of the nearest corner of the refference latitude and longitude.
        /// </summary>
        public string Corner
        {
            get
            {
                if (string.IsNullOrEmpty(_corner)) _corner = GeocoderHelper.GetDescripcionEsquinaMasCercana(Latitude, Longitude);

                return _corner;
            }
        }

        #endregion
    }
}