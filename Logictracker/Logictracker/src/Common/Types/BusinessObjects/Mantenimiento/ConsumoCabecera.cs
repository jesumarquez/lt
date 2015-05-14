using System;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Mantenimiento
{
    [Serializable]
    public class ConsumoCabecera : IAuditable, IHasVehiculo, IHasEmpleado, IHasProveedor, IHasDeposito, IHasDepositoDestino
    {
        public static class Estados
        {
            public const short Eliminado = -1;
            public const short Pendiente = 0;
            public const short Pagado = 1;
        }

        public static class TiposMovimiento
        {
            public const short ProveedorADeposito = 1;
            public const short ProveedorAVehiculo = 2;
            public const short DepositoAVehiculo = 3;
            public const short DepositoADeposito = 4;
        }

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual DateTime Fecha { get; set; }
        public virtual double KmDeclarados { get; set; }
        public virtual string NumeroFactura { get; set; }
        public virtual double ImporteTotal { get; set; }
        public virtual short Estado { get; set; }
        public virtual short TipoMovimiento { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Deposito Deposito { get; set; }
        public virtual Deposito DepositoDestino { get; set; }

        private ISet _detalles;
        public virtual ISet Detalles { get { return _detalles ?? (_detalles = new ListSet()); } }
    }
}