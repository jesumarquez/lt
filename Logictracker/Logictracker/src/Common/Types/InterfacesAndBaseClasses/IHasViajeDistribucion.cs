using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Types.InterfacesAndBaseClasses
{
    public interface IHasViajeDistribucion
    {
        ViajeDistribucion Viaje { get; }
    }
}
