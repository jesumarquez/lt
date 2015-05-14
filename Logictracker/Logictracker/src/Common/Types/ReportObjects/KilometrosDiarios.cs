using System;

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class KilometrosDiarios
    {
        public string Vehiculo { get; set; }
        public DateTime Fecha { get; set; }
        public double KmTotales { get; set; }
        public double KmEnRuta { get; set; }
    }
}
