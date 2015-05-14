#region Usings

using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;

#endregion

namespace Logictracker.Web.Documentos.Interfaces
{
    public interface ISaveStrategy
    {
        void Save(Documento doc, int idUsuario, DAOFactory daoF);
    }
}