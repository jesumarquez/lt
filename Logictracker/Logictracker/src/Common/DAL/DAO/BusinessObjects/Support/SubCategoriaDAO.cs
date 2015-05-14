using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Support;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Support
{
    public class SubCategoriaDAO: GenericDAO<Subcategoria>
    {
//        public SubCategoriaDAO(ISession session) : base(session) { }
        
        public List<Subcategoria> GetList(IEnumerable<int> empresas, IEnumerable<int> categorias)
        {
            return Query.FilterCategoria(Session, empresas, categorias)
                        .Where(s => !s.Baja)
                        .ToList();
        }
    }
}
