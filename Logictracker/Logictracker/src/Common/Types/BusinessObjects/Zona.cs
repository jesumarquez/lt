using Iesi.Collections;
using System;
using System.Linq;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class Zona : IAuditable, ISecurable, IHasTipoZona
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        #region ISecurable

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        #endregion

        public virtual TipoZona TipoZona { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Codigo { get; set; }
        public virtual int Prioridad { get; set; }
        public virtual bool Baja { get; set; }

        private ISet<ReferenciaGeografica> _referencias;
        public virtual ISet<ReferenciaGeografica> Referencias { get { return _referencias ?? (_referencias = new HashSet<ReferenciaGeografica>()); } }

        public virtual ReferenciaGeografica GetReferencia(int idReferencia)
        {
            return Referencias.Count > 0 ? Referencias.Cast<ReferenciaGeografica>().FirstOrDefault(r => r.Id == idReferencia) : null;
        }
    }
}