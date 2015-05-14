using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.CicloLogistico.Exceptions;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

namespace Logictracker.Process.CicloLogistico
{
    public class CicloLogisticoHormigon : CicloLogisticoBase
    {
        private const int MarginMinutes = 180;
        private const int EndMarginMinutes = 180;
        private const int InOutGecercaMinutes = 5;
        
        private Ticket Ticket { get; set; }
        private List<DetalleTicket> _detalles;
        private List<DetalleTicket> Detalles
        {
            get
            {
                return _detalles ?? (_detalles = Ticket.Detalles.OfType<DetalleTicket>()
                    .Where(d => d.EstadoLogistico != null)
                    .OrderBy(d => d.Programado.Value)
                    .ToList());
            }
        }

        private DetalleTicket _current;
        private DetalleTicket Current
        {
            get
            {
                return _current ?? (_current = Detalles.Where(e => !e.Automatico.HasValue && !e.Manual.HasValue).FirstOrDefault());
            }
        }
        
        protected override Coche Vehiculo { get { return Ticket.Vehiculo; } }
        protected override Empleado Empleado { get { return Ticket.Empleado; } }
        protected override DateTime MinDate { get { return Ticket.FechaTicket.Value.AddMinutes(-MarginMinutes); } }
        protected override DateTime MaxDate { get { return GetMaxDate().AddMinutes(EndMarginMinutes); } }

        public CicloLogisticoHormigon(Ticket ticket, DAOFactory daoFactory, IMessageSaver messageSaver)
            :base(daoFactory, messageSaver)
        {
            Ticket = ticket;
        }

        #region Process

        #region Inicio Ticket

