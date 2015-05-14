using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Tickets;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.BaseObjects
{
    /// <summary>
    /// Base class for messages.
    /// </summary>
    [Serializable]
    public abstract class LogMensajeBase : IAuditable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual double Latitud { get; set; }
        public virtual double Longitud { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
        public virtual Ticket Horario { get; set; }
        public virtual DetalleTicket DetalleHorario { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual DateTime Fecha { get; set; }
        public virtual DateTime? FechaAlta { get; set; }
        public virtual String Texto { get; set; }
        public virtual Coche Coche { get; set; }
        public virtual Accion Accion { get; set; }
        public virtual Empleado Chofer { get; set; }
        public virtual DateTime? Expiracion { get; set; }
        public virtual byte Estado { get; set; }
        public virtual Mensaje Mensaje { get; set; }
        public virtual DateTime? FechaFin { get; set; }
        public virtual Double? LatitudFin { get; set; }
        public virtual Double? LongitudFin { get; set; }
        public virtual Int32? VelocidadPermitida { get; set; }
        public virtual Int32? VelocidadAlcanzada { get; set; }
        public virtual Int32? IdPuntoDeInteres { get; set; }
        public virtual bool TieneFoto { get; set; }

        public virtual int Duracion
        {
            get
            {
                if (FechaFin.HasValue) return (int)FechaFin.Value.Subtract(Fecha).TotalSeconds;

                return 0;
            }
        }

        public virtual int Exceso
        {
            get
            {
                if (VelocidadPermitida.HasValue && VelocidadAlcanzada.HasValue) return VelocidadAlcanzada.Value - VelocidadPermitida.Value;

                return 0;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines wither if the giveen message has a valid position.
        /// </summary>
        /// <returns></returns>
        public virtual bool HasValidLatitudes() { return Math.Abs(Latitud) > 0.0 && Math.Abs(Longitud) > 0.0; }

        /// <summary>
        /// Determines if the givenn message has duration.
        /// </summary>
        /// <returns></returns>
        public virtual bool HasDuration() { return FechaFin.HasValue; }

        /// <summary>
        /// Gets the icon associated to the message.
        /// </summary>
        /// <returns></returns>
        public virtual string GetIconUrl()
        {
            if (Accion != null && Accion.ModificaIcono) return Accion.PathIconoMensaje;

            return Mensaje.Icono != null && Mensaje.Icono.Id > 0 ? Mensaje.Icono.PathIcono : Mensaje.TipoMensaje.Icono != null && Mensaje.TipoMensaje.Icono.Id > 0
                ? Mensaje.TipoMensaje.Icono.PathIcono : null;
        }

        #endregion
    }
}
