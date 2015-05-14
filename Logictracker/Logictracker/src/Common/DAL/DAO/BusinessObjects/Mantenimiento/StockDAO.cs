using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Mantenimiento
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class StockDAO : GenericDAO<Stock>
    {
//        public StockDAO(ISession session) : base(session) { }

        public List<Stock> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> depositos, IEnumerable<int> insumos)
        {
            return Query.FilterDeposito(Session, empresas, lineas, depositos)
                        .FilterInsumo(Session, empresas, lineas, insumos)
                        .ToList();
        }

        public Stock GetByDepositoAndInsumo(int depositoId, int insumoId)
        {
            return Query.Where(s => s.Deposito != null && s.Deposito.Id == depositoId).FirstOrDefault(s => s.Insumo != null && s.Insumo.Id == insumoId);
        }
    }
}
