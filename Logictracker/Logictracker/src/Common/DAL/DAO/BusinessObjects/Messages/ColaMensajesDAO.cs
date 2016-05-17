using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;
using System.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class ColaMensajesDAO : GenericDAO<ColaMensajes>
    {
        public ColaMensajes FindByName(string name)
        {
            return Query.Where(c => c.Nombre == name).FirstOrDefault();
        }
    }
}