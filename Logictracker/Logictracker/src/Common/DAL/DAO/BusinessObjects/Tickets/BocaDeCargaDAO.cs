using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Tickets;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Tickets
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class BocaDeCargaDAO: GenericDAO<BocaDeCarga>
    {
//        public BocaDeCargaDAO(ISession session) : base(session) { }

        #region Get Methods

        public BocaDeCarga GetByCode(int linea, string codigo)
        {
            return Query.FilterLinea(Session, new[]{-1}, new[]{linea})
                .Where(b => !b.Baja)
                .Where(b => b.Codigo == codigo)
                .Cacheable()
                .FirstOrDefault();
        }

        public BocaDeCarga GetByDescripcion(int linea, string descripcion)
        {
            return Query.FilterLinea(Session, new[] { -1 }, new[] { linea })
                        .Where(b => !b.Baja)
                        .Where(b => b.Descripcion == descripcion)
                        .Cacheable()
                        .FirstOrDefault();
        }

        public List<BocaDeCarga> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterLinea(Session, empresas, lineas)
                        .Where(b => !b.Baja)
                        .ToList();
        } 

        #endregion

        #region Override Methods

        public override void Delete(BocaDeCarga obj)
        {
            obj.Baja = true;
            SaveOrUpdate(obj);
        } 

        #endregion
    }
}
