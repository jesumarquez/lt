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
using Logictracker.Messages.Sender;
using Logictracker.Messages.Saver;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Messaging;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class MessagesController : BaseController
    {
        public IRouteService RouteService { get; set; }

        public DAOFactory DaoFactory { get; set; }


        // GET: api/Messages
        public IHttpActionResult Get()
        {
            try
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
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }

        // GET: api/Messages/5
        public IHttpActionResult Get(string id)
        {
            try
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
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
        }

        // POST: api/Messages
        public IHttpActionResult Post([FromBody]List<CustomMessage> messages)
        {
            try
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
                    if (!String.IsNullOrEmpty(message.codigomensaje) &&
                        message.codigomensaje.StartsWith("R"))
                    {
                        message.codigomensaje = message.codigomensaje.Substring(1, message.codigomensaje.Length - 1);
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

                                    Dispositivo device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
                                    if (device == null) continue;

                                    var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);
                                    if (employee == null) continue;

                                    var idRechazo = Convert.ToInt32(messageLog.Texto.Split(':')[0].Split(' ').Last());
                                    var rechazo = DaoFactory.TicketRechazoDAO.FindById(idRechazo);

                                    if (rechazo != null)
                                    {
                                        try
                                        {
                                            if (rechazo.UltimoEstado == TicketRechazo.Estado.Notificado1 ||
                                                rechazo.UltimoEstado == TicketRechazo.Estado.Notificado2 ||
                                                rechazo.UltimoEstado == TicketRechazo.Estado.Notificado3)
                                            {
                                                IMessageSaver saver = new MessageSaver(DaoFactory);
                                                var messagetEXT = MessageSender.CreateSubmitTextMessage(device, saver);
                                                string usuario = "";
                                                try
                                                {
                                                    rechazo.ChangeEstado(Logictracker.Types.BusinessObjects.Rechazos.TicketRechazo.Estado.Alertado, "Confirma atención", employee);
                                                    DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);

                                                    //El usuario tomo existosamente el rechazo                                                    
                                                    foreach (var item in rechazo.Detalle)
                                                    {
                                                        if (item.Estado == TicketRechazo.Estado.Alertado)
                                                        {
                                                            usuario = item.Empleado.Entidad.Descripcion;
                                                            break;
                                                        }
                                                    }
                                                    messagetEXT.AddMessageText("INFORME DE RECHAZO NRO " + idRechazo + " SE CONFIRMA LA ASISTENCIA PARA: " + usuario);
                                                    messagetEXT.Send();
                                                }
                                                catch (Exception ex)
                                                {                                                    
                                                    messagetEXT.AddMessageText("INFORME DE RECHAZO NRO " + idRechazo + " HA OCURRIDO UN ERROR, POR FAVOR INTENTE NUEVAMENTE ASISTIR EL RECHAZO");
                                                    messagetEXT.Send();
                                                    throw ex;
                                                }                                                
                                            }
                                            else
                                            {
                                                //El usuario ya fue alertado
                                                IMessageSaver saver = new MessageSaver(DaoFactory);
                                                var messagetEXT = MessageSender.CreateSubmitTextMessage(device, saver);
                                                string usuario = "";
                                                foreach (var item in rechazo.Detalle)
                                                {
                                                    if (item.Estado == TicketRechazo.Estado.Alertado)
                                                    {
                                                        usuario = item.Empleado.Entidad.Descripcion;
                                                        break;
                                                    }
                                                }
                                                messagetEXT.AddMessageText("INFORME DE RECHAZO NRO " + idRechazo + " EL RECHAZO YA FUE TOMADO POR: " + usuario);
                                                messagetEXT.Send();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            if (!ex.Message.ToString().Contains("Cambio de estado invalido"))
                                                throw ex;
                                        }
                                    }
                                    break;
                                }
                            default:
                                {
                                    TicketRechazo.Estado rechazoEstadoEnum = (TicketRechazo.Estado)int.Parse(message.codigomensaje.ToString());
                                    switch (rechazoEstadoEnum)
                                    {
                                        case TicketRechazo.Estado.RespuestaExitosa:
                                            {
                                                esMensajeOculto = true;
                                                var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
                                                if (device == null) continue;
                                                var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);
                                                if (employee == null) continue;
                                                var rechazo = DaoFactory.TicketRechazoDAO.FindById(message.Id);

                                                if (rechazo != null &&
                                                    message.Id > 0)
                                                {
                                                    try
                                                    {

                                                        if (rechazo.UltimoEstado != TicketRechazo.Estado.RespuestaExitosa)
                                                        {
                                                            rechazo.ChangeEstado(Logictracker.Types.BusinessObjects.Rechazos.TicketRechazo.Estado.RespuestaExitosa, message.Description, employee);
                                                            DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);
                                                        }

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        if (!ex.Message.ToString().Contains("Cambio de estado invalido"))
                                                            throw ex;
                                                    }
                                                }
                                                break;
                                            }
                                        case TicketRechazo.Estado.RespuestaConRechazo:
                                            {
                                                esMensajeOculto = true;

                                                var device = DaoFactory.DispositivoDAO.FindByImei(deviceId);
                                                if (device == null) continue;
                                                var employee = DaoFactory.EmpleadoDAO.FindEmpleadoByDevice(device);
                                                if (employee == null) continue;
                                                var rechazo = DaoFactory.TicketRechazoDAO.FindById(message.Id);
                                                if (rechazo != null &&
                                                    rechazo.Id != 0)
                                                {
                                                    try
                                                    {
                                                        if (rechazo.UltimoEstado != TicketRechazo.Estado.RespuestaConRechazo)
                                                        {
                                                            rechazo.ChangeEstado(Logictracker.Types.BusinessObjects.Rechazos.TicketRechazo.Estado.RespuestaConRechazo, message.Description, employee);
                                                            DaoFactory.TicketRechazoDAO.SaveOrUpdate(rechazo);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        if (!ex.Message.ToString().Contains("Cambio de estado invalido"))
                                                            throw ex;
                                                    }
                                                }
                                                break;
                                            }
                                        default:
                                            break;
                                    }
                                    break;
                                }
                        }


                    }
                    if (!esMensajeOculto)
                        mensajes.Add(logMensaje);
                }


                var value = RouteService.SendMessagesMobile(deviceId, mensajes);

                return CreatedAtRoute("DefaultApi", null, value);
            }
            catch (Exception error)
            {
                LogicTracker.App.Web.Api.Providers.LogWritter.writeLog(error);
                return BadRequest();
            }
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
