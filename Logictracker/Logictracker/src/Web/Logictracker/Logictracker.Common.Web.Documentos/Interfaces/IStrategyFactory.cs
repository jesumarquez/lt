#region Usings

using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;

#endregion

namespace Logictracker.Web.Documentos.Interfaces
{
    public interface IStrategyFactory
    {
        IPresentStrategy GetPresentStrategy(TipoDocumento tipoDocumento, IDocumentView view, DAOFactory daof);
        ISaverStrategy GetSaverStrategy(TipoDocumento tipoDocumento, IDocumentView view, DAOFactory daof);
    }
}
