using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logictracker.DAL.Factories;
using Logictracker.Tracker.Application.Services;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Moq;
using NUnit.Framework;
//using Spring.Messaging.Core;

namespace Logictracker.Tracker.Tests.App.Services
{
    [TestFixture]
    class ReceptionServiceTest
    {
        private Mock<DAOFactory> _daoFactoryMock;
        //private Mock<MessageQueueTemplate> _msmqTemplateMock;
        private IReceptionService _receptionService;

        private Dispositivo _device;

        [SetUp]
        public void Setup()
        {
            _device = new Dispositivo { Imei = "1234567890A", Id = 7890, Codigo = "SGH7100" };
            var employee = new Empleado { Empresa = new Empresa() { Id = 123456789 }, Linea = new Linea() { Id = 123 } };

            _daoFactoryMock = new Mock<DAOFactory>();
            _daoFactoryMock.Setup(x => x.DispositivoDAO.FindByImei(It.IsAny<string>())).Returns(_device);
            _daoFactoryMock.Setup(x => x.EmpleadoDAO.FindEmpleadoByDevice(It.IsAny<Dispositivo>())).Returns(employee);

            //_msmqTemplateMock = new Mock<MessageQueueTemplate>();
            //_msmqTemplateMock.Setup(x => x.ConvertAndSend(It.IsAny<object>()));
            
            _receptionService = new ReceptionService
            {
                DaoFactory = _daoFactoryMock.Object
            };
        }

        [Test]
        public void GetAllMessagesTest()
        {

        }
    }
}
