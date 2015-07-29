using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
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
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RouteService));

        public IList<Mensaje> GetAllMessages(string deviceId)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
            
            if (device == null) return new List<Mensaje>();

            var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);

            if (employee == null) return new List<Mensaje>();

            var companies = new[] { employee.Empresa.Id };
            //var lineas = employee.Linea != null ? new[] { employee.Linea.Id } : new int[] { };
            var lineas = new int[] {};

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

            var vehicle = DaoFactory.CocheDAO.FindByChofer(employee.Id);
            if (vehicle == null) return null;

            var companies = new[] { employee.Empresa.Id };
            var lineas = new int[] {};
            var vehiculos = new [] { vehicle.Id };
            var empleados = new[] {employee.Id};
            //var routes = DaoFactory.ViajeDistribucionDAO.GetList(companies, new int[] { }, null, null);
            var routes = DaoFactory.ViajeDistribucionDAO.GetList(companies, lineas, vehiculos, empleados);

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
            catch //AlreadyOpenException, QueueException, Exception
            {
                return "CLOG_MESSAGE_NOT_SENT";
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
            var evento = new MobileEvent(DateTime.UtcNow, jobId,coord.Latitude, coord.Longitude, jobStatus, messageId, deviceId);

            try
            {
                ciclo.ProcessEvent(evento);
            }
            catch(Exception ex)
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

            return DaoFactory.LogMensajeDAO.GetByVehicleAndCode(vehicle.Id,
                    MessageCode.SubmitTextMessage.GetMessageCode(), DateTime.UtcNow.Date, DateTime.UtcNow, 1);

        }

        public int GetMobileIdByImei(string deviceId)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
            return device.Id;
        }

        public string ReceiveMessageByRouteAndDelivery(int routeId, string messageCode, string text, DateTime dateTime, long deliveryId, float lat, float lon, string deviceId)
        {
            var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);

            if (device == null) return messageCode;

            var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);

            if (employee == null) return messageCode;

            //var companies = new[] { employee.Empresa.Id };
            //var lineas = employee.Linea != null ? new[] { employee.Linea.Id } : new int[] { };

            var route= DaoFactory.ViajeDistribucionDAO.FindById(routeId);
            var delivery = DaoFactory.EntregaDistribucionDAO.FindById(int.Parse(deliveryId.ToString()));
            var vehicle = DaoFactory.CocheDAO.FindByChofer(employee.Id);

            var msgSaver = new MessageSaver(DaoFactory);
            msgSaver.Save(null, messageCode, device, vehicle , employee, dateTime.ToUniversalTime(), new GPSPoint(dateTime.ToUniversalTime(),lat, lon), text, route, delivery);
            
            return string.Empty;
        }

        //public void CreateGarminMessage(int jobId, Coordinate coordinate, int messageId, short status)
        //{
        //    var garminEvent = new GarminEvent(DateTime.UtcNow, jobId, coordinate.Latitude, coordinate.Longitude, status,null);
        //    MessageQueueTemplate.ConvertAndSend(garminEvent);
        //}
    }
}
