using System;
using System.Linq;
using System.Drawing;
using log4net;
using Logictracker.DAL.Factories;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Spring.Messaging.Core;
using Logictracker.Model;
using Logictracker.Utils;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects;
using System.Globalization;
using Logictracker.Services.Helpers;

namespace Logictracker.Tracker.Application.Integration
{
    public class IntegrationService : IIntegrationService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IntegrationService));
        public WebServiceSos.Service WebServiceSos;
        public MessageQueueTemplate ServiceMessageQueueTemplate { get; set; }
        public DAOFactory DaoFactory { get; set; }

        private const string CodigoEmpresa = "LT";
        private const string NombreLinea = "Headquarters";

        public IntegrationService()
        {}
        public IntegrationService(DAOFactory daoFactory)
        {
            DaoFactory = daoFactory;
        }

        public enum CodigoEstado
        {
            Asignado = 35,
            AsignadoRechazado = 40,
            AsignadoAceptado = 45,
            Llegada = 50,
            FinalizadoAsistencia = 55,
            Finalizado = 60,
            Preasignado = 100,
            PreasignadoRechazado = 105,
            PreasignadoAceptado = 110,
            PreasignadoCancelado = 115
        }

        private enum Respuesta
        {
            Actualizado = 1,
            NoDisponible = 20051,
            Rechazado = 2,
            LlegadaNoActualizada = 3,
            AsistenciaNoActualizada = 4,
            FinalizacionNoActualizada = 5
        }
        
        public void CheckServices()
        {
            if (WebServiceSos==null)
                WebServiceSos = new WebServiceSos.Service();

            Logger.Info("Searching for a new alarm in S.O.S. service...");

            var rollback = WebServiceSos._alertasRollback("20160201304200");

            var response = WebServiceSos.ObtenerAlertas();
            //var fecha = DateTime.Now;
            //var response = string.Format("2015{0}{1}10{1}{2},1271,IAZ 581,CITROEN C 4 2.0 I BVA EXCLUSIV,NORMAL,290.4,SOBRE MANO DERECHA FRENTE CONCESIONARIA,SAN MARTIN  500,CORDOBA,COLON   1000,CORDOBA,PALACIO HEREDIA FERNANDO JAVIE,20,CORREA DE DISTRIBUCION,AMARILLO,TRASLADO,{1}/{0}/2015 {2}:{3},-31.3241410993924,-63.9724202099609,-31.3101359203789,-63.8783077646484,1", fecha.Month, fecha.Day, fecha.Hour, fecha.Minute);
            //response += string.Format("#2015{0}{1}10{1}{2},7611,IAZ 581,CITROEN C 4 2.0 I BVA EXCLUSIV,NORMAL,290.4,SOBRE MANO DERECHA FRENTE CONCESIONARIA,SAN MARTIN  500,CORDOBA,COLON   1000,CORDOBA,PALACIO HEREDIA FERNANDO JAVIE,CORREA DE DISTRIBUCION,AMARILLO,TRASLADO,{1}/{0}/2015 {2}:{3},-31.3241410993924,-63.9724202099609,-31.3101359203789,-63.8783077646484,5", fecha.Month, fecha.Day, fecha.Hour, fecha.Minute);
            
            if (response != "")
            {
                Logger.DebugFormat("WebService response: {0}", response);

                var alerts = response.Split('#');
                Logger.InfoFormat("Found {0} services", alerts.Length-1);

                foreach (var alert in alerts)
                {
                    if (alert == "") continue;

                    var novelty = TranslateFrameToClass.ParseFrame(alert);
                    ServiceMessageQueueTemplate.ConvertAndSend(novelty);
                }
            }
        }

        public void ProcessService(SosTicket ticket)
        {
            switch (ticket.EstadoServicio)
            {
                case SosTicket.EstadosServicio.Asignado: //servicio asignado
                    Asignar(ticket);
                    break;
                case SosTicket.EstadosServicio.Preasignado: //servicio prea asignado
                    Preasignar(ticket);
                    break;
                case SosTicket.EstadosServicio.AsignacionCancelada: //asignación cancelada
                    CancelarAsignacion(ticket);
                    break;
                case SosTicket.EstadosServicio.PreAsignacionCancelada: //pre asignado cancelado
                    CancelarPreasignacion(ticket);
                    break;
                case SosTicket.EstadosServicio.AsignacionYPreAsignacionCancelada: //asignación y pre asignado canceladas
                default: 
                    Cancelar(ticket);
                    break;
            }
        }

        private void Cancelar(SosTicket ticket)
        {
            CancelarAsignacion(ticket);
        }

        private void CancelarPreasignacion(SosTicket ticket)
        {
            CancelarAsignacion(ticket);
        }

        private void Preasignar(SosTicket ticket)
        {
            var storedTicket = DaoFactory.SosTicketDAO.FindByCodigo(ticket.NumeroServicio);

            if (storedTicket == null)
            {
                DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);
            }

            //S:	20151119303932
            //O:	SAN MARTIN  500, CORDOBA
            //D:	COLON 1000, CORDOBA
            //Di:	CORREA DE DISTRIBUCION
            var mensaje = string.Format("Preasignacion: {0}<br>O:{1}<br>D:{2}<br>Di:{3}",
                ticket.NumeroServicio,
                ticket.Origen.Direccion + ", " + ticket.Origen.Localidad,
                ticket.Destino.Direccion + ", " + ticket.Destino.Localidad,
                ticket.Diagnostico);

            SendQuestionToGarmin(mensaje, ticket.Distribucion);
        }

        private void Asignar(SosTicket ticket)
        {
            var storedTicket = DaoFactory.SosTicketDAO.FindByCodigo(ticket.NumeroServicio);
            ticket.Distribucion = storedTicket == null ? BuildRoute(ticket) : UpdateRoute(ticket);
            DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);

            //S:	20151119303932
            //O:	SAN MARTIN  500, CORDOBA
            //D:	COLON 1000, CORDOBA
            //Di:	CORREA DE DISTRIBUCION
            var mensaje = string.Format("Asignado: {0}<br>O:{1}<br>D:{2}<br>Di:{3}",
                ticket.NumeroServicio,
                ticket.Origen.Direccion + ", " + ticket.Origen.Localidad,
                ticket.Destino.Direccion + ", " + ticket.Destino.Localidad,
                ticket.Diagnostico);

            SendQuestionToGarmin(mensaje, ticket.Distribucion);
        }

        private void CancelarAsignacion(SosTicket ticket)
        {
            var storedTicket = DaoFactory.SosTicketDAO.FindByCodigo(ticket.NumeroServicio);

            if (storedTicket == null)
            {
                Logger.Error("No se puede cancelar un servicio no registrado");                
            }
            else
            {
                var viaje = ticket.Distribucion ?? DaoFactory.ViajeDistribucionDAO.FindByCodigo(ticket.NumeroServicio);
                var mensaje = string.Format("El servicio : {0}<br>Ha sido cancelado", ticket.NumeroServicio);
                SendMessageToGarmin(mensaje, viaje);
                viaje.Estado = ViajeDistribucion.Estados.Anulado;
            }    

        }

        private void SendMessageToGarmin(string msgText, ViajeDistribucion distribucion)
        {
            var message = MessageSender.CreateSubmitTextMessage(distribucion.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));            
            message.AddMessageText(msgText).AddTextMessageId((uint)distribucion.Id+1);
            message.Send();
            Logger.InfoFormat("Se notifico servicio {0} al vehiculo {1} [{2}]", distribucion.Codigo, distribucion.Vehiculo.Patente, msgText);

            var destinations = distribucion.Detalles.Where(d => d.PuntoEntrega != null
                                                                 && d.ReferenciaGeografica != null
                                                                 && Math.Abs(d.ReferenciaGeografica.Latitude) < 90
                                                                 && Math.Abs(d.ReferenciaGeografica.Longitude) < 180)
                                                    .Select(d => new Destination(d.Id, new GPSPoint(DateTime.UtcNow,
                                                                                                    (float)d.ReferenciaGeografica.Latitude,
                                                                                                    (float)d.ReferenciaGeografica.Longitude),
                                                                                 d.Descripcion,
                                                                                 d.PuntoEntrega.Descripcion,
                                                                                 d.ReferenciaGeografica.Direccion.Descripcion))
                                                    .ToArray();

            var msg = MessageSender.CreateUnloadRoute(distribucion.Vehiculo.Dispositivo, new MessageSaver(DaoFactory))
                                   .AddRouteId(distribucion.Id)
                                   .AddDestinations(destinations);
            msg.Send();
        }

        private void SendQuestionToGarmin(string msgText, ViajeDistribucion distribucion)
        {
            var cmt = DaoFactory.MensajeDAO.GetResponsesMessagesTable(distribucion.Vehiculo.Dispositivo.Id, 0);
            if ((cmt != null) && (cmt.Count > 0))
            {
               // var msgText = "Por favor informe estado: " + destDetail;
                var mensajes = cmt.Where(m => m.TipoMensaje.DeEstadoLogistico).OrderBy(m => m.Codigo);
                var replies = mensajes.Select(m => Convert.ToUInt32(m.Codigo)).ToArray();
               // var replies = new uint[] { 6000, 6058 };
                var message = MessageSender.CreateSubmitCannedResponsesTextMessage(distribucion.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));
                message.AddMessageText(msgText).AddTextMessageId((uint)distribucion.Id).AddCannedResponses(replies).AddAckEvent(MessageCode.GarminCannedMessageReceived.GetMessageCode());

                message.Send();
                Logger.Info("Solicitud de confirmacion enviada para el servicio : " + distribucion.Codigo);
            }
            else
                Logger.Error("Error generando pregunta: Canned Responses not found (" + distribucion.Codigo + ")");
        }

        private void SendQuestionPatenteToGarmin(string msgText, ViajeDistribucion distribucion)
        {   
            var message = MessageSender.CreateSubmitTextMessage(distribucion.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));
            message.AddMessageText(msgText).AddTextMessageId((uint)distribucion.Id);
            message.Send();
        }        

        private SosTicket GetTicket(SosTicket ticket)
        {
            var storedTicket = DaoFactory.SosTicketDAO.FindByCodigo(ticket.NumeroServicio);

            if (storedTicket == null)
            {
                ticket.Distribucion = BuildRoute(ticket);
                DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);
                return ticket;
            }

            storedTicket = UpdateTicket(storedTicket, ticket);
            //storedTicket.Distribucion = UpdateRoute(ticket);
            DaoFactory.SosTicketDAO.SaveOrUpdate(storedTicket);
            
            return storedTicket;
        }

        private SosTicket UpdateTicket(SosTicket oldTicket ,SosTicket newTicket)
        {
            oldTicket.CobroAdicional = newTicket.CobroAdicional;
            oldTicket.Color = newTicket.Color;
            oldTicket.Diagnostico = newTicket.Diagnostico;
            oldTicket.Marca = newTicket.Marca;
            oldTicket.Observacion = newTicket.Observacion;
            oldTicket.Operador = newTicket.Operador;
            oldTicket.Patente = newTicket.Patente;
            oldTicket.Prioridad = newTicket.Prioridad;
            oldTicket.EstadoServicio = newTicket.EstadoServicio;
            oldTicket.Origen = newTicket.Origen;
            oldTicket.Destino= newTicket.Destino;        

            return oldTicket;
        }
        
        private ViajeDistribucion BuildRoute(SosTicket service)
        {
            var empresa = DaoFactory.EmpresaDAO.FindByCodigo(CodigoEmpresa);
            var linea = DaoFactory.LineaDAO.FindByNombre(empresa.Id, NombreLinea);
            var cliente = DaoFactory.ClienteDAO.FindByCode(new[] { empresa.Id }, new[] { -1 }, "1");
            const int vigencia = 12;
            var fecha = service.HoraServicio;

            var viaje = new ViajeDistribucion();

            if (service.Distribucion == null)
            {
                //viaje
                viaje.Codigo = service.NumeroServicio;
                viaje.Empresa = empresa;
                viaje.Linea = linea;
                viaje.Estado = ViajeDistribucion.Estados.Pendiente;
                viaje.Tipo = ViajeDistribucion.Tipos.Desordenado;
                viaje.Inicio = fecha;
                viaje.RegresoABase = false;
                viaje.Fin = fecha.AddHours(1);
                viaje.Vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] {empresa.Id}, new[] {-1}, service.Movil.ToString());
                
                //base al inicio
                var entregaBase = new EntregaDistribucion();
                entregaBase.Linea = linea;
                entregaBase.Descripcion = linea.Descripcion;
                entregaBase.Estado = EntregaDistribucion.Estados.Pendiente;
                entregaBase.Programado = fecha;
                entregaBase.ProgramadoHasta = fecha.AddHours(1);
                entregaBase.Orden = viaje.Detalles.Count;
                entregaBase.Viaje = viaje;
                entregaBase.KmCalculado = 0;
                viaje.Detalles.Add(entregaBase);

                var nombreOrigen = service.NumeroServicio + " - O";
                var nombreDestino = service.NumeroServicio + " - D";
                
                TipoServicioCiclo tipoServicio = null;
                var tipoServ = DaoFactory.TipoServicioCicloDAO.FindDefault(new[] { empresa.Id }, new[] { -1 });
                if (tipoServ != null && tipoServ.Id > 0) tipoServicio = tipoServ;

                #region Origen

                var puntoEntregaO = DaoFactory.PuntoEntregaDAO.FindByCode(new[] { empresa.Id },
                                                                          new[] { -1 },
                                                                          new[] { cliente.Id },
                                                                          nombreOrigen);

                if (puntoEntregaO == null)
                {
                    var empresaGeoRef = empresa;
                    var lineaGeoRef = linea;

                    var puntoDeInteres = new ReferenciaGeografica
                    {
                        Codigo = nombreOrigen,
                        Descripcion = nombreOrigen,
                        Empresa = empresaGeoRef,
                        Linea = lineaGeoRef,
                        EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                        EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                        EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                        InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                        TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                        Vigencia = new Vigencia { Inicio = DateTime.UtcNow, Fin = fecha.AddHours(vigencia) },
                        Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                    };

                    var posicion = GetNewDireccion(service.Origen.Latitud, service.Origen.Longitud);
                    var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                    poligono.AddPoints(new[] { new PointF((float)service.Origen.Longitud, (float)service.Origen.Latitud) });

                    puntoDeInteres.Historia.Add(new HistoriaGeoRef
                    {
                        ReferenciaGeografica = puntoDeInteres,
                        Direccion = posicion,
                        Poligono = poligono,
                        Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                    });

                    DaoFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);

                    puntoEntregaO = new PuntoEntrega
                    {
                        Cliente = cliente,
                        Codigo = nombreOrigen,
                        Descripcion = nombreOrigen,
                        Telefono = string.Empty,
                        Baja = false,
                        ReferenciaGeografica = puntoDeInteres,
                        Nomenclado = true,
                        DireccionNomenclada = service.Origen.Direccion + ", " +service.Origen.Localidad,
                        Nombre = nombreOrigen
                    };
                }
                else
                {
                    if (!puntoEntregaO.ReferenciaGeografica.IgnoraLogiclink && (puntoEntregaO.ReferenciaGeografica.Latitude != service.Origen.Latitud || puntoEntregaO.ReferenciaGeografica.Longitude != service.Origen.Longitud))
                    {
                        puntoEntregaO.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                        puntoEntregaO.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                        var posicion = GetNewDireccion(service.Origen.Latitud, service.Origen.Longitud);
                        var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                        poligono.AddPoints(new[] { new PointF((float)service.Origen.Longitud, (float)service.Origen.Latitud) });

                        puntoEntregaO.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                    }

                    var end = fecha.AddHours(vigencia);
                    if (puntoEntregaO.ReferenciaGeografica.Vigencia.Fin < end)
                        puntoEntregaO.ReferenciaGeografica.Vigencia.Fin = end;

                    DaoFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntregaO.ReferenciaGeografica);
                }

                DaoFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntregaO);

                var entregaO = new EntregaDistribucion
                {
                    Cliente = cliente,
                    PuntoEntrega = puntoEntregaO,
                    Descripcion = nombreOrigen,
                    Estado = EntregaDistribucion.Estados.Pendiente,
                    Orden = viaje.Detalles.Count,
                    Programado = fecha,
                    ProgramadoHasta = fecha,
                    TipoServicio = tipoServicio,
                    Viaje = viaje
                };

                viaje.Detalles.Add(entregaO);
                
                #endregion

                #region Destino

                var puntoEntregaD = DaoFactory.PuntoEntregaDAO.FindByCode(new[] { empresa.Id },
                                                                          new[] { -1 },
                                                                          new[] { cliente.Id },
                                                                          nombreDestino);

                if (puntoEntregaD == null)
                {
                    var empresaGeoRef = empresa;
                    var lineaGeoRef = linea;

                    var puntoDeInteres = new ReferenciaGeografica
                    {
                        Codigo = nombreDestino,
                        Descripcion = nombreDestino,
                        Empresa = empresaGeoRef,
                        Linea = lineaGeoRef,
                        EsFin = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsFin,
                        EsInicio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsInicio,
                        EsIntermedio = cliente.ReferenciaGeografica.TipoReferenciaGeografica.EsIntermedio,
                        InhibeAlarma = cliente.ReferenciaGeografica.TipoReferenciaGeografica.InhibeAlarma,
                        TipoReferenciaGeografica = cliente.ReferenciaGeografica.TipoReferenciaGeografica,
                        Vigencia = new Vigencia { Inicio = DateTime.UtcNow, Fin = fecha.AddHours(vigencia) },
                        Icono = cliente.ReferenciaGeografica.TipoReferenciaGeografica.Icono
                    };

                    var posicion = GetNewDireccion(service.Destino.Latitud, service.Destino.Longitud);

                    var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                    poligono.AddPoints(new[] { new PointF((float)service.Destino.Longitud, (float)service.Destino.Latitud) });

                    puntoDeInteres.Historia.Add(new HistoriaGeoRef
                    {
                        ReferenciaGeografica = puntoDeInteres,
                        Direccion = posicion,
                        Poligono = poligono,
                        Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                    });

                    DaoFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoDeInteres);

                    puntoEntregaD = new PuntoEntrega
                    {
                        Cliente = cliente,
                        Codigo = nombreDestino,
                        Descripcion = nombreDestino,
                        Telefono = string.Empty,
                        Baja = false,
                        ReferenciaGeografica = puntoDeInteres,
                        Nomenclado = true,
                        DireccionNomenclada = service.Destino.Direccion + ", " + service.Destino.Localidad,
                        Nombre = nombreDestino
                    };
                }
                else
                {
                    if (!puntoEntregaD.ReferenciaGeografica.IgnoraLogiclink && (puntoEntregaD.ReferenciaGeografica.Latitude != service.Destino.Latitud || puntoEntregaD.ReferenciaGeografica.Longitude != service.Destino.Longitud))
                    {
                        puntoEntregaD.ReferenciaGeografica.Direccion.Vigencia.Fin = DateTime.UtcNow;
                        puntoEntregaD.ReferenciaGeografica.Poligono.Vigencia.Fin = DateTime.UtcNow;

                        var posicion = GetNewDireccion(service.Destino.Latitud, service.Destino.Longitud);
                        var poligono = new Poligono { Radio = 50, Vigencia = new Vigencia { Inicio = DateTime.UtcNow } };
                        poligono.AddPoints(new[] { new PointF((float)service.Destino.Longitud, (float)service.Destino.Latitud) });

                        puntoEntregaD.ReferenciaGeografica.AddHistoria(posicion, poligono, DateTime.UtcNow);
                    }

                    var end = fecha.AddHours(vigencia);
                    if (puntoEntregaD.ReferenciaGeografica.Vigencia.Fin < end)
                        puntoEntregaD.ReferenciaGeografica.Vigencia.Fin = end;

                    DaoFactory.ReferenciaGeograficaDAO.SaveOrUpdate(puntoEntregaD.ReferenciaGeografica);
                }

                DaoFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntregaD);

                var anterior = puntoEntregaO.ReferenciaGeografica;
                var siguiente = puntoEntregaD.ReferenciaGeografica;
                var o = new LatLon(anterior.Latitude, anterior.Longitude);
                var d = new LatLon(siguiente.Latitude, siguiente.Longitude);
                var directions = GoogleDirections.GetDirections(o, d, GoogleDirections.Modes.Driving, string.Empty, null);

                if (directions != null)
                {
                    var duracion = directions.Duration;
                    fecha = entregaO.Programado.Add(duracion);
                }

                var entregaD = new EntregaDistribucion
                {
                    Cliente = cliente,
                    PuntoEntrega = puntoEntregaD,
                    Descripcion = nombreDestino,
                    Estado = EntregaDistribucion.Estados.Pendiente,
                    Orden = viaje.Detalles.Count,
                    Programado = fecha,
                    ProgramadoHasta = fecha,
                    TipoServicio = tipoServicio,
                    Viaje = viaje
                };

                viaje.Detalles.Add(entregaD);
                viaje.Fin = fecha;

                #endregion             

            }
            return viaje;
        }
        
        private static Direccion GetNewDireccion(double latitud, double longitud)
        {
            return new Direccion
            {
                Altura = -1,
                IdMapa = -1,
                Provincia = string.Empty,
                IdCalle = -1,
                IdEsquina = -1,
                IdEntrecalle = -1,
                Latitud = latitud,
                Longitud = longitud,
                Partido = string.Empty,
                Pais = string.Empty,
                Calle = string.Empty,
                Descripcion = string.Format("({0}, {1})", latitud.ToString(CultureInfo.InvariantCulture), longitud.ToString(CultureInfo.InvariantCulture)),
                Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
            };
        }

        public virtual ViajeDistribucion UpdateRoute(SosTicket ticket)
        {
            var viaje = ticket.Distribucion ?? DaoFactory.ViajeDistribucionDAO.FindByCodigo(ticket.NumeroServicio);

            //si el servicio fue asignado cancelado, preasignado cancelado o ambos cancelados
            if (ticket.EstadoServicio == SosTicket.EstadosServicio.AsignacionCancelada
             || ticket.EstadoServicio == SosTicket.EstadosServicio.PreAsignacionCancelada
             || ticket.EstadoServicio == SosTicket.EstadosServicio.AsignacionYPreAsignacionCancelada)
            {
                viaje.Estado = ViajeDistribucion.Estados.Anulado;
            }
            
            var empresaId = DaoFactory.EmpresaDAO.FindByCodigo(CodigoEmpresa).Id;
            var lineaId = DaoFactory.LineaDAO.FindByNombre(empresaId, NombreLinea).Id;
            viaje.Inicio = ticket.HoraServicio;
            viaje.Vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] {empresaId}, new[] {lineaId} ,ticket.Movil.ToString());
  
            foreach (var entrega in viaje.Detalles)
            {
                if (entrega.Descripcion.Equals("Origen"))
                {
                    //entrega.Estado = EntregaDistribucion.Estados.Pendiente;
                    entrega.Programado = ticket.HoraServicio;
                    entrega.ProgramadoHasta = new DateTime(ticket.HoraServicio.Year, ticket.HoraServicio.Month, ticket.HoraServicio.Day,
                        ticket.HoraServicio.AddHours(1).Hour, ticket.HoraServicio.Minute, ticket.HoraServicio.Second);
                }
                if (entrega.Descripcion.Equals("Destino"))
                {
                    //entrega.Estado = EntregaDistribucion.Estados.Pendiente;
                    entrega.Programado = ticket.HoraServicio;
                    entrega.ProgramadoHasta = new DateTime(ticket.HoraServicio.Year, ticket.HoraServicio.Month, ticket.HoraServicio.Day,
                        ticket.HoraServicio.AddHours(1).Hour, ticket.HoraServicio.Minute, ticket.HoraServicio.Second);
                }
            }
            return viaje;
        }

        public void Close()
        {
            WebServiceSos.Abort();
            WebServiceSos.Dispose();
        }

        public void ResponseAsigno(ViajeDistribucion dist, bool accepted)
        {
            var ticket = DaoFactory.SosTicketDAO.FindByCodigo(dist.Codigo);

            if (accepted)
            {
                ticket.Asignado = DateTime.Now;
                ticket.AsignacionNotificada = true;
                ticket.EstadoServicio = (int) CodigoEstado.AsignadoAceptado;

                //envio de informacion del servicio
                var mensaje = string.Format("S:{0}<br>V:{1}<br>Di:{2}",
                   ticket.NumeroServicio + " P:" + ticket.Prioridad,
                   ticket.Patente.Substring(0,3) + "XXX " + ticket.Marca + " " + ticket.Color,
                   ticket.Tipo + " por "+ ticket.Diagnostico + " $: " + ticket.CobroAdicional);
                SendMessageToGarmin(mensaje, dist);

                //envio de ruta al garmin
                //SendRouteToGarmin();
            }
            else
            {
                ticket.Rechazado = true;
                ticket.Cancelado = DateTime.Now;
                ticket.CancelacionNotificada = true;
                ticket.EstadoServicio = (int)CodigoEstado.AsignadoRechazado;
            }
            DaoFactory.SosTicketDAO.SaveOrUpdate(ticket); 

            UpdateToSos(dist.Vehiculo.Interno, dist.Codigo, ticket.EstadoServicio, ticket.Diagnostico);
        }

        public void ResponsePreasigno(ViajeDistribucion dist, bool accepted)
        {
            var ticket = DaoFactory.SosTicketDAO.FindByCodigo(dist.Codigo);

            if (accepted)
            {
                ticket.Preasignado = DateTime.Now;
                ticket.PreasignacionNotificada = true;
                ticket.EstadoServicio = (int)CodigoEstado.PreasignadoAceptado;

                //envio de informacion del servicio
                var mensaje = string.Format("S:{0}<br>V:{1}<br>Di:{2}",
                   ticket.NumeroServicio + " P:" + ticket.Prioridad,
                   ticket.Patente.Substring(0, 3) + "XXX " + ticket.Marca + " " + ticket.Color,
                   ticket.Tipo + " por " + ticket.Diagnostico + " $: " + ticket.CobroAdicional);
                SendMessageToGarmin(mensaje, dist);

                //envio de ruta al garmin
                //SendRouteToGarmin();
            }
            else
            {
                ticket.Rechazado = true;
                ticket.Cancelado = DateTime.Now;
                ticket.CancelacionNotificada = true;
                ticket.EstadoServicio = (int)CodigoEstado.PreasignadoRechazado;
            }
            DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);

            UpdateToSos(dist.Vehiculo.Interno, dist.Codigo, ticket.EstadoServicio, ticket.Diagnostico);
        }

        private void UpdateToSos(string interno, string codigo, int estadoServicio, string diagnostico)
        {
            if (WebServiceSos == null)
                WebServiceSos = new WebServiceSos.Service();

            var res=WebServiceSos.ActualizarSvc(interno, codigo, estadoServicio, diagnostico.Split('-')[0]);
            Logger.Info("Webservice response: " + res);
        }

        public void ArrivalReport(ViajeDistribucion viaje)
        {
            var ticket = DaoFactory.SosTicketDAO.FindByCodigo(viaje.Codigo);
            if (ticket != null)
            {
                /*
                ticket.Distribucion = viaje;
                ticket.EstadoServicio = (int)CodigoEstado.Llegada;
                DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);

                UpdateToSos(viaje.Vehiculo.Interno, viaje.Codigo, ticket.EstadoServicio, ticket.Diagnostico);
                */
                var msgText = "Por favor, informe los 3 dígitos de la patente del vehículo correspondiente al servicio " + viaje.Codigo;
                SendQuestionPatenteToGarmin(msgText, viaje);
            }
        }

        public void ConfirmaPatente(SosTicket ticket, bool confirmacionOk)
        {
            if (confirmacionOk)
            {
                ticket.EstadoServicio = (int)CodigoEstado.Llegada;
                DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);

                UpdateToSos(ticket.Distribucion.Vehiculo.Interno, ticket.Distribucion.Codigo, ticket.EstadoServicio, ticket.Diagnostico);
            }
            else
            {
                var msgText = "Patente errónea. Por favor, informe los 3 dígitos de la patente del vehículo correspondiente al servicio " + ticket.Distribucion.Codigo;
                SendQuestionPatenteToGarmin(msgText, ticket.Distribucion);
            }
        }

        public void FinishReport(ViajeDistribucion viaje)
        {
            var ticket = DaoFactory.SosTicketDAO.FindByCodigo(viaje.Codigo);
            if (ticket != null)
            {
                ticket.Distribucion = viaje;
                ticket.EstadoServicio = (int)CodigoEstado.Finalizado;
                DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);

                UpdateToSos(viaje.Vehiculo.Interno, viaje.Codigo, ticket.EstadoServicio, ticket.Diagnostico);
            }
        }
    }
}
