using System;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class PositionEvent: IEvent
    {
        public string EventType { get { return EventTypes.Position; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }


        public PositionEvent(DateTime date, double latitud, double longitud)
        {
            Date = date;
            Latitud = latitud;
            Longitud = longitud;
        }
    }
}
