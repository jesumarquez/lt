using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Messages;

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    public class MensajeTraducidoDAO : GenericDAO<MensajeTraducido>
    {
        public string GetCodigoFinal(int idEmpresa, string codigoOriginal)
        {
            var codigoFinal = codigoOriginal;

            var traduccion = Query.FirstOrDefault(m => m.Empresa != null && m.Empresa.Id == idEmpresa && m.CodigoOriginal == codigoOriginal);

            if (traduccion != null) codigoFinal = traduccion.CodigoFinal;

            return codigoFinal;
        }
    }
}