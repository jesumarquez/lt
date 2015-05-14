using Logictracker.Services.Helpers;
using System;

namespace Logictracker.Process.CicloLogistico.Distribucion
{
    [Serializable]
    public class Intercalado
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public DateTime Hora { get; set; }

        public double CostoKm { get; set; }
        public double CostoKmOld { get; set; }
        public double CostoKmExtra
        {
            get { return CostoKm - CostoKmOld; }
        }

        public double CostoMin { get; set; }
        public double CostoMinOld { get; set; }
        public double CostoMinExtra
        {
            get { return CostoMin - CostoMinOld; }
        }

        public Directions ViajeAnterior { get; set; }
        public Directions ViajeIntercalado { get; set; }
    }
}
