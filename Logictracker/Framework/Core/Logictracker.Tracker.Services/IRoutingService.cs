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
        IList<Order> GetOrders(int distritoId, int baseId, int[] transportistaId);
        IList<OrderDetail> GetOrderDetails(int orderId);
        void Programming(Order order, string routeCode, int idVehicle, DateTime startDateTime, int cycleType);
    }
}
