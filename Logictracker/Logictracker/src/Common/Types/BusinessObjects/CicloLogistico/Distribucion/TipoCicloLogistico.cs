using System;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class TipoCicloLogistico : IAuditable, IHasEmpresa
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        private ISet<EstadoLogistico> _estados;

        public virtual Empresa Empresa { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }        
        public virtual bool Baja { get; set; }
        public virtual bool Default { get; set; }
        public virtual ISet<EstadoLogistico> Estados { get { return _estados ?? (_estados = new HashSet<EstadoLogistico>()); } }
    }
}