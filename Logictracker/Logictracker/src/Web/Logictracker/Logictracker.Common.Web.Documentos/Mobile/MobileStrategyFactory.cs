#region Usings

using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.Documentos.Interfaces;

#endregion

namespace Logictracker.Web.Documentos.Mobile
{
    public class MobileStrategyFactory: IStrategyFactory
    {
        #region Implementation of IStrategyFactory

        public IPresentStrategy GetPresentStrategy(TipoDocumento tipoDocumento, IDocumentView view, DAOFactory daof)
        {
            return new MobilePresenter(tipoDocumento, view, daof);
        }

        public ISaverStrategy GetSaverStrategy(TipoDocumento tipoDocumento, IDocumentView view, DAOFactory daof)
        {
            return new MobileSaver(tipoDocumento, view, daof);
        }

        #endregion
    }
}
