using System;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class ManualEvent: IEvent
    {
        public string EventType { get { return EventTypes.Manual; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }
        public string Mensaje { get; private set; }


        public ManualEvent(DateTime date, double latitud, double longitud, string mensaje)
        {
            Date = date;
            Latitud = latitud;
            Longitud = longitud;
            Mensaje = mensaje;
        }
    }
}
