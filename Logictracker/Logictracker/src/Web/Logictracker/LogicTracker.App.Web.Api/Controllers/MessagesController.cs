using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Rechazos;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class MessagesController : BaseController
    {
        public IRouteService RouteService { get; set; }

        public DAOFactory DaoFactory { get; set; }


        // GET: api/Messages
        public IHttpActionResult Get()
        {
            var messages = RouteService.GetMessagesMobile(GetDeviceId(Request));
            if (messages == null) return Unauthorized();
            if (messages.Count < 1) return Ok(new MessageList());
            var listMessage = new MessageList();
            var customMessageList = new List<CustomMessage>();
            foreach (var logMensaje in messages)
            {
                var message = new CustomMessage
                {
                    Id = logMensaje.Id,
                    Description = logMensaje.Texto.Split(':')[1].Trim(),
                    DateTime = logMensaje.Fecha,
                    Latitude = logMensaje.Latitud,
                    Longitude = logMensaje.Longitud
                };
                customMessageList.Add(message);
            }
            listMessage.MessageItems = customMessageList.ToArray();
            return Ok(listMessage);
        }

        // GET: api/Messages/5
        public IHttpActionResult Get(string id)
        {
            Regex r = new Regex(@"^\d{4}\d{2}\d{2}T\d{2}\d{2}\d{2}$");
            if (!r.IsMatch(id))
                return BadRequest();

             var dt = DateTime.ParseExact(id, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None);
             if (!dt.Day.Equals(DateTime.Now.Day))
                 dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                              

            var messages = RouteService.GetMessagesMobile(GetDeviceId(Request), dt);

            if (messages == null) return Unauthorized();

            if (messages.Count < 1) return Ok(new MessageList());

            var listMessage = new MessageList();
            var customMessageList = new List<CustomMessage>();
            foreach (var logMensaje in messages)
            {
                var message = new CustomMessage
                {
                    Id = logMensaje.Id,
                    Description = logMensaje.Texto,//.Split(':')[1].Trim(),
                    DateTime = logMensaje.Fecha,
                    Latitude = logMensaje.Latitud,
                    Longitude = logMensaje.Longitud,
                    codigomensaje = logMensaje.Mensaje.Codigo
                };
                customMessageList.Add(message);
            }
            listMessage.MessageItems = customMessageList.ToArray();
            return Ok(listMessage);
        }

        // POST: api/Messages
        public IHttpActionResult Post([FromBody]List<CustomMessage> messages)
        {
            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return Unauthorized();

            if (messages == null) return BadRequest();

            var mensajes = new List<LogMensaje>();
            foreach (var message in messages)
            {
                var logMensaje = new LogMensaje
                {
                    Fecha = message.DateTime,
                    Texto = message.Description,
                    Latitud = message.Latitude,
                    Longitud = message.Longitude
                };
                bool esMensajeOculto = false;
                if (!String.IsNullOrEmpty(message.codigomensaje))
                {
                    TicketRechazo.MotivoRechazo rechazoEnum = (TicketRechazo.MotivoRechazo)int.Parse(message.codigomensaje.ToString());
                    switch (rechazoEnum)
                    {
                        case TicketRechazo.MotivoRechazo.MalFacturado:
                        case TicketRechazo.MotivoRechazo.MalPedido:
                        case TicketRechazo.MotivoRechazo.NoEncontroDomicilio:
                        case TicketRechazo.MotivoRechazo.NoPedido:
                        case TicketRechazo.MotivoRechazo.Cerrado:
                        case TicketRechazo.MotivoRechazo.CaminoIntransitable:
                        case TicketRechazo.MotivoRechazo.FaltaSinCargo:
                        case TicketRechazo.MotivoRechazo.FueraDeHorario:
                        case TicketRechazo.MotivoRechazo.FueraDeZona:
                        case TicketRechazo.MotivoRechazo.ProductoNoApto:
                        case TicketRechazo.MotivoRechazo.SinDinero:
                            {
                                esMensajeOculto = true;
                                var messageLog = DaoFactory.LogMensajeDAO.FindById(message.Id);

                                List<EvenDistri> distri = DaoFactory.EvenDistriDAO.GetByMensajes(new List<LogMensaje>() { messageLog });
                                var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
                                if (device == null) continue;

                                var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);
                                if (employee == null) continue;

                                foreach (var item in distri)
                                {
                                    if (item.Entrega != null &&
                                        item.Entrega.PuntoEntrega != null)
                                    {
                                        var rechazo = DaoFactory.TicketRechazoDAO.GetByPuntoEntregaYFecha(item.Entrega.PuntoEntrega.Id, DateTime.Today, DateTime.UtcNow);
                                        if (rechazo != null)
                                        {
                                            try
                                            {
                                                rechazo.ChangeEstado(Logictracker.Types.BusinessObjects.Rechazos.TicketRechazo.Estado.AlertadoAutomatico, "Mensaje leído", employee);
                                                DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);
                                            }
                                            catch (Exception ex)
                                            {
                                                if (!ex.Message.ToString().Contains("Cambio de estado invalido"))
                                                    throw ex;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        default:
                            break;
                    }
                }
                if (!esMensajeOculto)
                    mensajes.Add(logMensaje);
            }            
           

            var value = RouteService.SendMessagesMobile(deviceId, mensajes);

            return CreatedAtRoute("DefaultApi", null, value);                              
        }

        // PUT: api/Messages/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Messages/5
        public void Delete(int id)
        {
        }
    }
}
