#region Usings

using System;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Documentos
{
    [Serializable]
    public class TipoDocumentoEstrategia : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion


        public virtual string Estrategia { get; set; }

        public virtual TipoDocumento TipoDocumento { get; set; }
    }
}
