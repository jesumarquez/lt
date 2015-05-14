using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.BaseObjects
{
    [Serializable]
    public abstract class LogEventoBase : IAuditable, IHasDispositivo, IHasSensor, IHasSubEntidad, IHasMensaje
    {
        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Mensaje Mensaje { get; set; }
        public virtual Accion Accion { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
        public virtual Sensor Sensor { get; set; }
        public virtual SubEntidad SubEntidad { get; set; }

        public virtual DateTime Fecha { get; set; }
        public virtual string Texto { get; set; }
        public virtual DateTime? Expiracion { get; set; }
        public virtual byte Estado { get; set; }
        public virtual DateTime? FechaFin { get; set; }
        
        public virtual int Duracion
        {
            get { return FechaFin.HasValue ? (int)FechaFin.Value.Subtract(Fecha).TotalSeconds : 0; }
        }

        public virtual bool HasDuration() { return FechaFin.HasValue; }

        public virtual string GetIconUrl()
        {
            if (Accion != null && Accion.ModificaIcono) return Accion.PathIconoMensaje;

            return Mensaje.Icono != null && Mensaje.Icono.Id > 0 ? Mensaje.Icono.PathIcono : Mensaje.TipoMensaje.Icono != null && Mensaje.TipoMensaje.Icono.Id > 0
                ? Mensaje.TipoMensaje.Icono.PathIcono : null;
        }
    }
}
