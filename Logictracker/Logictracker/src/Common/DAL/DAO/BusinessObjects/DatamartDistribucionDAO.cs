using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ReportObjects.Datamart;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class DatamartDistribucionDAO : GenericDAO<DatamartDistribucion>
    {
//        public DatamartDistribucionDAO(ISession session) : base(session) { }

        public List<DatamartDistribucion> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> departamentos, IEnumerable<int> centrosDeCosto, IEnumerable<int> vehiculos, DateTime? desde, DateTime? hasta)
        {
            var q = Query.FilterVehiculo(Session, empresas, lineas, transportistas, departamentos, centrosDeCosto, new[] {-1}, vehiculos)
                         .FilterCentroDeCostos(Session, empresas, lineas, departamentos, centrosDeCosto);

            if (desde.HasValue) q = q.Where(t => t.Fecha >= desde);
            if (hasta.HasValue) q = q.Where(t => t.Fecha < hasta);

            return q.ToList();
        }

        public void DeleteRecords(ViajeDistribucion viaje)
        {
            var registros = GetRecords(viaje);
            foreach (var registro in registros) Delete(registro);
        }

        public List<DatamartDistribucion> GetRecords(ViajeDistribucion viaje)
        {
            var idCentroDeCostos = viaje.CentroDeCostos != null ? viaje.CentroDeCostos.Id : -1;
            var idVehiculo = viaje.Vehiculo != null ? viaje.Vehiculo.Id : -1;

            return Query.FilterEmpresa(Session, new[] {viaje.Empresa.Id})
                        .FilterLinea(Session, new[] {viaje.Empresa.Id}, new[] {viaje.Linea.Id})
                        .FilterVehiculo(Session, new[] { viaje.Empresa.Id }, new[] { viaje.Linea.Id }, new[] { -1 }, new[] { -1 }, new[] { idCentroDeCostos}, new[] { -1 }, new[] { idVehiculo })
                        .FilterViajeDistribucion(Session, new[] { viaje.Empresa.Id }, new[] { viaje.Linea.Id }, new[] { -1 }, new[] { -1 }, new[] { idCentroDeCostos }, new[] { -1 }, new[] { idVehiculo }, new[] { viaje.Id })
                        .ToList();
        }
    }
}