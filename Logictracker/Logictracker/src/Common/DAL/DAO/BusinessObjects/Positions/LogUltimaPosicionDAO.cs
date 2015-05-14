#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using Urbetrack.DAL.DAO.BaseClasses;
using Urbetrack.Types.BusinessObjects.Dispositivos;
using Urbetrack.Types.BusinessObjects.Positions;
using Urbetrack.Types.BusinessObjects.Vehiculos;
using Urbetrack.Types.ValueObject.Positions;

#endregion

namespace Urbetrack.DAL.DAO.BusinessObjects.Positions
{
    /// <summary>
    /// Class for maintaining a view of the last position reported by each vehicle.
    /// </summary>
    public class LogUltimaPosicionDAO : MaintenanceDAO<LogUltimaPosicion>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
        public LogUltimaPosicionDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the last position for all the specified devices ids.
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public Dictionary<int, LogUltimaPosicionVo> GetLastDevicesPositions(IEnumerable<Dispositivo> devices)
        {
            var results = new Dictionary<int, LogUltimaPosicionVo>();
            var devicesIds = new List<int>();

            foreach (var device in devices)
            {
                if (device.IsLastPositionInCache()) results.Add(device.Id, device.RetrieveLastPosition());
                else devicesIds.Add(device.Id);
            }

            if (devicesIds.Count.Equals(0)) return results;

            var positions = Session.Query<LogUltimaPosicion>().Where(position => devicesIds.Contains(position.Dispositivo.Id)).ToList();

            foreach (var id in devicesIds)
            {
                var position = positions.Where(pos => pos.Dispositivo.Id.Equals(id)).OrderByDescending(pos => pos.FechaMensaje).FirstOrDefault();

                var positionVo = position != null ? new LogUltimaPosicionVo(position) : null;

                var device = position != null ? position.Dispositivo : devices.Where(dev => dev.Id.Equals(id)).First();

                device.StoreLastPosition(positionVo);

                results.Add(device.Id, positionVo);
            }

            return results;
        }

        /// <summary>
        /// Gets the last position for the specified device id.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public LogUltimaPosicionVo GetLastDevicePosition(Dispositivo device)
        {
            if (device.IsLastPositionInCache()) return device.RetrieveLastPosition();

            var lastPosition = Session.Query<LogUltimaPosicion>().Where(position => position.Dispositivo.Id == device.Id).OrderByDescending(pos => pos.FechaMensaje).FirstOrDefault();

            var lastPositionVo = lastPosition != null ? new LogUltimaPosicionVo(lastPosition) : null;

            device.StoreLastPosition(lastPositionVo);

            return lastPositionVo;
        }

        /// <summary>
        /// Gets the last position for all the specified vehicles ids.
        /// </summary>
        /// <param name="mobiles"></param>
        /// <returns></returns>
        public Dictionary<int, LogUltimaPosicionVo> GetLastVehiclesPositions(IEnumerable<Coche> mobiles)
        {
            var results = new Dictionary<int, LogUltimaPosicionVo>();
            var mobilesIds = new List<int>();

            foreach (var mobile in mobiles)
            {
                if (mobile.IsLastPositionInCache()) results.Add(mobile.Id, mobile.RetrieveLastPosition());
                else mobilesIds.Add(mobile.Id);
            }

            if (mobilesIds.Count.Equals(0)) return results;

            var positions = Session.Query<LogUltimaPosicion>().Where(position => mobilesIds.Contains(position.Coche.Id)).ToList();

            foreach (var id in mobilesIds)
            {
                var position = positions.Where(pos => pos.Coche.Id.Equals(id)).OrderByDescending(pos => pos.FechaMensaje).FirstOrDefault();

                var positionVo = position != null ? new LogUltimaPosicionVo(position) : null;

                var mobile = position != null ? position.Coche : mobiles.Where(vehicle => vehicle.Id.Equals(id)).First();

                mobile.StoreLastPosition(positionVo);
                
                results.Add(mobile.Id, positionVo);
            }

            return results;
        }

        /// <summary>
        /// Gets the last position for the specified vehicle id.
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public LogUltimaPosicionVo GetLastVehiclePosition(Coche mobile)
        {
			if (mobile == null) return null;

            if (mobile.IsLastPositionInCache()) return mobile.RetrieveLastPosition();

            var lastPosition = Session.Query<LogUltimaPosicion>().Where(position => position.Coche.Id == mobile.Id).OrderByDescending(pos => pos.FechaMensaje).FirstOrDefault();

            var lastPositionVo = lastPosition != null ? new LogUltimaPosicionVo(lastPosition) : null;

            mobile.StoreLastPosition(lastPositionVo);

            return lastPositionVo;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the positions deletion sql command.
        /// </summary>
        /// <returns></returns>
        protected override String GetDeleteCommand() { return "delete top(:n) from opeposi02 where opeposi02_fechora <= :date ; select @@ROWCOUNT as count;"; }

        #endregion
    }
}
