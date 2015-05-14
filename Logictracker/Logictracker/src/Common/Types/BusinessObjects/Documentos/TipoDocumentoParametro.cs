#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Documentos
{
    [Serializable]
    public class TipoDocumentoParametro : IAuditable
    {
        public TipoDocumentoParametro()
        {
            Repeticion = 1;
        }

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        public virtual string Nombre { get; set; }

        public virtual string TipoDato { get; set; }

        public virtual short Largo { get; set; }

        public virtual short Precision { get; set; }

        public virtual bool Obligatorio { get; set; }

        public virtual string Default { get; set; }

        public virtual TipoDocumento TipoDocumento { get; set; }

        public virtual double Orden { get; set; }

        public virtual short Repeticion { get; set; }
    }
}
