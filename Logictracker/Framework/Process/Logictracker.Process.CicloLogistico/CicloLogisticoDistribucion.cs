using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Logictracker.Cache;
using Logictracker.Configuration;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.CicloLogistico.Exceptions;
using Logictracker.Process.Geofences.Classes;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using Logictracker.AVL.Messages;
using Logictracker.Types.BusinessObjects.Rechazos;
using Logictracker.Tracker.Application.Integration;

namespace Logictracker.Process.CicloLogistico
{
    public class CicloLogisticoDistribucion : CicloLogisticoBase
    {
        private ViajeDistribucion Distribucion { get; set; }

        private List<EntregaDistribucion> _entregas;

        private EntregaDistribucion _current;
        private EntregaDistribucion Current
        {
            get
            {
				return _current ?? (_current = Entregas.FirstOrDefault(e => e.Estado == EntregaDistribucion.Estados.EnSitio));
            }
        }

        private string DesvioCacheKey { get { return string.Concat("(",Distribucion.Id, ").Desvio"); } }
        
        protected override Coche Vehiculo { get { return Distribucion.Vehiculo; } }
        protected override Empleado Empleado { get { return Distribucion.Empleado; } }
        protected override DateTime MinDate { get { return (Distribucion.InicioReal.HasValue ? Distribucion.InicioReal.Value : Distribucion.Inicio).AddMinutes(-Distribucion.Empresa.MarginMinutes); } }
        protected override DateTime MaxDate { get { return Distribucion.Fin.AddMinutes(Distribucion.Empresa.EndMarginMinutes); } }

        public CicloLogisticoDistribucion(ViajeDistribucion distribucion, DAOFactory daoFactory, IMessageSaver messageSaver)
            : base(daoFactory, messageSaver)
        {
            Distribucion = distribucion;
        }

        #region Process

        #region Inicio Ticket

        protected override void Process(InitEvent data)
        {
            if (Distribucion.Vehiculo == null) throw new NoVehicleException();
            if (Distribucion.Estado != ViajeDistribucion.Estados.Pendiente) throw new AlreadyOpenException();

            FirstPosition();

            try
            {
                var destinations = Distribucion.Detalles.Where(d => d.PuntoEntrega != null 
                                                                 && d.ReferenciaGeografica != null 
                                                                 && Math.Abs(d.ReferenciaGeografica.Latitude) < 90
                                                                 && Math.Abs(d.ReferenciaGeografica.Longitude) < 180)
                    .Select(d => new Destination(d.Id, new GPSPoint(data.Date,
                                                                    (float) d.ReferenciaGeografica.Latitude,
                                                                    (float) d.ReferenciaGeografica.Longitude),
                                                 d.Descripcion,
                                                 d.PuntoEntrega.Descripcion,
                                                 d.ReferenciaGeografica.Direccion.Descripcion))
                    .ToArray();

                switch (Distribucion.Vehiculo.Empresa.OrdenRutaGarmin)
                {
                    case Empresa.OrdenRuta.DescripcionAsc:
                        destinations = destinations.OrderBy(d => d.Text).ToArray();
                        break;
                    case Empresa.OrdenRuta.DescripcionDesc:
                        destinations = destinations.OrderByDescending(d => d.Text).ToArray();
                        break;
                }

                var expiration = Distribucion.Fin.AddMinutes(Distribucion.Empresa.EndMarginMinutes);

                var msg = MessageSender.CreateLoadRoute(Distribucion.Vehiculo.Dispositivo, MessageSaver)
                                       .AddRouteId(Distribucion.Id)
                                       .AddDestinations(destinations)
                                       .AddExpiration(expiration);

                if (Distribucion.Tipo == ViajeDistribucion.Tipos.Desordenado)
                {
                    var ordenar = Distribucion.Empresa.GetParameter(Empresa.Params.CicloDistribucionOrdenar);
                    if (ordenar != null && ordenar.ToLower() == "true") msg.AddParameter("sort", "true");
                }

                var sent = msg.Send();
                if (!sent) throw new QueueException();
            }
            catch (Exception ex)
            {
                STrace.Exception("ViajeDistribucion Inicio", ex, Distribucion.Vehiculo.Dispositivo.Id);
            }

            Distribucion.InicioReal = data.Date;
            Distribucion.Estado = ViajeDistribucion.Estados.EnCurso;

            if (Distribucion.ProgramacionDinamica)
            {
                var delta = data.Date.Subtract(Distribucion.Inicio);
                foreach (var entrega in Distribucion.Detalles)
                {
                    entrega.Programado = entrega.Programado.Add(delta);
                    entrega.ProgramadoHasta = entrega.ProgramadoHasta.Add(delta);
                }
            }

            DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(Distribucion);
            SaveMessage(MessageCode.CicloLogisticoIniciado.GetMessageCode(), data.Date, Distribucion, null);

            var docs = DaoFactory.DocumentoDAO.FindByVehiculo(Distribucion.Vehiculo.Id);
            if (docs.Count() > 0 && docs.Any(d => d.EnviadoAviso3))
                SaveMessage(MessageCode.CicloLogisticoIniciadoDocumentosInvalidos.GetMessageCode(), data.Date, Distribucion, null);
        }

        #endregion

        #region Cierre Ticket

        protected override void Process(CloseEvent data)
        {
            if (Distribucion.Estado != ViajeDistribucion.Estados.EnCurso)
            {
                STrace.Debug(GetType().FullName, Distribucion.Vehiculo.Dispositivo.Id, "Se esta intentando cerrar un Ciclo Logistico de Distribucion que no esta iniciado");
                return;
            }

            Distribucion.Fin = data.Date;
            Distribucion.Estado = ViajeDistribucion.Estados.Cerrado;
            DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(Distribucion);

            foreach (var detalle in Distribucion.Detalles.Where(x => x.Estado == EntregaDistribucion.Estados.Pendiente))
            {
                detalle.Estado = detalle.Viaje.Empresa.EstadoCierreDistribucion;
                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
            }
            foreach (var estadoCumplido in Distribucion.EstadosCumplidos)
            {
                if (!estadoCumplido.Inicio.HasValue || !estadoCumplido.Fin.HasValue)
                {
                    Distribucion.EstadosCumplidos.Remove(estadoCumplido);
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(Distribucion);
                    DaoFactory.EstadoDistribucionDAO.Delete(estadoCumplido);
                }   
            }

            // Informar al dispositivo que cierre la distribución
            if (data.InformDevice)
            {
                var destinations = Distribucion.Detalles.Where(d => d.PuntoEntrega != null
                                                                 && d.ReferenciaGeografica != null
                                                                 && Math.Abs(d.ReferenciaGeografica.Latitude) < 90
                                                                 && Math.Abs(d.ReferenciaGeografica.Longitude) < 180)
                    .Select(d => new Destination(d.Id, new GPSPoint(data.Date,
                                                                    (float) d.ReferenciaGeografica.Latitude,
                                                                    (float) d.ReferenciaGeografica.Longitude),
                                                 d.Descripcion,
                                                 d.PuntoEntrega.Descripcion,
                                                 d.ReferenciaGeografica.Direccion.Descripcion))
                    .ToArray();

                var msg = MessageSender.CreateUnloadRoute(Distribucion.Vehiculo.Dispositivo, MessageSaver)
                                       .AddRouteId(Distribucion.Id)
                                       .AddDestinations(destinations);
                msg.Send();
            }

            SaveMessage(MessageCode.CicloLogisticoCerrado.GetMessageCode(), data.Date, Distribucion, null);
            ClearGeocercasCache();

            if (Distribucion.Vehiculo.Empresa.IntegrationServiceEnabled)
            {
                var iService = new IntegrationService(DaoFactory);
                iService.FinishReport(Distribucion);
            }

            if (Distribucion.Empresa.InicioDistribucionSiguienteAlCerrar)
            {
                var nuevaDistribucion = DaoFactory.ViajeDistribucionDAO.FindPendiente(new[] { Distribucion.Empresa.Id },
                                                                                      new[] { -1 }, new[] { Distribucion.Vehiculo.Id }, 
                                                                                      DateTime.Today,
                                                                                      DateTime.Today.AddDays(1));
                if (nuevaDistribucion != null)
                {
                    var ev = new InitEvent(data.Date);
                    var ciclo = new CicloLogisticoDistribucion(nuevaDistribucion, DaoFactory, new MessageSaver(DaoFactory));
                    ciclo.ProcessEvent(ev);
                }
            }
        }

