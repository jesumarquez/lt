#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents the stadistics of all drivers of a mobile.
    /// </summary>
    [Serializable]
    public class MobileDrivers
    {
        /// <summary>
        /// The driver identification.
        /// </summary>
        public string Legajo { get; set; }

        /// <summary>
        /// The driver full name.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// The rfid card code.
        /// </summary>
        public string Tarjeta { get; set; }

        /// <summary>
        /// Total number of infractions.
        /// </summary>
        public int Infracciones { get; set; }

        /// <summary>
        /// The total driving time.
        /// </summary>
        public TimeSpan DrivingTime { get { return TimeSpan.FromHours(DrivingT); }  }

        /// <summary>
        /// Total amount of kilometers associated to the driver.
        /// </summary>
        public double Kilometros { get; set; }

        /// <summary>
        /// aux vars
        /// </summary>
        public double DrivingT { get; set; }
    }
}
