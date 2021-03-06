using System;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects
{
    [Serializable]
    public class LineaTelefonica : IAuditable
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string NumeroLinea { get; set; }
        public virtual string Imei { get; set; }
        public virtual bool Baja { get; set; }

        private ISet<VigenciaPlanLinea> _vigencias;

        public virtual ISet<VigenciaPlanLinea> Vigencias { get { return _vigencias ?? (_vigencias = new HashSet<VigenciaPlanLinea>()); } }

        public virtual VigenciaPlanLinea GetVigencia(DateTime date)
        {
            return Vigencias.Count > 0 ? Vigencias.Cast<VigenciaPlanLinea>().FirstOrDefault(h => h.Inicio <= date && (h.Fin == null || h.Fin > date)) : null;
        }

        public override string ToString() { return NumeroLinea; }
    }
}