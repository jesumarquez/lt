using System;
using Logictracker.DAL.Factories;
using Logictracker.Routing.Client;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Ordenes;

namespace Logictracker.Tracker.Application.Services
{
    public class RoutingService : IRoutingService
    {
        public DAOFactory DaoFactory { get; set; }
        public Problem Problem { get; set; }

        public RoutingService()
        {
            DaoFactory = new DAOFactory();
        }

        public void BuildProblem()
        {
            Problem = new Problem();
            Problem.AddVehicleType("vehicleType", 0, "2");
           
        }

        public void FillTable()
        {
            var order = new Order();
            order.Empresa = DaoFactory.EmpresaDAO.FindById(57);
            order.CodigoPedido = "DUMY01";
            order.FechaAlta = DateTime.Now;
            order.FechaEntrega = DateTime.Now.AddDays(1);
            order.FechaPedido = DateTime.Now.AddDays(-1);
            order.InicioVentana = "8";
            order.FinVentana = "12";

            var orderDetail = new OrderDetail();
            orderDetail.Cantidad = 20;
            orderDetail.Descuento = 10;
            orderDetail.Insumo = DaoFactory.InsumoDAO.FindById(1);
            orderDetail.PrecioUnitario = 5;
            orderDetail.Order = order;
            
            order.OrderDetails.Add(orderDetail);
            
            DaoFactory.OrderDAO.SaveOrUpdate(order);
        }
    }
}
