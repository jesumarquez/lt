#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Represents the total movement time for a mobile.
    /// </summary>
    [Serializable]
    public class MobilesTime
    {

        #region Public Properties

        /// <summary>
        /// The mobile code.
        /// </summary>
        public int Movil { get; set; }

        /// <summary>
        /// The mobile code.
        /// </summary>
        public string Intern { get; set; }

        /// <summary>
        /// The total amount of movement time associated to the mobile.
        /// </summary>
        public double ElapsedTime { get; set; }

        #endregion
    }
}