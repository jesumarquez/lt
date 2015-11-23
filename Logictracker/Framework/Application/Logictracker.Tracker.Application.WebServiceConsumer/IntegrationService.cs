using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using log4net;
using Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Spring.Messaging.Core;

namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public class IntegrationService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(IntegrationService));
        public MessageQueueTemplate TrackMessageQueueTemplate { get; set; }
        public DAOFactory DaoFactory { get; set; }

        public void NewServices(List<Novelty> novelties)
        {
            foreach (var novelty in novelties)
            {
                VerifyNoveltyState(novelty);
            }
        }

        private void VerifyNoveltyState(Novelty novelty)
        {
            var trip = DaoFactory.ViajeDistribucionDAO.FindByCodigo(novelty.NumeroServicio);
            if(trip==null)
                RegisterNovelty(novelty);
        }

        private void RegisterNovelty(Novelty novelty)
        {
            //solo esta para sos
            var empresa = DaoFactory.EmpresaDAO.FindByCodigo("SOS");
            Linea linea = null; //DaoFactory.LineaDAO.FindById(-1);
            Transportista transportista = null;

            //viaje
            var viaje = new ViajeDistribucion();
            viaje.Codigo = novelty.NumeroServicio;
            viaje.Empresa = empresa;
            viaje.Estado = 0;
            viaje.Tipo = 1;
            viaje.Linea = linea;
            //viaje.TipoCicloLogistico = DaoFactory.TipoCicloLogisticoDAO.FindById(cycleType);
            viaje.Inicio = novelty.HoraServicio;
            viaje.RegresoABase = true;
            viaje.Fin = new DateTime(novelty.HoraServicio.Year, novelty.HoraServicio.Month, novelty.HoraServicio.Day, novelty.HoraServicio.AddHours(1).Hour, novelty.HoraServicio.Minute, novelty.HoraServicio.Second);
            //viaje.CentroDeCostos = DaoFactory.CentroDeCostosDAO.FindById();            
            //viaje.Transportista = DaoFactory.TransportistaDAO.FindById(order.Transportista.Id);
            viaje.Vehiculo = null;//DaoFactory.CocheDAO.FindById(idVehicle);
            viaje.Empleado = null;//DaoFactory.EmpleadoDAO.FindById(idEmpleado);

            //base al inicio
            var entregaBase = new EntregaDistribucion
            {
                Linea = linea,
                Descripcion = linea.Descripcion,
                Estado = EntregaDistribucion.Estados.Pendiente,
                Programado = novelty.HoraServicio,
                ProgramadoHasta = new DateTime(novelty.HoraServicio.Year, novelty.HoraServicio.Month, novelty.HoraServicio.Day, novelty.HoraServicio.AddHours(1).Hour, novelty.HoraServicio.Minute, novelty.HoraServicio.Second);
                Orden = viaje.Detalles.Count,
                Viaje = viaje,
                KmCalculado = 0
            };
            viaje.Detalles.Add(entregaBase);

            //pedido

            //detalles

            DaoFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
        }
    }
}