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
            CaminoIntransitable = 6056,
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
                case MotivoRechazo.CaminoIntransitable:
                    return "CAMINO_INTRANSITABLE";
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
            SinAviso = 8,
            NoResuelta = 9,
            AltaErronea = 10,
            Duplicado = 11,
            NotificadoAutomatico = 12,
            AlertadoAutomatico = 13,            
            
            Notificado1 = 14,
            Notificado2 = 15,
            Notificado3 = 16,
            RespuestaExitosa = 17,
            RespuestaConRechazo = 18,            
            Rechazado = 19
        }

        public enum EstadoFinal
        {
            SolucionPendiente = 1,
            RechazoDuplicado = 2,
            RechazoErroneo = 3,
            ResueltoEntregado = 4,
            ResueltoSinEntrega = 5
        }

        public static Estado[] Next(Estado actual)
        {
            switch (actual)
            {
                case Estado.Pendiente:
                    return new[] { Estado.Duplicado, Estado.AltaErronea, Estado.Notificado1, Estado.Anulado };
                case Estado.Notificado: 
                    return new[] { Estado.Alertado };
                case Estado.NotificadoAutomatico: 
                    return new[] { Estado.AlertadoAutomatico };                
                case Estado.Alertado: 
                    return new[] { Estado.RespuestaExitosa, Estado.RespuestaConRechazo, Estado.Anulado };
                case Estado.AlertadoAutomatico: 
                    return new[] { Estado.Resuelto, Estado.Anulado, };
                case Estado.Resuelto: 
                    return new[] { Estado.Avisado, };
                case Estado.Anulado: 
                    return new Estado[] { };
                case Estado.Avisado:
                    return new[] { Estado.Entregado, Estado.Rechazado, Estado.NoResuelta, Estado.Anulado };
                case Estado.Entregado:
                    return new Estado[] { };
                case Estado.SinAviso:
                    return new Estado[] { };
                case Estado.NoResuelta:
                    return new Estado[] { };
                case Estado.AltaErronea:
                    return new Estado[] { };

                case Estado.Notificado1:
                    return new[] { Estado.Notificado2, Estado.Alertado, Estado.Anulado };
                case Estado.Notificado2:
                    return new[] { Estado.Notificado3, Estado.Alertado, Estado.Anulado };
                case Estado.Notificado3:
                    return new[] { Estado.Alertado, Estado.NoResuelta, Estado.Anulado };
                case Estado.RespuestaExitosa:
                    return new[] { Estado.Avisado, Estado.NoResuelta, Estado.Anulado };
                case Estado.RespuestaConRechazo: 
                    return new Estado[] { };
                case Estado.Rechazado: 
                    return new Estado[] { };
            }
            return new Estado[] { };
        }

        protected TicketRechazo() { }

        public TicketRechazo(string observacion, Empleado empleado, DateTime fechaHora)
        {
            if (empleado == null) throw new ArgumentNullException("empleado");

            Final = EstadoFinal.SolucionPendiente;

            var detalle = new DetalleTicketRechazo()
            {
                Estado = UltimoEstado = Estado.Pendiente,
                FechaHora = fechaHora,
                Observacion = observacion,
                Ticket = this,
                Empleado = empleado
            };

            FechaHora = fechaHora;

            Detalle.Add(detalle);
        }

        public virtual void ChangeEstado(Estado nuevoEstado, string observacion, Empleado empleado)
        {
            if (empleado == null) throw new ArgumentNullException("empleado");

            if (!Next(UltimoEstado).Any(e => e == nuevoEstado))
                throw new Exception(string.Format("Cambio de estado invalido {0} -> {1}", UltimoEstado, nuevoEstado));

            var detalle = new DetalleTicketRechazo
            {
                // Ultimo estado es calculado solo se actualiza en memoria 
                Estado = UltimoEstado = nuevoEstado,
                FechaHora = DateTime.UtcNow,
                Observacion = observacion,
                Ticket = this,
                Empleado = empleado
            };

            Detalle.Add(detalle);
        }

        public static string GetEstadoLabelVariableName(Estado estado)
        {
            switch (estado)
            {
                case Estado.Pendiente: return "PENDIENTE";
                case Estado.Notificado: return "NOTIFICADO";
                case Estado.NotificadoAutomatico: return "NOTIFICADO_AUTOMATICO";
                case Estado.Alertado: return "ALERTADO";
                case Estado.AlertadoAutomatico: return "ALERTADO_AUTOMATICO";
                case Estado.Resuelto: return "RESUELTO";
                case Estado.Anulado: return "ANULADO";
                case Estado.Avisado: return "AVISADO";
                case Estado.Entregado: return "ENTREGADO";
                case Estado.SinAviso: return "SIN_AVISO";
                case Estado.NoResuelta: return "NO_RESUELTA";
                case Estado.AltaErronea: return "ALTA_ERRONEA";
                case Estado.Duplicado: return "DUPLICADO";

                case Estado.Notificado1: return "NOTIFICADO_1";
                case Estado.Notificado2: return "NOTIFICADO_2";
                case Estado.Notificado3: return "NOTIFICADO_3";
                case Estado.RespuestaExitosa: return "RESPUESTA_EXITOSA";
                case Estado.RespuestaConRechazo: return "RESPUESTA_CON_RECHAZO";
                case Estado.Rechazado: return "RECHAZADO";                
                default: throw new ArgumentOutOfRangeException("estado");
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
        public virtual Estado UltimoEstado { get; protected set; }
        public virtual int Bultos { get; set; }

        private ISet<DetalleTicketRechazo> _detalles;

        public virtual Type TypeOf() { return GetType(); }

        public virtual ISet<DetalleTicketRechazo> Detalle
        {
            get { return _detalles ?? (_detalles = new HashSet<DetalleTicketRechazo>()); }
        }

        public virtual EstadoFinal Final { get; set; }
        public virtual bool EnHorario { get; set; }
        public virtual PuntoEntrega  Entrega { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual DateTime FechaHoraEstado { get; set; }
    }
}
