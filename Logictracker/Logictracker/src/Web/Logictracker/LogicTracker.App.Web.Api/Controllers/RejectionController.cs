using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.DAO.BusinessObjects.Rechazos;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using Logictracker.DAL.DAO.BusinessObjects;
using Logictracker.Types.BusinessObjects.Rechazos;
using System.Linq;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class RejectionController : BaseController
    {
        public IRouteService RouteService { get; set; }


        public IHttpActionResult Get()
        {
            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return BadRequest();
            List<Rechazo> retorno = new List<Rechazo>();
            return Ok(retorno);
        }
        
    
        public IHttpActionResult Get(int id)
        {
            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return BadRequest();

            List<Rechazo> retorno = new List<Rechazo>();

            var device = new DispositivoDAO().FindByImei(deviceId.ToString());
            var empleado = new EmpleadoDAO().FindEmpleadoByDevice(device);
            TicketRechazoDAO dao = new TicketRechazoDAO();
            var desde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            var hasta = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            var activos = dao.GetByEmpleadoAndFecha(empleado, desde, hasta).AsQueryable();
            List<TicketRechazo> list = activos.Where(x => !x.UltimoEstado.Equals(TicketRechazo.Estado.Resuelto) &&
                                 x.Id >= id).ToList();

            foreach (TicketRechazo item in list)
            {
                TicketRechazo.Estado[] estados = TicketRechazo.Next(item.UltimoEstado);
                List<Estado> estadosmodel = new List<Estado>();

                foreach (var itemEstado in item.Detalle)
                {
                    estadosmodel.Add(new Estado()
                    {
                        id = (int)itemEstado.Id,
                        fechahora = itemEstado.FechaHora,
                        empleado = itemEstado.Empleado.Entidad.Descripcion.ToString(),
                        estado = itemEstado.Estado.ToString(),
                        observacion = itemEstado.Observacion,
                        transportista = item.Transportista.Descripcion.ToString(),
                        enhorario = item.EnHorario.ToString(),
                        cliente = item.Cliente.ToString(),
                    });
                }
                retorno.Add(
                    new Rechazo()
                    {
                        id = item.Id,
                        fechahora = item.FechaHora,
                        motivo = item.Motivo.ToString(),
                        estado = item.UltimoEstado.ToString(),
                        bultos = item.Bultos.ToString(),
                        codentrega = item.Entrega.Codigo,
                        nombre = item.Entrega.Descripcion,
                        vendedor = item.Vendedor.Entidad.Descripcion.ToString(),
                        supventa = item.SupervisorVenta.Entidad.Descripcion.ToString(),
                        supruta = item.SupervisorRuta.Entidad.Descripcion.ToString(),
                        territorio = item.Territorio,
                        estados = estadosmodel.ToArray()
                    });
            }
            return Ok(retorno.ToArray());
        }


        [Route("api/Geocercas/geocerca")]
        [HttpPost]
        public IHttpActionResult actualizarGeocerca([FromBody]Geocerca geocerca)
        {
            if (geocerca == null) return BadRequest();
            return Ok("");
        }
    }
}