        protected override void Process(InitEvent data)
        {
            if (Ticket.Vehiculo == null) throw new NoVehicleException();
            if (Ticket.Estado != Ticket.Estados.Pendiente) throw new AlreadyOpenException();

            FirstPosition();

            Ticket.Dispositivo = Ticket.Vehiculo.Dispositivo;

            var primerDetalle = Detalles.First();
            var primerEstado = primerDetalle.EstadoLogistico;
            var messageCode = primerEstado.Mensaje.Codigo;
            var needsSend = true;


            if (primerEstado.EsPuntoDeControl == Estado.Evento.Iniciado)
            {
                primerDetalle.Automatico = primerDetalle.Manual = data.Date;
                if (Detalles.Count >= 2) messageCode = Detalles[1].EstadoLogistico.Mensaje.Codigo;
                else needsSend = false;
                DaoFactory.DetalleTicketDAO.SaveOrUpdate(primerDetalle);
            }

            var sent = !needsSend || MessageSender.CreateSetWorkflowState(Ticket.Vehiculo.Dispositivo, MessageSaver).AddWorkflowState(messageCode).Send();

            if (!sent) throw new QueueException();

            Ticket.Estado = Ticket.Estados.EnCurso;
            DaoFactory.TicketDAO.SaveOrUpdate(Ticket);

            SaveMessage(MessageCode.CicloLogisticoIniciado.GetMessageCode(), data.Date);

            if (primerEstado.EsPuntoDeControl == Estado.Evento.Iniciado)
                SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), primerDetalle.EstadoLogistico.Descripcion, data.Date.AddSeconds(1));
        }

        #endregion

        #region Cierre Ticket
        protected override void Process(CloseEvent data)
        {
            Ticket.FechaFin = data.Date;
            Ticket.Estado = Ticket.Estados.Cerrado;
            DaoFactory.TicketDAO.SaveOrUpdate(Ticket);
            SaveMessage(MessageCode.CicloLogisticoCerrado.GetMessageCode(), data.Date.AddSeconds(1));
            ClearGeocercasCache();
        }
        #endregion

        #region Manual
        protected override void Process(ManualEvent data)
        {
            var detalle = Detalles.Where(d => d.EstadoLogistico.Mensaje.Codigo == data.Mensaje && !d.Manual.HasValue).FirstOrDefault();

            if (detalle == null) return;

            detalle.Manual = data.Date;

            DaoFactory.DetalleTicketDAO.SaveOrUpdate(detalle);
            DaoFactory.TicketDAO.SaveOrUpdate(Ticket);

            SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), detalle.EstadoLogistico.Descripcion + " (Manual)", data);
        }
        #endregion

        #region Georeference
        protected override void Process(GeofenceEvent data)
        {
            var isLinea = data.Id == Ticket.Linea.ReferenciaGeografica.Id;
            var isObra = data.Id == Ticket.PuntoEntrega.ReferenciaGeografica.Id;
            var isLlegada = Ticket.BaseDestino == null 
                ? !isObra 
                : data.Id == Ticket.BaseDestino.ReferenciaGeografica.Id;

            var entrada = data.Evento == GeofenceEvent.EventoGeofence.Entrada;
            var salida = data.Evento == GeofenceEvent.EventoGeofence.Salida;

            if (isLlegada && entrada) ProcessEntradaBase(data);
            if (isLinea && salida) ProcessSalidaBase(data);
            if (isObra && entrada) ProcessEntradaObra(data);
            if (isObra && salida) ProcessSalidaObra(data);
        }
        private void ProcessEntradaBase(GeofenceEvent data)
        {
            // Obtengo el estado a procesar
            var detalle = GetDetalle(data, Estado.Evento.LlegaAPlanta);
            if (detalle == null) return;

            // Si ya hay una fecha para sale de planta de menos de 5 minutos antes, tomo esta entrada como una "entrada-salida"
            var salida = Detalles.Where(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.SaleDePlanta && d.Automatico.HasValue).FirstOrDefault();
            if (salida != null && data.Date.Subtract(salida.Automatico.Value) < TimeSpan.FromMinutes(InOutGecercaMinutes))
            {
                return;
            }

            if (!detalle.Automatico.HasValue)
            {
                // Guardo la fecha del evento
                detalle.Automatico = data.Date;
                DaoFactory.DetalleTicketDAO.SaveOrUpdate(detalle);

                Ticket.FechaFin = data.Date;
                Ticket.BaseLlegada = DaoFactory.LineaDAO.GetList(new[] { Ticket.Empresa != null ? Ticket.Empresa.Id : Ticket.Linea.Empresa.Id })
                                                        .Where(lin => lin.ReferenciaGeografica != null && lin.ReferenciaGeografica.Id == data.Id)
                                                        .FirstOrDefault();
                DaoFactory.TicketDAO.SaveOrUpdate(Ticket);

                SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), detalle.EstadoLogistico.Descripcion, data);
                SaveMessageAtraso(data, detalle);
            }
        }
        private void ProcessSalidaBase(GeofenceEvent data)
        {
            // Obtengo el estado a procesar
            var detalle = GetDetalle(data, Estado.Evento.SaleDePlanta);
            if (detalle == null) return;

            if (!detalle.Automatico.HasValue)
            {
                // Guardo la fecha del evento
                detalle.Automatico = data.Date;
                DaoFactory.DetalleTicketDAO.SaveOrUpdate(detalle);

                if (!Ticket.FechaDescarga.HasValue)
                    Ticket.FechaDescarga = data.Date;

                DaoFactory.TicketDAO.SaveOrUpdate(Ticket);

                SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), detalle.EstadoLogistico.Descripcion, data);
                SaveMessageAtraso(data, detalle);
            }
            else
            {
                // Si ya hay una fecha para el mismo evento analizo si la anterior es una "posicion loca"
                var entrada = Detalles.Where(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.LlegaAPlanta && d.Automatico.HasValue).FirstOrDefault();
                if (entrada != null && entrada.Automatico.Value.Subtract(detalle.Automatico.Value) < TimeSpan.FromMinutes(InOutGecercaMinutes))
                {
                    // Si el evento anterior era por una "posicion loca" guardo el nuevo horario
                    // y elimino el otro valor.
                    detalle.Automatico = data.Date;
                    entrada.Automatico = null;
                    DaoFactory.DetalleTicketDAO.SaveOrUpdate(detalle);
                    DaoFactory.DetalleTicketDAO.SaveOrUpdate(entrada);

                    if (!Ticket.FechaDescarga.HasValue) Ticket.FechaDescarga = data.Date;
                    DaoFactory.TicketDAO.SaveOrUpdate(Ticket);

                    SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), detalle.EstadoLogistico.Descripcion + " (Corrección)", data);
                    SaveMessageAtraso(data, detalle);
                }
            }
        }

        private void ProcessEntradaObra(GeofenceEvent data)
        {
            // Obtengo el estado a procesar
            var detalle = GetDetalle(data, Estado.Evento.LlegaAObra);
            if (detalle == null) return;

            if (!detalle.Automatico.HasValue)
            {
                // Guardo la fecha del evento
                detalle.Automatico = data.Date;
                DaoFactory.DetalleTicketDAO.SaveOrUpdate(detalle);

                SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), detalle.EstadoLogistico.Descripcion, data);
                SaveMessageAtraso(data, detalle);
            }
            else
            {
                var salida = Detalles.Where(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.SaleDeObra && d.Automatico.HasValue).FirstOrDefault();
                if (salida != null)
                {
                    // Si ya hay una fecha para el evento de salida, la borro: la salida siempre es la ultima.
                    salida.Automatico = null;
                    DaoFactory.DetalleTicketDAO.SaveOrUpdate(salida);
                }
            }
        }
        private void ProcessSalidaObra(GeofenceEvent data)
        {
            // Obtengo el estado a procesar
            var detalle = GetDetalle(data, Estado.Evento.SaleDeObra);
            if (detalle == null) return;

            // Guardo la fecha del evento
            detalle.Automatico = data.Date;
            DaoFactory.DetalleTicketDAO.SaveOrUpdate(detalle);

            SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), detalle.EstadoLogistico.Descripcion, data);
            SaveMessageAtraso(data, detalle);
        }

        #endregion

        #region Tolva
        protected override void Process(TolvaEvent data)
        {
            var estado = data.Estado == TolvaEvent.EstadoTolva.On
                             ? Estado.Evento.TolvaActiva
                             : Estado.Evento.TolvaInactiva;

            // Obtengo el estado a procesar
            var detalle = GetDetalle(data, estado);
            if (detalle == null) return;

            if (!IsInObra(data)) return;

            if (data.Estado == TolvaEvent.EstadoTolva.Off || !detalle.Automatico.HasValue)
            {
                // Guardo la fecha del evento
                detalle.Automatico = data.Date;
                DaoFactory.DetalleTicketDAO.SaveOrUpdate(detalle);

                SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), detalle.EstadoLogistico.Descripcion, data);
                SaveMessageAtraso(data, detalle);
            }
        }

        #endregion

        #region Trompo

        protected override void Process(TrompoEvent data)
        {
            short sentido;
            switch (data.Sentido)
            {
                case TrompoEvent.SentidoTrompo.Detenido: sentido = Estado.Evento.GiroTrompoDetenido; break;
                case TrompoEvent.SentidoTrompo.HorarioDerecha:
                    switch (data.Speed)
                    {
                        case TrompoEvent.VelocidadTrompo.Undefined: sentido = Estado.Evento.GiroTrompoDerecha; break;
                        case TrompoEvent.VelocidadTrompo.Slow: sentido = Estado.Evento.GiroTrompoHorarioLento; break;
                        case TrompoEvent.VelocidadTrompo.Fast: sentido = Estado.Evento.GiroTrompoHorarioRapido; break;
                        default: return;
                    }
                    break;
                case TrompoEvent.SentidoTrompo.AntihorarioIzquierda:
                    switch (data.Speed)
                    {
                        case TrompoEvent.VelocidadTrompo.Undefined: sentido = Estado.Evento.GiroTrompoIzquierda; break;
                        case TrompoEvent.VelocidadTrompo.Slow: sentido = Estado.Evento.GiroTrompoAntihorarioLento; break;
                        case TrompoEvent.VelocidadTrompo.Fast: sentido = Estado.Evento.GiroTrompoAntihorarioRapido; break;
                        default: return;
                    }
                    break;
                default: return;
            }

            // Obtengo el estado a procesar
            var detalle = GetDetalle(data, sentido);
            if (detalle == null) return;

            if (!IsInObra(data)) return;

            if (!detalle.Automatico.HasValue)
            {
                // Guardo la fecha del evento
                detalle.Automatico = data.Date;
                DaoFactory.DetalleTicketDAO.SaveOrUpdate(detalle);

                SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), detalle.EstadoLogistico.Descripcion, data);
                SaveMessageAtraso(data, detalle);
            }
        }

        #endregion

        #region Position
        protected override void Process(PositionEvent data)
        {
            ProcessGeocercas(data);
        }
        #endregion 

        #endregion

        #region AutoClose

        protected override void AutoCloseTicket()
        {
            // Si pasaron mas de EndMarginMinutes horas desde la hora final del ticket, lo cierro.
            var maxdate = GetMaxDate();
            var close = maxdate.AddMinutes(EndMarginMinutes) < DateTime.UtcNow;

            // Si el ultimo evento del ciclo ya fue procesado y está configurado como automático, cierro el ticket.
            var ultimoDetalle = Detalles.Last();
            close |= ultimoDetalle.Automatico.HasValue && ultimoDetalle.EstadoLogistico.Modo;
            close |= ultimoDetalle.Manual.HasValue && !ultimoDetalle.EstadoLogistico.Modo;

            if (close)
            {
                Ticket.FechaFin = DateTime.UtcNow;
                Ticket.Estado = Ticket.Estados.Cerrado;
                Ticket.UserField3 += "(auto cerrado "+EndMarginMinutes+"min)";
                
                DaoFactory.TicketDAO.SaveOrUpdate(Ticket);
                SaveMessage(MessageCode.CicloLogisticoCerrado.GetMessageCode(), Ticket.FechaFin.Value);
                ClearGeocercasCache();
            }
        } 
        
        #endregion

        
        protected override bool IgnoreEvent(IEvent data)
        {
            // Descarto si no hay detalles a procesar
            if (Detalles.Count == 0) return true;

            // Si esta eliminado, no se procesa un carajo
            if (Ticket.Estado == Ticket.Estados.Eliminado) return true;

            // Descarto si el evento es anterior al ticket
            if (data.Date < Ticket.FechaTicket.Value.AddMinutes(-MarginMinutes)) return true;

            // Descarto si el evento es posterior al ticket
            var maxDate = GetMaxDate();
            if (data.Date > maxDate.AddMinutes(EndMarginMinutes)) return true;

            return false;
        }

        #region Geocercas
        protected override IEnumerable<int> Geocercas
        {
            get
            {
                var geo = new List<int>(10);
                if (Ticket.BaseDestino == null)
                {
                    geo = DaoFactory.LineaDAO.GetList(new[] { Ticket.Empresa.Id }).Select(lin => lin.ReferenciaGeografica.Id).ToList();
                }
                else
                {
                    var salida = Ticket.Linea.ReferenciaGeografica != null ? Ticket.Linea.ReferenciaGeografica.Id : 0;
                    var llegada = Ticket.BaseDestino.ReferenciaGeografica != null ? Ticket.BaseDestino.ReferenciaGeografica.Id : 0;
                    if (salida > 0)
                    {
                        geo.Add(salida);
                    }
                    if (llegada > 0 && llegada != salida)
                    {
                        geo.Add(llegada);
                    }
                    
                }
                if (Ticket.PuntoEntrega.Nomenclado) geo.Add(Ticket.PuntoEntrega.ReferenciaGeografica.Id);
                return geo;
            }
        }

        protected override IEnumerable<int> Puntos
        {
            get { return new List<int>(); }
        }

        protected override string GetKeyGeocerca(int geocerca)
        {
	        var vid = 0;
			if (Ticket.Vehiculo != null) vid = Ticket.Vehiculo.Id;
            return String.Concat("Ticket[", vid, ",", geocerca, "]");
        } 
        #endregion

        #region Helper Functions
        
        private DateTime GetMaxDate()
        {
            var lastDetalle = Detalles.Last();
            var maxDate = lastDetalle.Programado.Value;
            if (lastDetalle.Automatico.HasValue && maxDate < lastDetalle.Automatico.Value) maxDate = lastDetalle.Automatico.Value;
            return maxDate;
        }
        private DateTime GetPrevMaxDate(DetalleTicket detalle)
        {
            var detallesPrevios = Detalles.Where(d => d.Programado < detalle.Programado && d.Automatico.HasValue);
            return detallesPrevios.Count() > 0 ? detallesPrevios.Max(d => d.Automatico.Value) : DateTime.MinValue;
        }
        private DateTime GetPostMinDate(DetalleTicket detalle)
        {
            var detallesPrevios = Detalles.Where(d => d.Programado > detalle.Programado && d.Automatico.HasValue);
            return detallesPrevios.Count() > 0 ? detallesPrevios.Min(d => d.Automatico.Value) : DateTime.MaxValue;
        }

        private bool IsInObra(IEvent data)
        {
            if (Ticket.PuntoEntrega == null || Ticket.PuntoEntrega.ReferenciaGeografica == null) return false;
            var pol = Ticket.PuntoEntrega.ReferenciaGeografica.Poligono;
            if (pol == null || !pol.Contains(data.Latitud, data.Longitud)) return false;
            return true;
        }

        private DetalleTicket GetDetalle(IEvent data, short evento)
        {
            // Obtengo el estado a procesar
            var detalle = Detalles.Where(d => d.EstadoLogistico.EsPuntoDeControl == evento).FirstOrDefault();

            // Si no hay estados de este tipo descarto el evento
            if (detalle == null) return null;

            // Si la fecha del evento no corresponde al rango horario que determinan los otros eventos, descarto el evento 
            if (data.Date < GetPrevMaxDate(detalle) || data.Date > GetPostMinDate(detalle)) return null;

            return detalle;
        } 
        
        #endregion

        #region SaveMessage
        private void SaveMessageAtraso(IEvent data, DetalleTicket detalle)
        {
            if (!detalle.Programado.HasValue) return;
            var atraso = data.Date.Subtract(detalle.Programado.Value).TotalMinutes;
            // Si se atraso más del margen de minutos genero un evento
            if (atraso > MarginMinutes)
            {
                SaveMessage(MessageCode.AtrasoTicket.GetMessageCode(), Convert.ToInt32(atraso) + "min", new GPSPoint(data.Date, (float)data.Latitud, (float)data.Longitud), data.Date);
            }
        } 
        #endregion

        #region State & Progress

        public override string CurrentState
        {
            get
            {
                return Current != null ? Current.EstadoLogistico.Codigo.ToString() : "";
            }
        }

        private int _currentStateCompleted = -1;
        public override int CurrentStateCompleted
        {
            get
            {
                if (_currentStateCompleted == -1)
                {
                    DetalleTicket cur = Current;
                    DetalleTicket prev = null;

                    foreach (var detalle in Detalles)
                    {
                        if (detalle == Current) break;
                        if (prev == null || detalle.Automatico.HasValue) prev = detalle;
                    }

                    var retraso = prev != null && prev.Automatico.HasValue
                                      ? prev.Automatico.Value.Subtract(prev.Programado.Value).TotalMinutes
                                      : 0;

                    var duracion = prev != null ? cur.Programado.Value.Subtract(prev.Programado.Value).TotalMinutes : 0;

                    var parcial = prev != null
                        ? DateTime.UtcNow.AddMinutes(retraso).Subtract(prev.Programado.Value).TotalMinutes
                        : (DateTime.UtcNow.AddMinutes(retraso).Subtract(cur.Programado.Value).TotalMinutes);
                    if (parcial < 0) parcial = duracion - parcial;

                    _currentStateCompleted = duracion > 0
                                         ? Math.Min(Convert.ToInt32(parcial * 100 / duracion), 100)
                                         : 0;
                    _delay = Convert.ToInt32(retraso);
                }
                return _currentStateCompleted;
            }
        }

        private int _totalCompleted = -1;
        public override int TotalCompleted
        {
            get
            {
                if (_totalCompleted == -1)
                {
                    var numStates = Detalles.Count;
                    var doneStates = Detalles.TakeWhile(detalle => detalle != Current).Count();
                    var current = CurrentStateCompleted;
                    var statePercent = 100.0 / numStates;

                    _totalCompleted = Convert.ToInt32((statePercent * doneStates) + (statePercent * current / 100));
                }
                return _totalCompleted;
            }
        }

        private int _delay;
        public override int Delay
        {
            get
            {
                var cur = CurrentStateCompleted;
                return _delay;
            }
        } 

        #endregion

        #region Reporte Retrasos

        private DateTime? _enGeocercaDesde;
        public override bool EnGeocerca
        {
            get
            {
                var estadoEntrada = Detalles.Where(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.LlegaAObra).FirstOrDefault();
                if (estadoEntrada == null || !estadoEntrada.Automatico.HasValue) return false;
                var estadoSalida = Detalles.Where(d => d.EstadoLogistico.EsPuntoDeControl == Estado.Evento.SaleDeObra).FirstOrDefault();
                if (estadoSalida == null || !estadoSalida.Automatico.HasValue)
                {
                    _enGeocercaDesde = estadoEntrada.Automatico.Value;
                    return true;
                }
                return false;
            }
        }
        public override DateTime? EnGeocercaDesde
        {
            get { return EnGeocerca ? _enGeocercaDesde : null; }
        }
        public override DateTime Iniciado
        {
            get { return Detalles.FirstOrDefault().Automatico.Value; }
        }
        public override string Interno
        {
            get { return Vehiculo != null ? Vehiculo.Interno : string.Empty; }
        }
        public override string Codigo
        {
            get { return Ticket != null ? Ticket.Codigo : string.Empty; }
        }
        public override string Cliente
        {
            get { return Ticket != null && Ticket.Cliente != null ? Ticket.Cliente.Descripcion : string.Empty; }
        }
        public override string Telefono
        {
            get { return Ticket != null && Ticket.Cliente != null ? Ticket.Cliente.Telefono : string.Empty; }
        }
        public override string PuntoEntrega
        {
            get { return Ticket != null && Ticket.PuntoEntrega != null ? Ticket.PuntoEntrega.Descripcion : string.Empty; }
        }

        #endregion
    }
}
