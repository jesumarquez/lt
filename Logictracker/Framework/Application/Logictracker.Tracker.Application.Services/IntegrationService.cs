using System;
using System.Linq;
using log4net;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.Factories;
using Logictracker.Messages.Saver;
using Logictracker.Messages.Sender;
using Logictracker.Messaging;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Spring.Messaging.Core;

namespace Logictracker.Tracker.Application.Services
{
    public class IntegrationService : IIntegrationService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IntegrationService));
        public wsRedSos.Service WebServiceSos;
        public MessageQueueTemplate ServiceMessageQueueTemplate { get; set; }
        public DAOFactory DaoFactory { get; set; }
        //public SosTicket SosTicket { get; set; }

        public enum EstadoServicio
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
                WebServiceSos = new wsRedSos.Service();

            Logger.Info("Searching for a new alarm in S.O.S. service...");

            //var response = WebServiceSos.ObtenerAlertas();
            var fecha = DateTime.Now;
            var response = string.Format("2015{0}{1}10{1}{2},7611,IAZ 581,CITROEN C 4 2.0 I BVA EXCLUSIV,NORMAL,290.4,SOBRE MANO DERECHA FRENTE CONCESIONARIA,SAN MARTIN  500,CORDOBA,COLON   1000,CORDOBA,PALACIO HEREDIA FERNANDO JAVIE,CORREA DE DISTRIBUCION,AMARILLO,TRASLADO,{1}/{0}/2015 {2}:{3},-31.3241410993924,-63.9724202099609,-31.3101359203789,-63.8783077646484,1",fecha.Month,fecha.Day,fecha.Hour,fecha.Minute);
            
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
            ticket = BuildTicket(ticket);

            var ack = SendMessageToGarmin(ticket.NotifyDriver(), ticket.Distribucion);

            //var estadoSvc= SosTicket.NotifyServer(respuestaGarmin);

            //WebServiceSos.ActualizarSvc(ticket.Movil.ToString(), ticket.NumeroServicio, estadoSvc, "Dato");
        }

        private bool SendMessageToGarmin(string msgText, ViajeDistribucion distribucion)
        {
            var tempId = int.Parse(DateTime.Now.Minute.ToString()) + "" + int.Parse(DateTime.Now.Second.ToString());
            var replies = new uint[]{6000, 6058};
            var message = MessageSender.CreateSubmitCannedResponsesTextMessage(distribucion.Vehiculo.Dispositivo, new MessageSaver(DaoFactory));
            message.AddMessageText(msgText).AddTextMessageId((uint) distribucion.Id).AddCannedResponses(replies).AddAckEvent(MessageCode.GarminCannedMessageReceived.GetMessageCode());

            message.Send();

            return true;
        }

        private SosTicket BuildTicket(SosTicket ticket)
        {
            var storedTicket = DaoFactory.SosTicketDAO.FindByCodigo(ticket.NumeroServicio);

            if (storedTicket == null)
            {
                ticket.SetStatus();
                ticket.Distribucion = BuildRoute(ticket);
                DaoFactory.SosTicketDAO.SaveOrUpdate(ticket);
                return ticket;
            }

            storedTicket = UpdateTicket(storedTicket, ticket);
            storedTicket.Distribucion = UpdateRoute(ticket);
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

            oldTicket.Origen = newTicket.Origen;
            oldTicket.Destino= newTicket.Destino;

            oldTicket.SetStatus();
            
            return oldTicket;
        }
        
        private ViajeDistribucion BuildRoute(SosTicket service)
        {
            //var empresa = DaoFactory.EmpresaDAO.FindByCodigo("SOS");
            var empresa = DaoFactory.EmpresaDAO.FindByCodigo("LT");
            //var linea = DaoFactory.LineaDAO.FindByNombre(empresa.Id, "Pompeya");
            var linea = DaoFactory.LineaDAO.FindByNombre(empresa.Id, "Headquarters");
            //var distribucion = DaoFactory.ViajeDistribucionDAO.FindByCodigo(service.NumeroServicio);

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
                viaje.Vehiculo = DaoFactory.CocheDAO.FindById(service.Movil);
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

            viaje.Inicio = ticket.HoraServicio;

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
        
        //private IStateService ChangeStatus(SosService service)
        //{
        //    switch (service.Estado)
        //    {
        //        case 1: //servicio asignado 
        //            return new AssignedState();
        //        case 2: //servicio prea asignado 
        //            return new PreassignedState();
        //        case 3: //asignación cancelada 
        //            return new AssignedCanceledState();
        //        case 4: //pre asignado cancelado
        //            return new PreassignedCanceledState();
        //        case 5: //asignación y pre asignado canceladas
        //        default:
        //            return new CanceledState();
        //    }
        //}
    }
}