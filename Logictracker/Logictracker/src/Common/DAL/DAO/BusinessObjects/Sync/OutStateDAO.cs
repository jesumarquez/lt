using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Sync;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.Sync
{
    public class OutStateDAO : GenericDAO<OutState>
    {
//        public OutStateDAO(ISession session) : base(session) { }

        public OutState GetItemState(int outqueue, string server)
        {
            return Session.QueryOver<OutState>()
                .Where(s => s.OutQueue.Id == outqueue)
                .And(s => s.Server == server)
                .Take(1)
                .List<OutState>()
                .FirstOrDefault();
        }
    }
}