using System;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class RouteEvent: IEvent
    {
        public static class Estados
        {
            public const short Cancelado = -1;
            public const short Enviado = 1;
        }
        public string EventType { get { return EventTypes.Route; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }

        public int RouteId { get; private set; }

        public short Estado { get; private set; }


        public RouteEvent(DateTime date, int routeId, double latitud, double longitud, short estado)
        {
            Date = date;
            Latitud = latitud;
            Longitud = longitud;
            RouteId = routeId;
            Estado = estado;
        }
    }
}
