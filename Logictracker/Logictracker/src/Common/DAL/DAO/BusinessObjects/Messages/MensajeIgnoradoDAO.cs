using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class MensajeIgnoradoDAO : GenericDAO<MensajeIgnorado>
    {
        public IEnumerable<string> GetCodigosByDispositivo(int idDispositivo)
        {
            return Query.Where(m => m.Dispositivo != null
                                 && m.Dispositivo.Id == idDispositivo)
                        .Select(m => m.Codigo);
        }
    }
}