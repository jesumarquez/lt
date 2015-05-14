#region Usings

using System.Web.UI.WebControls;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.Documentos.Interfaces;

#endregion

namespace Logictracker.Web.Documentos.Mobile
{
    public class MobilePresenter : GenericPresenter
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tipoDoc">El tipo de documento a procesar</param>
        /// <param name="view">La vista donde se va a presentar el documento</param>
        /// <param name="daof">DAOFactory</param>
        public MobilePresenter(TipoDocumento tipoDoc, IDocumentView view, DAOFactory daof) : base(tipoDoc,view,daof)
        {
            parser = new MobileParser(TipoDocumentoHelper);
        } 
        
        #endregion


        protected override void AddDate(string id, string style)
        {
            var date = new TextBox { ID = id, Width = Unit.Pixel(80) };
            date.Style.Value = style;

            AddControlToView(date);
        }
    }
}
