using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.ReportObjects.Datamart;
using Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico;
using Logictracker.Utils.NHibernate;
using NHibernate.Transform;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class DatamartDistribucionDAO : GenericDAO<DatamartDistribucion>
    {
        //public DatamartDistribucionDAO(ISession session) : base(session) { }

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

        public IEnumerable<DatamartDistribucion> GetRecords(int idViaje)
        {
            return Query.Where(dm => dm.Viaje.Id == idViaje);
        }

        public IEnumerable<DatamartDistribucion> GetRecords(IEnumerable<int> idsViajes)
        {
            return Query.Where(dm => idsViajes.Contains(dm.Viaje.Id));
        }

        public List<ReporteDistribucionVo> GetReporteDistribucion(int empresa, int linea, IEnumerable<int> vehiculos, int puntoEntrega, IEnumerable<int> estados, DateTime desde, DateTime hasta)
        {
            var vehiculosIds = Ids2DataTable(vehiculos);
            var estadosIds = Ids2DataTable(estados);
            
            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_DatamartDistribucionDAO_GetReporteDistribucion] :empresaId, :lineaId, :vehiculosIds, :puntoEntregaId, :estadosIds, :desde, :hasta;")
                              .SetInt32("empresaId", empresa)
                              .SetInt32("lineaId", linea)
                              .SetStructured("vehiculosIds", vehiculosIds)
                              .SetInt32("puntoEntregaId", puntoEntrega)
                              .SetStructured("estadosIds", estadosIds)
                              .SetDateTime("desde", desde)
                              .SetDateTime("hasta", hasta);

            sqlQ.SetResultTransformer(Transformers.AliasToBean(typeof(ReporteDistribucionVo)));
            var results = sqlQ.List<ReporteDistribucionVo>();
            return results.ToList();
        }
    }
}