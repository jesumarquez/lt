#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class MobilesKilometers
    {
        public int Movil { get; set; }

        public string Interno { get; set; }

        /// <summary>
        /// Amount of kilometeres traveled by this mobile in the specified period 
        /// </summary>
        public double Kilometers { get; set; }
    }
}
