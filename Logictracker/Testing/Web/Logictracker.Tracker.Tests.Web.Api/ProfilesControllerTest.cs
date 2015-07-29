using System.Net.Http;
using System.Web.Http;
using LogicTracker.App.Web.Api.Controllers;
using Logictracker.Tracker.Services;
using Moq;
using NUnit.Framework;

namespace Logictracker.Tracker.Tests.Web.Api
{
    [TestFixture]
    class ProfilesControllerTest
    {
        private Mock<IRouteService> _routeSevice;
        private ProfilesController _controller;

        [SetUp]
        public void Setup()
        {
            _routeSevice = new Mock<IRouteService>();

            var request = new HttpRequestMessage();
            request.Headers.Add("deviceId", "353771055938848");

            _controller = new ProfilesController 
            {
                RouteService = _routeSevice.Object,
                Request = request,
                Configuration = new HttpConfiguration()
            };
        }

        //[Test]
        //public void GetProfilesWithCorrectId()
        //{
        //    var mensajes = new List<Mensaje>();
        //    mensajes.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6050", Descripcion = "RECHAZADO" });
        //    mensajes.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6051", Descripcion = "RECHAZADO" });
        //    mensajes.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6052", Descripcion = "RECHAZADO" });
        //    mensajes.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6053", Descripcion = "RECHAZADO" });
        //    mensajes.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6054", Descripcion = "RECHAZADO" });
        //    mensajes.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeConfirmacion = true }, Codigo = "6055", Descripcion = "RECHAZADO" });
        //    mensajes.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeConfirmacion = true }, Codigo = "6056", Descripcion = "RECHAZADO" });

        //    var fakeProfile = new Profile()
        //    {
        //        Messages = new List<MessageType>()
        //        {
        //            new MessageType {type = 0, Code = "6050", Description = "RECHAZADO"},
        //            new MessageType {type = 0, Code = "6051", Description = "MAL FACTURADO"},
        //            new MessageType {type = 0, Code = "6052", Description = "MAL PEDIDO"},
        //            new MessageType {type = 0, Code = "6053", Description = "NO ENCONTRO DOMICILIO"},
        //            new MessageType {type = 0, Code = "6054", Description = "RECHAZO - NO PEDIDO"},
        //            new MessageType {type = 1, Code = "6055", Description = "Mensaje aceptado 1"},
        //            new MessageType {type = 1, Code = "6056", Description = "Mensaje aceptado 2"}
        //        }
        //    };

        //    _routeSevice.Setup(x => x.GetAllMessages(It.IsAny<string>())).Returns(mensajes);

        //    var request = new HttpRequestMessage();
        //    request.Headers.Add("deviceId", "353771055938848");
            
        //    _controller.Request = request;

        //    // Act
        //    IHttpActionResult actionResult = _controller.Get();
        //    var contentResult = actionResult as OkNegotiatedContentResult<Profile>;

        //    // Assert
        //    Assert.IsNotNull(contentResult);
        //    Assert.IsNotNull(contentResult.Content);
        //    Assert.AreEqual(7, contentResult.Content.Messages.Count);
        //}
    }
}
