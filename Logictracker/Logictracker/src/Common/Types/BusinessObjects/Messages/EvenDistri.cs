using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class EvenDistri : IHasViajeDistribucion, IAuditable
    {
        public virtual int Id { get; set; }
        public virtual LogMensaje LogMensaje { get; set; }
        public virtual ViajeDistribucion Viaje { get; set; }
        public virtual EntregaDistribucion Entrega { get; set; }
        public virtual DateTime Fecha { get; set; }

        public virtual Type TypeOf()
        {
            return GetType();
        }
    }
}