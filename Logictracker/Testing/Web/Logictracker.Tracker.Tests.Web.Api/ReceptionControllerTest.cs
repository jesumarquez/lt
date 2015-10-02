using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using LogicTracker.App.Web.Api.Controllers;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logitracker.Codecs.Sitrack;
using Moq;
using NUnit.Framework;

namespace Logictracker.Tracker.Tests.Web.Api
{
    class ReceptionControllerTest
    {
        private Mock<IReceptionService> _receptionSevice;
        private ReceptionController _controller;

        [SetUp]
        public void Setup()
        {
            _receptionSevice = new Mock<IReceptionService>();
            
            _controller = new ReceptionController
            {
                ReceptionService = _receptionSevice.Object,
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public void PutOnePositionTest()
        {
            _receptionSevice.Setup(x => x.ParseSitrackPositions(It.IsAny<List<SitrackFrame>>()));

            var sitrackReceptionArray = new SitrackReceptionModel[1];
            sitrackReceptionArray[0] = GetOneSitrackPositionJson(); 
            // Act
            IHttpActionResult actionResult = _controller.ReceivePosition(sitrackReceptionArray);
            var oneFrame = _controller.SitrackModelToFrame(sitrackReceptionArray);

            var contentResult = actionResult as OkResult;

            // Assert
            Assert.IsNotNull(contentResult);
            //_receptionSevice.Verify(x=>x.ParseSitrackPositions(oneFrame));
        }

        [Test]
        public void PutManyPositionsTest()
        {
            _receptionSevice.Setup(x => x.ParseSitrackPositions(It.IsAny<List<SitrackFrame>>()));

            var sitrackReceptionArray = GetManySitrackPositionJson();

            // Act
            IHttpActionResult actionResult = _controller.ReceivePosition(sitrackReceptionArray);

            var contentResult = actionResult as OkResult;

            // Assert
            Assert.IsNotNull(contentResult);
            //_receptionSevice.Verify(x=>x.ParseSitrackPositions(oneFrame));
        }

        private SitrackReceptionModel GetOneSitrackPositionJson()
        {
            var jsonString = @"{ 'id':'3708232932','reportDate':'2013-06-29T15:01:02Z','device_id':21501, 'holder_id':51258,
                'holder_domain':'AAA111','holder_name':'C11','event_id':2,'event_desc':'Reporte por tiempo',
                'latitude':-32.123456,'longitude':-68.123456,'location':'Calle, Municipio, Ciudad, Provincia, Pais',
                'course':359,'speed':45.3,'odometer':95327.1,'ignition':1,'ignitionDate':'2013-06-29T14:31:02Z'}";
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SitrackReceptionModel>(jsonString);
        }

        private SitrackReceptionModel[] GetManySitrackPositionJson()
        {
            var jsonString = @"[{'id':'3708232932','reportDate':'2013-06-29T15:01:02Z','device_id':21501,'holder_id':51258,
                'holder_domain':'AAA111','holder_name':'C11','event_id':2,'event_desc':'Reporte por tiempo',
                'latitude':-32.123456,'longitude':-68.123456,'location':'Calle, Municipio, Ciudad, Provincia, Pais',
                'course':359,'speed':45.3,'odometer':95327.1,'ignition':1,'ignitionDate':'2013-06-29T14:31:02Z'},
                {'id':'3708232933','reportDate':'2013-06-29T15:01:12Z','device_id':41502,'holder_id':51259,
                'holder_domain':'BBB222','holder_name':'C12','event_id':164,'event_desc':'Contacto OFF',
                'latitude':-32.123456,'longitude':-68.123456,'location':'Calle, Municipio, Ciudad, Provincia, Pais',
                'course':241,'speed':0.0,'odometer':145327.8,'ignition':0,'ignitionDate':'2013-06-29T14:51:02Z'},
                {'id':'3708233502','reportDate':'2013-06-29T15:01:35Z','device_id':41508,'holder_id':51260,
                'holder_domain':'CCC333','holder_name':'C13','event_id':2,'event_desc':'Reporte por tiempo',
                'latitude':-32.123456,'longitude':-68.123456,'location':'Calle, Municipio, Ciudad, Provincia, Pais',
                'course':125,'speed':117.4,'odometer':55981.8,'ignition':1,'ignitionDate':'2013-06-29T13:11:48Z'}]";
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SitrackReceptionModel[]>(jsonString);
        }

        private List<SitrackFrame> GetOneSitrackFrame()
        {
            var sitrackFrameList = new List<SitrackFrame>();
            sitrackFrameList.Add(new SitrackFrame
            {
                Course = 359,
                DeviceId = 21501,
                EventDesc = "Reporte por tiempo",
                EventId = 2,
                HolderDomain = "AAA111",
                HolderId = 51258,
                HolderName = "C11",
                Id = 3708232932,
                InputDate = DateTime.Now,
                Latitude = -32.123456,
                Location = "Calle, Municipio, Ciudad, Provincia, Pais",
                Longitude = -68.123456,
                ReportDate = DateTime.Parse("2013-06-29 T15:01:02Z"),
                Speed = 45.3,
                Validity = 10
            });
            return sitrackFrameList;
        }
    }
}
