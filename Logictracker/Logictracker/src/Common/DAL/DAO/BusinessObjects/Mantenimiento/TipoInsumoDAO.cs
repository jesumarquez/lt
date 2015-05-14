using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Mantenimiento;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Mantenimiento
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class TipoInsumoDAO : GenericDAO<TipoInsumo>
    {
//        public TipoInsumoDAO(ISession session) : base(session) { }

        #region Find Methods
        public TipoInsumo FindByCode(IEnumerable<int> empresas, IEnumerable<int> lineas, string code)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                .FilterLinea(Session, empresas, lineas, null)
                .Where(c => c.Codigo == code)
                .Cacheable()
                .FirstOrDefault();
        }
        #endregion

        #region Get Methods

        public List<TipoInsumo> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .ToList();
        } 

        #endregion

        #region Other Methods

        public bool IsCodeUnique(int empresa, int linea, int idTipoInsumo, string code)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                       .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                       .Where(m => m.Id != idTipoInsumo).FirstOrDefault(m => m.Codigo == code) == null;
        }

        #endregion
    }
}
