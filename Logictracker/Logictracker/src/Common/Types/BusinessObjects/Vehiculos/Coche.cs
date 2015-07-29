using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Iesi.Collections;
using Logictracker.Cache;
using Logictracker.Cache.Interfaces;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.ValueObject.Messages;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;

namespace Logictracker.Types.BusinessObjects.Vehiculos
{
    /// <summary>
    /// Represent a mobile for the application.
    /// </summary>
    [Serializable]
    public class Coche : IComparable, IDataIdentify, IAuditable, ISecurable, IHasTransportista, IHasCentroDeCosto, IHasTipoVehiculo, IHasMarca, IHasModelo, IHasChofer, IHasSubCentroDeCosto, IHasDepartamento
    {
        public static class Estados
        {
            public const short Activo = 0;
            public const short EnMantenimiento = 1;
            public const short Inactivo = 2;
            public const short Revisar = 3;
            public const short RevisarGarmin = 4;
        }
        public static class Totalizador
        {
            public const short Ninguno = 0;
            public const short EstadoMotor = 1;
            public const short EstadoGps = 2;
            public const short EstadoGarmin = 3;
            public const short TicketEnCurso = 4;
            public const short UltimoLogin = 5;

            public static string GetLabelVariableName(short totalizador)
            {
                switch (totalizador)
                {
                    case Ninguno: return "NINGUNO";
                    case EstadoMotor: return "ESTADO_MOTOR";
                    case EstadoGps: return "ESTADO_GPS";
                    case EstadoGarmin: return "ESTADO_GARMIN";
                    case TicketEnCurso: return "TICKET_EN_CURSO";
                    case UltimoLogin: return "ULTIMO_LOGIN";
                    default: return string.Empty;
                }
            }

            public static List<short> Totalizadores { get { return new List<short> { Ninguno, EstadoMotor, EstadoGps, EstadoGarmin, TicketEnCurso }; } }
        }

        #region Private Properties

        private ISet<MovOdometroVehiculo> _odometros;
        private ISet<Cliente> _clientes;
        private ISet<Shift> _turnos;

        #endregion

        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; } 

