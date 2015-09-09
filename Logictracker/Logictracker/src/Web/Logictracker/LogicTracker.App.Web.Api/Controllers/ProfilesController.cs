using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class ProfilesController : BaseController
    {
        public IRouteService RouteService { get; set; }

        public IHttpActionResult Get()
        {
            var deviceId = GetDeviceId(Request);
            if (deviceId == null) return BadRequest();          

            var mobileId = RouteService.GetMobileIdByImei(deviceId);
            DetalleDispositivoDAO detalle = new DetalleDispositivoDAO();
            IList<DetalleDispositivo> lista = detalle.GetDeviceDetails(mobileId);
            var routeMessages = RouteService.GetProfileMessages(deviceId);

            if (routeMessages == null) return Unauthorized();   

            var messages = new List<MessageType>();

            foreach (var msg in routeMessages)
            {
                var messageType = new MessageType { Type = 1, Code = msg.Codigo, Description = msg.Descripcion };

                if (msg.TipoMensaje.DeRechazo)
                    messageType.Type = 0;

                if (msg.TipoMensaje.DeUsuario)
                    messageType.Type = 2;

                messages.Add(messageType);
            }

            if (messages.Count <= 0) return NotFound();

            Profile profile = new Profile { Messages = messages, MobileId = mobileId };

            profile.modocamion = Convert.ToInt32(true);
            profile.modovendedor = Convert.ToInt32(false);
            profile.trackingudp = Convert.ToInt32(false);
            profile.modocamionvendedor = Convert.ToInt32(false);

            foreach (DetalleDispositivo item in lista)
            {
                switch (item.TipoParametro.Nombre)
                {
                    case "Modo_Camion":
                        {
                            bool result = true;
                            if (Boolean.TryParse(item.Valor, out result))
                                profile.modocamion = Convert.ToInt32(result);
                            else
                                profile.modocamion = Convert.ToInt32(true);
                        }
                        break;
                    case "Modo_Vendedor":
                        {
                            bool result = false;
                            if (Boolean.TryParse(item.Valor, out result))
                                profile.modovendedor = Convert.ToInt32(result);
                            else
                                profile.modovendedor = Convert.ToInt32(false);
                        }
                        break;
                    case "Tracking_UDP":
                        {
                            bool result = false;
                            if (Boolean.TryParse(item.Valor, out result))
                                profile.trackingudp = Convert.ToInt32(result);
                            else
                                profile.trackingudp = Convert.ToInt32(false);
                        }
                        break;
                    case "Modo_CamionVendedor":
                        {
                            bool result = false;
                            if (Boolean.TryParse(item.Valor, out result))
                                profile.modocamionvendedor = Convert.ToInt32(result);
                            else
                                profile.modocamionvendedor = Convert.ToInt32(false);
                        }
                        break;                        
                    default:
                        break;
                }
            }
            return Ok(profile);
        }
    }
}