        #endregion

        #region Georeference

        protected override void Process(GeofenceEvent data)
        {
            var entrada = data.Evento == GeofenceEvent.EventoGeofence.Entrada;
            var salida = data.Evento == GeofenceEvent.EventoGeofence.Salida;

            var dets = Distribucion.Detalles.Select((d, i) => new {Index = i, Detalle = d})
                                            .Where((x, i) => x.Detalle.ReferenciaGeografica.Id == data.Id);

            if (dets.Any())
            {
                foreach (var det in dets)
                {
                    var detalle = det != null ? det.Detalle : null;
                    var index = det != null ? det.Index : 0;

                    if (detalle != null)
                    {
                        if (detalle.Linea != null)
                        {
                            if (Distribucion.RegresoABase)
                            {
                                // Si es una entrada a base, no puede ser al registro de salida de base.
                                if (entrada)
                                {
                                    // Si salió de la base debe haber pasado un tiempo mínimo para registrar el regreso
                                    if (detalle.Salida.HasValue && data.Date < detalle.Salida.Value.AddMinutes(Distribucion.Empresa.MinutosMinimosDeViaje))
                                    {
                                        detalle.Estado = EntregaDistribucion.Estados.Pendiente;
                                        detalle.Entrada = null;
                                        detalle.Salida = null;
                                        var viaje = detalle.Viaje;                                        
                                        viaje.Estado = ViajeDistribucion.Estados.Pendiente;                                        
                                        viaje.InicioReal = null;
                                        DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
                                        return;
                                    }
                                    // Si no salió de la base y todas las entregas están pendientes, no registró el regreso
                                    if (!detalle.Salida.HasValue && detalle.Viaje.Detalles.All(d => d.Estado == EntregaDistribucion.Estados.Pendiente))
                                        return;

                                    detalle = Distribucion.Detalles.LastOrDefault(d => d.ReferenciaGeografica.Id == data.Id);
                                    if (detalle != null) index = Distribucion.Detalles.IndexOf(detalle);
                                }
                            }
                            else
                            {
                                // Si no regresa a base, las entradas a base no se procesan
                                if (detalle.Salida.HasValue && entrada)
                                {
                                    return;
                                }
                            }
                        }
                        // Se procesa:
                        //      si es desordenada
                        //      si es ordenada y no hay ninguno posterior con horario
                        //      si es la base y no regresa a base se procesa como ordenado
                        var process = Distribucion.Tipo == ViajeDistribucion.Tipos.Desordenado;
                        if (Distribucion.Tipo != ViajeDistribucion.Tipos.Desordenado || (detalle.Linea != null && !detalle.Viaje.RegresoABase))
                        {
                            process = Distribucion.Detalles.Where((d, i) => i > index).All(d => !d.Entrada.HasValue);
                        }
                        if (process)
                        {
                            DaoFactory.Session.Refresh(detalle);

                            if (entrada) ProcessEntrada(data, detalle);
                            if (salida) ProcessSalida(data, detalle);
                        }

                        if (detalle.Linea != null) break;
                    }
                }
            }
        }

        private void ProcessEntrada(GeofenceEvent data, EntregaDistribucion detalle)
        {
            if (!detalle.Entrada.HasValue)
            {
                var regresa = Distribucion.RegresoABase;
                var ultimo = detalle == Distribucion.Detalles.Last();
                var esbase = detalle.Linea != null;
                var pendiente = detalle.Estado == EntregaDistribucion.Estados.Pendiente;
                
                if (pendiente)
                {
                    if (regresa && ultimo && esbase)
                        detalle.Estado = EntregaDistribucion.Estados.Visitado;
                    else
                        detalle.Estado = EntregaDistribucion.Estados.EnSitio;    
                }

                // Guardo la fecha del evento
                detalle.Entrada = data.Date;
                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);

                SaveMessage(MessageCode.EstadoLogisticoCumplidoEntrada.GetMessageCode(), "(" + detalle.Orden + ") -> " + detalle.Viaje.Codigo + " - " + detalle.Descripcion, data, detalle.Viaje, detalle);
                SaveMessageAtraso(data, detalle);
            }
            else if (detalle.Salida.HasValue && !detalle.Viaje.Detalles.Any(entrega => entrega.Id != detalle.Id && entrega.Entrada.HasValue && entrega.Entrada.Value > detalle.Salida.Value))
            {
                // Si ya hay una fecha para el evento de salida, la borro: la salida siempre es la ultima.
                // Solamente si pasaron menos de 15 minutos desde la salida anterior.
                detalle.Salida = null;
                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
            }
            else if (!detalle.Salida.HasValue && detalle.Estado == EntregaDistribucion.Estados.EnSitio)
            {
                detalle.Entrada = data.Date;
                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);

