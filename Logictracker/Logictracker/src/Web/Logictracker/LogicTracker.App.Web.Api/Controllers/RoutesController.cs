﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Services.Helpers;
using System.Linq;
using System.Threading;
using System.Device.Location;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using LogicTracker.App.Web.Api.Providers;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class RoutesController : BaseController
    {
        public IRouteService RouteService { get; set; }

        public static string ROUTE_STATUS_PENDING = "0";
        public static string ROUTE_STATUS_ACTIVE = "1";
        public static string ROUTE_STATUS_FINALIZE = "9";

        public const short Eliminado = -1;
        public const short Pendiente = 0;
        public const short EnCurso = 1;
        public const short Anulado = 8;
        public const short Cerrado = 9;


        // GET: api/Routes/
       // [LogicTracker.App.Web.Api.Providers.CompressContent]
        public IHttpActionResult Get()
        {
            try
            {
                var deviceId = GetDeviceId(Request);
                if (deviceId == null) return BadRequest();

                var routes = RouteService.GetAvailableRoutes(deviceId);

                if (routes == null) return Unauthorized();
                if (routes.Count < 1) return Ok(new RouteList());

                var listRoute = new RouteList
                {
                    CompanyId = routes[0].Empresa.Id,
                    LineId = routes[0].Linea.Id,
                    DateTime = DateTime.UtcNow
                };
                var items = new List<RouteItem>();
                foreach (var viajeDistribucion in routes)
                {
                    if (!viajeDistribucion.Estado.ToString().Equals(ROUTE_STATUS_FINALIZE))
                    {
                        int index = 0;
                        if (viajeDistribucion.Detalles.Count > 0)
                            index = viajeDistribucion.Detalles.Count - 1;
                        RouteItem element = new RouteItem()
                         {
                             Code = viajeDistribucion.Codigo,
                             DeliveriesNumber = index,
                             Id = viajeDistribucion.Id,
                             Places = PlacesToDescription(viajeDistribucion),
                             Status = viajeDistribucion.Estado.ToString(),
                             StartDateTime = viajeDistribucion.Inicio

                         };
                        if (viajeDistribucion.Vehiculo != null)
                        {
                            element.patente = viajeDistribucion.Vehiculo.Patente;
                            element.interno = viajeDistribucion.Vehiculo.Interno;
                        }
                        items.Add(element);
                    }
                }

                listRoute.RouteItems = items.ToArray();
                return Ok(listRoute);
            }
            catch(Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }


        public class ParameterFiter
        {
            public string EmpresaId { get; set; }
            public string LineaId { get; set; }
            public string TransportistaId { get; set; }
            public string query { get; set; }
        }


        [Route("api/routes/idfilter")]
        [HttpPost]
        [CompressContent]
        public IHttpActionResult Post([FromBody]ParameterFiter idfilter)
        {
            try
            {
                if (idfilter == null) return BadRequest();
                var deviceId = GetDeviceId(Request);
                if (deviceId == null) return BadRequest();

                var routes = RouteService.GetAvailableRoutes(deviceId, idfilter.EmpresaId, idfilter.LineaId, idfilter.TransportistaId, idfilter.query);

                if (routes == null) return Unauthorized();
                if (routes.Count < 1) return Ok(new RouteList());

                var listRoute = new RouteList
                {
                    CompanyId = routes[0].Empresa.Id,
                    LineId = routes[0].Linea.Id,
                    DateTime = DateTime.UtcNow
                };
                var items = new List<RouteItem>();
                foreach (var viajeDistribucion in routes)
                {
                    if (viajeDistribucion.Estado.ToString().Equals(Pendiente.ToString()))
                    {
                        RouteItem element = new RouteItem()
                        {
                            Code = viajeDistribucion.Codigo,
                            DeliveriesNumber = viajeDistribucion.Detalles.Count - 1,
                            Id = viajeDistribucion.Id,
                            Places = PlacesToDescription(viajeDistribucion),
                            Status = viajeDistribucion.Estado.ToString(),
                            StartDateTime = viajeDistribucion.Inicio

                        };
                        if (viajeDistribucion.Vehiculo != null)
                        {
                            element.patente = viajeDistribucion.Vehiculo.Patente;
                            element.interno = viajeDistribucion.Vehiculo.Interno;
                        }
                        items.Add(element);
                    }
                }

                listRoute.RouteItems = items.ToArray();
                return Ok(listRoute);
            }
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }

        public class ParameterStartRute
        {
            public string Patente { get; set; }
            public string Ruta { get; set; }
            public string Chofer { get; set; }
        }

        [Route("api/routes/idstartrute")]
        [HttpPost]
        [CompressContent]
        public IHttpActionResult Post([FromBody]ParameterStartRute idstartrute)
        {
            try
            {
                if (idstartrute == null) return BadRequest();
                var deviceId = GetDeviceId(Request);
                if (deviceId == null) return BadRequest();

                ViajeDistribucion route = RouteService.setRoute(deviceId, idstartrute.Patente, idstartrute.Chofer, idstartrute.Ruta);

                if (route == null) return Unauthorized();

                var listRoute = new RouteList
                {
                    CompanyId = route.Empresa.Id,
                    LineId = route.Linea.Id,
                    DateTime = DateTime.UtcNow
                };
                var items = new List<RouteItem>();
                RouteItem element = new RouteItem()
                {
                    Code = route.Codigo,
                    DeliveriesNumber = route.Detalles.Count - 1,
                    Id = route.Id,
                    Places = PlacesToDescription(route),
                    Status = route.Estado.ToString(),
                    StartDateTime = route.Inicio

                };
                if (route.Vehiculo != null)
                {
                    element.patente = route.Vehiculo.Patente;
                    element.interno = route.Vehiculo.Interno;
                }
                items.Add(element);


                listRoute.RouteItems = items.ToArray();
                return Ok(listRoute);
            }
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }

        
        private string PlacesToDescription(ViajeDistribucion viajeDistribucion)
        {
            var places = new StringBuilder();
            foreach (var entregaDistribucion in viajeDistribucion.Detalles)
            {
                if(entregaDistribucion.PuntoEntrega!=null)
                    places.Append(entregaDistribucion.Descripcion + ", ");
            }
            return places.ToString();
        }

        // GET: api/Routes/1234567890A
        public IHttpActionResult Get(int id)
        {
            try
            {
                var deviceId = GetDeviceId(Request);
                EmpleadoDAO empleado = new EmpleadoDAO();// emple //fecha
                DispositivoDAO dispositivo = new DispositivoDAO();
                var device = dispositivo.FindByImei(deviceId);
                var employee = empleado.FindEmpleadoByDevice(device);

                var trip = RouteService.GetDistributionRouteById(id);

                if (trip == null) return NotFound();

                var route = new Route
                {
                    Code = trip.Codigo,
                    Id = trip.Id,
                    Status = trip.Estado
                };

                var jobs = new List<Job>();
                foreach (var detail in trip.Detalles)
                {
                    var job = new Job();
                    job.Id = detail.Id;
                    job.StartDate = detail.Programado.ToString("yyyy-MM-ddTHH:mm:ss");
                    job.EndDate = detail.ProgramadoHasta.ToString("yyyy-MM-ddTHH:mm:ss");
                    job.State = detail.Estado == 3 ? 0 : detail.Estado;
                    if (detail.PuntoEntrega != null && detail.PuntoEntrega.Descripcion != null)
                    {
                        job.Code = detail.PuntoEntrega.Codigo;
                        job.Name = detail.PuntoEntrega.Descripcion;
                        job.clienttype = detail.PuntoEntrega.Cliente.Descripcion;
                    }
                    else
                    {
                        if (detail.Cliente != null)
                        {
                            job.ClientName = detail.Cliente.Descripcion;
                            job.Code = detail.Cliente.Codigo;
                        }
                    }
                    if (detail.ReferenciaGeografica != null &&
                        detail.ReferenciaGeografica.Direccion != null &&
                        detail.ReferenciaGeografica.Direccion.Descripcion != null &&
                        !String.IsNullOrEmpty(detail.ReferenciaGeografica.Direccion.Calle))
                    {
                        if (detail.ReferenciaGeografica.Direccion.Altura > 0)
                            job.direccionreal = detail.ReferenciaGeografica.Direccion.Calle.ToString() + " "
                                + detail.ReferenciaGeografica.Direccion.Altura + " , " + detail.ReferenciaGeografica.Direccion.Partido;
                        else
                            job.direccionreal = detail.ReferenciaGeografica.Direccion.Descripcion;
                    }
                    job.Description = detail.Descripcion;

                    job.Order = detail.Orden;
                    job.Location = new Location();

                    job.Volumen = (float)detail.Volumen;
                    job.Quantity = detail.Bultos;
                    job.Value = (float)detail.Valor;
                    job.Weight = (float)detail.Peso;

                    if (detail.PuntoEntrega != null)
                    {
                        job.Location.Latitude = (float)detail.PuntoEntrega.ReferenciaGeografica.Latitude;
                        job.Location.Longitude = (float)detail.PuntoEntrega.ReferenciaGeografica.Longitude;
                    }
                    else
                    {
                        job.Location.Latitude = (float)detail.Linea.ReferenciaGeografica.Latitude;
                        job.Location.Longitude = (float)detail.Linea.ReferenciaGeografica.Longitude;
                    }

                    if (detail.PuntoEntrega != null)
                        jobs.Add(job);
                }
                route.Jobs = jobs.ToArray();

                if (trip.Recepcion == null)
                {
                    ViajeDistribucionDAO vd = new ViajeDistribucionDAO();
                    trip.Recepcion = DateTime.UtcNow;
                    if (trip.Empleado == null)
                    {
                        trip.Empleado = employee;
                    }
                    vd.SaveOrUpdate(trip);
                }

                return Ok(route);
            }
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }
             

        // POST: api/Routes
        public IHttpActionResult Post([FromBody]RouteState routeState)
        {
            try
            {
                var deviceId = GetDeviceId(Request);


                if (deviceId == null) return Unauthorized();

                if (routeState == null) return BadRequest();

                foreach (var jobState in routeState.JobStates)
                {
                    var coord = new Coordinate()
                    {
                        IsValid = true,
                        Latitude = jobState.Latitude,
                        Longitude = jobState.Longitude
                    };

                    //RouteService.CreateGarminMessage(jobState.JobId, coord, jobState.MessageId, jobState.JobStatus);
                    jobState.JobStatus = RouteService.ReportDelivery(routeState.RouteId, jobState.JobId, coord, jobState.MessageId, jobState.JobStatus, deviceId, routeState.dateTime);
                }
                //syncList.RouteStatus   = RouteStatus.Completed; 
                return CreatedAtRoute("DefaultApi", new { id = routeState.RouteId }, content: routeState);
            }
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }


        // POST: api/Routes
        public IHttpActionResult Post(int id, [FromBody]RouteEvent routeEvent)
        {
            try
            {
                if (routeEvent == null) return BadRequest();

                string commandStatus;

                switch (routeEvent.RouteCommand.ToUpper())
                {
                    case "START":
                        {
                            commandStatus = RouteService.StartRoute(id);
                        }
                        break;
                    case "FINALIZE":
                        commandStatus = RouteService.FinalizeRoute(id);
                        break;
                    default:
                        return Unauthorized();
                }
                return Ok(commandStatus);
            }
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }

        // DELETE: api/Routes/5
        public void Delete(int id)
        {
        }

        [Route("api/routes/{routeId}/messages")]
        [HttpPost]
        public IHttpActionResult SendDeliveryMessage(int routeId, [FromBody]Message[] messages)
        {
            try
            {
                if (messages == null) return BadRequest();

                var msgCodes = new List<string>();

                var deviceId = GetDeviceId(Request);
                if (deviceId == null) return Unauthorized();

                foreach (var message in messages)
                {
                    msgCodes.Add(RouteService.SendMessageByRouteAndDelivery(routeId, message.MessageType.Code,
                        message.MessageType.Description, message.DateTime.ToLocalTime(), message.JobId, message.Latitude,
                        message.Longitude, deviceId));
                }

                return Ok(msgCodes.ToArray());
            }
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }            
    }
}
