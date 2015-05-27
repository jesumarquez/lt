using System;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.Entidades
{
    [Serializable]
    public class EntidadPadre : IAuditable, ISecurable, IHasTipoEntidad, IHasDispositivo
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual string Url { get; set; }
        public virtual bool Baja { get; set; }

        public virtual ReferenciaGeografica ReferenciaGeografica { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
        public virtual TipoEntidad TipoEntidad { get; set; }
        
        private ISet<DetalleValor> _detalles;
        public virtual ISet<DetalleValor> Detalles { get { return _detalles ?? (_detalles = new HashSet<DetalleValor>()); } }

        public virtual DetalleValor GetDetalle(int idDetalle)
        {
            return Detalles.Count > 0 ? Detalles.Cast<DetalleValor>().FirstOrDefault(d => d.Detalle.Id == idDetalle) : null;
        }

        public override string ToString() { return Descripcion; }
    }
}