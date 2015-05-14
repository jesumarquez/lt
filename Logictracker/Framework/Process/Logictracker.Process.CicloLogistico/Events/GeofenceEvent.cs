using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class GeofenceEvent:IEvent
    {
        public enum EventoGeofence
        {
            Entrada = 0,
            Salida = 1
        }
        public string EventType { get { return EventTypes.Geofence; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }
        public Empleado Chofer { get; private set; }

        public int Id { get; private set; }
        public EventoGeofence Evento { get; private set; }

        public GeofenceEvent(DateTime date, int id, EventoGeofence evento, double latitud, double longitud, Empleado chofer)
        {
            Date = date;
            Id = id;
            Evento = evento;
            Latitud = latitud;
            Longitud = longitud;
            Chofer = chofer;
        }
    }
}
