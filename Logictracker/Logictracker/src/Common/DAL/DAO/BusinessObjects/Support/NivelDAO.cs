using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Support;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Support
{
    public class NivelDAO: GenericDAO<Nivel>
    {
//        public NivelDAO(ISession session) : base(session) { }

        public List<Nivel> GetList(IEnumerable<int> empresas)
        {
            return Query.FilterEmpresa(Session, empresas)
                        .Where(n => !n.Baja)
                        .ToList();
        }
    }
}
