using System;
using System.Globalization;
using Logictracker.Cache;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Utils;

namespace Logictracker.Types.ValueObject.Positions
{
    /// <summary>
    /// Value object for representing last position.
    /// </summary>
    [Serializable]
    public class LogUltimaPosicionVo : LogPosicionVo
    {

        #region Public Properties

        public Int32 EstadoReporte { get { return GetReportStatus(); } }
        public string IconoOnline { get; set;}
        public double MinutosEnAmarillo { get; set; }
        public double HorasEnRojo { get; set; }

        public DateTime? FechaUltimoMensajesDescartado
        {
            get
            {
                var key = makeLastDiscardedPositionReceivedAtKey(IdDispositivo);
                var dt = LogicCache.Retrieve<string>(key);
                if (String.IsNullOrEmpty(dt))
                    return null;
                else
                    return DateTime.ParseExact(dt, "O", CultureInfo.InvariantCulture,
                                                      DateTimeStyles.AdjustToUniversal);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new position vo based on the givenn position.
        /// </summary>
        /// <param name="lastPosition"></param>
        public LogUltimaPosicionVo(LogPosicionBase lastPosition) : base(lastPosition)
        {
            if (lastPosition != null && lastPosition.Coche != null && lastPosition.Coche.Empresa != null)
            {
                MinutosEnAmarillo = lastPosition.Coche.Empresa.MinutosEnAmarillo;
                HorasEnRojo = lastPosition.Coche.Empresa.HorasEnRojo;
            }
            else
            {
                MinutosEnAmarillo = 5;
                HorasEnRojo = 48;
            }
        }

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

            var castObj = obj as LogUltimaPosicionVo;

            return castObj != null && Id == castObj.Id && Id != 0;
        }

        /// <summary>
        /// Get the hash code associated to the position based on its id.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 27 * 57 * Id.GetHashCode(); }

        #endregion

        #region Private Methods

        private String makeLastDiscardedPositionReceivedAtKey(int deviceId)
        {
            return "device_" + deviceId + "_lastDiscardedPositionReceivedAt";
        }

        /// <summary>
        /// Gets the mobile or device report status based on the last reported position datetime.
        /// </summary>
        private int GetReportStatus()
        {
            var fechaUltimoDescarte = FechaUltimoMensajesDescartado;

            DateTime ultimaFecha = (fechaUltimoDescarte != null && (fechaUltimoDescarte.Value > FechaMensaje)) ? fechaUltimoDescarte.Value : FechaMensaje;
            
            var minutosDesdeDescarteOPosicion = DateTime.UtcNow.Subtract(ultimaFecha).TotalMinutes;

            var isRojo = (minutosDesdeDescarteOPosicion > (TimeSpan.FromHours(HorasEnRojo).TotalMinutes));
            var isAmarillo = (minutosDesdeDescarteOPosicion > MinutosEnAmarillo);

            var isDescargando = (isAmarillo) &&
                (FechaRecepcion.Subtract(FechaMensaje).TotalMinutes > (MinutosEnAmarillo/2)) && 
                (DateTime.UtcNow.Subtract(FechaRecepcion).TotalMinutes < MinutosEnAmarillo);

            return (isDescargando ? 1 : (isRojo ? 3 : (isAmarillo ? 2 : 0)));
        }

        #endregion
    }
}