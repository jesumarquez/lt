using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Positions;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class RoutesController : BaseController
    {
        public IRouteService RouteService { get; set; }

        public static string ROUTE_STATUS_PENDING = "0";
        public static string ROUTE_STATUS_ACTIVE = "1";
        public static string ROUTE_STATUS_FINALIZE = "9";


        // GET: api/Routes/
        public IHttpActionResult Get()
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
                    items.Add(new RouteItem()
                    {
                        Code = viajeDistribucion.Codigo,
                        DeliveriesNumber = viajeDistribucion.Detalles.Count - 1,
                        Id = viajeDistribucion.Id,
                        Places = PlacesToDescription(viajeDistribucion),
                        Status = viajeDistribucion.Estado.ToString(),
                        StartDateTime = viajeDistribucion.Inicio
                    });
                }
            }

            listRoute.RouteItems = items.ToArray();
            return Ok(listRoute);
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
                job.Code = detail.Id.ToString();
                job.StartDate = detail.Programado.ToString("yyyy-MM-ddTHH:mm:ss");
                job.EndDate = detail.ProgramadoHasta.ToString("yyyy-MM-ddTHH:mm:ss");
                job.State = detail.Estado==3 ? 0 : detail.Estado;
                if (detail.PuntoEntrega != null && detail.PuntoEntrega.Descripcion != null) 
                    job.Name = detail.PuntoEntrega.Descripcion;
                else
                {
                    if (detail.Cliente != null) job.ClientName = detail.Cliente.Descripcion;
                }
                job.Description = detail.Descripcion;
                job.Order = detail.Orden;
                job.Location = new Location();

                if (detail.PuntoEntrega != null)
                {
                    job.Location.Latitude = (float) detail.PuntoEntrega.ReferenciaGeografica.Latitude;
                    job.Location.Longitude = (float) detail.PuntoEntrega.ReferenciaGeografica.Longitude;
                }
                else
                {
                    job.Location.Latitude = (float) detail.Linea.ReferenciaGeografica.Latitude;
                    job.Location.Longitude = (float) detail.Linea.ReferenciaGeografica.Longitude;
                }

                if (detail.PuntoEntrega!=null)
                    jobs.Add(job);
            }

            route.Jobs = jobs.ToArray();
            return Ok(route);
        }
        
        // POST: api/Routes
        public IHttpActionResult Post([FromBody]RouteState routeState)
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
                jobState.JobStatus = RouteService.ReportDelivery(routeState.RouteId, jobState.JobId, coord, jobState.MessageId, jobState.JobStatus, deviceId);
            }
            //syncList.RouteStatus   = RouteStatus.Completed; 
            return CreatedAtRoute("DefaultApi", new { id = routeState.RouteId }, content: routeState);
        }


        // POST: api/Routes
        public IHttpActionResult Post(int id, [FromBody]RouteEvent routeEvent)
        {
            if (routeEvent == null) return BadRequest();

            string commandStatus;

            switch (routeEvent.RouteCommand.ToUpper())
            {
                case "START":
                    commandStatus = RouteService.StartRoute(id);
                    break;
                case "FINALIZE":
                    commandStatus = RouteService.FinalizeRoute(id);
                    break;
                default:
                    return Unauthorized();
            }
            return Ok(commandStatus);
        }

        // DELETE: api/Routes/5
        public void Delete(int id)
        {
        }

        [Route("api/routes/{routeId}/messages")]
        [HttpPost]
        public IHttpActionResult SendDeliveryMessage(int routeId, [FromBody]Message[] messages)
        {
            if (messages == null) return BadRequest();

            var msgCodes= new List<string>();

            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return Unauthorized();

            foreach (var message in messages)
            {
                msgCodes.Add(RouteService.SendMessageByRouteAndDelivery(routeId, message.MessageType.Code,
                    message.MessageType.Description, message.DateTime.ToLocalTime(), message.JobId, message.Latitude,
                    message.Longitude, deviceId) );
            }

            return Ok(msgCodes.ToArray());
        }            
    }
}
