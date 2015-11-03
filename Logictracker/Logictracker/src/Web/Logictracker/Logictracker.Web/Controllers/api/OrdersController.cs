using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using Logictracker.DAL.Factories;
using Logictracker.Tracker.Application.Services;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Web.Models;
using Newtonsoft.Json.Linq;
using NHibernate.Mapping;

namespace Logictracker.Web.Controllers.api
{
    public class OrdersController : ApiController
    {
        IRoutingService RoutingService { get; set; }

        public OrdersController()
        {
            RoutingService = new RoutingService();
        }

         //GET: api/Orders
        public IHttpActionResult Get()
        {
            var orders = RoutingService.GetOrders();

            var orderList = new List<OrderModel>();
            foreach (var order in orders)
            {
                var orderModel = new OrderModel();
                orderModel.CodigoPedido = order.CodigoPedido;
                if (order.Empleado != null) orderModel.Empleado = order.Empleado.Entidad.Descripcion;
                if (order.Empleado != null) orderModel.IdEmpleado = order.Empleado.Entidad.Id;
                if (order.Empresa != null) orderModel.Empresa = order.Empresa.RazonSocial;
                if (order.Empresa != null) orderModel.IdEmpresa = order.Empresa.Id;
                orderModel.FechaAlta = order.FechaAlta;
                orderModel.FechaEntrega = order.FechaEntrega;
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

        // GET: api/Orders/5
        public IHttpActionResult Get(int id)
        {
            var orderDetails = RoutingService.GetOrderDetails(id);

            var orderDetailList = new List<OrderDetailModel>();
            foreach (var orderDetail in orderDetails)
            {
                var orderDetailModel = new OrderDetailModel();
                orderDetailModel.Id = orderDetail.Id;
                orderDetailModel.OrderId = id;
                if (orderDetail.Insumo != null) orderDetailModel.Insumo= orderDetail.Insumo.Descripcion;
                orderDetailModel.PrecioUnitario = orderDetail.PrecioUnitario;
                orderDetailModel.Cantidad = orderDetail.Cantidad;
                orderDetailModel.Descuento= orderDetail.Descuento;
                
                orderDetailList.Add(orderDetailModel);                           
            }
            return Ok(orderDetailList);
        }

        // POST: api/Orders
        public IHttpActionResult Post([FromBody] OrderSelectionModel orderSelectionModel)
        {
            foreach (var orderModel in orderSelectionModel.OrderList)
            {
                if (orderModel.Selected)
                {
                    RoutingService.Programming(orderModel.CodigoPedido, 
                        orderModel.IdEmpleado, 
                        orderModel.IdEmpresa, 
                        orderModel.FechaAlta,
                        orderModel.FechaEntrega,
                        orderModel.FechaPedido,
                        orderModel.FinVentana,
                        orderModel.InicioVentana,
                        orderModel.Id,
                        orderModel.IdPuntoEntrega,
                        orderModel.IdTransportista,
                        orderSelectionModel.RouteCode,
                        orderSelectionModel.IdVehicle,
                        orderSelectionModel.StartDateTime,
                        orderSelectionModel.LogisticsCycleType);
                }
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
