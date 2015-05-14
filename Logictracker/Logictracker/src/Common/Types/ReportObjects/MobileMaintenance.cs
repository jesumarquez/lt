#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Represents mobile maintencance data.
    /// </summary>
    [Serializable]
    public class MobileMaintenance
    {
        #region Public Properties

        /// <summary>
        /// Vehicle ID
        /// </summary>
        public int IdVehiculo { get; set; }
        /// <summary>
        /// The mobile intern.
        /// </summary>
        public string Interno { get; set; }

        /// <summary>
        /// The mobile domain.
        /// </summary>
        public string Patente { get; set; }

        /// <summary>
        /// A refference for this mobile.
        /// </summary>
        public string Referencia { get; set; }

        /// <summary>
        /// The vehicle type.
        /// </summary>
        public string TipoVehiculo { get; set; }

        /// <summary>
        /// Total movement hours for the specified range.
        /// </summary>
        public String HorasMarcha { get { return String.Format("{0:HH:mm:ss}", new DateTime(2000, 1, 1, TimeSpan.FromHours(HsMarcha).Hours, TimeSpan.FromHours(HsMarcha).Minutes, TimeSpan.FromHours(HsMarcha).Seconds)); } }

        /// <summary>
        /// Total kilometers for the specified range.
        /// </summary>
        public double Kilometros { get; set; }

        /// <summary>
        /// Total time spent in base for the specified range.
        /// </summary>
        public String HorasPlanta { get { return  String.Format("{0:00}",TimeSpan.FromHours(HsPlanta).Hours) + ":" + String.Format("{0:00}",TimeSpan.FromHours(HsPlanta).Minutes); } }

        /// <summary>
        /// Total time spent in workshop for the specified range.
        /// </summary>
        public String HorasTaller { get { return String.Format("{0:00}",TimeSpan.FromHours(HsTaller).Hours) + ":" + String.Format("{0:00}",TimeSpan.FromHours(HsTaller).Minutes); } }

        /// <summary>
        /// Auxiliar variables for calculatin elapsed time.
        /// </summary>
        public double HsMarcha { get; set; }
        public double HsPlanta { get; set; }
        public double HsTaller { get; set; }

        #endregion
    }
}
