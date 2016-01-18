using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Messages.Saver;
using Logictracker.Messaging;
using Logictracker.Process.CicloLogistico;
using Logictracker.Process.CicloLogistico.Events;
using Logictracker.Process.CicloLogistico.Exceptions;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Utils;



namespace Logictracker.Tracker.Application.Services
{
    public class RouteService : IRouteService
    {
        public DAOFactory DaoFactory { get; set; }

        // public MessageQueueTemplate MessageQueueTemplate { get; set; }
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(RouteService));

        public IList<Mensaje> GetProfileMessages(string deviceId)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);

            if (device == null) return new List<Mensaje>();

            var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);

            if (employee == null) return new List<Mensaje>();

            var companies = new[] { employee.Empresa.Id };
            //var lineas = employee.Linea != null ? new[] { employee.Linea.Id } : new int[] { };
            var lineas = new int[] { };

            var confirmationMessages = DaoFactory.MensajeDAO.GetMensajesDeConfirmacion(companies, lineas);
            var rejectionMessages = DaoFactory.MensajeDAO.GetMensajesDeRechazo(companies, lineas);
            var userMessages = DaoFactory.MensajeDAO.GetMensajesDeUsuario(companies, lineas);

            return confirmationMessages.Concat(rejectionMessages).Concat(userMessages).ToList();
        }

        public Empleado GetEmployeeByDeviceImei(string imei)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(imei);
            return DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);
        }

        public ViajeDistribucion GetDistributionRouteById(int routeId)
        {
            return DaoFactory.ViajeDistribucionDAO.FindById(routeId);
        }

        public IList<ViajeDistribucion> GetAvailableRoutes(string deviceId)
        {
            var employee = GetEmployeeByDeviceImei(deviceId);
            if (employee == null) return null;

            // var vehicle = DaoFactory.CocheDAO.FindByChofer(employee.Id);
            //if (vehicle == null) return null; 
           
            var companies = new[] { employee.Empresa.Id };

            var lineas = new int[] { };

            if (employee.Dispositivo.Linea != null)
            {
                lineas = new int[] { employee.Dispositivo.Linea.Id };
            }
            var vehiculos = new int[] { }; //vehicle.Id 
            var empleados = new int[] {  };
            //var routes = DaoFactory.ViajeDistribucionDAO.GetList(companies, new int[] { }, null, null);
            var routes = DaoFactory.ViajeDistribucionDAO.GetList(companies, lineas, vehiculos, empleados).Where(x => x.Vehiculo != null && x.Vehiculo.Dispositivo != null);

            return routes.Where(viajeDistribucion => viajeDistribucion.Inicio.Date.Equals(DateTime.Now.Date)).ToList();
        }

        public string StartRoute(int routeId)
        {
            var ticket = DaoFactory.ViajeDistribucionDAO.FindById(routeId);
            var ciclo = new CicloLogisticoDistribucion(ticket, DaoFactory, new MessageSaver(DaoFactory));
            var evento = new InitEvent(DateTime.UtcNow);

            try
            {
                ciclo.ProcessEvent(evento);
                return "CLOG_START_SENT";
            }
            catch (NoVehicleException)
            {
                return "TICKET_NO_VEHICLE_ASSIGNED";
            }
            catch (Exception ex) //AlreadyOpenException, QueueException, Exception
            {
                return "CLOG_MESSAGE_NOT_SENT" + ex.Message.ToString() + ex.StackTrace.ToString();
            }
        }

        public string FinalizeRoute(int routeId)
        {
            var ticket = DaoFactory.ViajeDistribucionDAO.FindById(routeId);
            var opened = DaoFactory.ViajeDistribucionDAO.FindEnCurso(ticket.Vehiculo);

            if (opened == null) return "ROUTE_CLOSED";

            var ciclo = new CicloLogisticoDistribucion(opened, DaoFactory, new MessageSaver(DaoFactory));
            var evento = new CloseEvent(DateTime.UtcNow);
            ciclo.ProcessEvent(evento);
            return "CLOG_FINALIZE_SENT";
        }

        public short ReportDelivery(int routeId, long jobId, Coordinate coord, int messageId, short jobStatus, string deviceId)
        {
            var ticket = DaoFactory.ViajeDistribucionDAO.FindById(routeId);
            var ciclo = new CicloLogisticoDistribucion(ticket, DaoFactory, new MessageSaver(DaoFactory));
            var evento = new MobileEvent(DateTime.UtcNow, jobId, coord.Latitude, coord.Longitude, jobStatus, messageId, deviceId);

            try
            {
                ciclo.ProcessEvent(evento);
            }
            catch (Exception ex)
            {

                return -9;
            }
            return evento.Estado;

        }

        public IList<LogMensaje> GetMessagesMobile(string deviceId)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
            if (device == null) return null;

            var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);
            if (employee == null) return null;

            var vehicle = DaoFactory.CocheDAO.FindByChofer(employee.Id);
            if (vehicle == null) return null;

            return DaoFactory.LogMensajeDAO.GetByVehicleAndCode(vehicle.Id, MessageCode.SubmitTextMessage.GetMessageCode(), DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow, 1);

        }

        public bool SendMessagesMobile(string deviceId, List<LogMensaje> mensajes)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
            if (device == null) return false;

            var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);
            if (employee == null) return false;

            var vehicle = DaoFactory.CocheDAO.FindByChofer(employee.Id);
            if (vehicle == null) return false;

            foreach (var mensaje in mensajes)
            {
                mensaje.IdCoche = vehicle.Id;
                mensaje.CodigoMensaje = MessageCode.SubmitTextMessage.GetMessageCode();
                mensaje.Dispositivo = device;

                var msg = new MessageSaver(DaoFactory);

                var position = new GPSPoint();

                if ((mensaje.Latitud != 0) && (mensaje.Longitud != 0))
                    position = new GPSPoint(mensaje.Fecha, (float)mensaje.Latitud, (float)mensaje.Longitud);

                msg.Save(MessageCode.TextEvent.GetMessageCode(), vehicle, employee, mensaje.Fecha, position, mensaje.Texto);

                //var codes = new List<string> { MessageCode.TextEvent.GetMessageCode() };
                //mensaje.Mensaje = DaoFactory.MensajeDAO.FindByCodes(codes.AsEnumerable()).FirstOrDefault();
                //DaoFactory.LogMensajeDAO.Save(mensaje);                 
            }
            return true;
        }

        public int GetMobileIdByImei(string deviceId)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
            return device.Id;
        }

        public IList<LogMensaje> GetMessagesMobile(string deviceId, DateTime dt)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
            if (device == null) return null;

            var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);
            if (employee == null) return null;

            var vehicle = DaoFactory.CocheDAO.FindByChofer(employee.Id);
            if (vehicle == null) return null;

            List<int> vehicles = new List<int>();
            vehicles.Add(vehicle.Id);

            List<LogMensaje> lista = new List<LogMensaje>();
            List<LogMensaje> remover = new List<LogMensaje>();
            List<string> listacodigosrechazos = (List<string>)DaoFactory.MensajeDAO.FindByEmpresaYLineaAndUser(employee.Empresa, employee.Linea, null).Where(x=> x.TipoMensaje.DeRechazo).Select(x => x.Codigo).ToList();

            lista.AddRange(DaoFactory.LogMensajeDAO.GetByVehiclesAndCodes(vehicles, listacodigosrechazos, dt, DateTime.UtcNow, 1).Where(x => x.Texto.ToUpper().Contains("INFORME")));
            
            foreach (var item in lista)
            {

                int start = item.Texto.ToString().IndexOf("NRO ");
                start = start + 4;
                int end = item.Texto.ToString().IndexOf(":");
                string numero = item.Texto.Substring(start, end - start);
                var idRechazo = Convert.ToInt32(numero);
                var rechazo = DaoFactory.TicketRechazoDAO.FindById(idRechazo);
                if (!(!rechazo.UltimoEstado.Equals("Notificado1") ||
                    !rechazo.UltimoEstado.Equals("Notificado2") ||
                    !rechazo.UltimoEstado.Equals("Notificado3")))
                {
                    remover.Add(item);
                }

                if (rechazo != null)
                {
                    try
                    {
                        if (rechazo.Vendedor.Id == employee.Id &&
                            rechazo.UltimoEstado == Types.BusinessObjects.Rechazos.TicketRechazo.Estado.Pendiente)
                        {
                            rechazo.ChangeEstado(Types.BusinessObjects.Rechazos.TicketRechazo.Estado.Notificado1, "Recepción OK", employee);
                            DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.ToString().Contains("Cambio de estado invalido"))
                        throw ex;
                    }
                }
            }

            foreach (var item in remover)
            {
                lista.Remove(item);
            }
            
           lista.AddRange(DaoFactory.LogMensajeDAO.GetByVehicleAndCode(vehicle.Id, MessageCode.SubmitTextMessage.GetMessageCode(), dt, DateTime.UtcNow, 1));

           foreach (var item in remover)
           {
               string texto = item.Texto.Split(new String[] { ":" }, StringSplitOptions.None)[0].ToString();
               lista.RemoveAll(x => x.Texto.Contains(texto));
           }            
           return lista;
            //return DaoFactory.LogMensajeDAO.GetByVehicleAndCode(vehicle.Id, MessageCode.SubmitTextMessage.GetMessageCode(), dt, DateTime.UtcNow, 1);

        }

        public string SendMessageByRouteAndDelivery(int routeId, string messageCode, string text, DateTime dateTime, long deliveryId, float lat, float lon, string deviceId)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);

            if (device == null) return messageCode;

            var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);

            if (employee == null) return messageCode;

            //var companies = new[] { employee.Empresa.Id };
            //var lineas = employee.Linea != null ? new[] { employee.Linea.Id } : new int[] { };

            var route = DaoFactory.ViajeDistribucionDAO.FindById(routeId);
            var delivery = DaoFactory.EntregaDistribucionDAO.FindById(int.Parse(deliveryId.ToString()));
            var vehicle = DaoFactory.CocheDAO.FindByChofer(employee.Id);

            GPSPoint point = null;
            if ((lat != 0) && (lon != 0))
                point = new GPSPoint(dateTime.ToUniversalTime(), lat, lon);

            var msgSaver = new MessageSaver(DaoFactory);
            var description = "Ciclo Logistico ->" + text;

            if ((route != null) && (delivery != null))
                description = string.Format("Ciclo Logistico {0} - {1} : {2}", route.Codigo, delivery.Descripcion, text);

            msgSaver.Save(null, MessageCode.TextEvent.GetMessageCode(), device, vehicle, employee, dateTime.ToUniversalTime(), point, description, route, delivery);
            return string.Empty;
        }

        //public void CreateGarminMessage(int jobId, Coordinate coordinate, int messageId, short status)
        //{
        //    var garminEvent = new GarminEvent(DateTime.UtcNow, jobId, coordinate.Latitude, coordinate.Longitude, status,null);
        //    MessageQueueTemplate.ConvertAndSend(garminEvent);
        //}
    }
}
