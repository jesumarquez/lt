using System;
using Logictracker.Cache;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Dispatcher.Core.AuxClasses;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Utils;

namespace Logictracker.Dispatcher.Core
{
    public abstract class DeviceBaseHandler<T> : BaseHandler<T> where T : IMessage
    {
        #region Protected Properties

        /// <summary>
        /// The device that reported the event.
        /// </summary>
        protected Dispositivo Dispositivo;

        /// <summary>
        /// The vehicle associated to the device.
        /// </summary>
        protected Coche Coche;

        /// <summary>
        /// Details about the current device parameters.
        /// </summary>
        protected DeviceParameters DeviceParameters;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Generates device parameters to speed up handling.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override HandleResults OnHandleMessage(T message)
        {
            try
            {
                if (message.DeviceId == 0)
                    return HandleResults.BreakSuccess;
            } catch {}
            
            try
            {
                SessionHelper.CreateSession();
                var te = new TimeElapsed();
                Dispositivo = DaoFactory.DispositivoDAO.FindById(message.DeviceId);
                var totalSecs = te.getTimeElapsed().TotalSeconds;
                if (totalSecs > 1) STrace.Error("DispatcherLock", message.DeviceId, "DispositivoDAO.FindById: " + totalSecs);

                if (Dispositivo == null) return HandleResults.BreakSuccess;

                te.Restart();
                CalculateDeviceParameters();
                totalSecs = te.getTimeElapsed().TotalSeconds;
                if (totalSecs > 1) STrace.Error("DispatcherLock", message.DeviceId, "CalculateDeviceParameters: " + totalSecs);

                te.Restart();
                Coche = DaoFactory.CocheDAO.FindMobileByDevice(Dispositivo.Id);
                totalSecs = te.getTimeElapsed().TotalSeconds;
                if (totalSecs > 1) STrace.Error("DispatcherLock", message.DeviceId, "FindMobileByDevice: " + totalSecs);

                var result = OnDeviceHandleMessage(message);

                SessionHelper.CloseSession();

                return result;
            }
            catch
            {
                if (Coche != null) Coche.ClearCache();

                if (Dispositivo != null) Dispositivo.ClearCache();

                throw;
            }
            finally
            {
                Dispositivo = null;
                Coche = null;                    
            }
        }

        /// <summary>
        /// Performs handler main tasks.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected abstract HandleResults OnDeviceHandleMessage(T message);

        /// <summary>
        /// Gets the current driver.
        /// </summary>
        /// <param name="rfid"></param>
        /// <returns>Returns the current driver associated to the device or null in case of none.</returns>
        protected Empleado GetChofer(string rfid)
        {
            Empleado empleado = null;

            var ent = Coche as ISecurable ?? Dispositivo;
            var empresa = ent.Empresa != null ? ent.Empresa.Id : -1;
            
            if (!string.IsNullOrEmpty(rfid))
            {
                empleado = DaoFactory.EmpleadoDAO.FindByRfid(empresa, rfid);
                if (empleado != null && empleado.Id == 0) empleado = null;
            }
           
            if (empleado == null)
            {
                if (Coche == null && Dispositivo != null)
                    Coche = DaoFactory.CocheDAO.FindMobileByDevice(Dispositivo.Id);

                if (Coche != null)
                {
                    var last = DaoFactory.LogUltimoLoginDAO.GetLastVehicleRfidEvent(Coche);
                    if (last != null && last.Fecha.HasValue && last.Fecha.Value > DateTime.Now.AddHours(-24))
                        empleado = DaoFactory.EmpleadoDAO.FindById(last.IdChofer);
                    
                    if (empleado != null && empleado.Id == 0) empleado = null;
                }
            }

            return empleado;
        }

        protected Empleado GetChoferForLogin(string rfid)
        {
            Empleado empleado = null;

            var ent = Coche as ISecurable ?? Dispositivo;
            var empresa = ent.Empresa != null ? ent.Empresa.Id : -1;

            if (!string.IsNullOrEmpty(rfid))
            {
                empleado = DaoFactory.EmpleadoDAO.FindByRfid(empresa, rfid);
                if (empleado != null && empleado.Id == 0) empleado = null;
            }

            return empleado;
        }

        /// <summary>
        /// Determines if the givenn date is a valid one. Invalid dates are those far into the past or the future.
        /// </summary>
        /// <param name="fecha"></param>
        /// <param name="dp"></param>
        /// <returns></returns>
        protected static bool FechaInvalida(DateTime fecha, DeviceParameters dp)
        {
            var fechaFuturo = fecha >= DateTime.UtcNow.AddMinutes(dp.FuturePositionMinutes);
            var fechaPasado = fecha < DateTime.UtcNow.AddDays(-dp.PositionsListExpiration);

            return fechaFuturo || fechaPasado;
        }

        /// <summary>
        /// Determines if the givenn position is out of range.
        /// </summary>
        /// <param name="posicion"></param>
        /// <returns></returns>
        protected static bool FueraDelGlobo(GPSPoint posicion ) { return posicion.Lat == 0 || Math.Abs(posicion.Lat) > 90 || posicion.Lon == 0 || Math.Abs(posicion.Lon) > 180; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Adds or refreshes the devices parameters dictionary.
        /// </summary>
        private void CalculateDeviceParameters()
        {
            var key = String.Format("deviceParameters:{0}", Dispositivo.Id);

            var parameters = LogicCache.Retrieve<DeviceParameters>(typeof(DeviceParameters), key);

            if (parameters != null)
            {
                if (parameters.NeedsUpdate)
                {
                    parameters.UpdateParameters(Dispositivo);

                    LogicCache.Store(typeof(DeviceParameters), key, parameters);
                }

                DeviceParameters = parameters;
            }
            else
            {
                parameters = new DeviceParameters(Dispositivo);

                LogicCache.Store(typeof(DeviceParameters), key, parameters);

                DeviceParameters = parameters;
            }
        }

        #endregion
    }
}