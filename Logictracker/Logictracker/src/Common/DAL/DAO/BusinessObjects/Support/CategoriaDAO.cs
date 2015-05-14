using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Support;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Support
{
    public class CategoriaDAO: GenericDAO<Categoria>
    {
//        public CategoriaDAO(ISession session) : base(session) { }
        
        public List<Categoria> GetList(IEnumerable<int> empresas)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .Where(c => !c.Baja)
                        .ToList();
        }
    }
}
