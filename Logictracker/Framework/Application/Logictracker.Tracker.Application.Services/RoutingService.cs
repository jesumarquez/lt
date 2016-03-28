using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.Routing.Client;
using Logictracker.Services.Helpers;
using Logictracker.Tracker.Services;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
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
            var order = new Order
            {
                Empresa = DaoFactory.EmpresaDAO.FindById(57),
                CodigoPedido = "DUMY01",
                FechaAlta = DateTime.Now,
                FechaEntrega = DateTime.Now.AddDays(1),
                FechaPedido = DateTime.Now.AddDays(-1),
                InicioVentana = "8",
                FinVentana = "12"
            };

            var orderDetail = new OrderDetail
            {
                Cantidad = 20,
                Descuento = 10,
                Insumo = DaoFactory.InsumoDAO.FindById(1),
                PrecioUnitario = 5,
                Order = order
            };

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

        private ViajeDistribucion GetViaje(Order order, string routeCode, int idVehicle, DateTime startDateTime, int cycleType, int idVehicleType)
        {

            var viaje = DaoFactory.ViajeDistribucionDAO.FindByCodigo(order.Empresa.Id, -1, routeCode);

            if (viaje != null) return viaje;
            
            viaje = new ViajeDistribucion
            {
                Empresa = DaoFactory.EmpresaDAO.FindById(order.Empresa.Id),
                Estado = ViajeDistribucion.Estados.Pendiente,
                Tipo = ViajeDistribucion.Tipos.Desordenado,
                Linea = order.Linea,
                Vehiculo = (idVehicle != -2) ? DaoFactory.CocheDAO.FindById(idVehicle) : null,
                TipoCicloLogistico = (cycleType != -2) ? DaoFactory.TipoCicloLogisticoDAO.FindById(cycleType) : null,
                TipoCoche = DaoFactory.TipoCocheDAO.FindById(idVehicleType),
                Empleado = null,
                Codigo = routeCode,
                Inicio = startDateTime,
                RegresoABase = true,
                Transportista =
                    (order.Transportista.Id != 0)
                        ? DaoFactory.TransportistaDAO.FindById(order.Transportista.Id)
                        : null
            };

            //base al inicio
            var entregaBase = new EntregaDistribucion
            {
                Linea = viaje.Linea,
                Descripcion = viaje.Linea.Descripcion,
                Estado = EntregaDistribucion.Estados.Pendiente,
                Programado = startDateTime,
                ProgramadoHasta = startDateTime,
                Orden = viaje.Detalles.Count,
                Viaje = viaje,
                KmCalculado = 0
            };

            viaje.Detalles.Add(entregaBase);

            return viaje;
        }

        public void Programming(Order order, string routeCode, int idVehicle, DateTime startDateTime, int cycleType, int idVehicleType)
        {
            var viaje = GetViaje(order,routeCode,idVehicle,startDateTime,cycleType,idVehicleType);

            var entrega = new EntregaDistribucion
            {
                Viaje = viaje,
                Estado = EntregaDistribucion.Estados.Pendiente,
                Orden = viaje.Detalles.Count,
                Descripcion = order.CodigoPedido,
                PuntoEntrega = DaoFactory.PuntoEntregaDAO.FindById(order.PuntoEntrega.Id)
            };

            var ultimo = viaje.Detalles.Count == 1 ? viaje.Detalles.Last().ReferenciaGeografica : viaje.Detalles[viaje.Detalles.Count - 2].ReferenciaGeografica;
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

            viaje.Fin = viaje.Detalles.Last().ProgramadoHasta;

            DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);

            foreach (var punto in viaje.Detalles.Where(d => d.PuntoEntrega != null).Select(d => d.PuntoEntrega))
            {
                punto.ReferenciaGeografica.Vigencia.Fin = viaje.Fin.AddMinutes(viaje.Empresa.EndMarginMinutes);
                DaoFactory.PuntoEntregaDAO.SaveOrUpdate(punto);
            }

            var dict = new Dictionary<int, List<int>>();
            var todaslaslineas = DaoFactory.LineaDAO.GetList(new[] { viaje.Empresa.Id }).Select(l => l.Id).ToList();
            todaslaslineas.Add(-1);
            dict.Add(viaje.Empresa.Id, todaslaslineas);
            DaoFactory.ReferenciaGeograficaDAO.UpdateGeocercas(dict);

            // Si todos los OrdetDetails están ruteados entonces setea la orden en programado
            order.Programado = order.OrderDetails.All(od => od.Estado != OrderDetail.Estados.Pendiente);

            DaoFactory.OrderDAO.SaveOrUpdate(order);
        }

    }
}
