using System;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.Tickets
{
    [Serializable]
    public class Ticket : IAuditable, ISecurable, IHasVehiculo, IHasCliente, IHasPuntoEntrega, IHasEmpleado, IHasPedido
    {
        public static class Estados
        {
            public const short Eliminado = -1;
            public const short Pendiente = 0;
            public const short EnCurso = 1;
            public const short Anulado = 8;
            public const short Cerrado = 9;
        }

        /// <summary>
        /// Hardcode for Cache
        /// </summary>
        public const string CurrentCacheKey = "Ticket.Current";

        Type IAuditable.TypeOf() { return GetType(); }

        private ISet<DetalleTicket> _detalles;

        public virtual ISet<DetalleTicket> Detalles
        {
            get { return _detalles ?? (_detalles = new HashSet<DetalleTicket>()); }
        }

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Linea BaseDestino { get; set; }
        public virtual Linea BaseLlegada { get; set; }

        public virtual int Id { get; set; }

        public virtual Pedido Pedido { get; set; }

        public virtual string Codigo { get; set; }

        public virtual Coche Vehiculo { get; set; }

        public virtual Cliente Cliente { get; set; }

        public virtual Empleado Empleado { get; set; }

        public virtual PuntoEntrega PuntoEntrega { get; set; }

        public virtual Dispositivo Dispositivo { get; set; }

        public virtual Estado EstadoLogistico { get; set; }

        public virtual DateTime? FechaTicket { get; set; }

        public virtual DateTime? FechaDescarga { get; set; }

        public virtual DateTime? FechaFin { get; set; }

        public virtual short Estado { get; set; }

        public virtual string CantidadPedido { get; set; }

        public virtual string Unidad { get; set; }

        public virtual string CantidadCarga { get; set; }

        public virtual string CantidadCargaReal { get; set; }

        public virtual string DescripcionProducto { get; set; }

        public virtual string CodigoProducto { get; set; }

        public virtual string CumulativeQty { get; set; }

        public virtual string SourceStation { get; set; }

        public virtual string SourceFile { get; set; }
        
        public virtual string UserField1 { get; set; }

        public virtual string UserField2 { get; set; }

        public virtual string UserField3 { get; set; }

        public virtual int? OrdenDiario { get; set; }

        public virtual Usuario UsuarioAnulacion { get; set; }

        public virtual string MotivoAnulacion { get; set; }

        public virtual DateTime? FechaAnulacion { get; set; }

        public virtual bool ASincronizar { get; set; }
        public virtual DateTime? FechaSincronizado { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var ticket = obj as Ticket;
            if (ticket == null) return false;
            return Id == ticket.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public virtual DetalleTicket GetDetalleProximo()
        {
            return Detalles
				.OfType<DetalleTicket>()
				.Where(d => !d.Automatico.HasValue && !d.Manual.HasValue)
                .OrderBy(d => d.Programado)
				.FirstOrDefault();
        }

        public virtual void Anular(string motivo, Usuario usuario)
        {
            Estado = Estados.Anulado;
            FechaAnulacion = DateTime.UtcNow;
            MotivoAnulacion = motivo;
            UsuarioAnulacion = usuario;
            if (Pedido != null) ASincronizar = true;
        }
    }
}
