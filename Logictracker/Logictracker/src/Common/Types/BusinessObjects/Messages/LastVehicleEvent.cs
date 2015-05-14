using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class LastVehicleEvent : IHasVehiculo, IAuditable
    {
        public virtual int Id { get; set; }
        public virtual LogMensaje LogMensaje { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual int TipoEvento { get; set; }

        public virtual Type TypeOf()
        {
            return GetType();
        }
    }
}