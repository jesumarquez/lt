#region Usings

using System;
using Logictracker.Types.BusinessObjects.BaseObjects;

#endregion

namespace Logictracker.Types.BusinessObjects.Messages
{
    /// <summary>
    /// Represents the las rfid event recieved for the vehicle.
    /// </summary>
    [Serializable]
    public class LogUltimoLogin : LogMensajeBase
    {
        #region Constructors

        public LogUltimoLogin() {}

        public LogUltimoLogin(LogMensajeBase mensaje)
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
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines if the givenn objects equals the current logmensaje.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var castObj = obj as LogUltimoLogin;

            return (castObj != null) && (Id == castObj.Id) && (Id != 0);
        }

        /// <summary>
        /// Gets the hash code for the current logmensaje.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 27 * 57 * Id.GetHashCode(); }

        #endregion
    }
}
