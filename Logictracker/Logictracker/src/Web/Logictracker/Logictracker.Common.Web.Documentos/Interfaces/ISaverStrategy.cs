#region Usings

using Logictracker.Types.BusinessObjects.Documentos;

#endregion

namespace Logictracker.Web.Documentos.Interfaces
{
    public interface ISaverStrategy
    {
        void Save(Documento doc);
    }
}