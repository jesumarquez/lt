using System;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class TolvaEvent:IEvent
    {
        public enum EstadoTolva
        {
            Off = 1,
            On = 0
        }
        public string EventType { get { return EventTypes.Tolva; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }

        public EstadoTolva Estado { get; private set; }

        public TolvaEvent(DateTime date, EstadoTolva state, double latitud, double longitud)
        {
            Date = date;
            Estado = state;
            Latitud = latitud;
            Longitud = longitud;
        }
    }
}
