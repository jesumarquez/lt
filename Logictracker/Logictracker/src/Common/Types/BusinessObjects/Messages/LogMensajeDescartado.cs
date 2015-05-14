using System;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Utils;

namespace Logictracker.Types.BusinessObjects.Messages
{
    /// <summary>
    /// Class that represents a discarted event.
    /// </summary>
    public class LogMensajeDescartado : LogMensajeBase
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new discarted position.
        /// </summary>
        public LogMensajeDescartado() {}

        /// <summary>
        /// Instanciates a new discarted position with the givenn reason code and position values.
        /// </summary>
        public LogMensajeDescartado(LogMensaje mensaje, DiscardReason reasonCode)
        {
            Latitud = mensaje.Latitud;
            Longitud = mensaje.Longitud;
            Dispositivo = mensaje.Dispositivo;
            Horario = mensaje.Horario;
            DetalleHorario = mensaje.DetalleHorario;
            Usuario = mensaje.Usuario;
            Fecha = mensaje.Fecha;
            Texto = mensaje.Texto;
            Coche = mensaje.Coche;
            Accion = mensaje.Accion;
            Chofer = mensaje.Chofer;
            Expiracion = mensaje.Expiracion;
            Estado = mensaje.Estado;
            Mensaje = mensaje.Mensaje;
            FechaFin = mensaje.FechaFin;
            LatitudFin = mensaje.LatitudFin;
            LongitudFin = mensaje.LongitudFin;
            VelocidadPermitida = mensaje.VelocidadPermitida;
            VelocidadAlcanzada = mensaje.VelocidadAlcanzada;
            IdPuntoDeInteres = mensaje.IdPuntoDeInteres;
            MotivoDescarte = (int)reasonCode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The discart reason.
        /// </summary>
        public virtual int MotivoDescarte { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the givenn objects is equivalent to the current event.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            if (this == obj) return true;

            var castObj = obj as LogMensajeDescartado;

            return castObj != null && Id == castObj.Id && Id != 0;
        }

        /// <summary>
        /// Get the hash code associated to the position based on its id.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 27 * 57 * Id.GetHashCode(); }

        #endregion
    }
}
