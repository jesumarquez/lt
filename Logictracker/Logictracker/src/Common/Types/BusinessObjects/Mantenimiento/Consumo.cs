using System;
using Urbetrack.Types.InterfacesAndBaseClasses;
using Urbetrack.Types.BusinessObjects.Vehiculos;

namespace Urbetrack.Types.BusinessObjects.Mantenimiento
{
    [Serializable]
    public class Consumo : IAuditable, IHasVehiculo
    {
        public static class Estados
        {
            public const short Eliminado = -1;
            public const short Pendiente = 0;
            public const short Pagado = 1;
        }

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual DateTime Fecha { get; set; }
        public virtual double KmDeclarados { get; set; }
        public virtual string UnidadMedida { get; set; }
        public virtual double Cantidad { get; set; }
        public virtual string NumeroFactura { get; set; }
        public virtual double Importe { get; set; }
        public virtual short Estado { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Insumo Insumo { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}