#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents a significant piece of route for a givenn mobile and day.
    /// </summary>
    [Serializable]
    public class MobileRoutes
    {
        #region Public Properties

        /// <summary>
        /// The Id of the mobile associated to the route fragment.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The mobile associated to the route fragment.
        /// </summary>
        public string Inter { get; set; }

        /// <summary>
        /// The vehicle type of the mobile associated to the event.
        /// </summary>
        public string VehicleType { get; set; }

        /// <summary>
        /// The employee that was driving the associated mobile during this route fragment.
        /// </summary>
        public string Driver { get; set; }

        /// <summary>
        /// The route initial time.
        /// </summary>
        public DateTime InitialTime { get; set; }

        /// <summary>
        /// The route final time.
        /// </summary>
        public DateTime FinalTime { get; set; }

        /// <summary>
        /// The total kilometers traveled in this route fragment.
        /// </summary>
        public double Kilometers { get; set; }

        /// <summary>
        /// The route fragment elapsed time.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// The number of spedd infractions that toke place during this route fragment.
        /// </summary>
        public int Infractions { get; set; }

        /// <summary>
        /// The total amount of time elapsed on infractions.
        /// </summary>
        public double InfractionsDuration { get; set; }

        /// <summary>
        /// The min speed reached in this route fragment.
        /// </summary>
        public int MinSpeed { get; set; }

        /// <summary>
        /// The average speed for this route fragment.
        /// </summary>
        public int AverageSpeed { get; set; }

        /// <summary>
        /// The max speed reached during this route fragment.
        /// </summary>
        public int MaxSpeed { get; set; }

        /// <summary>
        /// The associated geocerca where the route fragment toke place if any.
        /// </summary>
        public string Geocerca { get; set; }

        /// <summary>
        /// The mobile engine status during this route fragment.
        /// </summary>
        public string EngineStatus { get; set; }

        /// <summary>
        /// The vehicle movement status during this route fragment.
        /// </summary>
        public string VehicleStatus { get; set; }

        public double Consumo { get; set; }

        public double HsMarcha { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if two givenn mobile route fragments have the same driving caracteristics.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public bool EqualState(MobileRoutes route)
        {
            return Driver.Equals(route.Driver) && Geocerca.Equals(route.Geocerca) && EngineStatus.Equals(route.EngineStatus) && VehicleStatus.Equals(route.VehicleStatus);
        }

        #endregion
    }
}