using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class TipoZonaDAO: GenericDAO<TipoZona>
    {
//        public TipoZonaDAO(ISession session) : base(session) { }

        #region Find Methods
        public TipoZona FindByCodigo(int empresa, int linea, string codigo)
        {
            return Query.FilterEmpresa(Session, new[] { empresa }, null)
                .FilterLinea(Session, new[] { empresa }, new[] { linea }, null)
                .FirstOrDefault(x => x.Codigo == codigo);
        } 
        #endregion

        #region Get Methods
        public List<TipoZona> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .ToList();
        } 
        #endregion

        
    }
}
