using System;
using System.Collections.Generic;
using Logictracker.Configuration;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

namespace Logictracker.DAL.DAO.BusinessObjects.Dispositivos
{
    public class DetalleDispositivoDAO : GenericDAO<DetalleDispositivo>
    {
        #region Constructor

    	/// <summary>
    	/// Instanciates a new data access class using the provided nhibernate sessions.
    	/// </summary>
    	/// <param name="session"></param>
//    	public DetalleDispositivoDAO(ISession session) : base(session) { }

        #endregion

        /// <summary>
        /// Get the detailS of the parameters selected for the devices chosen 
        /// </summary>
        /// <param name="dispositivos"></param>
        /// <param name="parametros"></param>
        /// <returns></returns>
        public IEnumerable<DetalleDispositivo> GetDevicesDetail(List<int> dispositivos, List<int> parametros)
        {
            if (dispositivos.Count == 0 || parametros.Count == 0) return new List<DetalleDispositivo>();

            var dc = DetachedCriteria.For<DetalleDispositivo>("dd")
                .CreateAlias("Dispositivo", "d", JoinType.InnerJoin)
                .CreateAlias("TipoParametro", "tp", JoinType.InnerJoin)
                .Add(Restrictions.In("d.Id", dispositivos))
                .Add(Restrictions.In("tp.Id", parametros))
                .Add(Restrictions.EqProperty("d.TipoDispositivo", "tp.DispositivoTipo"))
                .SetProjection(Projections.Property("dd.Id"));

            var c = Session.CreateCriteria<DetalleDispositivo>()
                .Add(Subqueries.PropertyIn("Id", dc));

            return c.List<DetalleDispositivo>();
        }

        public IList<DetalleDispositivo> GetDeviceDetails(int deviceId)
        {
            var dc = DetachedCriteria.For<DetalleDispositivo>("dd")
                .CreateAlias("Dispositivo", "d", JoinType.InnerJoin)
                .CreateAlias("TipoParametro", "tp", JoinType.InnerJoin)
                .Add(Restrictions.Eq("d.Id", deviceId))
                .Add(Restrictions.EqProperty("d.TipoDispositivo", "tp.DispositivoTipo"))
                .SetProjection(Projections.Property("dd.Id"));

            var c = Session.CreateCriteria<DetalleDispositivo>()
                .Add(Subqueries.PropertyIn("Id", dc));

            return c.List<DetalleDispositivo>();
        }
 
        public DetalleDispositivo GetDeviceDetail(int deviceId, string parameterName)
        {
            var dc = DetachedCriteria.For<DetalleDispositivo>("dd")
                .CreateAlias("Dispositivo", "d", JoinType.InnerJoin)
                .CreateAlias("TipoParametro", "tp", JoinType.InnerJoin)
                .Add(Restrictions.Eq("d.Id", deviceId))
                .Add(Restrictions.Eq("tp.Nombre", parameterName))
                .Add(Restrictions.EqProperty("d.TipoDispositivo", "tp.DispositivoTipo"))
                .SetMaxResults(1)
                .SetProjection(Projections.Property("dd.Id"));

            var c = Session.CreateCriteria<DetalleDispositivo>()
                .Add(Subqueries.PropertyIn("Id", dc));

            return c.UniqueResult<DetalleDispositivo>();
        }

        private const string XbeeFirmware = "known_xbee_firmware_version";
        private const string XbeeHardware = "known_xbee_hardware_version";
        private const string QtreeRevisionNumber = "known_qtree_revision";
        private const string QtreeFileName = "param_qtree_file";
        private const string QtreeTypeValue = "param_qtree_type";
        private const string KnownFirmware = "known_firmware_signature";

        private const string AccesoIp = "puerta.acceso.ip";
        private const string AccesoHabilitado = "puerta.acceso.habilitado";
        private const string AccesoUsuario = "puerta.acceso.usuario";
        private const string AccesoPassword = "puerta.acceso.password";
        private const string AccesoInicio = "puerta.acceso.inicio";

        private const string DiscardsInvalidPositions = "discards_invalid_positions";
        private const string CorrectsGeoreferenciation = "corrects_georeferenciation";
        private const string GeneratesStoppedEvents = "generates_stopped_events";
        private const string CalculatesOdometers = "calculates_odometers";
        private const string InformsHdop = "informs_hdop";
        private const string MaxHdop = "max_hdop";
        private const string StoppedGeocercaRadius = "stopped_gocerca_radius";
        private const string StoppedEventTime = "stopped_event_time";
        private const string PositionsListExpiration = "positions_list_expiration";
        private const string FuturePositionMinutes = "future_position_minutes";
        private const string SpeedTicketMinimunDuration = "speed_ticket_minimum_duration";
        private const string SpeedTicketDelvelovermindur = "speed_ticket_delvelovermindur";
        private const string RefreshMinutes = "refresh_minutes";
        private const string ProcesarCicloLogisticoFlag = "ProcesarCicloLogisticoFlag";
        private const string LastDateTimeProcessed = "LastDateTimeProcessed";
        private const string GteMessagingDevice = "GTE_MESSAGING_DEVICE";

