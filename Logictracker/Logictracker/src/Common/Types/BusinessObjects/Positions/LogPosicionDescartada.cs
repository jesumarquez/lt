using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

namespace Logictracker.Types.BusinessObjects.Positions
{
    /// <summary>
    /// Class that represents a discarted position.
    /// </summary>
    public class LogPosicionDescartada : LogPosicionBase
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new discarted position.
        /// </summary>
        public LogPosicionDescartada() {}

        /// <summary>
        /// Instanciates a new discarted position with the givenn reason code and position values.
        /// </summary>
        public LogPosicionDescartada(LogPosicionBase position, DiscardReason reasonCode)
        {
            FechaRecepcion = position.FechaRecepcion;
            FechaMensaje = position.FechaMensaje;
            Longitud = position.Longitud;
            Latitud = position.Latitud;
            Dispositivo = position.Dispositivo;
            Coche = position.Coche;
            Velocidad = position.Velocidad;
            VeloCalculada = position.VeloCalculada;
            Altitud = position.Altitud;
            Curso = position.Curso;
            Status = position.Status;
            MotorOn = position.MotorOn;
			MotivoDescarte = (int)reasonCode;
        }
        public LogPosicionDescartada(GPSPoint position, Dispositivo dispositivo, Coche coche, DiscardReason reasonCode)
            : base(position, dispositivo, coche)
        {
			MotivoDescarte = (int)reasonCode;
        }
        public LogPosicionDescartada(GPSPoint position, Coche coche, DiscardReason reasonCode)
            :base(position, coche)
        {
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
        /// Determines if the givenn objects is equivalent to the current position.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var castObj = obj as LogPosicionDescartada;
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
