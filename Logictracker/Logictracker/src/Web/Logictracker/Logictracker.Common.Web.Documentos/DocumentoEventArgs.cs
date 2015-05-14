#region Usings

using System;
using Logictracker.Types.BusinessObjects.Documentos;

#endregion

namespace Logictracker.Web.Documentos
{
    public class DocumentoParametroEventArgs : EventArgs
    {
        public DocumentoParametroEventArgs(string fieldName, string style)
            :this(fieldName, style, null)
        {
        }

        public DocumentoParametroEventArgs(string fieldName, string style, TipoDocumentoParametro parametro)
        {
            Parametro = parametro;
            FieldName = fieldName;
            Style = style;
        }

        public string FieldName { get; set; }
        public string Style { get; set; }
        public TipoDocumentoParametro Parametro { get; set; }
    }

    public class DocumentoLiteralEventArgs : EventArgs
    {
        public DocumentoLiteralEventArgs(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}