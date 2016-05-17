using System;

namespace Logictracker.Types.ReportObjects
{
    [Serializable]
    public class RankingTransportistas
    {
        public int IdTransportista { get; set; }
        public string Transportista { get; set; }
        public int Vehiculos { get; set; }
        public double Kilometros { get; set; }
        public TimeSpan HorasMovimiento { get { return TimeSpan.FromHours(Hours); } }
        public double Puntaje { get; set; }
        public int InfraccionesLeves { get; set; }
        public int InfraccionesMedias { get; set; }
        public int InfraccionesGraves { get; set; }
        public int InfraccionesTotales { get { return InfraccionesLeves + InfraccionesMedias + InfraccionesGraves; } }
        public double Hours { get; set; }
    }
}