#region Usings

using System;

#endregion

namespace Logictracker.Types.ReportObjects
{
    /// <summary>
    /// Class that represents the stadistics of all drivers of a mobile by day.
    /// </summary>
    [Serializable]
    public class MobileDriversByDay
    {

        public int IdMovil { get; set; }

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
        public TimeSpan DrivingTime { get; set; }

        /// <summary>
        /// Total amount of kilometers associated to the driver.
        /// </summary>
        public double Kilometros { get; set; }

        /// <summary>
        /// aux var
        /// </summary>
        public DateTime Fecha { get; set; }

        public override bool Equals(object obj)
        {
            var castObj = obj as MobileDriversByDay;

            return castObj != null && castObj.Fecha.Equals(Fecha) && castObj.Nombre.Equals(Nombre);
        }

        public override int GetHashCode() { return Fecha.GetHashCode() + Nombre.GetHashCode(); }
    }
}
