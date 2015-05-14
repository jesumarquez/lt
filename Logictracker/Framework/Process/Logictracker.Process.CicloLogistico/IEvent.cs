using System;

namespace Logictracker.Process.CicloLogistico
{
    public interface IEvent
    {
        string EventType { get; }
        DateTime Date { get; }
        double Latitud { get; }
        double Longitud { get; }
    }
}
