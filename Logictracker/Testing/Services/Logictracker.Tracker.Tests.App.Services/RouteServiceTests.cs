using NUnit.Framework;

namespace Logictracker.Tracker.Tests.App.Services
{
    [TestFixture]
    public class RouteServiceTests
    {
        //private Mock<DAOFactory> _daoFactoryMock;
        //private IRouteService _routeService;

        [SetUp]
        public void Setup()
        {
        //    var device = new Dispositivo {Imei = "1234567890A", Id = 7890, Codigo = "SGH7100"};
        //    var employee = new Empleado {Empresa = new Empresa() {Id = 123456789}, Linea = new Linea() {Id = 123}};
            
        //    _daoFactoryMock = new Mock<DAOFactory>();
        //    _daoFactoryMock.Setup(x => x.DispositivoDAO.FindByImei(It.IsAny<string>())).Returns(device);
        //    _daoFactoryMock.Setup(x => x.EmpleadoDAO.FindEmpleadoByDevice(It.IsAny<Dispositivo>())).Returns(employee);

        //    _routeService = new RouteService
        //    {
        //        DaoFactory = _daoFactoryMock.Object
        //    };
        }

        [Test]
        public void GetAllMessagesTest()
        {
            ////arrange
            //var acceptanceMessages = new List<Mensaje>();
            //acceptanceMessages.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6050", Descripcion = "RECHAZADO" });
            //acceptanceMessages.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6051", Descripcion = "RECHAZADO" });
            //acceptanceMessages.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6052", Descripcion = "RECHAZADO" });
            //acceptanceMessages.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6053", Descripcion = "RECHAZADO" });
            //acceptanceMessages.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeRechazo = true }, Codigo = "6054", Descripcion = "RECHAZADO" });
           
            //var rejectMessages = new List<Mensaje>();
            //rejectMessages.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeConfirmacion = true }, Codigo = "6055", Descripcion = "RECHAZADO" });
            //rejectMessages.Add(new Mensaje() { TipoMensaje = new TipoMensaje() { DeConfirmacion = true }, Codigo = "6056", Descripcion = "RECHAZADO" });

            //_daoFactoryMock.Setup(x => x.MensajeDAO.GetMensajesDeConfirmacion(It.IsAny<int[]>(), It.IsAny<int[]>())).Returns(acceptanceMessages);
            //_daoFactoryMock.Setup(x => x.MensajeDAO.GetMensajesDeRechazo(It.IsAny<int[]>(), It.IsAny<int[]>())).Returns(rejectMessages);

            ////act
            //var messages = _routeService.GetAllMessages("1234567890A");

            ////assert
            //Assert.AreEqual(acceptanceMessages.Count + rejectMessages.Count, messages.Count);
        }
    }
}
