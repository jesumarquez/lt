using System;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class InitEvent:IEvent
    {
        public string EventType { get { return EventTypes.Init; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }


        public InitEvent(DateTime date)
        {
            Date = date;
        }
    }
}
