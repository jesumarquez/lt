using System;

namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public class Novelty
    {
        public int NumeroServicio { get; set; }
        public VehicleDataSos Vehiculo { get; set; }
        public string Diagnostico { get; set; }
        public string Prioridad { get; set; }
        public DateTime HoraServicio { get; set; }
        public string CobroAdicional { get; set; }
        public int Estado { get; set; }
        public LocationSos Origen  { get; set; }
        public LocationSos Destino { get; set; }
    }
}