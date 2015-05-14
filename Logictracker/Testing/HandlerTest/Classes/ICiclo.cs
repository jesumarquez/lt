using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace HandlerTest.Classes
{
    public interface ICiclo
    {
        event EventHandler MostrarEnMapaChanged;
        event EventHandler DistribucionChanged;
        ViajeDistribucion Distribucion { get; }
        bool MostrarEnMapa { get; }
    }
}
