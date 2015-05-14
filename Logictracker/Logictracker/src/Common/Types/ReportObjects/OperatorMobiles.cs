#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class OperatorMobiles
    {
        /// <summary>
        /// The mobile identification.
        /// </summary>
        public string Patente { get; set; }

        /// <summary>
        /// The mobile intern.
        /// </summary>
        public string Interno { get; set; }

        /// <summary>
        /// Total number of infractions.
        /// </summary>
        public int Infracciones { get; set; }

        /// <summary>
        /// The total driving time.
        /// </summary>
        public TimeSpan DrivingTime { get { return TimeSpan.FromHours(HsDriving); } }

        /// <summary>
        /// Total amount of kilometers associated to the driver.
        /// </summary>
        public double Kilometros { get; set; }

        /// <summary>
        /// Aux time variables.
        /// </summary>
        public double HsDriving { get; set; }

    }
}
