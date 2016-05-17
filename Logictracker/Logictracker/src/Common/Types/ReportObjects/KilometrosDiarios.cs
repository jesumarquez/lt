using System;

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class KilometrosDiarios
    {
        public int IdVehiculo { get; set; }
        public string Patente { get; set; }
        public DateTime Fecha { get; set; }
        public double KmTotales { get; set; }
        public double KmEnRuta { get; set; }
        public double HorasDeMarcha { get; set; }
        public double HorasMovimiento { get; set; }
        public double HorasDetenido { get; set; }
        public double HorasSinReportar { get; set; }
        public double HorasDetenidoEnTaller { get; set; }
        public double HorasDetenidoEnBase { get; set; }
        public double HorasDetenidoConTarea { get; set; }
        public double HorasDetenidoSinTarea { get; set; }
    }
}
