﻿using System;
using System.Collections.Generic;
using Logictracker.DAL.Factories;
using Logictracker.Routing.Client;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Types.BusinessObjects.Tickets;

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

        public IList<Order> GetOrders(int empresaId)
        {
            var company = DaoFactory.EmpresaDAO.FindById(empresaId);
            return DaoFactory.OrderDAO.FindByCustomer(company);
        }

        public IList<OrderDetail> GetOrderDetails(int orderId)
        {
            var order = DaoFactory.OrderDAO.FindById(orderId);
            return order.OrderDetails;
        }

        public void Programming(string codigoPedido, int idEmpleado, int idEmpresa, DateTime horaInicio, DateTime? fechaEntrega, DateTime fechaPedido, string finVentana, string inicioVentana, int id, int idPuntoEntrega, int idTransportista, string routeCode, int idVehicle, DateTime startDateTime, int logisticsCycleType)
        {

            var viaje = DaoFactory.ViajeDistribucionDAO.FindByCodigo(idEmpresa, -1, routeCode);
            if (viaje == null)
            {
                viaje = new ViajeDistribucion();
                viaje.Empresa = DaoFactory.EmpresaDAO.FindById(idEmpresa);
                viaje.Estado = 0;
                viaje.Tipo = 0;
                //viaje.Linea = DaoFactory.LineaDAO.FindById(idEmpresa,);
                //viaje.CentroDeCostos = DaoFactory.CentroDeCostosDAO.FindById();
                viaje.Vehiculo = null;//DaoFactory.CocheDAO.FindById(idVehicle);
                viaje.TipoCicloLogistico = DaoFactory.TipoCicloLogisticoDAO.FindById(logisticsCycleType);
                viaje.Empleado = null;//DaoFactory.EmpleadoDAO.FindById(idEmpleado);
                viaje.Codigo = routeCode;
                viaje.Inicio = startDateTime;
                viaje.Fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18,0,0);
            }

            //transportista?
            var entrega = new EntregaDistribucion();
            entrega.Viaje = viaje;
            entrega.Estado = 0;
            //entrega.Id =
            entrega.Descripcion = codigoPedido; 
            entrega.PuntoEntrega = DaoFactory.PuntoEntregaDAO.FindById(idPuntoEntrega);
            entrega.Programado = horaInicio;
            if (fechaEntrega != null) 
                entrega.ProgramadoHasta = (DateTime) fechaEntrega;
            else
            {
                entrega.ProgramadoHasta = DateTime.Now.AddDays(1);                
            }
            //entrega.FechaMin = fechaEntrega;
            viaje.Detalles.Add(entrega);

            DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);

            //var order = DaoFactory.OrderDAO.FindById(orderId);
        }

        public void Programming(Order order, string routeCode, int idVehicle, DateTime startDateTime, int logisticsCycleType)
        {
            var viaje = DaoFactory.ViajeDistribucionDAO.FindByCodigo(order.Empresa.Id, -1, routeCode);
            if (viaje == null)
            {
                viaje = new ViajeDistribucion();
                viaje.Empresa = DaoFactory.EmpresaDAO.FindById(order.Empresa.Id);
                viaje.Estado = 0;
                viaje.Tipo = 0;
                //viaje.Linea = DaoFactory.LineaDAO.FindById(idEmpresa,);
                //viaje.CentroDeCostos = DaoFactory.CentroDeCostosDAO.FindById();
                viaje.Vehiculo = null;//DaoFactory.CocheDAO.FindById(idVehicle);
                viaje.TipoCicloLogistico = DaoFactory.TipoCicloLogisticoDAO.FindById(logisticsCycleType);
                viaje.Empleado = null;//DaoFactory.EmpleadoDAO.FindById(idEmpleado);
                viaje.Codigo = routeCode;
                viaje.Inicio = startDateTime;
                viaje.Fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 0, 0);
            }

            //transportista?
            var entrega = new EntregaDistribucion();
            entrega.Viaje = viaje;
            entrega.Estado = 0;
            //entrega.Id =
            entrega.Descripcion = order.CodigoPedido;
            entrega.PuntoEntrega = DaoFactory.PuntoEntregaDAO.FindById(order.PuntoEntrega.Id);
            entrega.Programado = startDateTime;
            if (order.FechaEntrega!= null)
                entrega.ProgramadoHasta = (DateTime)order.FechaEntrega;
            else
            {
                entrega.ProgramadoHasta = DateTime.Now.AddDays(1);
            }
            //entrega.FechaMin = fechaEntrega;
            viaje.Detalles.Add(entrega);

            DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);

            var order2Remove = DaoFactory.OrderDAO.FindById(order.Id);
            order2Remove.Programado = true;
            DaoFactory.OrderDAO.SaveOrUpdate(order2Remove);
        }
    }
}
