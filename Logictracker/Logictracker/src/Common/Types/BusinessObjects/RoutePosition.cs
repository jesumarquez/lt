#region Usings

using System;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects
{
    /// <summary>
    /// Class that represents the information of a specific route position.
    /// </summary>
    public class RoutePosition : IAuditable
    {
        #region Constructors

        /// <summary>
        /// Generates a new route position based on the givenn database position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="historical"></param>
        /// <param name="motivoDescarte"></param>
        public RoutePosition(LogPosicionBase position, Boolean historical, int motivoDescarte)
        {
            Altitude = position.Altitud;
            Course = position.Curso;
            Date = position.FechaMensaje;
            Recieved = position.FechaRecepcion;
            Id = position.Id;
            Latitude = position.Latitud;
            Longitude = position.Longitud;
            Speed = position.Velocidad;
            Status = position.Status;
            MotorOn = position.MotorOn;
            Historical = historical;
            MotivoDescarte = motivoDescarte;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The latitude of this position.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude of the position.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// The speed of the mobile at this position.
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// The time when the mobile reported this position.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The time when the position was saved into database.
        /// </summary>
        public DateTime Recieved { get; set; }

        /// <summary>
        /// The mobile route course when the position was reported.
        /// </summary>
        public double Course { get; set; }

        /// <summary>
        /// The mobile altitude for the current position.
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// The status of the mobile at the time this position was reported.
        /// </summary>
        public Byte? Status { get; set; }

        public bool? MotorOn { get; set; }

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        /// <summary>
        /// Determines if the position is online or historical.
        /// </summary>
        public bool Historical { get; set; }

        /// <summary>
        /// Determines if the position was descarted.
        /// </summary>
        public int MotivoDescarte { get; set; }

        /// <summary>
        /// Determines if the current positions was enqueued or not.
        /// </summary>
        public virtual bool WasEnqueued { get { return Recieved.Subtract(Date).TotalMinutes > 5; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whiter if the givenn position equals the current position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool EqualsPosition(RoutePosition position) { return Latitude.Equals(position.Latitude) && Longitude.Equals(position.Longitude) && Speed.Equals(position.Speed); }

        #endregion
    }
}
