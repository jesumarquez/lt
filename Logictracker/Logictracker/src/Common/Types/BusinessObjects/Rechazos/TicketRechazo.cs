using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Rechazos
{
    public class TicketRechazo : IAuditable, IHasEmpresa, IHasLinea, IHasCliente
    {
        #region Motivo
        public enum MotivoRechazo
        {
            MalFacturado = 6051,
            MalPedido = 6052,
            NoEncontroDomicilio = 6053,
            NoPedido = 6054,
            Cerrado = 6055,
            CaminoIntransitable = 6055,
            FaltaSinCargo = 6057,
            FueraDeHorario = 6058,
            FueraDeZona = 6059,
            ProductoNoApto = 6060,
            SinDinero = 6061
        }

        public static string GetMotivoLabelVariableName(MotivoRechazo motivoRechazo)
        {
            switch (motivoRechazo)
            {
                case MotivoRechazo.MalFacturado:
                    return "MAL_FACTURADO";
                case MotivoRechazo.MalPedido:
                    return "MAL_PEDIDO";
                case MotivoRechazo.NoEncontroDomicilio:
                    return "NO_ENCONTRO_DOMICILIO";
                case MotivoRechazo.NoPedido:
                    return "NO_PEDIDO";
                case MotivoRechazo.Cerrado:
                    return "CERRADO";
                case MotivoRechazo.FaltaSinCargo:
                    return "FALTA_SIN_CARGO";
                case MotivoRechazo.FueraDeHorario:
                    return "FUERA_DE_HORARIO";
                case MotivoRechazo.FueraDeZona:
                    return "FUERA_DE_ZONA";
                case MotivoRechazo.ProductoNoApto:
                    return "PRODUCTO_NO_APTO";
                case MotivoRechazo.SinDinero:
                    return "SIN_DINERO";
                default:
                    throw new ArgumentOutOfRangeException("motivoRechazo");
            }
        }

        #endregion

        #region Estado

        public enum Estado
        {
            Pendiente = 1,
            Notificado = 2,
            Alertado = 3,
            Resuelto = 4,
            Anulado = 5,
            Avisado = 6,
            Entregado = 7,
            SinAviso = 77,
            NoResuelta = 8,
            AltaErronea = 9,
            Duplicado = 10,
        }


        public enum EstadoFinal
        {
            SolucionPendiente=1,
            RechazoDuplicado=2,
            RechazoErroneo=3,
            ResueltoEntregado=4,
            ResueltoSinEntrega=5
        }

        public static Estado[] Next(Estado actual)
        {
            switch (actual)
            {
                case Estado.Pendiente:
                    return new[] { Estado.Duplicado, Estado.AltaErronea, Estado.Notificado };
                case Estado.Notificado:
                    return new[] { Estado.Alertado, };
                case Estado.Alertado:
                    return new[] { Estado.Resuelto, Estado.Anulado, };
                case Estado.Resuelto:
                    return new[] { Estado.Avisado, };
                case Estado.Anulado:
                    return new Estado[] { };
                case Estado.Avisado:
                    return new[] { Estado.Entregado, Estado.NoResuelta, };
                case Estado.Entregado:
                    return new Estado[] { };
                case Estado.SinAviso:
                    return new Estado[] { };
                case Estado.NoResuelta:
                    return new Estado[] { };
                case Estado.AltaErronea:
                    return new Estado[] { };
            }
            return new Estado[] {};
        }


        protected TicketRechazo()
        {
            
        }

        public TicketRechazo(string observacion , Usuario usuario , DateTime fechaHora)
        {
            Final = EstadoFinal.SolucionPendiente;

            var detalle = new DetalleTicketRechazo()
            {
                Estado = Estado.Pendiente,
                FechaHora = fechaHora,
                Observacion = observacion,
                Ticket = this,
                Usuario = usuario
            };
            
            FechaHora = fechaHora;
            
            Detalle.Add(detalle);
        }

        public virtual void ChangeEstado(Estado nuevoEstado , string observacion , Usuario usuario)
        {
            if (!Next(UltimoEstado).Any(e => e == nuevoEstado))
                throw new Exception(string.Format("Cambio de estado invalido {0} -> {1}", UltimoEstado, nuevoEstado));

            var detalle = new DetalleTicketRechazo
            {
                Estado = nuevoEstado,
                FechaHora = DateTime.UtcNow,
                Observacion = observacion,
                Ticket = this,
                Usuario = usuario
            };
            
            Detalle.Add(detalle);

        }
        
        public static string GetEstadoLabelVariableName(Estado estado)
        {
            switch (estado)
            {
                case Estado.Pendiente:
                    return "PENDIENTE";
                case Estado.Notificado:
                    return "NOTIFICADO";
                case Estado.Alertado:
                    return "ALERTADO";
                case Estado.Resuelto:
                    return "RESUELTO";
                case Estado.Anulado:
                    return "ANULADO";
                case Estado.Avisado:
                    return "AVISADO";
                case Estado.Entregado:
                    return "ENTREGADO";
                case Estado.SinAviso:
                    return "SIN_AVISO";
                case Estado.NoResuelta:
                    return "NO_RESUELTA";
                case Estado.AltaErronea:
                    return "ALTA_ERRONEA";
                default:
                    throw new ArgumentOutOfRangeException("estado");
            }
        }

        #endregion

        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }

        public virtual Linea Linea { get; set; }

        public virtual DateTime FechaHora { get; set; }

        public virtual Cliente Cliente { get; set; }

        public virtual Empleado Vendedor { get; set; }

        public virtual Empleado SupervisorRuta { get; set; }

        public virtual Empleado SupervisorVenta { get; set; }
        public virtual string Territorio { get; set; }

        public virtual MotivoRechazo Motivo { get; set; }

        public virtual Estado UltimoEstado { get { return Detalle.OrderByDescending(e => e.FechaHora).First().Estado; } }

        public virtual int Bultos { get; set; }

        private ISet<DetalleTicketRechazo> _detalles;

        public  virtual Type TypeOf() { return GetType(); }

        public virtual ISet<DetalleTicketRechazo> Detalle
        {
            get { return _detalles ?? (_detalles = new HashSet<DetalleTicketRechazo>()); }
        }

        public virtual EstadoFinal Final { get; set; }
    }
}