        public virtual Modelo Modelo { get; set; }
        public virtual Transportista Transportista { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual DateTime? FechaVto { get; set; }
        public virtual Empleado Chofer { get; set; }
        public virtual TipoCoche TipoCoche { get; set; }
        public virtual CocheOperacion CocheOperacion { get; set; }
        public virtual DateTime? DtCambioEstado { get; set; }
        public virtual string Interno { get; set; }
        public virtual Marca Marca { get; set; }
        public virtual short AnioPatente { get; set; }
        public virtual string Referencia { get; set; }
        public virtual short Estado { get; set; }
        public virtual double InitialOdometer { get; set; } //The vehicle initial odometer value. Used with the ApplicationOdometer for totalizer.
        public virtual double ApplicationOdometer { get; set; } //The odometer that the system kept of the vehicle since its first report.
        public virtual double PartialOdometer { get; set; } //A user reseteable odometer.
        public virtual DateTime? LastOdometerReset { get; set; } //The las datetime when the user reseted the PartialOdometer.
        public virtual double KilometrosDiarios { get; set; }
        public virtual CentroDeCostos CentroDeCostos { get; set; }
        public virtual SubCentroDeCostos SubCentroDeCostos { get; set; }
        public virtual DateTime? LastOdometerUpdate { get; set; }
        public virtual string ModeloDescripcion { get; set; }
        public virtual string Patente { get; set; }
        public virtual string Poliza { get; set; }
        public virtual string NroChasis { get; set; }
        public virtual string NroMotor { get; set; }
        public virtual double DailyOdometer { get; set; }
        public virtual DateTime? LastDailyOdometerRaise { get; set; }
        public virtual bool ControlaKm { get; set; }
        public virtual bool ControlaHs { get; set; }
        public virtual bool ControlaTurnos { get; set; }
        public virtual bool ControlaServicios { get; set; }
        public virtual int PorcentajeProductividad { get; set; }
        public virtual int Capacidad { get; set; }
        public virtual bool IdentificaChoferes { get; set; }
        public virtual bool ReportaAssistCargo { get; set; }
        public virtual bool EsPuerta { get; set; }
        
        public virtual double TotalOdometer { get { return InitialOdometer + ApplicationOdometer; } }

        public virtual ISet<MovOdometroVehiculo> Odometros { get { return _odometros ?? (_odometros = new HashSet<MovOdometroVehiculo>()); } }

        public virtual ISet<Cliente> Clientes { get { return _clientes ?? (_clientes = new HashSet<Cliente>()); } }

        public virtual ISet<Shift> Turnos { get { return _turnos ?? (_turnos = new HashSet<Shift>()); } }

        public virtual int VelocidadPromedio { get; set; }

        public virtual int CapacidadCarga { get; set; }


        // Cuando se cambia el dispositivo 
        // se guarda el valor viejo para poder actualizar la cache
        public virtual Dispositivo OldDispositivo { get; set; }

        private Dispositivo _dispositivo;
        public virtual Dispositivo Dispositivo
        {
            get { return _dispositivo; }
            set
            {
                if (OldDispositivo == null)
                {
                    OldDispositivo = _dispositivo;
                }
                _dispositivo = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compares this vehicle to the givenn object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var coche = obj as Coche;

            return coche == null ? 1 : Interno.CompareTo(coche.Interno);
        }

        /// <summary>
        /// Converts the vehicle to its representative string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return Interno; }

        public virtual string CompleteDescripcion()
        {
            return Interno + " - " + Patente;
        }

        /// <summary>
        /// Removes the specified odometer from the vehicle.
        /// </summary>
        /// <param name="odometro"></param>
        public virtual void RemoveOdometro(MovOdometroVehiculo odometro)
        {
            Odometros.Remove(odometro);

            odometro.Vehiculo = null;
        }

        /// <summary>
        /// Assigns the specified odometer to the current vehicle.
        /// </summary>
        /// <param name="odometro"></param>
        public virtual void AddOdometro(MovOdometroVehiculo odometro) { Odometros.Add(odometro); }

        /// <summary>
        /// Overrides the equals in order to compare by id.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == this) return true;

            var objCast = obj as Coche;

            return objCast != null && Id == objCast.Id && objCast.Id != 0;
        }

        /// <summary>
        /// Gets the vehicles hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return Id.GetHashCode(); }

        /// <summary>
        /// Checks if the vehicle is assigned to any client passed by parameter.
        /// </summary>
        /// <param name="clientes"></param>
        /// <returns></returns>
        public virtual bool IsAssignedToAnyClient(ICollection<int> clientes) { return Clientes.Cast<Cliente>().Any(c => clientes.Contains(c.Id)); }

        /// <summary>
        /// Retrieves from cache the vehicle last position.
        /// </summary>
        /// <returns></returns>
        public virtual LogUltimaPosicionVo RetrieveLastPosition() { return this.Retrieve<LogUltimaPosicionVo>("lastPosition"); }
        public virtual LogPosicionBase RetrieveNewestReceivedPosition()
        {
            var key = MakeNewestDeviceStatusKey(Dispositivo.Id);
            var deviceStatus = LogicCache.Retrieve<DeviceStatus>(typeof(DeviceStatus), key);
            return new LogPosicionBase(deviceStatus.Position, this);
        }
        public virtual LogUltimaPosicionVo RetrieveNewestReceivedPositionVo()
        {
            var key = MakeNewestDeviceStatusKey(Dispositivo.Id);
            
            var deviceStatus = LogicCache.Retrieve<DeviceStatus>(typeof(DeviceStatus), key);
            var pos = new LogPosicionBase(deviceStatus.Position, this);
            var ret = new LogUltimaPosicionVo(pos);
            
            return ret;
        }

        /// <summary>
        /// Determines if the current vehicle has the last reported position in cache.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean IsLastPositionInCache() { return this.KeyExists("lastPosition"); }
        public virtual Boolean IsNewestPositionReceivedInCache()
        {
            if (Dispositivo == null) return false;

            var key = MakeNewestDeviceStatusKey(Dispositivo.Id);
            return LogicCache.KeyExists(typeof(DeviceStatus), key);
        }

        private static string MakeNewestDeviceStatusKey(int deviceId)
        {
            return "device_" + deviceId + "_newestDeviceStatus";
        }

        /// <summary>
        /// Stores in cache the givenn position as the vehicles last position.
        /// </summary>
        /// <param name="position"></param>
        public virtual void StoreLastPosition(LogUltimaPosicionVo position) { this.Store("lastPosition", position); }

        /// <summary>
        /// Retrieves from cache the vehicle last login.
        /// </summary>
        /// <returns></returns>
        public virtual LogUltimoLoginVo RetrieveLastLogin() { return this.Retrieve<LogUltimoLoginVo>("lastLogin"); }

        /// <summary>
        /// Deletes the date of the last message associated to the specified code.
        /// </summary>
        /// <param name="codigo"></param>
        public virtual void DeleteLastMessageDate(String codigo) { this.Delete(GetLastMessageDateKey(codigo)); }

        /// <summary>
        /// Stores the last message date associated to the current code.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="dateTime"></param>
        public virtual void StoreLastMessageDate(String codigo, DateTime? dateTime) { this.Store(GetLastMessageDateKey(codigo), dateTime); }

        /// <summary>
        /// Retrieves the last message date associated to the givenn code.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public virtual DateTime? RetrieveLastMessageDate(String codigo) { return (DateTime?)this.Retrieve<Object>(GetLastMessageDateKey(codigo)); }

        /// <summary>
        /// Determines if the last message date is present in cache for the associated code.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public virtual Boolean HasLastMessageDate(String codigo) { return this.KeyExists(GetLastMessageDateKey(codigo)); }

        public virtual Boolean HasLastEngineEvent() { return this.KeyExists(GetLastEngineEventKey()); }

        public virtual Boolean HasLastGarminStatusEvent() { return this.KeyExists(GetLastGarminStatusEventKey()); }

        public virtual Boolean HasLastPrivacyEvent() { return this.KeyExists(GetLastPrivacyEventKey()); }

        /// <summary>
        /// Determines if the current vehicle has the last reported login in cache.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean IsLastLoginInCache() { return this.KeyExists("lastLogin"); }

        /// <summary>
        /// Stores in cache the givenn message as the vehicles last login.
        /// </summary>
        /// <param name="message"></param>
        public virtual void StoreLastLogin(LogUltimoLoginVo message) { this.Store("lastLogin", message); }

        /// <summary>
        /// Determines if the current vehicle is stopped.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean IsStopped() { return this.KeyExists("stopped"); }

        /// <summary>
        /// Sets the current stopped event initial position.
        /// </summary>
        /// <param name="startPosition"></param>
        public virtual void StartStoppedEvent(LogPosicionVo startPosition) { this.Store("stopped", startPosition); }

        /// <summary>
        /// Ends the current stopped event for the vehicle.
        /// </summary>
        public virtual void EndStoppedEvent() { this.Delete("stopped"); }

        /// <summary>
        /// Gets the current stopped event start position.
        /// </summary>
        /// <returns></returns>
        public virtual LogPosicionVo GetStoppedEventStart() { return this.Retrieve<LogPosicionVo>("stopped"); }

        /// <summary>
        /// Clears vehicles cached data.
        /// </summary>
        public virtual void ClearCache()
        {
            this.Delete("lastPosition");
            this.Delete("lastLogin");
            this.Delete("stopped");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the key associated to the specified code.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        private String GetLastMessageDateKey(String codigo) { return String.Format("lastMessageDate:{0}:{1}", Id, codigo); }

        private String GetLastEngineEventKey() { return String.Format("lastEngineEvent:{0}", Id); }

        private String GetLastGarminStatusEventKey() { return String.Format("lastGarminStatusEvent:{0}", Id); }

        private String GetLastPrivacyEventKey() { return String.Format("lastPrivacyEvent:{0}", Id); }

        #endregion

        /// <summary>
        /// Resets the vehicle partial odometer.
        /// </summary>
        public virtual void ResetPartialOdometer()
        {
            PartialOdometer = 0;
            LastOdometerReset = DateTime.UtcNow;
        }

        #region Garmin Active Stop

        public virtual void StoreActiveStop(int id)
        {
            if (HasActiveStop()) DeleteActiveStop();

            this.Store("activeStop", id.ToString("#0"));
        }

        public virtual int GetActiveStop()
        {
            var ret = 0;

            if (HasActiveStop())
            {
                var str = this.Retrieve<string>("activeStop");
                int.TryParse(str, out ret);
            }
            return ret;
        }

        public virtual bool HasActiveStop() { return this.KeyExists("activeStop"); }

        public virtual void DeleteActiveStop() { this.Delete("activeStop"); }

        #endregion

        #region ETA

        public virtual DateTime? EtaReceivedAt()
        {
            var key = string.Empty;
            DateTime? dt = null;
            
            if (Dispositivo != null) key = "device_" + Dispositivo.Id + "_ETA_ReceivedAt";
            if (LogicCache.KeyExists(typeof(string), key))
            {
                var ret = LogicCache.Retrieve<string>(key);
                dt = DateTime.ParseExact(ret, "O", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            }
            return dt;
        }
        public virtual DateTime? EtaEstimated()
        {
            var key = string.Empty;
            DateTime? dt = null;

            if (Dispositivo != null) key = "device_" + Dispositivo.Id + "_ETA_Estimated";
            if (LogicCache.KeyExists(typeof(string), key))
            {
                var ret = LogicCache.Retrieve<string>(typeof(string), key);
                dt = DateTime.ParseExact(ret, "O", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            }
            return dt;
        }
        public virtual int EtaDistanceTo()
        {
            var key = string.Empty;
            var dist = 0;

            if (Dispositivo != null) key = "device_" + Dispositivo.Id + "_ETA_DistanceTo";
            if (LogicCache.KeyExists(typeof(string), key))
            {
                var ret = LogicCache.Retrieve<string>(key);
                int.TryParse(ret, out dist);
            }
            return dist;
        }

        #endregion
    }
}