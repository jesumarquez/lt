#region Usings

using System;
using Logictracker.Types.BusinessObjects.BaseObjects;

#endregion

namespace Logictracker.Types.ValueObject.Messages
{
    /// <summary>
    /// Value object class for caching logins.
    /// </summary>
    [Serializable]
    public class LogUltimoLoginVo
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new vo login based in a login event.
        /// </summary>
        /// <param name="mensaje"></param>
        public LogUltimoLoginVo(LogMensajeBase mensaje)
        {
            Id = mensaje.Id;
            IdDispositivo = mensaje.Dispositivo.Id;
            Fecha = mensaje.Fecha;
            IdCoche = mensaje.Coche.Id;
            IdChofer = mensaje.Chofer != null ? mensaje.Chofer.Id : 0;
            Chofer = mensaje.Chofer != null ? mensaje.Chofer.Entidad.Descripcion : String.Empty;
        }

        #endregion

        #region Public Properties

        public Int32 Id { get; set; }
        public Int32 IdDispositivo { get; set; }
        public DateTime? Fecha { get; set; }
        public Int32 IdCoche { get; set; }
        public Int32 IdChofer { get; set; }
        public String Chofer { get; set; }

        #endregion
    }
}