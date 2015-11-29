using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class EntregaDistribucion: IAuditable, IHasViajeDistribucion
    {
        public static class Estados
        {
            public const short Cancelado = -1;
            public const short Pendiente = 0;//api
            public const short EnSitio = 1;
            public const short Visitado = 2;
            public const short SinVisitar = 3;
            public const short EnZona = 4;
            public const short NoCompletado = 8; //api
            public const short Completado = 9;//api
            public const short Restaurado = -19;//api
            // Si se agregan estados aca, hay que agregarlos en BindingManager.EstadosEntregaDistribucion

            public static string GetLabelVariableName(short estado)
            {
                switch(estado)
                {
                    case Cancelado: return "ENTREGA_STATE_CANCELADO"; 
                    case Pendiente: return "ENTREGA_STATE_PENDIENTE";
                    case EnSitio: return "ENTREGA_STATE_ENSITIO";
                    case Visitado: return "ENTREGA_STATE_VISITADO";
                    case SinVisitar: return "ENTREGA_STATE_SINVISITAR";
                    case EnZona: return "ENTREGA_STATE_ENZONA";
                    case NoCompletado: return "ENTREGA_STATE_NOCOMPLETADO";
                    case Completado: return "ENTREGA_STATE_COMPLETADO";
                    
                    default: return "ENTREGA_STATE_PENDIENTE";
                }
            }

            public static List<short> EstadosOk { get { return new List<short> { Completado, Visitado, EnSitio, EnZona }; } }
            public static List<short> EstadosFinales { get { return new List<short> { Completado, Visitado, Cancelado }; } }

            public static List<int> TodosEstados
            {
                get { return new List<int> { Completado, Visitado, Cancelado, EnSitio, EnZona, Pendiente, SinVisitar, NoCompletado}; }
            }
        }

        public virtual int Id { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual ViajeDistribucion Viaje { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual PuntoEntrega PuntoEntrega { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual int Orden { get; set; }
        public virtual DateTime Programado { get; set; }
        public virtual DateTime ProgramadoHasta { get; set; }
        public virtual DateTime? Manual { get; set; }
        public virtual DateTime? Entrada { get; set; }
        public virtual DateTime? Salida { get; set; }
        public virtual short Estado { get; set; }
        public virtual TipoServicioCiclo TipoServicio { get; set; }
        public virtual int Bultos { get; set; }
        public virtual double Volumen { get; set; }
        public virtual double Peso { get; set; }
        public virtual double Valor { get; set; }

        public virtual string Comentario1 { get; set; }
        public virtual string Comentario2 { get; set; }
        public virtual string Comentario3 { get; set; }

        public virtual double? KmGps { get; set; }
        public virtual double? KmCalculado { get; set; }
        public virtual double? KmControlado { get; set; }

        public virtual DateTime? RecepcionConfirmacion { get; set; }
        public virtual DateTime? LecturaConfirmacion { get; set; }
        public virtual LogMensaje MensajeConfirmacion { get; set; }

        public virtual DateTime? GarminUnreadInactiveAt { get; set; }
        public virtual DateTime? GarminReadInactiveAt { get; set; }

        public virtual DateTime? GarminETA { get; set; }
        public virtual DateTime? GarminETAInformedAt { get; set; }

        public virtual ReferenciaGeografica ReferenciaGeografica
        {
            get { return PuntoEntrega != null ? PuntoEntrega.ReferenciaGeografica : Linea.ReferenciaGeografica; }
        }

        Type IAuditable.TypeOf() { return GetType(); }

        public virtual DateTime ManualOSalida
        {
            get
            {
                return Manual.HasValue
                           ? Manual.Value
                           : Salida.HasValue
                                 ? Salida.Value
                                 : Entrada.HasValue
                                       ? Entrada.Value
                                       : Programado;
            }
        }
        public virtual DateTime ManualOEntrada
        {
            get
            {
                return Manual.HasValue
                           ? Manual.Value
                           : Entrada.HasValue
                                 ? Entrada.Value
                                 : Salida.HasValue
                                       ? Salida.Value
                                       : Programado;
            }
        }
        public virtual DateTime FechaMin
        {
            get
            {
                var fechaMin = DateTime.MaxValue;
                if (Manual.HasValue) fechaMin = Manual.Value;
                if (Entrada.HasValue && Entrada.Value < fechaMin) fechaMin = Entrada.Value;
                if (Salida.HasValue && Salida.Value < fechaMin) fechaMin = Salida.Value;

                return fechaMin;
            }
        }

        public virtual DateTime? SalidaOManualExclusiva
        {
            get
            {
                return Salida.HasValue
                           ? Salida.Value
                           : Manual.HasValue
                                 ? Manual.Value
                                 : (DateTime?)null;
            }
        }
        public virtual DateTime? EntradaOManualExclusiva
        {
            get
            {
                return Entrada.HasValue
                           ? Entrada.Value
                           : Manual.HasValue
                                 ? Manual.Value
                                 : (DateTime?)null;
            }
        }

        private IList<EvenDistri> _eventos;
        public virtual IList<EvenDistri> EventosDistri
        {
            get { return _eventos ?? (_eventos = new List<EvenDistri>()); }
            set { _eventos = value; }
        }

        public virtual IEnumerable<LogMensaje> GetEventos()
        {
            return EventosDistri.Select(e => e.LogMensaje);
        }
    }
}
