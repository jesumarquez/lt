using System;

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class OdometroStatus
    {
        public string Interno { get; set; }
        public string Tipo { get; set; }
        public string Patente { get; set; }
        public string CentroDeCosto { get; set; }
        public string Odometro { get; set; }
        public string Referencia { get; set; }
        public double? KilometrosReferencia { get; set; }
        public int? TiempoReferencia { get; set; }
        public double? HorasReferencia { get; set; }
        public double? KilometrosFaltantes { get; set; }
        public int? TiempoFaltante { get; set; }
        public double? HorasFaltantes { get; set; }
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public DateTime UltimoUpdate { get; set; }
        public string Responsable { get; set; }
        public int Priority { get; set; }
    }
}
