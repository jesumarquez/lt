using System;
using System.Collections.Generic;
using System.Web.Http;
using Logictracker.Tracker.Application.Services;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class OrdersController : ApiController
    {
        IRoutingService RoutingService { get; set; }
        
        public OrdersController()
        {
            RoutingService = new RoutingService();
        }

        //GET: api/distrito/91/Orders/base/321/ordenes
         [Route("api/distrito/{distritoId}/base/{baseId}/ordenes")]
        public IHttpActionResult Get(int distritoId, int baseId, [FromUri] int[] transportistaId)
        {
            var orders = RoutingService.GetOrders(distritoId, baseId, transportistaId);

            var orderList = new List<OrderModel>();
            foreach (var order in orders)
            {
                var orderModel = new OrderModel();
                orderModel.CodigoPedido = order.CodigoPedido;
                if (order.Empleado != null) orderModel.Empleado = order.Empleado.Entidad.Descripcion;
                if (order.Empleado != null) orderModel.IdEmpleado = order.Empleado.Entidad.Id;
                if (order.Empresa != null) orderModel.Empresa = order.Empresa.RazonSocial;
                if (order.Empresa != null) orderModel.IdEmpresa = order.Empresa.Id;
                
                orderModel.BaseId = order.Linea.Id;
                orderModel.FechaAlta = order.FechaAlta;
                if (order.FechaEntrega != null)
                    orderModel.FechaEntrega = order.FechaEntrega.Value;
                else
                    orderModel.FechaEntrega = null;
                orderModel.FechaPedido = order.FechaPedido;
                orderModel.FinVentana = order.FinVentana;
                orderModel.InicioVentana = order.InicioVentana;
                orderModel.Id = order.Id;
                if (order.PuntoEntrega != null) orderModel.PuntoEntrega = order.PuntoEntrega.Descripcion;
                if (order.PuntoEntrega != null) orderModel.IdPuntoEntrega = order.PuntoEntrega.Id;
                if (order.Transportista != null) orderModel.Transportista = order.Transportista.Descripcion;
                if (order.Transportista != null) orderModel.IdTransportista = order.Transportista.Id;
                orderModel.Selected = false;
                orderList.Add(orderModel);
            }

            return Ok(orderList);
        }

        //GET: api/Ordenes/91/Orders/1
        [Route("api/distrito/{distritoId}/base/{baseId}/ordenes/{id}")]
        public IHttpActionResult Get(int distritoId, int baseId, int transportistaId, int orderId)
        {
            var orderDetails = RoutingService.GetOrderDetails(orderId);

            var orderDetailList = new List<OrderDetailModel>();
            foreach (var orderDetail in orderDetails)
            {
                var orderDetailModel = new OrderDetailModel();
                orderDetailModel.Id = orderDetail.Id;
                orderDetailModel.OrderId = orderId;
                if (orderDetail.Insumo != null) orderDetailModel.Insumo= orderDetail.Insumo.Descripcion;
                orderDetailModel.PrecioUnitario = orderDetail.PrecioUnitario;
                orderDetailModel.Cantidad = orderDetail.Cantidad;
                orderDetailModel.Descuento= orderDetail.Descuento;
                
                orderDetailList.Add(orderDetailModel);                           
            }
            return Ok(orderDetailList);
        }

        // POST: api/Orders/91/Orders
        [Route("api/distrito/{distritoId}/ordenes")]
        [HttpPost]
        public IHttpActionResult Post(int customerId, [FromBody] OrderSelectionModel orderSelectionModel)
        {
            foreach (var orderModel in orderSelectionModel.OrderList)
            {
                if (!orderModel.Selected) continue;

                var order = new Order();
                order.CodigoPedido = orderModel.CodigoPedido;
                order.Empleado = new Empleado {Id = orderModel.IdEmpleado};
                order.Empresa = new Empresa {Id = orderModel.IdEmpresa};
                order.Linea = new Linea {Id = orderModel.BaseId};
                order.Transportista = new Transportista {Id = orderModel.IdTransportista};
                order.FechaAlta = orderModel.FechaAlta;
                order.FechaEntrega = orderModel.FechaEntrega;
                order.FechaPedido = orderModel.FechaPedido;
                order.FinVentana = orderModel.FinVentana;
                order.InicioVentana = orderModel.InicioVentana;
                order.Id = orderModel.Id;
                order.PuntoEntrega = new PuntoEntrega { Id = orderModel.IdPuntoEntrega};
                order.Transportista = new Transportista {Id = orderModel.IdTransportista};
                    
                RoutingService.Programming(order,orderSelectionModel.RouteCode,orderSelectionModel.IdVehicle,
                    orderSelectionModel.StartDateTime, orderSelectionModel.LogisticsCycleType);
            }
            
            return Ok();
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
