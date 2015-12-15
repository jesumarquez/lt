using System;
using System.Linq;
using log4net;
using log4net.Util;
using Logictracker.DAL.Factories;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Tickets;
using Spring.Messaging.Core;

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
            PreasignadoRehazado = 105,
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

            var response = WebServiceSos.ObtenerAlertas();
            //var fecha = DateTime.Now;
            //var response = string.Format("2015{0}{1}10{1}{2},1271,IAZ 581,CITROEN C 4 2.0 I BVA EXCLUSIV,NORMAL,290.4,SOBRE MANO DERECHA FRENTE CONCESIONARIA,SAN MARTIN  500,CORDOBA,COLON   1000,CORDOBA,PALACIO HEREDIA FERNANDO JAVIE,20,CORREA DE DISTRIBUCION,AMARILLO,TRASLADO,{1}/{0}/2015 {2}:{3},-31.3241410993924,-63.9724202099609,-31.3101359203789,-63.8783077646484,1", fecha.Month, fecha.Day, fecha.Hour, fecha.Minute);
            //response += string.Format("#2015{0}{1}10{1}{2},7611,IAZ 581,CITROEN C 4 2.0 I BVA EXCLUSIV,NORMAL,290.4,SOBRE MANO DERECHA FRENTE CONCESIONARIA,SAN MARTIN  500,CORDOBA,COLON   1000,CORDOBA,PALACIO HEREDIA FERNANDO JAVIE,CORREA DE DISTRIBUCION,AMARILLO,TRASLADO,{1}/{0}/2015 {2}:{3},-31.3241410993924,-63.9724202099609,-31.3101359203789,-63.8783077646484,5", fecha.Month, fecha.Day, fecha.Hour, fecha.Minute);
            
            if (response != "")
            {
                var alerts = response.Split('#');
                Logger.InfoFormat("Found {0} services", alerts.Length);
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
                case 1: //servicio asignado
                    Asignar(ticket);
                    break;
                case 2: //servicio prea asignado
                    Preasignar(ticket);
                    break;
                case 3: //asignación cancelada
                    CancelarAsignacion(ticket);
                    break;
                case 4: //pre asignado cancelado
                    CancelarPreasignacion(ticket);
                    break;
                default: //asignación y pre asignado canceladas
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
            var message = MessageSender.CreateSubmitCannedMessage(distribucion.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));
            message.AddMessageText(msgText).AddTextMessageId((uint)distribucion.Id).AddAckEvent(MessageCode.GarminCannedMessageReceived.GetMessageCode());

            message.Send();
            Logger.InfoFormat("Se notifico servicio {0} al vehiculo {1} [{2}]", distribucion.Codigo, distribucion.Vehiculo.Patente, msgText);
        }

        private void SendQuestionToGarmin(string msgText, ViajeDistribucion distribucion)
        {
            var cmt = DaoFactory.MensajeDAO.GetResponsesMessagesTable(distribucion.Vehiculo.Dispositivo.Id, 0);
            if ((cmt != null) && (cmt.Count > 0))
            {
               // var msgText = "Por favor informe estado: " + destDetail;
                var mensajes = cmt.Where(m => !m.TipoMensaje.DeEstadoLogistico).OrderBy(m => m.Codigo);
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

            var viaje = new ViajeDistribucion();

            if (service.Distribucion == null)
            {
                //viaje
                viaje.Codigo = service.NumeroServicio;
                viaje.Empresa = empresa;
                viaje.Estado = 0;
                viaje.Tipo = 1;
                viaje.Linea = linea;
                viaje.Inicio = service.HoraServicio;
                viaje.RegresoABase = true;
                viaje.Fin = new DateTime(service.HoraServicio.Year, service.HoraServicio.Month, service.HoraServicio.Day,
                    service.HoraServicio.AddHours(1).Hour, service.HoraServicio.Minute, service.HoraServicio.Second);
                viaje.Vehiculo = DaoFactory.CocheDAO.FindByInterno(new[] {empresa.Id}, new[] {linea.Id} ,service.Movil.ToString());
                //viaje.TipoCicloLogistico = DaoFactory.TipoCicloLogisticoDAO.FindById(cycleType);            
                //viaje.CentroDeCostos = DaoFactory.CentroDeCostosDAO.FindById();            
                //viaje.Transportista = DaoFactory.TransportistaDAO.FindById(order.Transportista.Id);
                //viaje.Vehiculo = null;//DaoFactory.CocheDAO.FindById(idVehicle);
                //viaje.Empleado = null;//DaoFactory.EmpleadoDAO.FindById(idEmpleado);

                //base al inicio
                var entregaBase = new EntregaDistribucion();
                entregaBase.Linea = linea;
                entregaBase.Descripcion = "Base";
                entregaBase.Estado = EntregaDistribucion.Estados.Pendiente;
                entregaBase.Programado = service.HoraServicio;
                entregaBase.ProgramadoHasta = new DateTime(service.HoraServicio.Year, service.HoraServicio.Month,
                    service.HoraServicio.Day,
                    service.HoraServicio.AddHours(1).Hour, service.HoraServicio.Minute, service.HoraServicio.Second);
                entregaBase.Orden = viaje.Detalles.Count;
                entregaBase.Viaje = viaje;
                entregaBase.KmCalculado = 0;
                viaje.Detalles.Add(entregaBase);

                //origen
                var origen = new EntregaDistribucion
                {
                    Linea = linea,
                    Descripcion = "Origen",
                    Estado = EntregaDistribucion.Estados.Pendiente,
                    Programado = service.HoraServicio,
                    ProgramadoHasta =
                        new DateTime(service.HoraServicio.Year, service.HoraServicio.Month, service.HoraServicio.Day,
                            service.HoraServicio.AddHours(1).Hour, service.HoraServicio.Minute,
                            service.HoraServicio.Second),
                    Orden = viaje.Detalles.Count,
                    Viaje = viaje,
                    KmCalculado = 0
                };
                viaje.Detalles.Add(origen);

                //destino
                var destino = new EntregaDistribucion
                {
                    Linea = linea,
                    Descripcion = "Destino",
                    Estado = EntregaDistribucion.Estados.Pendiente,
                    Programado = service.HoraServicio,
                    ProgramadoHasta =
                        new DateTime(service.HoraServicio.Year, service.HoraServicio.Month, service.HoraServicio.Day,
                            service.HoraServicio.AddHours(1).Hour, service.HoraServicio.Minute,
                            service.HoraServicio.Second),
                    Orden = viaje.Detalles.Count,
                    Viaje = viaje,
                    KmCalculado = 0
                };

                viaje.Detalles.Add(destino);
                viaje.AgregarBaseFinal();
            }
            return viaje;
        }

        public virtual ViajeDistribucion UpdateRoute(SosTicket ticket)
        {
            var viaje = ticket.Distribucion ?? DaoFactory.ViajeDistribucionDAO.FindByCodigo(ticket.NumeroServicio);

            //si el servicio fue asignado cancelado, preasignado cancelado o ambos cancelados
            if ((ticket.EstadoServicio == 3) || (ticket.EstadoServicio == 4) || (ticket.EstadoServicio == 5))
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

        public void ResponseTicket(ViajeDistribucion dist, bool accepted)
        {
            var ticket = DaoFactory.SosTicketDAO.FindByCodigo(dist.Codigo);

            if (accepted)
            {
                ticket.Asignado = DateTime.Now;
                ticket.AsignacionNotificada = true;

                //envio de informacion del servicio
                var mensaje = string.Format("S:{0}<br>V:{1}<br>Di:{2}",
                   ticket.NumeroServicio + " P:" + ticket.Prioridad,
                   ticket.Patente.Substring(0,3) + "_xXx " + ticket.Marca + " " + ticket.Color,
                   ticket.Tipo + " por "+ ticket.Diagnostico + " $: " + ticket.CobroAdicional);
                SendMessageToGarmin(mensaje, dist);

                //envio de ruta al garmin
                //SendRouteToGarmin();
            }
            else
            {
                ticket.Cancelado = DateTime.Now;
                ticket.CancelacionNotificada = true;
            }
            DaoFactory.SosTicketDAO.SaveOrUpdate(ticket); 

            var idMovil = dist.Vehiculo.Id;
            var idTicket = dist.Codigo;
            var serviceStatus = ticket.EstadoServicio;
            var data = "0";

            WebServiceSos.ActualizarSvc(idMovil.ToString(), idTicket, serviceStatus, data);
        }

        public void ArrivalReport(ViajeDistribucion dist)
        {
            var ticket = DaoFactory.SosTicketDAO.FindByCodigo(dist.Codigo);
            var viaje = ticket.Distribucion ?? DaoFactory.ViajeDistribucionDAO.FindByCodigo(ticket.NumeroServicio);
            viaje.Estado = ViajeDistribucion.Estados.Cerrado;
            ticket.Distribucion = viaje;
            ticket.EstadoServicio = (int)CodigoEstado.Llegada;
            DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);

            var idMovil = dist.Vehiculo.Id;
            var idTicket = dist.Codigo;
            var serviceStatus = ticket.EstadoServicio;
            var data = "0";

            WebServiceSos.ActualizarSvc(idMovil.ToString(), idTicket, serviceStatus, data);
        }
    }
}
