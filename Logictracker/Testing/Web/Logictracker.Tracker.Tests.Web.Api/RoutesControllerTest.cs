using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using LogicTracker.App.Web.Api.Controllers;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Positions;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Moq;
using NUnit.Framework;

namespace Logictracker.Tracker.Tests.Web.Api
{
    [TestFixture]
    public class RoutesControllerTest
    {
        private Mock<IRouteService> _routeSevice;
        private RoutesController _controller;
        
        [SetUp]
        public void Setup()
        {
            _routeSevice = new Mock<IRouteService>();

            var request = new HttpRequestMessage();
            request.Headers.Add("deviceId", "353771055938848");

            _controller = new RoutesController
            {
                RouteService = _routeSevice.Object,
                Request = request,
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        public void GetRoutesTest()
        {
            _routeSevice.Setup(x => x.GetAvailableRoutes(It.IsAny<string>())).Returns(GetViajeDistribucionList());

            // Act
            IHttpActionResult actionResult = _controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<RouteList>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsInstanceOf(typeof(RouteList), contentResult.Content, "the content is not a list of routes");
        }

        [Test]
        public void SyncRouteTest()
        {
             //Arrange
            var routeState = getFakeRouteState();
            _routeSevice.Setup(x => x.ReportDelivery(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Coordinate>(), It.IsAny<int>(), It.IsAny<short>(), It.IsAny<string>(), DateTime.UtcNow)).Returns(0);

            // Act
            IHttpActionResult actionResult = _controller.Post(routeState);
            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<RouteState>;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(1, createdResult.RouteValues["id"]);
            Assert.AreEqual(1, createdResult.Content.RouteStatus);
        }

        private RouteState getFakeRouteState()
        {
            var myroutestate = new RouteState()
            {
                RouteId = 1,
                RouteCode = "UnitTestRoute",
                RouteStatus = 1
            };
            var jobstates = new List<JobState>();
            var jobstate = new JobState()
            {
                JobId = 1,
                Latitude = 3,
                Longitude = 4,
                JobStatus = 9, //confirmado
                MessageId = 2010
            };
            jobstates.Add(jobstate);
            jobstate = new JobState()
            {
                JobId = 2,
                Latitude = 5,
                Longitude = 6,
                JobStatus = 3, //pendiente
                MessageId = 210
            };
            jobstates.Add(jobstate);
            jobstate = new JobState()
            {
                JobId = 3,
                Latitude = 7,
                Longitude = 8,
                JobStatus = -1, //rechazado
                MessageId = 211
            };
            jobstates.Add(jobstate);

            myroutestate.JobStates = jobstates.ToArray();
            return myroutestate;
        }

        private static IList<ViajeDistribucion> GetViajeDistribucionList()
        {
            var viajeDistribucionList = new List<ViajeDistribucion>();
            var distribucion = new ViajeDistribucion()
            {
                Id = 1,
                Codigo = "UNITTEST1",
                Estado = 0,
                Empresa = new Empresa() { Id = 1 },
                Linea = new Linea() { Id = 10 },
                Detalles = new List<EntregaDistribucion>()
                {
                    new EntregaDistribucion()
                    {
                        Id = 1,
                        Programado = new DateTime(2014, 04, 15, 9, 5, 23),
                        ProgramadoHasta = new DateTime(2014, 04, 16, 9, 5, 23),
                        Estado = 0,
                        Descripcion = "descripcion",
                        Orden = 0,
                        Linea = new Linea()
                        {
                            ReferenciaGeografica = new ReferenciaGeografica()
                        }
                    }
                }
            };
            viajeDistribucionList.Add(distribucion);

            distribucion = new ViajeDistribucion()
            {
                Id = 2,
                Codigo = "UNITTEST2",
                Estado = 3,
                Empresa = new Empresa() { Id = 1 },
                Linea = new Linea() { Id = 10 },
                Detalles = new List<EntregaDistribucion>()
                {
                    new EntregaDistribucion()
                    {
                        Id = 1,
                        Programado = new DateTime(2014, 04, 15, 9, 5, 23),
                        ProgramadoHasta = new DateTime(2014, 04, 16, 9, 5, 23),
                        Estado = 0,
                        Descripcion = "descripcion",
                        Orden = 0,
                        Linea = new Linea()
                        {
                            ReferenciaGeografica = new ReferenciaGeografica()
                        }
                    }
                }
            };
            viajeDistribucionList.Add(distribucion);

            distribucion = new ViajeDistribucion()
            {
                Id = 1,
                Codigo = "UNITTEST3",
                Estado = 9,
                Empresa = new Empresa() { Id = 1 },
                Linea = new Linea() { Id = 10 },
                Detalles = new List<EntregaDistribucion>()
                {
                    new EntregaDistribucion()
                    {
                        Id = 1,
                        Programado = new DateTime(2014, 04, 15, 9, 5, 23),
                        ProgramadoHasta = new DateTime(2014, 04, 16, 9, 5, 23),
                        Estado = 0,
                        Descripcion = "descripcion",
                        Orden = 0,
                        Linea = new Linea()
                        {
                            ReferenciaGeografica = new ReferenciaGeografica()
                        }
                    }
                }
            };
            viajeDistribucionList.Add(distribucion);

            return viajeDistribucionList;
        }

        private static Route getFakeRoute()
        {
            return new Route
            {
                Code = "JDG654DKJHASD",
                Id = 1,
                Jobs = new Job[]
                {
                    new Job
                    {
                        Id = 1,
                        Code = "00001",
                        StartDate = "2015-02-06T09:00:00",
                        EndDate = "2015-02-06T18:00:00",
                        State = 0,
                        Name = "Nombre",
                        Description =
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                        Order = 0,
                        Location =
                            new Location
                            {
                                Code = "ASD234ASD",
                                Description = "Las Heras 3900",
                                Latitude = (float) -34.581742,
                                Longitude = (float) -58.414279
                            }
                    },
                    new Job
                    {
                        Id = 2,
                        Code = "00002",
                        StartDate = "2015-02-06T09:00:00",
                        EndDate = "2015-02-06T18:00:00",
                        State = 0,
                        Name = "Nombre",
                        Description =
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                        Order = 0,
                        Location =
                            new Location
                            {
                                Code = "ASD234ASD",
                                Description = "Lugar de entrega",
                                Latitude = (float) -34.558567,
                                Longitude = (float) -58.486098
                            }
                    },
                    new Job
                    {
                        Id = 3,
                        Code = "00003",
                        StartDate = "2015-02-06T09:00:00",
                        EndDate = "2015-02-06T18:00:00",
                        State = 0,
                        Name = "Nombre",
                        Description =
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                        Order = 0,
                        Location =
                            new Location
                            {
                                Code = "ASD234ASD",
                                Description = "Lugar de entrega",
                                Latitude = (float) -34.603604,
                                Longitude = (float) -58.381577
                            }
                    },
                    new Job
                    {
                        Id = 4,
                        Code = "00004",
                        StartDate = "2015-02-06T09:00:00",
                        EndDate = "2015-02-06T18:00:00",
                        State = 0,
                        Name = "Nombre",
                        Description =
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                        Order = 0,
                        Location =
                            new Location
                            {
                                Code = "ASD234ASD",
                                Description = "Lugar de entrega",
                                Latitude = (float) -34.558567,
                                Longitude = (float) -58.486098
                            }
                    }
                }
            };
        }
    }
}
