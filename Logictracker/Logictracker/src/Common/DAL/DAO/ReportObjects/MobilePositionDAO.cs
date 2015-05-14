using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;
using Logictracker.Types.ValueObject.Messages;
using Logictracker.Types.ValueObject.Positions;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class MobilePositionDAO : ReportDAO
    {
        #region Private Const Properties

        /// <summary>
        /// Position state constant values.
        /// </summary>
        private const int Minutesred = 2880;
        private const int Minutesyellow = 5;

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobilePositionDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the last position of each device of the selected location associated to the givenn mobile type.
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<MobilePosition> GetDevicesLastPosition(IEnumerable<Dispositivo> devices)
        //{
        //    var rfids = DAOFactory.LogUltimoLoginDAO.GetLastDevicesRfidEvents(devices);
        //    var positions = DAOFactory.LogPosicionDAO.GetLastDevicesPositions(devices);

        //    return devices.Select(device => GetLastPosition(device, positions[device.Id], rfids[device.Id])).ToList();
        //}

        //public IEnumerable<MobilePosition> GetDevicesLastPosition(IEnumerable<Coche> coches)
        //{
        //    var devices = coches.Where(c => c.Dispositivo != null).Select(c => c.Dispositivo);
        //    var rfids = DAOFactory.LogUltimoLoginDAO.GetLastDevicesRfidEvents(devices);
        //    var positions = DAOFactory.LogPosicionDAO.GetLastVehiclesPositions(coches);

        //    return coches.Select(c => GetLastPosition(c, positions[c.Id], rfids[c.Dispositivo.Id])).ToList();
        //}

        /// <summary>
        /// Get the last position reported by the givenn device.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        //public MobilePosition GetDeviceLastPosition(Dispositivo device)
        //{
        //    var lastPosition = DAOFactory.LogPosicionDAO.GetLastDevicePosition(device);

        //    if (lastPosition == null) return null;

        //    var lastRfid = DAOFactory.LogUltimoLoginDAO.GetLastDeviceRfidEvent(device);

        //    return GetLastPosition(device, lastPosition, lastRfid);
        //}

        /// <summary>
        /// Gets the last position of each mobile of the selected location and type..
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MobilePosition> GetMobilesLastPosition(IEnumerable<Coche> mobiles)
        {
            var rfids = DAOFactory.LogUltimoLoginDAO.GetLastVehiclesRfidEvents(mobiles);
            var positions = DAOFactory.LogPosicionDAO.GetLastVehiclesPositions(mobiles);

            return mobiles.Select(mobile => GetLastPosition(mobile, positions[mobile.Id], rfids[mobile.Id])).ToList();
        }

        /// <summary>
        /// Get the last position reported by the givenn device.
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public MobilePosition GetMobileLastPosition(Coche mobile)
        {
            var lastPosition = DAOFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(mobile);

            if (lastPosition == null) return null;

            var lastRfid = DAOFactory.LogUltimoLoginDAO.GetLastVehicleRfidEvent(mobile);

            return GetLastPosition(mobile, lastPosition, lastRfid);
        }

        /// <summary>
        /// Gets the last N position of each mobile of the selected location.
        /// </summary>
        /// <param name="n"></param>
        /// <param name="device"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public IEnumerable<MobilePosition> GetLastNPosition(int n, int device, int mobile)
        {
            var results = new List<MobilePosition>();
            var dispositivo = DAOFactory.DispositivoDAO.FindById(device);
            var coche = DAOFactory.CocheDAO.FindById(mobile);

            if (coche == null) return results;
            var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;

            var lastPositions = DAOFactory.LogPosicionDAO.GetLastNPositions(mobile, n, maxMonths);

            if (lastPositions.Count().Equals(0)) return results;

            return (from lastPosition in lastPositions
                    let lastRfid = DAOFactory.LogMensajeDAO.GetLastVehicleRfidEvent(coche.Id, lastPosition.FechaMensaje)
                    select GetPosition(coche, dispositivo, lastPosition, lastRfid)).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get s amobile position.
        /// </summary>
        /// <param name="coche"></param>
        /// <param name="dispositivo"></param>
        /// <param name="lastPosition"></param>
        /// <param name="lastRfid"></param>
        /// <returns></returns>
        private static MobilePosition GetPosition(Coche coche, Dispositivo dispositivo, LogPosicionBase lastPosition, LogMensajeBase lastRfid)
        {
            return new MobilePosition
                       {
                           Dispositivo = dispositivo.Codigo,
                           Interno = coche.Interno,
                           Fecha = lastPosition.FechaMensaje.ToDisplayDateTime(),
                           IdDispositivo = dispositivo.Id,
                           FechaAlta = lastPosition.FechaRecepcion.ToDisplayDateTime(),
                           Latitud = lastPosition.Latitud,
                           Longitud = lastPosition.Longitud,
                           Responsable = coche.Chofer != null ? coche.Chofer.Entidad.Descripcion : string.Empty,
                           IdMovil = coche.Id,
                           IdBase = coche.Linea != null ? coche.Linea.Id : 0,
                           Velocidad = lastPosition.Velocidad,
                           IdPosicion = lastPosition.Id,
                           //Firmware = dispositivo.FullFirmwareVersion,
                           EstadoMovil = GetStatusDescription(coche.Estado),
                           EstadoDispositivo = GetStatusDescription(dispositivo.Estado),
                           ReferenciaVehiculo = coche.Referencia,
                           TipoDispositivo = GetDeviceTypeDescription(dispositivo),
                           EstadoReporte = GetReportStatus(lastPosition),
                           Distrito = MobilePosition.GetDistrito(coche),
                           Base = MobilePosition.GetBase(coche),
                           TipoVehiculo = coche.TipoCoche.Descripcion,
                           IdDistrito = coche.Empresa != null ? coche.Empresa.Id : 0,
                           //Qtree = dispositivo.QtreeRevision,
                           Chofer = lastRfid != null ? lastRfid.Chofer != null ? lastRfid.Chofer.Entidad.Descripcion : string.Empty : string.Empty,
                           UltimoLogin = lastRfid != null ? lastRfid.Fecha.ToDisplayDateTime() : DateTime.MinValue
                       };
        }

        /// <summary>
        /// Gets the last vehicle position.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="lastPosition"></param>
        /// <param name="lastRfid"></param>
        /// <returns></returns>
        private static MobilePosition GetLastPosition(Coche mobile, LogUltimaPosicionVo lastPosition, LogUltimoLoginVo lastRfid)
        {
            var position = new MobilePosition(mobile, lastPosition, lastRfid);

            UpdateDates(position);

            position.EstadoMovil = GetStatusDescription(mobile.Estado);

            if (lastPosition != null) position.EstadoDispositivo = GetStatusDescription(lastPosition.EstadoDispositivo);

            return position;
        }

        /// <summary>
        /// Gets the last device position.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="lastPosition"></param>
        /// <param name="lastRfid"></param>
        /// <returns></returns>
		private MobilePosition GetLastPosition(Dispositivo device, LogUltimaPosicionVo lastPosition, LogUltimoLoginVo lastRfid)
		{
			var coche = lastPosition != null ? DAOFactory.CocheDAO.FindById(lastPosition.IdCoche) : DAOFactory.CocheDAO.FindMobileByDevice(device.Id);

			var position = new MobilePosition(device, coche, lastPosition, lastRfid);

			UpdateDates(position);

			if (lastPosition != null)
			{
				position.EstadoMovil = GetStatusDescription(coche.Estado);
				position.EstadoDispositivo = GetStatusDescription(lastPosition.EstadoDispositivo);
			}

			return position;
		}

        /// <summary>
        /// Converts to display datetime the dates of the givenn position.
        /// </summary>
        /// <param name="position"></param>
        private static void UpdateDates(MobilePosition position)
        {
            if (position.Fecha.HasValue) position.Fecha = position.Fecha.Value.ToDisplayDateTime();
            if (position.FechaAlta.HasValue) position.FechaAlta = position.FechaAlta.Value.ToDisplayDateTime();
            if (position.UltimoLogin.HasValue) position.UltimoLogin = position.UltimoLogin.Value.ToDisplayDateTime();
        }

        /// <summary>
        /// Gets the device type description
        /// </summary>
        /// <param name="dispositivo"></param>
        /// <returns></returns>
        private static string GetDeviceTypeDescription(Dispositivo dispositivo) { return string.Concat(dispositivo.TipoDispositivo.Fabricante, " - ", dispositivo.TipoDispositivo.Modelo); }

        /// <summary>
        /// Gets the description associated to the specified state.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private static string GetStatusDescription(int status)
        {
            switch (status)
            {
                case Coche.Estados.Activo: return CultureManager.GetLabel("VEHICLESTATE_ACTIVE");
                case Coche.Estados.EnMantenimiento: return CultureManager.GetLabel("VEHICLESTATE_TALLER");
                case Coche.Estados.Inactivo: return CultureManager.GetLabel("VEHICLESTATE_INACTIVE");
                case Coche.Estados.Revisar: return CultureManager.GetLabel("VEHICLESTATE_REVISAR");
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the mobile or device report status based on the last reported position datetime.
        /// </summary>
        /// <param name="position"></param>
        private static int GetReportStatus(LogPosicionBase position)
        {
            if (position == null) return 3;

            var minutosDesdeReporte = DateTime.UtcNow.Subtract(position.FechaMensaje);

            if (minutosDesdeReporte > TimeSpan.FromMinutes(Minutesred)) return 2;

            return minutosDesdeReporte > TimeSpan.FromMinutes(Minutesyellow) ? 1 : 0;
        }

        #endregion
    }
}
