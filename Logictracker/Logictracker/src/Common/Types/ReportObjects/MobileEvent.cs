#region Usings

using System;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Messages;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents any kind of event of a mobile.
    /// </summary>
    [Serializable]
    public class MobileEvent
    {
        #region Private Properties

        private string _finalCross;
        private string _initialCross;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialices the mobile event.
        /// </summary>
        public MobileEvent()
        {
            MobileType = string.Empty;
            Message = string.Empty;
            Driver = string.Empty;
            Intern = string.Empty;
        }

        #endregion

        #region Public Properties

        public bool TieneFoto { get; set;}

        /// <summary>
        /// The person in charge of the mobile.
        /// </summary>
        public string Responsable { get; set; }

        /// <summary>
        /// The intern number of the mobile.
        /// </summary>
        public string Intern { get; set; }

        /// <summary>
        /// The id of the driver of the mobile at the time the event toke place.
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// The date and time of the event.
        /// </summary>
        public DateTime EventTime { get; set; }

        public DateTime? Reception { get; set; }

        /// <summary>
        /// The associated message to the event.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The latitude where the mobile was when the event toke place.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude where the mobile was when the event toke place.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Gets the nearest corner to the place where the event started.
        /// </summary>
        public string InitialCross  { get { return _initialCross ?? (_initialCross = GeocoderHelper.GetDescripcionEsquinaMasCercana(Latitude, Longitude)); } }

        /// <summary>
        /// Gets the nearest corner to the place where the event finished.
        /// </summary>
        public string FinalCross
        {
            get
            {
                return _finalCross ?? (_finalCross = FinalLatitude.HasValue && FinalLongitude.HasValue  
                    ? GeocoderHelper.GetDescripcionEsquinaMasCercana(FinalLatitude.Value, FinalLongitude.Value) : string.Empty);
            }
        }

        /// <summary>
        /// Gets the event duration.
        /// </summary>
        public TimeSpan Duration { get { return HasDuration() ? EventEndTime.Value.Subtract(EventTime) : TimeSpan.Zero; } }

        /// <summary>
        /// The associated icon to the message.
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// The final time of the event.
        /// </summary>
        public DateTime? EventEndTime { get; set; }

        /// <summary>
        /// The final latitude for the event.
        /// </summary>
        public Double? FinalLatitude { get; set; }

        /// <summary>
        /// The final longitude for the event.
        /// </summary>
        public Double? FinalLongitude { get; set; }

        /// <summary>
        /// The type of the mobile associated to the device.
        /// </summary>
        public string MobileType { get; set; }

        public bool Atendido { get; set; }

        public Usuario Usuario { get; set; }

        public DateTime? Fecha { get; set; }

        public AtencionEvento AtencionEvento { get; set; }

        /// <summary>
        /// The message id.
        /// </summary>
        public int Id { get; set; }

        public int? IdPuntoInteres { get; set; }

        public int IdMensaje { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines wither if the giveen message has a valid position.
        /// </summary>
        /// <returns></returns>
        public bool HasValidLatitudes() { return Math.Abs(Latitude) > 0.0 && Math.Abs(Longitude) > 0.0; }

        /// <summary>
        /// Determines if the givenn message has duration.
        /// </summary>
        /// <returns></returns>
        public bool HasDuration() { return EventEndTime.HasValue; }

        #endregion
    }
}
