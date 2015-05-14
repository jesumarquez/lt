using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class EvenDistriDAO : GenericDAO<EvenDistri>
    {
//        public EvenDistriDAO(ISession session) : base(session) { }

        public List<EvenDistri> GetByMensajes(IEnumerable<LogMensaje> mensajes)
        {
           return Query.Where(ed => mensajes.Contains(ed.LogMensaje))
                       .ToList();
        }
    }
}