using System;
using System.Collections;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects.Ordenes;

namespace Logictracker.Tracker.Services
{
    public interface IRoutingService
    {
        void BuildProblem();
        void FillTable();
        IList<Order> GetOrders();
        IList<OrderDetail> GetOrderDetails(int orderId);
        void Programming(string codigoPedido, int idEmpleado, int idEmpresa, DateTime horaInicio, DateTime fechaEntrega, DateTime fechaPedido, string finVentana, string inicioVentana, int id, int idPuntoEntrega, int idTransportista, string routeCode, int idVehicle, DateTime startDateTime, int logisticsCycleType);
    }
}
