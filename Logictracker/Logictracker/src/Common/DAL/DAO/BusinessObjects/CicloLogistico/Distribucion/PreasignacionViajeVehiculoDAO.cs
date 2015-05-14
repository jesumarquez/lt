using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.CicloLogistico.Distribucion
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class PreasignacionViajeVehiculoDAO : GenericDAO<PreasignacionViajeVehiculo>
    {
//        public PreasignacionViajeVehiculoDAO(ISession session) : base(session) { }

        public PreasignacionViajeVehiculo FindByCodigo(int empresa, int linea, int transportista, string codigo)
        {
            return Query.FilterEmpresa(Session, new[]{empresa}, null)
                        .FilterLinea(Session, new[]{empresa}, new[]{linea}, null)
                        .FilterTransportista(Session, new[] {empresa}, new[] {linea}, new[] {transportista})
                        .FirstOrDefault(t => t.Codigo == codigo);
        }

        public List<PreasignacionViajeVehiculo> FindByCodigos(int empresa, int linea, IEnumerable<string> codigos)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                        .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                        .Where(t => codigos.Contains(t.Codigo))
                        .ToList();
        }

        public List<PreasignacionViajeVehiculo> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> transportistas, IEnumerable<int> vehiculos)
        {   
            return Query.FilterEmpresa(Session, empresas)
                        .FilterLinea(Session, empresas, lineas)
                        .FilterTransportista(Session, empresas, lineas, transportistas)
                        .FilterVehiculo(Session, empresas, lineas, transportistas, new[] { -1 }, new[] { -1 }, new[] { -1 }, vehiculos)
                        .ToList();
        }
    }
}
