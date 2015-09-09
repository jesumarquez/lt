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
            return Ok();
        }

 

        // GET: api/Routes/1234567890A
        public IHttpActionResult Get(int id)
        {
            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return BadRequest();

            var items = new List<Geocerca>();


            PuertaAccesoDAO puertas = new PuertaAccesoDAO(); // puerta
            ReferenciaGeograficaDAO referencias = new ReferenciaGeograficaDAO();
            EmpleadoDAO empleado = new EmpleadoDAO();// emple //fecha
            DispositivoDAO dispositivo = new DispositivoDAO();          

            var device = dispositivo.FindByImei(deviceId);
            var employee = empleado.FindEmpleadoByDevice(device);

            List<int> empresas = new List<int>();
            empresas.Add(employee.Empresa.Id);
            var lineas = new int[] { };

            List<PuertaAcceso> lista = puertas.GetList(empresas, lineas);
            foreach (var item in lista)
            {
                if (item.ReferenciaGeografica != null &&
                    !items.Exists(x => x.id.Equals(item.Id)))
                {
                    if (item.ReferenciaGeografica.Id > id)
                    {
                        string radio = "50";
                        if (item.ReferenciaGeografica.Poligono != null)
                            radio = item.ReferenciaGeografica.Poligono.Radio.ToString();
                        string calle = "";
                        string altura = "";
                        if (item.ReferenciaGeografica.Direccion != null)
                        {
                            calle = item.ReferenciaGeografica.Direccion.Calle;
                            altura = item.ReferenciaGeografica.Direccion.Altura.ToString();
                        }
                        items.Add(new Geocerca()
                        {
                            id = Convert.ToString(item.ReferenciaGeografica.Id),
                            latitude = item.ReferenciaGeografica.Latitude.ToString(),
                            nombre = item.Descripcion,
                            radio = radio,
                            longitude = item.ReferenciaGeografica.Longitude.ToString(),
                            calle = calle,
                            altura = altura,
                            idpuerta = item.Id.ToString()
                        });
                    }
                }
            }
            return Ok(items.ToArray().OrderBy(item => item.id).ToArray());
        }

        [Route("api/Geocercas/geocerca")]
        [HttpPost]
        public IHttpActionResult actualizarGeocerca([FromBody]Geocerca geocerca)
        {
            if (geocerca == null) return BadRequest();
            var msgCodes = new List<string>();
            var deviceId = GetDeviceId(Request);
            if (deviceId == null) 
                return Unauthorized();

            DateTime horaentrada = DateTime.ParseExact(geocerca.horaentrada.ToString(), "yyyy-MM-dd-HH.mm.ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            DateTime horasalida = DateTime.ParseExact(geocerca.horariosalida.ToString(), "yyyy-MM-dd-HH.mm.ss.fff", System.Globalization.CultureInfo.InvariantCulture);

            EmpleadoDAO empleado = new EmpleadoDAO();// emple //fecha
            DispositivoDAO dispositivo = new DispositivoDAO();      
            PuertaAccesoDAO puertas = new PuertaAccesoDAO(); // puerta

            var device = dispositivo.FindByImei(deviceId);
            var employee = empleado.FindEmpleadoByDevice(device);

            List<int> empresas = new List<int>();
            empresas.Add(employee.Empresa.Id);
            var lineas = new int[] { };
            PuertaAcceso puerta = puertas.GetList(empresas, lineas).Where(x => x.Id.ToString().Equals(geocerca.idpuerta)).First();
            if (puerta != null)
            {
                EventoAcceso entrada = new EventoAcceso();
                entrada.Empleado = employee;
                entrada.Alta = DateTime.UtcNow;
                entrada.Entrada = true;

                entrada.Fecha = horaentrada.ToUniversalTime();
                entrada.Puerta = puerta;

                EventoAccesoDAO eventos = new EventoAccesoDAO();
                eventos.Save(entrada);

                EventoAcceso salida = new EventoAcceso();
                salida.Empleado = employee;
                salida.Alta = DateTime.UtcNow;
                salida.Entrada = false;
                salida.Fecha = horasalida.ToUniversalTime();
                salida.Puerta = puerta;

                eventos.Save(salida);
            }
            return Ok(msgCodes.ToArray());

            

            
           
        }

    }
}