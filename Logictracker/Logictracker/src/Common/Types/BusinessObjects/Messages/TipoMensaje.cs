using System;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;
using System.Collections.Generic;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class TipoMensaje : IAuditable, ISecurable
    {
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        private ISet<Mensaje> _mensajes;

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }

        public virtual string Codigo { get; set; }
        public virtual string Descripcion { get; set; }
        public virtual Icono Icono { get; set; }
        public virtual bool EsGenerico { get; set; }
        public virtual bool DeUsuario { get; set; }
        public virtual bool DeMantenimiento { get; set; }
        public virtual bool DeEstadoLogistico { get; set; }
        public virtual bool DeCombustible { get; set; }
        public virtual bool DeConfirmacion { get; set; }
        public virtual bool DeRechazo { get; set; }
        public virtual bool DeAtencion { get; set; }
        public virtual bool Baja { get; set; }

        public virtual ISet<Mensaje> Mensajes { get { return _mensajes ?? (_mensajes = new HashSet<Mensaje>()); } }

        #region Public Methods

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var tipoMensaje = obj as TipoMensaje;

            return tipoMensaje != null && Id.Equals(tipoMensaje.Id);
        }

        public override int GetHashCode() { return Id.GetHashCode(); }

        public override string ToString() { return Descripcion; }

        public virtual bool EsDeSistema() { return !DeEstadoLogistico && !DeUsuario && !DeMantenimiento; }

        public virtual void AddMensaje(Mensaje mensaje) { Mensajes.Add(mensaje); }

        public virtual void ClearMensajes() { Mensajes.Clear(); }

        #endregion
    }
}