        public string GetXbeeFirmwareValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, XbeeFirmware).As((String)null); }
        public string GetXbeeHardwareValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, XbeeHardware).As((String)null); }
        public string GetQtreeRevisionNumberValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, QtreeRevisionNumber).As(string.Empty); }
        public string GetQtreeFileNameValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, QtreeFileName).As(string.Empty); }
        public string GetQtreeTypeValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, QtreeTypeValue).As(string.Empty); }
        public string GetKnownVersionValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, KnownFirmware).As(string.Empty); }
        public string GetFullFirmwareVersionValue(Dispositivo dispositivo) 
        {
            var xbeeHardware = GetXbeeHardwareValue(dispositivo.Id);
            var xbeeFirmware = GetXbeeFirmwareValue(dispositivo.Id);
            var knownFirmware = GetKnownVersionValue(dispositivo.Id);

            var firmware = knownFirmware == null || String.IsNullOrEmpty(knownFirmware) ? dispositivo.Firmware != null ? dispositivo.Firmware.Nombre : String.Empty : knownFirmware;
            return xbeeHardware == null && xbeeFirmware == null ? firmware : String.Concat(firmware, String.Format(" (Xbee HD: {0} - SF: {1})", xbeeHardware, xbeeFirmware));
        }

        public string GetAccesoIpValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, AccesoIp).As(string.Empty); }
        public bool GetAccesoHabilitadoValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, AccesoHabilitado).AsBoolean(false); }
        public string GetAccesoUsuarioValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, AccesoUsuario).As(string.Empty); }
        public string GetAccesoPasswordValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, AccesoPassword).As(string.Empty); }
        public DateTime GetAccesoInicioValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, AccesoInicio).As(DateTime.Today); }

        public bool GetDiscardsInvalidPositionsValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, DiscardsInvalidPositions).AsBoolean(true); }
        public bool GetCorrectsGeoreferenciationValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, CorrectsGeoreferenciation).AsBoolean(true); }
        public bool GetGeneratesStoppedEventsValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, GeneratesStoppedEvents).AsBoolean(true); }
        public bool GetCalculatesOdometersValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, CalculatesOdometers).AsBoolean(true); }
        public bool GetInformsHdopValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, InformsHdop).AsBoolean(true); }
        public int GetMaxHdopValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, MaxHdop).As(Config.Dispatcher.DispatcherMaxHdop); }
        public int GetStoppedGeocercaRadiusValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, StoppedGeocercaRadius).As(Config.Dispatcher.DispatcherStoppedGeocercaRadius); }
        public int GetStoppedEventTimeValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, StoppedEventTime).As(Config.Dispatcher.DispatcherStoppedGeocercaTime); }
        public int GetPositionsListExpirationValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, PositionsListExpiration).As(Config.Dispatcher.DispatcherDeviceQueueExpiration); }
        public int GetFuturePositionMinutesValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, FuturePositionMinutes).As(Config.Dispatcher.DispatcherPositionsTimeTolerance); }
        public int GetSpeedTicketMinimunDurationValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, SpeedTicketMinimunDuration).As(Config.Dispatcher.DispatcherMinimumSpeedTicketDuration); }
        public int GetSpeedTicketDelvelovermindurValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, SpeedTicketDelvelovermindur).As(Config.Dispatcher.DispatcherDeltaSpeedTicketSpeed_Overrides_MinimumSpeedTicketDuration); }
        public int GetRefreshMinutesValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, RefreshMinutes).As(Config.Dispatcher.DispatcherDeviceParametersRefresh); }
        public bool GetProcesarCicloLogisticoFlagValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, ProcesarCicloLogisticoFlag).AsBoolean(true); }

        public string GetLastDateTimeProcessedValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, LastDateTimeProcessed).As(string.Empty); }
        public string GetGteMessagingDeviceValue(int idDispositivo) { return GetDeviceDetail(idDispositivo, GteMessagingDevice).As((String)null); }
        
    }
}