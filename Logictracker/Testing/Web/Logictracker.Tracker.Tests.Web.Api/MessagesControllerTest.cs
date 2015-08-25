using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using LogicTracker.App.Web.Api.Controllers;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.Messages;
using Moq;
using NUnit.Framework;

namespace Logictracker.Tracker.Tests.Web.Api
{
    [TestFixture]
    class MessagesControllerTest
    {
        private Mock<IRouteService> _routeService;
        private MessagesController _controller;

        [SetUp]
        public void Setup()
        {
            _routeService = new Mock<IRouteService>();

            var request = new HttpRequestMessage();
            request.Headers.Add("deviceId", "353771055938848");

            _controller = new MessagesController
            {
                RouteService = _routeService.Object,
                Request = request,
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public void CanGetMessagesTest()
        {
            _routeService.Setup(x => x.GetMessagesMobile(It.IsAny<string>())).Returns(SampleMessages());

            var actionResult = _controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<MessageList>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsInstanceOf(typeof(MessageList), contentResult.Content, "the content is not a list of messages");

            var messages = contentResult.Content.MessageItems;
            Assert.AreEqual(SampleMessages().Count, messages.Count());            
        }

        [Test]
        public void CanGetNewMessagesTest()
        {
            _routeService.Setup(x => x.GetMessagesMobile(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(SampleMessages());

            var actionResult = _controller.Get("20150810T131205");
            var contentResult = actionResult as OkNegotiatedContentResult<MessageList>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsInstanceOf(typeof(MessageList), contentResult.Content, "the content is not a list of messages");

            var messages = contentResult.Content.MessageItems;
            Assert.AreEqual(SampleMessages().Count, messages.Count());
        }

        private IList<LogMensaje> SampleMessages()
        {
            var listMessage = new List<LogMensaje>();

            var msg = new LogMensaje
            {
                Id = 4545,
                Texto = "Mensaje Saliente: unit test msg1",
                Fecha = DateTime.Now,
                Latitud = 0.0,
                Longitud = 0.0
            };
            listMessage.Add(msg);

            msg = new LogMensaje
            {
                Id = 4546,
                Texto = "Mensaje Saliente: unit test msg2",
                Fecha = DateTime.Now,
                Latitud = 0.0,
                Longitud = 0.0
            };
            listMessage.Add(msg);

            msg = new LogMensaje
            {
                Id = 4547,
                Texto = "Mensaje Saliente: unit test msg3",
                Fecha = DateTime.Now,
                Latitud = 0.0,
                Longitud = 0.0
            };
            listMessage.Add(msg);

            return listMessage;
        }

        private List<CustomMessage> SampleCustomMessages()
        {
            var listMessage = new List<CustomMessage>();

            var msg = new CustomMessage
                {
                    Id = 1,
                    Description = "mensaje de prueba 1",
                    DateTime = DateTime.UtcNow,
                    Latitude = 3.71,
                    Longitude = -71.5
                };
            listMessage.Add(msg);

            msg = new CustomMessage
                {
                    Id = 2,
                    Description = "mensaje de prueba 2",
                    DateTime = DateTime.UtcNow,
                    Latitude = 3.72,
                    Longitude = -72.5
                };
            listMessage.Add(msg);

            return listMessage;
        }
    }
}
