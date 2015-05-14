using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;
using System.Collections.Generic;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class IconoDAO : GenericDAO<Icono>
    {
//        public IconoDAO(ISession session) : base(session) { }

        public List<Icono> GetList(int empresa, int linea)
        {
            return Query.FilterEmpresa(Session, new[] {empresa})
                .FilterLinea(Session, new[] { empresa }, new[] { linea })
                .ToList();
        }
    }
}