                SaveMessage(MessageCode.EstadoLogisticoCumplidoEntrada.GetMessageCode(), "(" + detalle.Orden + ") -> " + detalle.Viaje.Codigo + " - " + detalle.Descripcion, data, detalle.Viaje, detalle);
                SaveMessageAtraso(data, detalle);
            }
        }

        private void ProcessSalida(GeofenceEvent data, EntregaDistribucion detalle)
        {
            if (!detalle.Salida.HasValue && detalle.Entrada.HasValue)
            {
                // Chequeo que hay detenciones para procesar la salida
                //var maxMonths = detalle.Viaje.Vehiculo.Empresa != null ? detalle.Viaje.Vehiculo.Empresa.MesesConsultaPosiciones : 3;
                //var detenciones = DaoFactory.LogMensajeDAO.GetByVehicleAndCodeWithSession(detalle.Viaje.Vehiculo.Id,
                //                                                               MessageCode.StoppedEvent.GetMessageCode(),
                //                                                               detalle.Entrada.Value, 
                //                                                               data.Date,
                //                                                               maxMonths);
                var duracion = data.Date.Subtract(detalle.Entrada.Value);

                //if (!detenciones.Any(d => d.Duracion > Distribucion.Empresa.SegundosMinimosEnDetencion))
                if (duracion.TotalSeconds < Distribucion.Empresa.SegundosMinimosEnDetencion)
                {
                    // Si no hay, borro la entrada y la entrega vuelve a Pendiente
                    if (detalle.Estado == EntregaDistribucion.Estados.EnSitio)
                    {
                        //detalle.Entrada = null;
                        //detalle.Estado = EntregaDistribucion.Estados.Pendiente;

                        //DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
                        return;
                    }
                }

                // Guardo la fecha del evento
                detalle.Salida = data.Date;

                if (detalle.Estado == EntregaDistribucion.Estados.Pendiente
                    || detalle.Estado == EntregaDistribucion.Estados.EnSitio)
                    detalle.Estado = EntregaDistribucion.Estados.Visitado;

                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
                SaveMessage(MessageCode.EstadoLogisticoCumplidoSalida.GetMessageCode(), "(" + detalle.Orden + ") -> " + detalle.Viaje.Codigo + " - " + detalle.Descripcion, data, detalle.Viaje, detalle);
                //SaveMessageAtraso(data, detalle);

                //EnviarAvisoSiguienteDestino(detalle);
            }
            else
            {
                var esBaseInicio = detalle == Distribucion.Detalles.First() && detalle.Linea != null;
                if (!detalle.Salida.HasValue && esBaseInicio)
                {
                    detalle.Salida = data.Date;

                    if (detalle.Estado == EntregaDistribucion.Estados.Pendiente
                        || detalle.Estado == EntregaDistribucion.Estados.EnSitio)
                        detalle.Estado = EntregaDistribucion.Estados.Visitado;

                    DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(detalle);
                    
                    SaveMessage(MessageCode.EstadoLogisticoCumplidoSalida.GetMessageCode(), "(" + detalle.Orden + ") -> " + detalle.Viaje.Codigo + " - " + detalle.Descripcion, data, detalle.Viaje, detalle);
                    
                    //EnviarAvisoSiguienteDestino(detalle);
                }
            }
        }

        private void EnviarAvisoSiguienteDestino(EntregaDistribucion detalle, DateTime ETA)
        {
            var parameters = new List<string> { ETA.Subtract(DateTime.UtcNow).ToString() };

            // Configuración del MailSender
            var configFile = Config.Mailing.MailingArriboConfiguration;
            var sender = new MailSender(configFile);

            // Obtengo el/los destinatarios
            var destinatariosSms = detalle.PuntoEntrega.Mail.Replace(',', ';');
            var destinos = destinatariosSms.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var destinatario in destinos.Where(destinatario => !string.IsNullOrEmpty(destinatario)))
            {
                // Envio mail
                sender.Config.ToAddress = destinatario.Trim();
                sender.SendMail(parameters.ToArray());
            }            
        }

        private void EnviarAvisoSiguienteDestino(EntregaDistribucion detalle)
        {
            // Obtengo el indice del evento en la coleccion
            var indice = Distribucion.Detalles.IndexOf(detalle);
            // Chequeo que haya más eventos en la coleccion
            if (indice + 1 >= Distribucion.EntregasTotalCountConBases) return;

            try
            {
                // Obtengo el siguiente evento
                var siguienteDetalle = Distribucion.Detalles[indice + 1];

                // Chequeo que tenga un punto de entrega y un mail definido
                if (siguienteDetalle.PuntoEntrega != null && !string.IsNullOrEmpty(siguienteDetalle.PuntoEntrega.Mail))
                {
                    // Calculo la distancia en Km entre los 2 puntos de entrega
                    var distanciaKm = Distancias.Loxodromica(detalle.ReferenciaGeografica.Latitude,
                                                             detalle.ReferenciaGeografica.Longitude,
                                                             siguienteDetalle.ReferenciaGeografica.Latitude,
                                                             siguienteDetalle.ReferenciaGeografica.Longitude) / 1000;
                    // Obtengo la velocidad promedio del vehiculo
                    var velocidadPromedio = Distribucion.Vehiculo.VelocidadPromedio != 0
                                                ? Distribucion.Vehiculo.VelocidadPromedio
                                                : Distribucion.Vehiculo.TipoCoche.VelocidadPromedio;
                    // Calculo los minutos que restan para la entrega del proximo pedido
                    var minutosEntrega = distanciaKm / velocidadPromedio * 60;

                    // Armo el mensaje
                    var parameters = new List<string> { minutosEntrega.ToString(CultureInfo.InvariantCulture) };

                    // Configuración del MailSender
                    var configFile = Config.Mailing.MailingArriboConfiguration;
                    var sender = new MailSender(configFile);

                    // Obtengo el/los destinatarios
                    var destinatariosSms = siguienteDetalle.PuntoEntrega.Mail.Replace(',', ';');
                    var destinos = destinatariosSms.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var destinatario in destinos.Where(destinatario => !string.IsNullOrEmpty(destinatario)))
                    {
                        // Envio mail
                        sender.Config.ToAddress = destinatario.Trim();
                        sender.SendMail(parameters.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                STrace.Exception(GetType().FullName, ex, Vehiculo.Dispositivo.Id,
                                 String.Format("Error enviando Mail al siguiente destino del Ciclo {0}",
                                               Distribucion.Codigo));
            }
        }

        #endregion

        #region Garmin

        protected override void Process(GarminEvent data)
        {
            if (Distribucion == null)
                STrace.Error(typeof(CicloLogisticoDistribucion).FullName, 0, "Distribucion NULL");                
            else
                if (Distribucion.Vehiculo == null)
                    STrace.Error(typeof(CicloLogisticoDistribucion).FullName, 0, String.Format("Distribucion {0}, Vehiculo NULL", Distribucion.Id));                
                else
                    if (Distribucion.Vehiculo.Dispositivo == null)
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, 0, String.Format("Distribucion {0}, Vehiculo {1}, Dispositivo NULL", Distribucion.Id, Distribucion.Vehiculo.Id));            

            if (data == null)
                STrace.Error(typeof(CicloLogisticoDistribucion).FullName, 0, "data NULL");            

            STrace.Debug(GetType().FullName, Distribucion.Vehiculo.Dispositivo.Id,
                         string.Format("Procesando evento Garmin. Ticket={0} Vehiculo={1} Id={2} Estado={3} Fecha={4}",
                                       Distribucion.Id, Distribucion.Vehiculo.Id, data.DetailId, data.Estado,
                                       data.Date.ToString("dd/MM/yyyy HH:mm:ss")));
            
            var detalle = Distribucion.Detalles.FirstOrDefault(d => d.Id == data.DetailId);
            
            //Se producian demasiados unloadroutes
            //if (detalle == null || detalle.Viaje.Estado != ViajeDistribucion.Estados.EnCurso)
            //{
            //    var destinations = detalle.Viaje.Detalles.Where(d => d.PuntoEntrega != null
            //                                                     && d.ReferenciaGeografica != null
            //                                                     && Math.Abs(d.ReferenciaGeografica.Latitude) < 90
            //                                                     && Math.Abs(d.ReferenciaGeografica.Longitude) < 180)
            //        .Select(d => new Destination(d.Id, new GPSPoint(data.Date,
            //                                                        (float)d.ReferenciaGeografica.Latitude,
            //                                                        (float)d.ReferenciaGeografica.Longitude),
            //                                     d.Descripcion,
            //                                     d.PuntoEntrega.Descripcion,
            //                                     d.ReferenciaGeografica.Direccion.Descripcion))
            //        .ToArray();
              
            //    var msg = MessageSender.CreateUnloadRoute(Distribucion.Vehiculo.Dispositivo, MessageSaver)
            //                            .AddRouteId(detalle.Viaje.Id)
            //                            .AddDestinations(destinations);
            //    msg.Send();
            //    return;
            //}

            if (detalle == null) return;

            DaoFactory.Session.Refresh(detalle);

            var destDetail = detalle.Descripcion + " (" + detalle.Orden + ")";

            switch (data.Estado)
            {
                case GarminEvent.StopStatus.Active:
                    Vehiculo.StoreActiveStop(detalle.Id);
                    var id = Vehiculo.GetActiveStop();
                    SaveMessage(MessageCode.GarminStopStatus.GetMessageCode(), ": " + destDetail + "(" + id + ")", data);
                    break;
                case GarminEvent.StopStatus.Deleted:
                    if (detalle.Manual.HasValue)
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, detalle.Viaje.Vehiculo.Dispositivo.Id, "Error generando pregunta: Ya se ha confirmado la realización de la entrega(" + destDetail + ")");
                        return;
                    }
                    if (detalle.Viaje.Estado == ViajeDistribucion.Estados.Cerrado)
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, detalle.Viaje.Vehiculo.Dispositivo.Id, "Error generando pregunta: El viaje " + detalle.Viaje.Codigo + " se encuentra cerrado.");
                        return;
                    }

                    detalle.Manual = data.Date;
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(detalle.Viaje);

                    SaveMessage(MessageCode.GarminStopStatus.GetMessageCode(), ", <b>se ha arribado al destino</b>: " + destDetail, data);
                    SaveMessage(MessageCode.GarminStopStatusDeleted.GetMessageCode(), ", <b>entrega eliminada del dispositivo remoto</b>: " + destDetail, data);

                    var cmt = DaoFactory.MensajeDAO.GetResponsesMessagesTable(Distribucion.Vehiculo.Dispositivo.Id, 0);
                    if ((cmt != null) && (cmt.Count > 0))
                    {
                        var msgText = "Por favor informe estado: " + destDetail;
                        var mensajes = cmt.Where(m => !m.TipoMensaje.DeEstadoLogistico).OrderBy(m => m.Codigo);

                        var replies = mensajes.Select(m => Convert.ToUInt32(m.Codigo)).ToArray();
                        var message = MessageSender.CreateSubmitCannedResponsesTextMessage(Distribucion.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));
                        message.AddMessageText(msgText).AddTextMessageId(Convert.ToUInt32(detalle.Id)).AddCannedResponses(replies).AddAckEvent(MessageCode.GarminCannedMessageReceived.GetMessageCode());

                        message.Send();
                    }
                    else
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, Distribucion.Vehiculo.Dispositivo.Id, "Error generando pregunta: No se han encontrado Canned Responses (" + destDetail + ")");
                    }
                    break;
                case GarminEvent.StopStatus.ReadInactive:
                    detalle.GarminReadInactiveAt = data.Date;
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(detalle.Viaje);
                    SaveMessage(MessageCode.GarminStopStatus.GetMessageCode(), ", el destino ha sido <b>LEIDO</b>: " + destDetail, data);
                    break;
                case GarminEvent.StopStatus.UnreadInactive:
                    if (!detalle.Viaje.Recepcion.HasValue || detalle.Viaje.Recepcion.Value > data.Date)
                        detalle.Viaje.Recepcion = data.Date;

                    detalle.GarminUnreadInactiveAt = data.Date;
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(detalle.Viaje);
                    SaveMessage(MessageCode.GarminStopStatus.GetMessageCode(), ", el destino ha sido <b>RECIBIDO</b> (sin leer aun): " + destDetail, data);
                    break;
                case GarminEvent.StopStatus.Done:
                    if (detalle.Manual.HasValue)
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, Distribucion.Vehiculo.Dispositivo.Id, "Error generando pregunta: Ya se ha confirmado la realización de la entrega(" + destDetail + ")");
                        return;
                    }

                    detalle.Manual = data.Date;
                    detalle.Estado = EntregaDistribucion.Estados.Completado;
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(detalle.Viaje);
                    
                    SaveMessage(MessageCode.GarminStopStatus.GetMessageCode(), ": " + destDetail, data);
                    
                    var descriptiva = "-> " + detalle.Viaje.Codigo + " - " + destDetail;
                    SaveMessage(MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode(), descriptiva, data, detalle.Viaje, detalle);
                    SaveMessageAtraso(data, detalle);

                    var dest = new Destination(detalle.Id, 
                                               new GPSPoint(data.Date,
                                                           (float)detalle.ReferenciaGeografica.Latitude,
                                                           (float)detalle.ReferenciaGeografica.Longitude),
                                               detalle.Descripcion,
                                               detalle.PuntoEntrega.Descripcion,
                                               detalle.ReferenciaGeografica.Direccion.Descripcion);

                    var ms = MessageSender.CreateUnloadStop(Distribucion.Vehiculo.Dispositivo, MessageSaver)
                                          .AddDestinations(new[] { dest });
                    ms.Send();

                    if (Distribucion.Vehiculo.Empresa.IntegrationServiceEnabled)
                    {
                        // Si no es la última entrega
                        if (Distribucion.Detalles.Last().Id != detalle.Id)
                        {
                            var intService = new IntegrationService(DaoFactory);
                            intService.ArrivalReport(Distribucion);
                        }
                    }
                    break;
                case EntregaDistribucion.Estados.Completado:
                case EntregaDistribucion.Estados.Cancelado:
                case EntregaDistribucion.Estados.NoCompletado:
                    detalle.Estado = data.Estado == EntregaDistribucion.Estados.Cancelado ? EntregaDistribucion.Estados.NoCompletado : data.Estado;
                    
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(detalle.Viaje);

                    try
                    {
                        if (detalle.PuntoEntrega != null && detalle.ReferenciaGeografica != null
                            && Math.Abs(detalle.ReferenciaGeografica.Latitude) < 90
                            && Math.Abs(detalle.ReferenciaGeografica.Longitude) < 180)
                        {
                            var destination = new Destination(detalle.Id, new GPSPoint(data.Date,
                                                                                       (float)detalle.ReferenciaGeografica.Latitude,
                                                                                       (float)detalle.ReferenciaGeografica.Longitude),
                                                              detalle.Descripcion,
                                                              detalle.PuntoEntrega.Descripcion,
                                                              detalle.ReferenciaGeografica.Direccion.Descripcion);

                            var msg = MessageSender.CreateUnloadStop(Distribucion.Vehiculo.Dispositivo, MessageSaver)
                                                   .AddDestinations(new[] {destination});
                            msg.Send();
                        }
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception(GetType().FullName, ex);
                    }
                    break;
                default:
                    return;
            }
        }

        protected override void Process(RouteEvent data)
        {
            // Valido que el evento sea para esta distribución.
            if (data.RouteId != Distribucion.Id)
            {
                STrace.Trace(GetType().FullName, Distribucion.Vehiculo.Dispositivo.Id,
                             String.Format(
                                 "El evento de ruta no corresponde a la ruta activa. Ruta={0} Vehiculo={1} IdRutaEvento={2} Estado={3} Fecha={4}",
                                 Distribucion.Id, Distribucion.Vehiculo.Id, data.RouteId, data.Estado,
                                 data.Date.ToString("dd/MM/yyyy HH:mm:ss")));
                return;
            }

            if (data.Estado == RouteEvent.Estados.Cancelado)
            {
                var close = EventFactory.GetCloseEvent(data.Date, false);
                Process(close as CloseEvent);
            }

            if (data.Estado == RouteEvent.Estados.Enviado)
            {
                Distribucion.Recepcion = data.Date;
                DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(Distribucion);
            }
        }

        #endregion

        #region Mobile

        protected override void Process(MobileEvent data)
        {
            if (Distribucion == null)
                STrace.Error(typeof(CicloLogisticoDistribucion).FullName, 0, "Distribucion NULL");
            else
                if (Distribucion.Vehiculo == null)
                    STrace.Error(typeof(CicloLogisticoDistribucion).FullName, 0, String.Format("Distribucion {0}, Vehiculo NULL", Distribucion.Id));
                else
                    if (Distribucion.Vehiculo.Dispositivo == null)
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, 0, String.Format("Distribucion {0}, Vehiculo {1}, Dispositivo NULL", Distribucion.Id, Distribucion.Vehiculo.Id));

            if (data == null)
                STrace.Error(typeof(CicloLogisticoDistribucion).FullName, 0, "data NULL");

            STrace.Debug(GetType().FullName, Distribucion.Vehiculo.Dispositivo.Id,
                         string.Format("Procesando evento Mobile. Ticket={0} Vehiculo={1} Id={2} Estado={3} Fecha={4}",
                                       Distribucion.Id, Distribucion.Vehiculo.Id, data.DetailId, data.Estado,
                                       data.Date.ToString("dd/MM/yyyy HH:mm:ss")));

            var detalle = Distribucion.Detalles.FirstOrDefault(d => d.Id == data.DetailId);

            if (detalle == null) return;

            DaoFactory.Session.Refresh(detalle);

            var destDetail = detalle.Descripcion + " (" + detalle.Orden + ")";
            
            var gpsPoint = new GPSPoint(data.Date,
                (float)detalle.ReferenciaGeografica.Latitude,
                (float)detalle.ReferenciaGeografica.Longitude);

            switch (data.Estado)
            {
                case EntregaDistribucion.Estados.Completado:
                    if (detalle.Manual.HasValue)
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, Distribucion.Vehiculo.Dispositivo.Id, "Error generando pregunta: Ya se ha confirmado la realización de la entrega(" + destDetail + ")");
                        return;
                    }
                  /*  if (detalle.Viaje.Estado == ViajeDistribucion.Estados.Cerrado && detalle.Viaje.Fin < data.Date)
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, detalle.Viaje.Vehiculo.Dispositivo.Id, "Error generando pregunta: El viaje " + detalle.Viaje.Codigo + " se encuentra cerrado.");
                        return;
                    }*/

                    detalle.Manual = data.Date;
                    detalle.Estado = EntregaDistribucion.Estados.Completado;
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(detalle.Viaje);

                    var messageDesc = DaoFactory.MensajeDAO.GetByCodigo(data.MessageId.ToString(), detalle.Viaje.Vehiculo.Dispositivo.Id);

                    var text = "***";
                    if (messageDesc != null) text = messageDesc.Descripcion;
                    
                    var descriptiva = ": " + detalle.Viaje.Codigo + " - " + destDetail + " -> " + text;
                    
                    SaveMessage(MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode(), descriptiva, data, detalle.Viaje, detalle);
                    SaveMessageAtraso(data, detalle);

                    var dest = new Destination(detalle.Id,
                                               gpsPoint,
                                               detalle.Descripcion,
                                               detalle.PuntoEntrega.Descripcion,
                                               detalle.ReferenciaGeografica.Direccion.Descripcion);

                    var ms = MessageSender.CreateUnloadStop(Distribucion.Vehiculo.Dispositivo, MessageSaver)
                                          .AddDestinations(new[] { dest });
                    ms.Send();
                    break;
                case EntregaDistribucion.Estados.Restaurado:
                    detalle.Manual = data.Date;
                    detalle.Estado = EntregaDistribucion.Estados.Completado;
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(detalle.Viaje);

                    SaveMessage(MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode(), ", <b>reactivada y confirmada<b>: " + destDetail, data);

                    var descripcion = "-> " + detalle.Viaje.Codigo + " - " + destDetail;
                    SaveMessage(MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode(), descripcion, data, detalle.Viaje, detalle);
                    SaveMessageAtraso(data, detalle);

                    var destiny = new Destination(detalle.Id,
                                               gpsPoint,
                                               detalle.Descripcion,
                                               detalle.PuntoEntrega.Descripcion,
                                               detalle.ReferenciaGeografica.Direccion.Descripcion);

                    var msg = MessageSender.CreateUnloadStop(Distribucion.Vehiculo.Dispositivo, MessageSaver).AddDestinations(new[] { destiny });
                    msg.Send();

                    break;
                case EntregaDistribucion.Estados.Pendiente:
                    detalle.Manual = null;
                    detalle.Estado = EntregaDistribucion.Estados.Pendiente;
                    DaoFactory.ViajeDistribucionDAO.Update(detalle.Viaje);

                    SaveMessage(MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode(), ", <b>reactivada y pendiente<b>: " + destDetail, data);

                    descripcion = "-> " + detalle.Viaje.Codigo + " - " + destDetail;
                    SaveMessage(MessageCode.EstadoLogisticoCumplidoManualRealizado.GetMessageCode(), descripcion, data, detalle.Viaje, detalle);
                    SaveMessageAtraso(data, detalle);

                    destiny = new Destination(detalle.Id,
                                               gpsPoint,
                                               detalle.Descripcion,
                                               detalle.PuntoEntrega.Descripcion,
                                               detalle.ReferenciaGeografica.Direccion.Descripcion);

                    msg = MessageSender.CreateUnloadStop(Distribucion.Vehiculo.Dispositivo, MessageSaver).AddDestinations(new[] { destiny });
                    msg.Send();

                    break;
                case EntregaDistribucion.Estados.Cancelado:
                case EntregaDistribucion.Estados.NoCompletado:
                    if (detalle.Manual.HasValue)
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, detalle.Viaje.Vehiculo.Dispositivo.Id, "Error generando pregunta: Ya se ha confirmado la realización de la entrega(" + destDetail + ")");
                        return;
                    }
                   /* if (detalle.Viaje.Estado == ViajeDistribucion.Estados.Cerrado)
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, detalle.Viaje.Vehiculo.Dispositivo.Id, "Error generando pregunta: El viaje " + detalle.Viaje.Codigo + " se encuentra cerrado.");
                        return;
                    }*/

                    detalle.Manual = data.Date;
                    detalle.Estado = EntregaDistribucion.Estados.NoCompletado;
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(detalle.Viaje);

                    var textMessage = DaoFactory.MensajeDAO.GetByCodigo(data.MessageId.ToString(), detalle.Viaje.Vehiculo.Dispositivo.Id);

                    var textMessageDesc = "***";
                    if (textMessage != null) textMessageDesc = textMessage.Descripcion;

                    var descriptiva2 = ": " + detalle.Viaje.Codigo + " - " + destDetail + " -> " + textMessageDesc;
                    
                    SaveMessage(MessageCode.EstadoLogisticoCumplidoManualNoRealizado.GetMessageCode(), descriptiva2, data, detalle.Viaje, detalle);
                    
                    //SaveMessage(MessageCode.EstadoLogisticoCumplido.GetMessageCode(), ", <b>se ha arribado al destino</b>: " + destDetail, data);
                    //SaveMessage(MessageCode.GarminStopStatusDeleted.GetMessageCode(), ", <b>entrega eliminada del dispositivo remoto</b>: " + destDetail, data);

                    var cmt = DaoFactory.MensajeDAO.GetResponsesMessagesTable(Distribucion.Vehiculo.Dispositivo.Id, 0);
                    if ((cmt != null) && (cmt.Count > 0))
                    {
                        var msgText = "Por favor informe estado: " + destDetail;
                        var mensajes = cmt.Where(m => !m.TipoMensaje.DeEstadoLogistico).OrderBy(m => m.Codigo);

                        var replies = mensajes.Select(m => Convert.ToUInt32(m.Codigo)).ToArray();
                        var message = MessageSender.CreateSubmitCannedResponsesTextMessage(Distribucion.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));
                        message.AddMessageText(msgText).AddTextMessageId(Convert.ToUInt32(detalle.Id)).AddCannedResponses(replies).AddAckEvent(MessageCode.GarminCannedMessageReceived.GetMessageCode());

                        message.Send();
                    }
                    else
                    {
                        STrace.Error(typeof(CicloLogisticoDistribucion).FullName, Distribucion.Vehiculo.Dispositivo.Id, "Error generando pregunta: No se han encontrado Canned Responses (" + destDetail + ")");
                    }

                    if (detalle.Viaje.Empresa.DistribucionGeneraRechazo)
                    {
                        STrace.Error("RECHAZO", "DistribucionGeneraRechazo");
                        var chofer = detalle.Viaje.Vehiculo.Chofer;
                        if (chofer != null)
                        {
                            try
                            {
                                var rechazo = new TicketRechazo(textMessageDesc, chofer, data.Date);
                                rechazo.Empresa = detalle.Viaje.Empresa;
                                rechazo.Linea = detalle.Viaje.Linea;
                                rechazo.Transportista = detalle.Viaje.Transportista ?? detalle.Viaje.Vehiculo.Transportista;
                                rechazo.Cliente = detalle.PuntoEntrega.Cliente;
                                rechazo.Entrega = detalle.PuntoEntrega;
                                rechazo.Bultos = detalle.Bultos;
                                var vendedor = detalle.PuntoEntrega.Responsable;
                                rechazo.Vendedor = vendedor;
                                var supervisorVenta = vendedor != null ? vendedor.Reporta1 : null;
                                rechazo.SupervisorVenta = supervisorVenta;
                                var supervisorRuta = supervisorVenta != null ? supervisorVenta.Reporta1 : null;
                                rechazo.SupervisorRuta = supervisorRuta;
                                rechazo.Motivo = (TicketRechazo.MotivoRechazo) data.MessageId;

                                STrace.Error("RECHAZO", "Guardando");                            
                                DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);

                                if (vendedor != null)
                                {
                                    var coche = DaoFactory.CocheDAO.FindByChofer(vendedor.Id);
                                    if (coche != null)
                                    {
                                        var mensajeVo = DaoFactory.MensajeDAO.GetByCodigo(data.MessageId.ToString(), coche.Empresa, coche.Linea);
                                        if (mensajeVo != null)
                                        {
                                            var mensaje = DaoFactory.MensajeDAO.FindById(mensajeVo.Id);

                                            var newEvent = new LogMensaje
                                            {
                                                Coche = coche,
                                                Chofer = vendedor,
                                                Dispositivo = coche.Dispositivo,
                                                Expiracion = data.Date.AddDays(1),
                                                Fecha = data.Date,
                                                FechaAlta = DateTime.UtcNow,
                                                FechaFin = data.Date,
                                                IdCoche = coche.Id,
                                                Latitud = data.Latitud,
                                                LatitudFin = data.Latitud,
                                                Longitud = data.Longitud,
                                                LongitudFin = data.Longitud,
                                                Mensaje = mensaje,
                                                Texto = "INFORME DE RECHAZO NRO " + rechazo.Id + ": " + mensaje.Descripcion + " -> " + rechazo.Entrega.Descripcion
                                            };

                                            DaoFactory.LogMensajeDAO.Save(newEvent);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                STrace.Exception("RECHAZO", ex);
                            }
                        }                        
                        else
                        {
                            STrace.Error("RECHAZO", "chofer == null");
                        }
                    }
                    break;
                default:
                    return;
            }

            SaveConfirmationMessage(gpsPoint, detalle, data.MessageId.ToString());

            try
            {
                if (detalle.PuntoEntrega != null && detalle.ReferenciaGeografica != null
                    && Math.Abs(detalle.ReferenciaGeografica.Latitude) < 90
                    && Math.Abs(detalle.ReferenciaGeografica.Longitude) < 180)
                {
                    var destination = new Destination(detalle.Id, new GPSPoint(data.Date,
                                                                               (float)detalle.ReferenciaGeografica.Latitude,
                                                                               (float)detalle.ReferenciaGeografica.Longitude),
                                                      detalle.Descripcion,
                                                      detalle.PuntoEntrega.Descripcion,
                                                      detalle.ReferenciaGeografica.Direccion.Descripcion);

                    var msg = MessageSender.CreateUnloadStop(Distribucion.Vehiculo.Dispositivo, MessageSaver)
                                           .AddDestinations(new[] { destination });
                    msg.Send();
                }
            }
            catch (Exception ex)
            {
                STrace.Exception(GetType().FullName, ex);
            }
        }


        #endregion

        #region Position

        protected override void Process(PositionEvent data)
        {
            //ProcessGeocercas(data);

            if (Distribucion.Tipo == ViajeDistribucion.Tipos.RecorridoFijo)
            {
                var segmentos = Distribucion.SegmentosRecorrido;
                if (!segmentos.Any()) return;

                var masCercano = segmentos.OrderBy(s => Math.Pow(s.Inicio.Latitud + s.Fin.Latitud - 2*data.Latitud, 2) 
                                                      + Math.Pow(s.Inicio.Longitud + s.Fin.Longitud - 2*data.Longitud, 2))
                                          .First();

                var dhi = Math.Sqrt(Math.Pow(data.Latitud - masCercano.Inicio.Latitud, 2)
                                  + Math.Pow(data.Longitud - masCercano.Inicio.Longitud, 2));
                var dhf = Math.Sqrt(Math.Pow(data.Latitud - masCercano.Fin.Latitud, 2)
                                  + Math.Pow(data.Longitud - masCercano.Fin.Longitud, 2));
                var per = dhi/(dhi + dhf);

                var latitud = masCercano.Inicio.Latitud + (masCercano.Fin.Latitud - masCercano.Inicio.Latitud)*per;
                var longitud = masCercano.Inicio.Longitud + (masCercano.Fin.Longitud - masCercano.Inicio.Longitud)*per;

                var metros = Distancias.Loxodromica(data.Latitud, data.Longitud, latitud, longitud);

                var estadoDesvio = EstadoDesvio;
                var nuevoEstadoDesvio = metros > Distribucion.Desvio
                                            ? EstadosGeocerca.Fuera
                                            : EstadosGeocerca.Dentro;
                if (estadoDesvio == EstadosGeocerca.Dentro && nuevoEstadoDesvio == EstadosGeocerca.Fuera)
                {
                    // Sale del recorrido
                    SaveMessage(MessageCode.DesvioRecorrido.GetMessageCode(), Distribucion.Codigo,
                                new GPSPoint(data.Date, (float) data.Latitud, (float) data.Longitud), data.Date);
                }
                else if (estadoDesvio == EstadosGeocerca.Fuera && nuevoEstadoDesvio == EstadosGeocerca.Dentro)
                {
                    // Entra al recorrido
                    SaveMessage(MessageCode.VueltaAlRecorrido.GetMessageCode(), Distribucion.Codigo,
                                new GPSPoint(data.Date, (float) data.Latitud, (float) data.Longitud), data.Date);
                }
                EstadoDesvio = nuevoEstadoDesvio;
            }
        }

        #endregion

        #region EstadoLogistico

        public void ProcessEstadoLogistico(Event evento, string code)
        {
            if (Distribucion.TipoCicloLogistico != null)
            {
                var estados = Distribucion.TipoCicloLogistico.Estados;
                var aIniciar = estados.Where(e => e.MensajeInicio != null && e.MensajeInicio.Codigo == code);
                var aCerrar = estados.Where(e => e.MensajeFin != null && e.MensajeFin.Codigo == code);

                foreach (var item in aCerrar)
                {
                    if (item.ControlInverso)
                    {
                        if (!item.Iterativo && Distribucion.EstadosCumplidos.Any(e => e.EstadoLogistico.Id == item.Id))
                            continue;

                        var inicio = DaoFactory.LogMensajeDAO.GetLastByVehicleAndCode(Distribucion.Vehiculo.Id, item.MensajeInicio.Codigo, Distribucion.InicioReal.Value, evento.GeoPoint.Date, 1);
                        if (inicio != null)
                        {
                            var estadoCumplido = new EstadoDistribucion();
                            estadoCumplido.EstadoLogistico = item;
                            estadoCumplido.Inicio = inicio.Fecha;
                            estadoCumplido.Fin = evento.GeoPoint.Date;
                            estadoCumplido.Viaje = Distribucion;
                            Distribucion.EstadosCumplidos.Add(estadoCumplido);
                        }
                    }
                    else
                    {
                        var abiertos = Distribucion.EstadosCumplidos.Where(ec => ec.EstadoLogistico.Id == item.Id && ec.Inicio.HasValue && !ec.Fin.HasValue);
                        foreach (var abierto in abiertos)
                        {
                            abierto.Fin = evento.GeoPoint.Date;
                        }
                    }
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(Distribucion);
                }

                foreach (var item in aIniciar)
                {
                    if (item.ControlInverso) continue;
                    if (!item.Iterativo && Distribucion.EstadosCumplidos.Any(e => e.EstadoLogistico.Id == item.Id)) continue;

                    var estadoCumplido = new EstadoDistribucion();
                    estadoCumplido.EstadoLogistico = item;
                    estadoCumplido.Inicio = evento.GeoPoint.Date;
                    estadoCumplido.Viaje = Distribucion;
                    Distribucion.EstadosCumplidos.Add(estadoCumplido);
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(Distribucion);
                }
            }
        }

        public void ProcessEstadoLogistico(string codigo, DateTime fecha, int idGeocerca)
        {
            if (Distribucion.TipoCicloLogistico != null)
            {
                var estados = Distribucion.TipoCicloLogistico.Estados;
                var tipoGeocerca = DaoFactory.ReferenciaGeograficaDAO.FindById(idGeocerca).TipoReferenciaGeografica;
                var aIniciar = estados.Where(e => e.MensajeInicio != null && e.MensajeInicio.Codigo == codigo);
                var aCerrar = estados.Where(e => e.MensajeFin != null && e.MensajeFin.Codigo == codigo);

                foreach (var item in aCerrar)
                {
                    if (item.TipoGeocercaFin != null && item.TipoGeocercaFin.Id != tipoGeocerca.Id) continue;

                    if (item.ControlInverso)
                    {
                        if (!item.Iterativo && Distribucion.EstadosCumplidos.Any(e => e.EstadoLogistico.Id == item.Id)) continue;

                        var inicio = DaoFactory.LogMensajeDAO.GetLastBySPVehicleAndCode(Distribucion.Vehiculo.Id, item.MensajeInicio.Codigo, Distribucion.InicioReal.Value, fecha);
                        if (inicio != null)
                        {
                            var estadoCumplido = new EstadoDistribucion();
                            estadoCumplido.EstadoLogistico = item;
                            estadoCumplido.Inicio = inicio.Fecha;
                            estadoCumplido.Fin = fecha;
                            estadoCumplido.Viaje = Distribucion;
                            Distribucion.EstadosCumplidos.Add(estadoCumplido);
                        }
                    }
                    else
                    {
                        var abiertos = Distribucion.EstadosCumplidos.Where(ec => ec.EstadoLogistico.Id == item.Id && ec.Inicio.HasValue && !ec.Fin.HasValue);
                        foreach (var abierto in abiertos)
                        {
                            abierto.Fin = fecha;
                        }
                    }

                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(Distribucion);
                }

                foreach (var item in aIniciar)
                {
                    if (item.ControlInverso) continue;
                    if (!item.Iterativo && Distribucion.EstadosCumplidos.Any(e => e.EstadoLogistico.Id == item.Id)) continue;
                    if (item.TipoGeocercaInicio != null && item.TipoGeocercaInicio.Id != tipoGeocerca.Id) continue;

                    var estadoCumplido = new EstadoDistribucion();
                    estadoCumplido.EstadoLogistico = item;
                    estadoCumplido.Inicio = fecha;
                    estadoCumplido.Viaje = Distribucion;
                    Distribucion.EstadosCumplidos.Add(estadoCumplido);
                    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(Distribucion);
                }
            }
        }

        #endregion

        #endregion

        #region AutoClose

        protected override void AutoCloseTicket()
        {
            if (Distribucion.Estado == ViajeDistribucion.Estados.Eliminado ||
                Distribucion.Estado == ViajeDistribucion.Estados.Anulado ||
                Distribucion.Estado == ViajeDistribucion.Estados.Cerrado)
                return;
            
            if (Distribucion.RegresoABase)
            {
                // Si termina en base y llegó a base lo cierro
                var ultimo = Distribucion.Detalles.Last(d => d.Linea != null);
                if (ultimo.Entrada.HasValue)
                {
                    CerrarDistribucion();
                    STrace.Trace("CierreCicloLogistico", Distribucion.Vehiculo != null && Distribucion.Vehiculo.Dispositivo != null ? Distribucion.Vehiculo.Dispositivo.Id : 0, string.Format("Viaje {0} a cerrar por regreso a base.", Distribucion.Id));
                    return;
                }
            }
            else
            {
                if (Distribucion.Tipo != ViajeDistribucion.Tipos.Desordenado)
                {
                    // Si el ultimo evento del ciclo ya fue procesado, cierro el ticket.
                    if (Distribucion.Empresa.CierreDistribucionCompleta &&
                        Distribucion.EntregasTotalCountConBases > 0 && 
                        Distribucion.Detalles.Last().Salida.HasValue)
                    {
                        CerrarDistribucion();
                        STrace.Trace("CierreCicloLogistico", Distribucion.Vehiculo != null && Distribucion.Vehiculo.Dispositivo != null ? Distribucion.Vehiculo.Dispositivo.Id : 0, string.Format("Viaje {0} a cerrar por cumplir última entrega.", Distribucion.Id));
                        return;
                    }
                }
                else
                {
                    // Si todas las entregas ya fueron realizadas cierro el ticket.
                    if (Distribucion.Empresa.CierreDistribucionCompleta &&
                        Distribucion.Detalles.Where(d => d.PuntoEntrega != null).All(d => EntregaDistribucion.Estados.EstadosFinales.Contains(d.Estado)))
                    {
                        CerrarDistribucion();
                        STrace.Trace("CierreCicloLogistico", Distribucion.Vehiculo != null && Distribucion.Vehiculo.Dispositivo != null ? Distribucion.Vehiculo.Dispositivo.Id : 0, string.Format("Viaje {0} a cerrar por cumplir todas las entregas.", Distribucion.Id));
                        return;
                    }
                }
            }

            var cerrarPorTiempo = Distribucion.Empresa.CicloDistribucionCerrar;
            // Si pasaron mas de EndMarginMinutes horas desde la hora final del ticket, lo cierro.
            var close = cerrarPorTiempo && Distribucion.Fin.AddMinutes(Distribucion.Empresa.EndMarginMinutes) < DateTime.UtcNow;
            if (close)
            {
                CerrarDistribucion();
                STrace.Trace("CierreCicloLogistico", Distribucion.Vehiculo != null && Distribucion.Vehiculo.Dispositivo != null ? Distribucion.Vehiculo.Dispositivo.Id : 0, string.Format("Viaje {0} a cerrar por tiempo.", Distribucion.Id));
            }
        }

        public void CerrarDistribucion()
        {
            var evento = EventFactory.GetCloseEvent(DateTime.UtcNow, true);
            Process(evento as CloseEvent);
        }

        #endregion

        private List<EntregaDistribucion> Entregas
        {
            get
            {
                return _entregas ?? (_entregas = Distribucion.Detalles
                                                     .OrderBy(d => d.Programado)
                                                     .ToList());
            }
        }
        
        private EstadosGeocerca EstadoDesvio
        {
            get
            {
                return LogicCache.KeyExists(GetType(), DesvioCacheKey)
                           ? (EstadosGeocerca)
                             (LogicCache.Retrieve<object>(GetType(), DesvioCacheKey) ?? EstadosGeocerca.Desconocido)
                           : EstadosGeocerca.Desconocido;
            }
            set { LogicCache.Store(GetType(), DesvioCacheKey, value); }
        }

        protected override bool IgnoreEvent(IEvent data)
        {
            // Descarto si no hay detalles a procesar
            if (Entregas.Count == 0) return true;

            // Si esta eliminado, no se procesa 
            if (Distribucion.Estado == ViajeDistribucion.Estados.Eliminado) return true;

            bool isInit = data.EventType == EventTypes.Init;
            bool isClose = data.EventType == EventTypes.Close;

            // Descarto si el evento es anterior al ticket
            if (!isInit && !isClose && data.Date < MinDate) return true;

            // Descarto si el evento es posterior al ticket
            if (!isInit && !isClose && data.Date > MaxDate) return true;

            return false;
        }

        #region Geocercas

        protected override IEnumerable<int> Geocercas
        {
            get
            {
                return Distribucion.Detalles.Where(d => d.PuntoEntrega == null || d.PuntoEntrega.Nomenclado)
                                            .Select(d => d.ReferenciaGeografica.Id);
            }
        }

        protected override IEnumerable<int> Puntos
        {
            get
            {
                return Distribucion.Detalles.Where(d => d.PuntoEntrega == null || d.PuntoEntrega.Nomenclado)
                                            .Where(d => d.Estado == EntregaDistribucion.Estados.Pendiente || d.Estado == EntregaDistribucion.Estados.EnSitio)
                                            .Select(d => d.ReferenciaGeografica.Id);
            }
        }

        protected override string GetKeyGeocerca(int geocerca)
        {
            return string.Concat("Ticket[", Distribucion.Vehiculo.Id, ",", geocerca, "]");
        }

        #endregion

        #region SaveMessage

        private void SaveMessageAtraso(IEvent data, EntregaDistribucion detalle)
        {
            var atraso = data.Date.Subtract(detalle.ProgramadoHasta);
            if (atraso.TotalMinutes > 1)
            {
                SaveMessage(MessageCode.AtrasoTicket.GetMessageCode(),
                            Convert.ToInt32(atraso.TotalMinutes) + "min (" + detalle.Orden + ") -> " + detalle.Viaje.Codigo + " - " + detalle.Descripcion,
                            new GPSPoint(data.Date, (float) data.Latitud, (float) data.Longitud),
                            data.Date);
            }
        }

        private void SaveConfirmationMessage(GPSPoint gpsPoint, EntregaDistribucion entrega, string codigoMensaje)
        {
            //static IEvent GetEvent(DAOFactory daoFactory, GPSPoint inicio, string codigo, Int32? idPuntoDeInteres,
            //Int64 extraData, Int64 extraData2, Int64 extraData3, Coche vehiculo, Empleado chofer)
            // extraData = ID Device
            // extraData2 = ID Entrega
            // extraData3 = Codigo Mensaje
            //var mensajeVo = DaoFactory.MensajeDAO.GetByCodigo(codigoMensaje.ToString("#0"), veh.Empresa, veh.Linea);

            var descriptiva = " - " + entrega.Viaje.Codigo + " - " + entrega.Descripcion;

            var ms = new MessageSaver(DaoFactory);
            var log = ms.Save(null, Convert.ToString(codigoMensaje), entrega.Viaje.Vehiculo.Dispositivo, entrega.Viaje.Vehiculo, entrega.Viaje.Empleado, gpsPoint.Date, gpsPoint, descriptiva, entrega.Viaje, entrega);

            try
            {
                entrega.MensajeConfirmacion = log as LogMensaje;
                DaoFactory.EntregaDistribucionDAO.SaveOrUpdate(entrega);
            }
            catch (Exception){ }
        }

        #endregion

        #region State & Progress

        private int _currentStateCompleted = -1;
        private int _delay;

        private int _totalCompleted = -1;

        public override string CurrentState
        {
            get { return Current != null ? Current.Orden.ToString(CultureInfo.InvariantCulture) : ""; }
        }

        public override int CurrentStateCompleted
        {
            get
            {
                if (_currentStateCompleted == -1)
                {
                    var cur = Current;
                    EntregaDistribucion prev = null;

                    foreach (var detalle in Entregas)
                    {
                        if (detalle == Current) break;
                        if (prev == null || detalle.Entrada.HasValue) prev = detalle;
                    }

                    var inside = cur.Entrada.HasValue;

                    var retraso = inside
                                      ? cur.Entrada.Value.Subtract(cur.Programado).TotalMinutes
                                      : prev != null && prev.Entrada.HasValue
                                            ? prev.Entrada.Value.Subtract(prev.Programado).TotalMinutes
                                            : 0;

                    var duracion = prev != null ? cur.Programado.Subtract(prev.Programado).TotalMinutes : 0;

                    var parcial = DateTime.UtcNow.AddMinutes(retraso).Subtract(cur.Programado).TotalMinutes;

                    _currentStateCompleted = duracion > 0
                                                 ? parcial < duracion
                                                       ? Convert.ToInt32(parcial*100/duracion)
                                                       : 100
                                                 : 0;
                    _delay = Convert.ToInt32(retraso);
                }
                return _currentStateCompleted;
            }
        }

        public override int TotalCompleted
        {
            get
            {
                if (_totalCompleted == -1)
                {
                    var numStates = Entregas.Count;
                    var doneStates = Entregas.TakeWhile(detalle => detalle != Current).Count();
                    var current = CurrentStateCompleted;
                    var statePercent = 100.0/numStates;

                    _totalCompleted = Convert.ToInt32((statePercent*doneStates) + (statePercent*100/current));
                }
                return _totalCompleted;
            }
        }

        public override int Delay
        {
            get
            {
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
                var entrega = Entregas.FirstOrDefault(d => d.Entrada.HasValue && !d.Salida.HasValue);
                if (entrega == null) return false;

                _enGeocercaDesde = entrega.Entrada.Value;
                return true;
            }
        }

        public override DateTime? EnGeocercaDesde
        {
            get { return EnGeocerca ? _enGeocercaDesde : null; }
        }

        public override DateTime Iniciado
        {
            get { return Distribucion.InicioReal.HasValue ? Distribucion.InicioReal.Value : Distribucion.Inicio; }
        }

        public override string Interno
        {
            get { return Vehiculo != null ? Vehiculo.Interno : string.Empty; }
        }

        public override string Codigo
        {
            get { return Distribucion != null ? Distribucion.Codigo : string.Empty; }
        }

        public override string Cliente
        {
            get { return Current != null && Current.Cliente != null ? Current.Cliente.Descripcion : string.Empty; }
        }

        public override string Telefono
        {
            get { return Current != null && Current.Cliente != null ? Current.Cliente.Telefono : string.Empty; }
        }

        public override string PuntoEntrega
        {
            get
            {
                return Current != null
                           ? Current.PuntoEntrega != null
                                 ? Current.PuntoEntrega.Descripcion
                                 : (Current.Linea != null ? Current.Linea.Descripcion : string.Empty)
                           : string.Empty;
            }
        }

        #endregion

        public void Regenerar(DateTime desde, DateTime hasta)
        {
            Regeneracion = true;
            var maxMonths = Vehiculo.Empresa != null ? Vehiculo.Empresa.MesesConsultaPosiciones : 3;
            var logMensajes = DaoFactory.LogMensajeDAO.GetEvents(Vehiculo.Id, desde, hasta, maxMonths);

            foreach (var logMensaje in logMensajes)
            {
                if (logMensaje.Latitud == 0 || logMensaje.Longitud == 0)
                {
                    var pos = DaoFactory.LogPosicionDAO.GetFirstPositionOlderThanDate(Vehiculo.Id, logMensaje.Fecha, maxMonths);
                    logMensaje.Latitud = pos.Latitud;
                    logMensaje.Longitud = pos.Longitud;
                    DaoFactory.LogMensajeDAO.SaveOrUpdate(logMensaje);
                }
                var evento = EventFactory.GetEvent(DaoFactory, logMensaje);
                if (evento == null) continue;
                ProcessEvent(evento, true);
                
                if (Distribucion.Estado == ViajeDistribucion.Estados.Cerrado) break;
            }
            Regeneracion = false;
        } 
    }
}