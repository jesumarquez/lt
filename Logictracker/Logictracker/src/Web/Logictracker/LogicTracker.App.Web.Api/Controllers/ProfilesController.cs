using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;

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

            return Ok(new Profile {Messages = messages, MobileId = mobileId});
        }
    }
}
