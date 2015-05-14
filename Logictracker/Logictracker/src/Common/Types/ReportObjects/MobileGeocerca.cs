using System;

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class MobileGeocerca
    {
        public string Interno { get; set; }
        public string Patente { get; set; }
        public int IdGeo { get; set; }
        public string Geocerca { get; set; }
        public string TipoGeocerca { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Salida { get; set; }
        public TimeSpan Duracion { get; set; }
        public TimeSpan ProximaGeocerca { get; set; }
        public double Recorrido { get; set; }
        public int IdMovil { get; set; }
    }
}
