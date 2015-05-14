using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Types.ReportObjects.Datamart;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class DatamartViajeDAO : GenericDAO<DatamartViaje>
    {
//        public DatamartViajeDAO(ISession session) : base(session) { }

        public void DeleteRecords(ViajeDistribucion distribucion)
        {
            var registros = GetRecords(distribucion);
            foreach (var registro in registros) Delete(registro);
        }

        public List<DatamartViaje> GetRecords(ViajeDistribucion viaje)
        {
            return Query.FilterEmpresa(Session, new[] { viaje.Empresa.Id })
                        .FilterLinea(Session, new[] { viaje.Empresa.Id }, new[] { viaje.Linea.Id })
                        .FilterVehiculo(Session, new[] { viaje.Empresa.Id }, new[] { viaje.Linea.Id }, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { viaje.Vehiculo != null ? viaje.Vehiculo.Id : -1 })
                        .FilterViajeDistribucion(Session, new[] { viaje.Empresa.Id }, new[] { viaje.Linea.Id }, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { -1 }, new[] { viaje.Vehiculo != null ? viaje.Vehiculo.Id : -1 }, new[] { viaje.Id })
                        .ToList();
        }

        public List<DatamartViaje> GetList(int[] idsViajes)
        {
            return Query.Where(dm => idsViajes.Contains(dm.Viaje.Id))
                        .ToList();
        }
    }
}