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

        public void DeleteRecords(int idViaje)
        {
            var registros = GetRecords(idViaje);
            foreach (var registro in registros) Delete(registro);
        }

        public List<DatamartDistribucion> GetRecords(int idViaje)
        {
            return Query.Where(dm => dm.Viaje.Id == idViaje).ToList();
        }
    }
}