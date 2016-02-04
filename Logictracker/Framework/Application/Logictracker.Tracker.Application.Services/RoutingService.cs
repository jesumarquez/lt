using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Routing.Client;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects.Ordenes;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Services.Helpers;

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

        public IList<Order> GetOrders(int distritoId, int baseId, int[] transportistaId)
        {
            var company = DaoFactory.EmpresaDAO.FindById(distritoId);
            var ordenes = DaoFactory.OrderDAO.FindByCustomer(company);

            if (baseId > 0)
                ordenes = ordenes.Where(o => o.Linea.Id == baseId).ToList();
            //if(transportistaId>0)
                //ordenes = ordenes.Where(o => o.Transportista.Id == transportistaId).ToList();

            return ordenes;
        }

        public IList<OrderDetail> GetOrderDetails(int orderId)
        {
            var order = DaoFactory.OrderDAO.FindById(orderId);
            return order.OrderDetails;
        }

        //public void Programming(string codigoPedido, int idEmpleado, int idEmpresa, DateTime horaInicio, DateTime? fechaEntrega, DateTime fechaPedido, string finVentana, string inicioVentana, int id, int idPuntoEntrega, int idTransportista, string routeCode, int idVehicle, DateTime startDateTime, int logisticsCycleType)
        //{

        //    var viaje = DaoFactory.ViajeDistribucionDAO.FindByCodigo(idEmpresa, -1, routeCode);
        //    if (viaje == null)
        //    {
        //        viaje = new ViajeDistribucion();
        //        viaje.Empresa = DaoFactory.EmpresaDAO.FindById(idEmpresa);
        //        viaje.Estado = 0;
        //        viaje.Tipo = 0;
        //        //viaje.Linea = DaoFactory.LineaDAO.FindById(idEmpresa,);
        //        //viaje.CentroDeCostos = DaoFactory.CentroDeCostosDAO.FindById();
        //        viaje.Vehiculo = null;//DaoFactory.CocheDAO.FindById(idVehicle);
        //        viaje.TipoCicloLogistico = DaoFactory.TipoCicloLogisticoDAO.FindById(logisticsCycleType);
        //        viaje.Empleado = null;//DaoFactory.EmpleadoDAO.FindById(idEmpleado);
        //        viaje.Codigo = routeCode;
        //        viaje.Inicio = startDateTime;
        //        viaje.Fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18,0,0);
        //    }

        //    //transportista?
        //    var entrega = new EntregaDistribucion();
        //    entrega.Viaje = viaje;
        //    entrega.Estado = 0;
        //    //entrega.Id =
        //    entrega.Descripcion = codigoPedido; 
        //    entrega.PuntoEntrega = DaoFactory.PuntoEntregaDAO.FindById(idPuntoEntrega);
        //    entrega.Programado = horaInicio;
        //    if (fechaEntrega != null) 
        //        entrega.ProgramadoHasta = (DateTime) fechaEntrega;
        //    else
        //    {
        //        entrega.ProgramadoHasta = DateTime.Now.AddDays(1);                
        //    }
        //    //entrega.FechaMin = fechaEntrega;
        //    viaje.Detalles.Add(entrega);

        //    DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);

        //    //var order = DaoFactory.OrderDAO.FindById(orderId);
        //}

        public void Programming(Order order, string routeCode, int idVehicle, DateTime startDateTime, int cycleType)
        {
            var viaje = DaoFactory.ViajeDistribucionDAO.FindByCodigo(order.Empresa.Id, -1, routeCode);
            
            Linea linea = null;
            if (order.Linea.Id > 1)
                linea = DaoFactory.LineaDAO.FindById(order.Linea.Id);

            if (viaje == null)
            {
                viaje = new ViajeDistribucion();
                viaje.Empresa = DaoFactory.EmpresaDAO.FindById(order.Empresa.Id);
                viaje.Estado = ViajeDistribucion.Estados.Pendiente;
                viaje.Tipo = ViajeDistribucion.Tipos.Desordenado;
                viaje.Linea = order.Linea;
                //viaje.CentroDeCostos = DaoFactory.CentroDeCostosDAO.FindById();
                viaje.Vehiculo = (idVehicle != -2)? DaoFactory.CocheDAO.FindById(idVehicle) : null;
                viaje.TipoCicloLogistico = (cycleType != -2) ? DaoFactory.TipoCicloLogisticoDAO.FindById(cycleType) : null;
                viaje.Empleado = null;//DaoFactory.EmpleadoDAO.FindById(idEmpleado);
                viaje.Codigo = routeCode;
                viaje.Inicio = startDateTime;
                viaje.RegresoABase = true;
                viaje.Transportista = (order.Transportista.Id != 0) ? DaoFactory.TransportistaDAO.FindById(order.Transportista.Id) : null;

                //base al inicio
                var entregaBase = new EntregaDistribucion
                {
                    Linea = linea,
                    Descripcion = linea.Descripcion,
                    Estado = EntregaDistribucion.Estados.Pendiente,
                    Programado = startDateTime,
                    ProgramadoHasta = startDateTime,
                    Orden = viaje.Detalles.Count,
                    Viaje = viaje,
                    KmCalculado = 0
                };
                viaje.Detalles.Add(entregaBase);
            }

            var entrega = new EntregaDistribucion();
            entrega.Viaje = viaje;
            entrega.Estado = EntregaDistribucion.Estados.Pendiente;
            entrega.Orden = viaje.Detalles.Count;
            entrega.Descripcion = order.CodigoPedido;
            entrega.PuntoEntrega = DaoFactory.PuntoEntregaDAO.FindById(order.PuntoEntrega.Id);

            var ultimo = viaje.Detalles.Count == 1 ? viaje.Detalles.Last().ReferenciaGeografica : viaje.Detalles[viaje.Detalles.Count-2].ReferenciaGeografica;
            var origen = new LatLon(ultimo.Latitude, ultimo.Longitude);
            var destino = new LatLon(entrega.ReferenciaGeografica.Latitude, entrega.ReferenciaGeografica.Longitude);
            var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, null);

            if (directions != null)
            {
                var distancia = directions.Distance / 1000.0;
                var duracion = directions.Duration;
                var fecha = viaje.Detalles.Last().Programado.Add(duracion);

                entrega.Programado = fecha;
                entrega.ProgramadoHasta = fecha;
                entrega.KmCalculado = distancia;
            }

            viaje.Detalles.Add(entrega);
            viaje.AgregarBaseFinal();

            viaje.Fin = viaje.Detalles.LastOrDefault().ProgramadoHasta;            

            DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);

            var order2Remove = DaoFactory.OrderDAO.FindById(order.Id);
            order2Remove.Programado = true;
                      
            DaoFactory.OrderDAO.SaveOrUpdate(order2Remove);
        }
    }
}
