#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Utils;

#endregion

namespace Logictracker.Types.ValueObject.Positions
{
    /// <summary>
    /// Value object for representing a position.
    /// </summary>
    [Serializable]
    public class LogPosicionVo
    {
        #region Public Properties

        public Int32 Id { get; set; }
        public DateTime FechaRecepcion { get; set; }
        public DateTime FechaMensaje { get; set; }
        public Double Longitud { get; set; }
        public Double Latitud { get; set; }
        public Double Altitud { get; set; }
        public Int32 IdDispositivo { get; set; }
        public String Dispositivo { get; set; }
        public String TipoDispositivo { get; set; }
        //public String Firmware { get; set; }
        //public String Qtree { get; set; }
        public short EstadoDispositivo { get; set; }
        public Int32 IdCoche { get; set; }
        public Int32 IdTipoCoche { get; set; }
        public String IconoDefault { get; set; }
        public String IconoNormal { get; set; }
        public String IconoAtraso { get; set; }
        public String IconoAdelanto { get; set; }
        public String TipoCoche { get; set; }
        public String Coche { get; set; }
        public String ReferenciaCoche { get; set; }
        public String CentroDeCosto { get; set; }
        public Int32 Velocidad { get; set; }
        public String Responsable { get; set; }
        public Boolean ValidParents { get; set; }
        public Double Curso { get; set; }
        public bool? MotorOn { get; set; }
        public float? HDop { get; set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new position vo based on the givenn position.
        /// </summary>
        /// <param name="lastPosition"></param>
        public LogPosicionVo(LogPosicionBase lastPosition)
        {
            var te = new TimeElapsed();
            Id = lastPosition.Id;
            FechaRecepcion = lastPosition.FechaRecepcion;
            FechaMensaje = lastPosition.FechaMensaje;
            Longitud = lastPosition.Longitud;
            Latitud = lastPosition.Latitud;
            Altitud = lastPosition.Altitud;
            IdDispositivo = lastPosition.Dispositivo.Id;
            try
            {
                Dispositivo = lastPosition.Dispositivo.Codigo;
                TipoDispositivo = String.Format("{0} - {1}", lastPosition.Dispositivo.TipoDispositivo.Fabricante, lastPosition.Dispositivo.TipoDispositivo.Modelo);
                //Firmware = lastPosition.Dispositivo.FullFirmwareVersion;
                //Qtree = lastPosition.Dispositivo.QtreeRevision;
                EstadoDispositivo = lastPosition.Dispositivo.Estado;
            }
            catch{}
            
            Velocidad = lastPosition.Velocidad;
            Curso = lastPosition.Curso;
            MotorOn = lastPosition.MotorOn;
            HDop = lastPosition.HDop;

            var totalSecs = te.getTimeElapsed().TotalSeconds;
            if (totalSecs > 1) STrace.Error("DispatcherLock", lastPosition.Dispositivo.Id, "LogPosicionVo #1: " + totalSecs);
            
            te.Restart();
            ApplyVehicleData(lastPosition.Coche);
            totalSecs = te.getTimeElapsed().TotalSeconds;
            if (totalSecs > 1) STrace.Error("DispatcherLock", lastPosition.Dispositivo.Id, "ApplyVehicleData: " + totalSecs);

            te.Restart();
            HasValidParents(lastPosition);
            totalSecs = te.getTimeElapsed().TotalSeconds;
            if (totalSecs > 1) STrace.Error("DispatcherLock", lastPosition.Dispositivo.Id, "HasValidParents: " + totalSecs);
        }

        #endregion

        #region Public Methods

        public void ApplyVehicleData(Coche vehicle)
        {   
            IdCoche = vehicle.Id;
            IdTipoCoche = vehicle.TipoCoche.Id;

            if (vehicle.Empresa.IconoPorCentroDeCosto && vehicle.CentroDeCostos != null)
            {                
                var path = vehicle.Empresa.GetUrlIcono(vehicle.CentroDeCostos.Codigo);
                IconoDefault = path;
                IconoNormal = path;
                IconoAtraso = path;
                IconoAdelanto = path;                
            }
            else
            {                
                IconoDefault = vehicle.TipoCoche.IconoDefault.PathIcono;
                IconoNormal = vehicle.TipoCoche.IconoNormal.PathIcono;
                IconoAtraso = vehicle.TipoCoche.IconoAtraso.PathIcono;
                IconoAdelanto = vehicle.TipoCoche.IconoAdelanto.PathIcono;
            }
            Coche = vehicle.Interno;
            TipoCoche = vehicle.TipoCoche.Descripcion;
            try
            {
                Responsable = vehicle.Chofer != null && vehicle.Chofer.Entidad != null ? vehicle.Chofer.Entidad.Descripcion : string.Empty;
            }
            catch (Exception)
            {
                Responsable = string.Empty;
            }
            ReferenciaCoche = vehicle.Referencia;
            CentroDeCosto = vehicle.CentroDeCostos != null ? vehicle.CentroDeCostos.Descripcion : String.Empty;
        }

        /// <summary>
        /// Determines if the givenn objects is equivalent to the current position.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var castObj = obj as LogPosicionVo;

            return castObj != null && Id == castObj.Id && Id != 0;
        }

        /// <summary>
        /// Get the hash code associated to the position based on its id.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 27 * 57 * Id.GetHashCode(); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if the current device has valid parents.
        /// </summary>
        /// <returns></returns>
        private void HasValidParents(LogPosicionBase position)
        {
            var validCompany = position.Coche.Empresa == null || !position.Coche.Empresa.Baja;
            var validLocation = position.Coche.Linea == null || !position.Coche.Linea.Baja;

            ValidParents = validCompany || validLocation;
        }

        #endregion
    }
}