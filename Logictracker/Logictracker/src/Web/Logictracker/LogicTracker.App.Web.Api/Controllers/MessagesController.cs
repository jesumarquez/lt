using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class MessagesController : ApiController
    {
        public IRouteService RouteService { get; set; }

        // GET: api/Messages
        public IHttpActionResult Get()
        {
            var messages = RouteService.GetMessagesMobile("353771055938848");

            var messageTypeList = new List<MessageType>();
            foreach (var logMensaje in messages)
            {
                var messageType = new MessageType
                {
                    Code = logMensaje.CodigoMensaje,
                    Description = "[" + logMensaje.Fecha.ToLocalTime().ToString("g") + "] " + logMensaje.Texto.Split(':')[1].Trim(),
                    type = 2
                };
                messageTypeList.Add(messageType);
            }
            
            return Ok(messageTypeList.ToArray());
        }

        // GET: api/Messages/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Messages
        public void Post([FromBody]string value)
        {
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
