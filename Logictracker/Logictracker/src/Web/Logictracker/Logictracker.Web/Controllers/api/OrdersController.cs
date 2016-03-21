using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BusinessObjects.Ordenes;
using Logictracker.DAL.Factories;
using Logictracker.Tracker.Application.Services;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Web.Models;

namespace Logictracker.Web.Controllers.api
{
    public class OrdenesController : EntityController<Order, OrderDAO, OrderModel, OrdenesMapper>
    {
        IRoutingService RoutingService { get; set; }
        private OrdenesDetailMapper MapperDetail {get; set; }

        public OrdenesController()
        {
            RoutingService = new RoutingService();
            MapperDetail = new OrdenesDetailMapper();
        }

        //GET: api/distrito/91/Orders/base/321/ordenes
        [Route("api/distrito/{distritoId}/base/{baseId}/ordenes")]
        public IHttpActionResult Get(int distritoId, int baseId, [FromUri] int[] transportistaId)
        {
            var ordersList = RoutingService.GetOrders(distritoId, baseId, transportistaId)
                .Select(e => Mapper.EntityToModel(e, new OrderModel())).ToList();

            return Ok(ordersList);
        }

        //GET: api/Ordenes/91/Orders/1
        [Route("api/distrito/{distritoId}/base/{baseId}/ordenes/{id}")]
        public IHttpActionResult Get(int distritoId, int baseId, int transportistaId, int orderId)
        {
            var orderDetails = RoutingService.GetOrderDetails(orderId)
                .Where(o => o.Estado == OrderDetail.Estados.Pendiente)
                .Select(e => MapperDetail.EntityToModel(e, new OrderDetailModel()));

            return Ok(orderDetails.ToList());
        }

        // POST: api/Orders/91/Orders
        [Route("api/distrito/{distritoId}/base/{baseId}/ordenes")]
        [HttpPost]
        public IHttpActionResult Post(int distritoId, int baseId, [FromBody] OrderSelectionModel orderSelectionModel)
        {
            var routeCode = BuildRouteCode(orderSelectionModel.StartDateTime,
                orderSelectionModel.IdVehicle,
                orderSelectionModel.IdVehicleType,
                orderSelectionModel.LogisticsCycleType,
                distritoId,
                baseId);

            // Agrupo por OrderId
            var odByOrderId = orderSelectionModel.OrderDetailList.Where(od => od.Cuaderna != 0).GroupBy(od => od.OrderId);

            // 

            foreach (var group in odByOrderId)
            {
                var order = EntityDao.FindById(group.Key);
                group.ToList().ForEach(od =>
                {
                    // Se asigna el ajuste y la cuaderna asignada
                    var orderDetail = order.OrderDetails.Single(item => item.Id == od.Id);
                    orderDetail.Ajuste = od.Ajuste;
                    orderDetail.Cuaderna = od.Cuaderna;
                    orderDetail.Estado = OrderDetail.Estados.Ruteado;
                });

                // Programo por Orden
                RoutingService.Programming(order, routeCode, orderSelectionModel.IdVehicle,
                orderSelectionModel.StartDateTime, orderSelectionModel.LogisticsCycleType, orderSelectionModel.IdVehicleType);
            }

            return Ok();
        }

        [Route("api/ordenes/datasource")]
        public DataSourceResult GetDataSource(
               [ModelBinder(typeof(WebApiDataSourceRequestModelBinder))] DataSourceRequest request)
        {
            var ordenes = EntityDao.FindAll().Where(o => !o.Programado);
            return ordenes.ToDataSourceResult(request, e => Mapper.EntityToModel(e, new OrderModel()));
        }

        [Route("api/ordenes/{id}")]
        public IHttpActionResult Get(int id, [FromUri] int[] insumos)
        {
            var orden = EntityDao.FindById(id);

            var details = orden.OrderDetails;

            if (insumos.Any())
                details = details.Where(od => insumos.Contains(od.Insumo.Id)).ToList();

            return Json(details.Select(od => MapperDetail.EntityToModel(od, new OrderDetailModel())));
        }

        private static string BuildRouteCode(DateTime date, int vehicleId, int vechicleTypeId, int logisticCycleTypeId, int distritoId, int baseId)
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

    }
}
