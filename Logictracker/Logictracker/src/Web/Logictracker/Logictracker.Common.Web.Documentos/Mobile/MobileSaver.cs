#region Usings

using System;
using System.Web.UI.WebControls;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Web.Documentos.Helpers;
using Logictracker.Web.Documentos.Interfaces;

#endregion

namespace Logictracker.Web.Documentos.Mobile
{
    public class MobileSaver : GenericSaver
    {
        public MobileSaver(TipoDocumento tipoDoc, IDocumentView view, DAOFactory daof) : base(tipoDoc, view, daof){ }


        #region ISaverStrategy Members

        protected override void AfterValidate(Documento doc)
        {
            doc.Descripcion = doc.Codigo;
            doc.Fecha = DateTime.UtcNow;
            doc.Linea = null;
        }

        #endregion

        #region Protected Methods

        protected override void Validate()
        {
            var txtCodigo = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_CODIGO) as TextBox;

            if (txtCodigo == null) throw new ApplicationException("No se encontro el campo Codigo");

            var txtFecha = GetControlFromView(TipoDocumentoHelper.CONTROL_NAME_FECHA) as TextBox;
            var date = GetRequiredValidDate("Fecha", txtFecha.Text);
            if (date.Year < 1900) throw new ApplicationException("Ingrese una fecha válida");
        }

        #endregion
    }

}
