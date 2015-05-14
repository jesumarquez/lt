#region Usings

using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ValueObject.Messages;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    /// <summary>
    /// Last login data access class
    /// </summary>
    public class LogUltimoLoginDAO : GenericDAO<LogUltimoLogin>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="statelessSession"></param>
//        public LogUltimoLoginDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the current driver for all the specified vehicles ids.
        /// </summary>
        /// <param name="mobiles"></param>
        /// <returns></returns>
        public Dictionary<int, Empleado> GetCurrentDrivers(IEnumerable<Coche> mobiles)
        {
            var mobilesIds = mobiles.Select(mobile => mobile.Id).ToList();

            var messages = Session.Query<LogUltimoLogin>().Where(mensaje => mobilesIds.Contains(mensaje.Coche.Id)).ToList();

            return mobilesIds.ToDictionary(id => id, id => (from mensaje in messages where mensaje.Coche.Id.Equals(id) select mensaje.Chofer).FirstOrDefault());
        }

        /// <summary>
        /// Gets the last rfid event for the specified mobile and date.
        /// </summary>
        /// <param name="coche"></param>
        /// <returns></returns>
        public LogUltimoLoginVo GetLastVehicleRfidEvent(Coche coche)
        {
            if (coche.IsLastLoginInCache()) return coche.RetrieveLastLogin();

            var lastLogin = Session.Query<LogUltimoLogin>().FirstOrDefault(mensaje => mensaje.Coche.Id == coche.Id);

            var lastLoginVo = lastLogin != null ? new LogUltimoLoginVo(lastLogin) : null;

            coche.StoreLastLogin(lastLoginVo);

            return lastLoginVo;
        }

        /// <summary>
        /// Gets the last rfid event for the specified device.
        /// </summary>
        /// <param name="dispositivo"></param>
        /// <returns></returns>
        public LogUltimoLoginVo GetLastDeviceRfidEvent(Dispositivo dispositivo)
        {
            if (dispositivo.IsLastLoginInCache()) return dispositivo.RetrieveLastLogin();

            var lastLogin = Session.Query<LogUltimoLogin>().FirstOrDefault(mensaje => mensaje.Dispositivo.Id == dispositivo.Id);

            var lastLoginVo = lastLogin != null ? new LogUltimoLoginVo(lastLogin) : null;

            dispositivo.StoreLastLogin(lastLoginVo);

            return lastLoginVo;
        }

        /// <summary>
        /// Gets the last rfid event for all specified vehicles ids.
        /// </summary>
        /// <param name="mobiles"></param>
        /// <returns></returns>
        public Dictionary<int, LogUltimoLoginVo> GetLastVehiclesRfidEvents(IEnumerable<Coche> mobiles)
        {
            var results = new Dictionary<int, LogUltimoLoginVo>();
            var mobilesIds = new List<int>();

            foreach (var mobile in mobiles)
            {
                if (mobile.IsLastLoginInCache()) results.Add(mobile.Id, mobile.RetrieveLastLogin());
                else mobilesIds.Add(mobile.Id);
            }

            if (mobilesIds.Count.Equals(0)) return results;

            var messages = Session.Query<LogUltimoLogin>().Where(mensaje => mobilesIds.Contains(mensaje.Coche.Id)).ToList();
            foreach (var id in mobilesIds)
            {
                var message = messages.FirstOrDefault(mensaje => mensaje.Coche.Id.Equals(id));

                var messageVo = message != null ? new LogUltimoLoginVo(message) : null;

                var mobile = message != null
                                 ? message.Coche
                                 : mobiles.First(vehicle => vehicle.Id.Equals(id));

                mobile.StoreLastLogin(messageVo);

                results.Add(mobile.Id, messageVo);
            }

            return results;
        }

        /// <summary>
        /// Gets the last rfid event for all specified devices ids.
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public Dictionary<int, LogUltimoLoginVo> GetLastDevicesRfidEvents(IEnumerable<Dispositivo> devices)
        {
            var results = new Dictionary<int, LogUltimoLoginVo>();
            var devicesIds = new List<int>();

            foreach (var device in devices)
            {
                if (device.IsLastLoginInCache()) results.Add(device.Id, device.RetrieveLastLogin());
                else devicesIds.Add(device.Id);
            }

            if (devicesIds.Count.Equals(0)) return results;

            var messages = Session.Query<LogUltimoLogin>().Where(mensaje => devicesIds.Contains(mensaje.Dispositivo.Id)).ToList();

            foreach (var id in devicesIds)
            {
                var message = messages.FirstOrDefault(mensaje => mensaje.Dispositivo.Id.Equals(id));

                var messageVo = message != null ? new LogUltimoLoginVo(message) : null;

                var device = message != null ? message.Dispositivo : devices.First(dev => dev.Id.Equals(id));

                device.StoreLastLogin(messageVo);

                results.Add(device.Id, messageVo);
            }

            return results;
        }

        #endregion
    }
}
