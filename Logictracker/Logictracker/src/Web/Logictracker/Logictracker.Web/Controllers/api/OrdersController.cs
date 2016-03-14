using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Logictracker.Tracker.Application.Services;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Web.Models;
using Logictracker.DAL.Factories;
using Logictracker.Culture;
using Kendo.Mvc.UI;
using System.Web.Http.ModelBinding;
using Logictracker.DAL.DAO.BusinessObjects.Ordenes;
using Kendo.Mvc.Extensions;

namespace Logictracker.Web.Controllers.api
{
    public class OrdenesController : EntityController<Order, OrderDAO, OrderModel, OrdenesMapper>
    {
        IRoutingService RoutingService { get; set; }
        readonly OrdenDetallesMapper ordenDetalleMapper;

        public OrdenesController()
        {
            RoutingService = new RoutingService();
            ordenDetalleMapper = new OrdenDetallesMapper();
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
                if (order.PuntoEntrega != null)
                {
                    orderModel.PuntoEntrega = order.PuntoEntrega.Descripcion;
                    orderModel.IdPuntoEntrega = order.PuntoEntrega.Id;
                    orderModel.PuntoEntregaLatitud = order.PuntoEntrega.ReferenciaGeografica.Latitude;
                    orderModel.PuntoEntregaLongitud = order.PuntoEntrega.ReferenciaGeografica.Longitude;
                }
                if (order.Transportista != null) orderModel.Transportista = order.Transportista.Descripcion;
                if (order.Transportista != null) orderModel.IdTransportista = order.Transportista.Id;
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
                var orderDetailModel = new OrderDetailModel
                {
                    Id = orderDetail.Id,
                    OrderId = orderId,
                    PrecioUnitario = orderDetail.PrecioUnitario,
                    Cantidad = orderDetail.Cantidad,
                    Descuento = orderDetail.Descuento,
                    Ajuste = orderDetail.Ajuste,
                    ChocheId = 0,
                    Cuaderna = -1,
                };

                if (orderDetail.Insumo != null) orderDetailModel.Insumo = orderDetail.Insumo.Descripcion;
           
                orderDetailList.Add(orderDetailModel);
            }
            return Ok(orderDetailList);
        }

        // POST: api/Orders/91/Orders
        [Route("api/distrito/{distritoId}/base/{baseId}/ordenes")]
        [HttpPost]
        public IHttpActionResult Post(int distritoId, int baseId, [FromBody] OrderSelectionModel orderSelectionModel)
        {
            // Hay que definir como sería el routeCode ahora que también hay TipoCoche, por ahora solo se la pasa
            var routeCode = BuildRouteCode(orderSelectionModel.StartDateTime,
                orderSelectionModel.IdVehicle,
                orderSelectionModel.IdVehicleType,
                orderSelectionModel.LogisticsCycleType,
                distritoId,
                baseId);

            // Debería borrarse
            //foreach (var orderModel in orderSelectionModel.OrderList)
            //{
            //    var order = new Order();
            //    order.CodigoPedido = orderModel.CodigoPedido;
            //    order.Empleado = new Empleado { Id = orderModel.IdEmpleado };
            //    order.Empresa = new Empresa { Id = orderModel.IdEmpresa };
            //    order.Linea = new Linea { Id = orderModel.BaseId };
            //    order.Transportista = new Transportista { Id = orderModel.IdTransportista };
            //    order.FechaAlta = orderModel.FechaAlta;
            //    order.FechaEntrega = orderModel.FechaEntrega;
            //    order.FechaPedido = orderModel.FechaPedido;
            //    order.FinVentana = orderModel.FinVentana;
            //    order.InicioVentana = orderModel.InicioVentana;
            //    order.Id = orderModel.Id;
            //    order.PuntoEntrega = new PuntoEntrega { Id = orderModel.IdPuntoEntrega };
            //    order.Transportista = new Transportista { Id = orderModel.IdTransportista };

            //    RoutingService.Programming(order, routeCode, orderSelectionModel.IdVehicle,
            //        orderSelectionModel.StartDateTime, orderSelectionModel.LogisticsCycleType);
            //}

            Order o = null;
            foreach (var orderDetailModel in orderSelectionModel.OrderDetailList)
            {
                o = EntityDao.FindById(orderDetailModel.OrderId);
                if (o == null) return InternalServerError();

                // Se asigna el ajuste y la cuaderna asignada
                var orderDetail = o.OrderDetails.Single(od => od.Id == orderDetailModel.Id);
                orderDetail.Ajuste = orderDetailModel.Ajuste;
                orderDetail.Cuaderna = orderDetailModel.Cuaderna;
                orderDetail.Estado = OrderDetail.Estados.Ruteado;

                RoutingService.Programming(o, routeCode, orderSelectionModel.IdVehicle,
                    orderSelectionModel.StartDateTime, orderSelectionModel.LogisticsCycleType, orderSelectionModel.IdVehicleType);
            }

            return Ok();
        }

        [Route("api/ordenes/datasource")]
        public DataSourceResult GetDataSource(
               [ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            IQueryable<Order> ordenes = EntityDao.FindAll();
            ordenes = ordenes.Where(o => !o.Programado);            

            return ordenes.ToDataSourceResult(request, e => Mapper.EntityToModel(e, new OrderModel()));
        }

        [Route("api/ordenes/{id}")]
        public IHttpActionResult Get(int id, [FromUri] int[] insumos)
        {
            Order orden = EntityDao.FindById(id);
            var details = orden.OrderDetails;
            if (insumos.Length > 0)
                details = details.Where(od => insumos.Contains(od.Insumo.Id)).ToList();

            return Json(details.Select(od => ordenDetalleMapper.EntityToModel(od, new OrderDetailModel())));
        }

        private string BuildRouteCode(DateTime date, int vehicleId, int vechicleTypeId, int logisticCycleTypeId, int distritoId, int baseId)
        {
            var daoF = new DAOFactory();

            var patente = vehicleId >= 0
                ? daoF.CocheDAO.FindById(vehicleId).Patente
                : CultureManager.GetControl("DDL_NONE");

            var tipoCoche = vechicleTypeId >= 0
                ? daoF.TipoCocheDAO.FindById(vechicleTypeId).Codigo
                : CultureManager.GetControl("DDL_NONE");

            var tipoCiclo = logisticCycleTypeId >= 0
                ? daoF.TipoCicloLogisticoDAO.FindById(logisticCycleTypeId).Codigo
                : CultureManager.GetControl("DDL_NONE");

            //YYYYMMDD_DOMINIO_TIPO_CICLO_{N} 
            var routeCode = string.Format("{0}_{1}_{2}",
                    date.ToString("yyyyMMdd"),
                    patente,
                    tipoCiclo);

            var cantViajes = daoF.ViajeDistribucionDAO.FindByCodeLike(distritoId, baseId, routeCode).Count();

            if (cantViajes > 0)
            {
                routeCode += "_" + cantViajes;
            }

            return routeCode;
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
