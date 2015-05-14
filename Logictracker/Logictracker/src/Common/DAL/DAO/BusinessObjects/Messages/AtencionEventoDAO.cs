using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class AtencionEventoDAO : GenericDAO<AtencionEvento>
    {
//        public AtencionEventoDAO(ISession session) : base(session) { }

        public AtencionEvento GetByEvento(int idEvento)
        {
           return Query.FirstOrDefault(ae => ae.LogMensaje.Id == idEvento);
        }
    }
}