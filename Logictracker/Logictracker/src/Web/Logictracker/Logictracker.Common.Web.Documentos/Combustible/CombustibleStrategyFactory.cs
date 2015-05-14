#region Usings

using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.Documentos.Interfaces;

#endregion

namespace Logictracker.Web.Documentos.Combustible
{
    public class CombustibleStrategyFactory: IStrategyFactory
    {
        #region Implementation of IStrategyFactory

        public IPresentStrategy GetPresentStrategy(TipoDocumento tipoDocumento, IDocumentView view, DAOFactory daof)
        {
            return new CombustiblePresenter(tipoDocumento, view, daof);
        }

        public ISaverStrategy GetSaverStrategy(TipoDocumento tipoDocumento, IDocumentView view, DAOFactory daof)
        {
            return new CombustibleSaver(tipoDocumento, view, daof);
        }

        #endregion
    }
}
