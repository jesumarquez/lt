using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogicTracker.App.Web.Api.Filters;
using LogicTracker.App.Web.Api.Models;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.Messages;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class OrdersController : ApiController
    {
        public IRouteService RouteService { get; set; }

        // GET: api/Orders
        public IHttpActionResult Get()
        {
           // var order = 
            return NotFound();
        }

        // GET: api/Orders/5
        [SecureResourceAttribute]
        public IHttpActionResult Get(int id)
        {
            var trip = RouteService.GetDistributionRouteById(id);

            if (trip == null) return NotFound();

            var order = new Order
            {
                Code = trip.Codigo,
                Id = trip.Id,
                Status = trip.Estado,
                StartDateTime = trip.Inicio,
                RealStartDateTime= trip.InicioReal,
                EndDateTime = trip.Fin
            };

            var orderJobs = new List<OrderJob>();
            foreach (var detail in trip.Detalles)
            {
                if (detail.PuntoEntrega != null)
                {
                    var orderJob = new OrderJob();

                    orderJob.Id = detail.Id;
                    orderJob.Description = detail.Descripcion;
                    orderJob.Order = detail.Orden;

                    orderJob.Location = new Location();
                    if (detail.PuntoEntrega != null)
                    {
                        orderJob.Location.Latitude = (float) detail.PuntoEntrega.ReferenciaGeografica.Latitude;
                        orderJob.Location.Longitude = (float) detail.PuntoEntrega.ReferenciaGeografica.Longitude;
                    }
                    else
                    {
                        orderJob.Location.Latitude = (float) detail.Linea.ReferenciaGeografica.Latitude;
                        orderJob.Location.Longitude = (float) detail.Linea.ReferenciaGeografica.Longitude;
                    }

                    orderJob.Programmed = detail.Programado;
                    orderJob.ProgrammedTo = detail.ProgramadoHasta;
                    orderJob.ManualDayTime = detail.Manual;
                    orderJob.ManualOrEntrance = detail.ManualOEntrada;
                    orderJob.ManualOrExit = detail.ManualOSalida;
                    orderJob.EntranceDateTime = detail.Entrada;
                    orderJob.ExitDateTime = detail.Salida;
                    orderJob.State = detail.Estado;
                    orderJob.ReceptionDateTime = detail.RecepcionConfirmacion;
                    orderJob.ReadingConfirmationDateTime = detail.LecturaConfirmacion;
                    orderJob.ConfirmationMessage = ConvertToMessage(detail.MensajeConfirmacion);
                    orderJob.EntranceOrExclusiveManual = detail.EntradaOManualExclusiva;
                    orderJob.ExitOrExclusiveManual = detail.SalidaOManualExclusiva;
                    orderJob.GarminEta = detail.GarminETA;
                    orderJob.GarminETAInformedAt = detail.GarminETAInformedAt;
                    orderJob.GarminReadInactiveAt = detail.GarminReadInactiveAt;
                    orderJob.GarminUnreadInactiveAt = detail.GarminUnreadInactiveAt;

                    orderJobs.Add(orderJob);
                }
            }

            order.OrderJobs = orderJobs.ToArray();
            return Ok(order);
        }

        private Message ConvertToMessage(LogMensaje mensajeConfirmacion)
        {
            return new Message();
        }

        // POST: api/Orders
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Orders/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Orders/5
        public void Delete(int id)
        {
        }
    }
}
