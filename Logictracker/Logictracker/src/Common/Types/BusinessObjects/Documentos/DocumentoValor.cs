#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Documentos
{
    [Serializable]
    public class DocumentoValor : IAuditable
    {

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Valor { get; set; }

        public virtual Documento Documento { get; set; }

        public virtual TipoDocumentoParametro Parametro { get; set; }

        public virtual short Repeticion { get; set; }
    }
}
