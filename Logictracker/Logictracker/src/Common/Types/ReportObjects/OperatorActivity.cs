#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class OperatorActivity
    {
        public int OperatorId { get; set; }

        /// <summary>
        /// The name of the employee
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Total traveled kilometers.
        /// </summary>
        public double Kilometers { get; set; }

        /// <summary>
        /// Total movement time.
        /// </summary>
        public TimeSpan Movement { get { return TimeSpan.FromHours(MovementHours); } }

        /// <summary>
        /// Total sttoped time.
        /// </summary>
        public TimeSpan Stopped { get { return TimeSpan.FromHours(StoppedHours);} }

        /// <summary>
        /// Number of infractions commited.
        /// </summary>
        public int Infractions { get; set; }

        /// <summary>
        /// Total time elapsed within infractions.
        /// </summary>
        public TimeSpan InfractionsTime { get { return TimeSpan.FromMinutes(InfractionsMinutes); }}

        /// <summary>
        /// The max speed reached.
        /// </summary>
        public int MaxSpeed { get; set; }

        /// <summary>
        /// The average speed.
        /// </summary>
        public int AverageSpeed { get; set; }

        public double StoppedHours { get; set; }
        public double MovementHours { get; set; }
        public double InfractionsMinutes { get; set; }
    }
}
