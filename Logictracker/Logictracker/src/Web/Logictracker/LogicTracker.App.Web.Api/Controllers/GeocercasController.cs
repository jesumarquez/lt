using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using LogicTracker.App.Web.Api.Controllers;
using LogicTracker.App.Web.Api.Models;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class GeocercasController : BaseController
    {
        public IRouteService RouteService { get; set; }
        public static string ROUTE_STATUS_PENDING = "0";
        public static string ROUTE_STATUS_ACTIVE = "1";
        public static string ROUTE_STATUS_FINALIZE = "9";


        public IHttpActionResult Get()
        {
            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return BadRequest();


            PuertaAccesoDAO puertas = new PuertaAccesoDAO(); // puerta
            ReferenciaGeograficaDAO referencias = new ReferenciaGeograficaDAO();
            EmpleadoDAO empleado = new EmpleadoDAO();// emple //fecha
            
            DispositivoDAO dispositivo = new DispositivoDAO();
            EventoAccesoDAO eventos = new EventoAccesoDAO();
           // eventos.Save()

              var device = dispositivo.FindByImei(deviceId);
            var employee = empleado.FindEmpleadoByDevice(device);

            List<int> empresas = new List<int>();
            empresas.Add(employee.Empresa.Id);
              var lineas = new int[] {};

             List<PuertaAcceso> lista = puertas.GetList(empresas, lineas);
             foreach (var item in lista)
             {
                // item.ZonaAccesoEntrada.
             }
  
                     
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
                        Places = "",
                        Status = viajeDistribucion.Estado.ToString(),
                        StartDateTime = viajeDistribucion.Inicio
                    });
                }
            }
            listRoute.RouteItems = items.ToArray();
            return Ok(listRoute);
        }

 

        // GET: api/Routes/1234567890A
        public IHttpActionResult Get(int id)
        {            
            return Ok();
        }

        [Route("api/routes/{routeId}/messages")]
        [HttpPost]
        public IHttpActionResult actualizarGeocerca([FromBody]Message[] messages)
        {
            if (messages == null) return BadRequest();

            var msgCodes = new List<string>();

            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return Unauthorized();


            /*
             ACTUALIZO GEOCERCA
             */

            return Ok(msgCodes.ToArray());
        }

    }
}