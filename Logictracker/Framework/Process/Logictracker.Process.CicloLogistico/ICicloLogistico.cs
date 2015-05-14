using System;

namespace Logictracker.Process.CicloLogistico
{
    public interface ICicloLogistico
    {
        void ProcessEvent(IEvent data);
        void Regenerate();

        //Mimico
        string CurrentState { get; }
        int CurrentStateCompleted { get; }
        int TotalCompleted { get; }
        int Delay { get; }

        //Reporte Retrasos
        bool EnGeocerca { get; }
        DateTime? EnGeocercaDesde { get; }
        DateTime Iniciado { get; }
        string Interno { get; }
        string Codigo { get; }
        string Cliente { get; }
        string Telefono { get; }
        string PuntoEntrega { get; }
    }
}
