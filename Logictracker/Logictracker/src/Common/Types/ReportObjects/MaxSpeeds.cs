#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents the max speed reached per day.
    /// </summary>
    [Serializable]
    public class MaxSpeeds
    {
        #region Public Properties

        /// <summary>
        /// Day of month.
        /// </summary>
        public DateTime Dia { get; set; }

        /// <summary>
        /// Speed reached that day.
        /// </summary>
        public int Velocidad { get; set; }

        /// <summary>
        /// Vehicul ou operador con el que se cometio esa velocidad máxima.
        /// </summary>
        public String CometidoPor { get; set; }

        #endregion
    }
}
