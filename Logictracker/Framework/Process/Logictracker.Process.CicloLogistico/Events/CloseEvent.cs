using System;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class CloseEvent:IEvent
    {
        public string EventType { get { return EventTypes.Close; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }

        public bool InformDevice { get; private set; }


        public CloseEvent(DateTime date)
        {
            Date = date;
            InformDevice = true;
        }

        public CloseEvent(DateTime date, bool informDevice)
        {
            Date = date;
            InformDevice = informDevice;
        }
    }
}
