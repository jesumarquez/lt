#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections;
using Logictracker.Cache;
using Logictracker.Cache.Interfaces;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Types.ValueObject.Messages;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;

#endregion

namespace Logictracker.Types.BusinessObjects.Dispositivos
{
    /// <summary>
    /// Class that represents an application device.
    /// </summary>
    [Serializable]
    public class Dispositivo : IComparable, IDataIdentify, IAuditable, ISecurable, IHasTipoDispositivo
    {
        public static class Estados
        {
            public const short Activo = 0;
            public const short EnMantenimiento = 1;
            public const short Inactivo = 2;
        }

        #region Private Properties

        private ISet<DetalleDispositivo> _detallesDispositivo;
        private ISet<ConfiguracionDispositivo> _configuraciones;

        #endregion

        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; } 

        public virtual TipoDispositivo TipoDispositivo { get; set; }
        public virtual short PollInterval { get; set; }
        public virtual int Port { get; set; }
        public virtual short Estado { get; set; }
        public virtual DateTime? DtCambioEstado { get; set; }
        public virtual Firmware FlashedFirmware { get; set; }
        public virtual LineaTelefonica LineaTelefonica { get; set; }
        public virtual Precinto Precinto { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Imei { get; set; }
        public virtual string Tablas { get; set; }
        public virtual string Clave { get; set; }
        public virtual string Telefono { get; set; }
        public virtual int IdNum { get; set; }
        public virtual bool Verificado { get; set; }
        
        /// <summary>
        /// The firmware assigned to this particular device or to its generic type.
        /// </summary>
        public virtual Firmware Firmware { get { return FlashedFirmware ?? TipoDispositivo.Firmware; } }

        /// <summary>
        /// The base firmware for the current device.
        /// </summary>
        public virtual Firmware BaseFirmware { get { return TipoDispositivo.BaseFirmware ?? Firmware; } }

        /// <summary>
        /// Gets device parameters configuration.
        /// </summary>
        public virtual ISet<DetalleDispositivo> DetallesDispositivo { get { return _detallesDispositivo ?? (_detallesDispositivo = new HashSet<DetalleDispositivo>()); } }

        /// <summary>
        /// A list of assigned configurations.
        /// </summary>
        public virtual ISet<ConfiguracionDispositivo> Configuraciones { get { return _configuraciones ?? (_configuraciones = new HashSet<ConfiguracionDispositivo>()); } }

        public virtual String ConfiguracionAsString { get { return Configuraciones == null ? String.Empty : String.Join(", ", Configuraciones.Cast<ConfiguracionDispositivo>().Select(c => c.Descripcion).ToArray()); } }

		#endregion

        
        #region Public Methods

        /// <summary>
        /// Determines if the givenn objects equals the current one.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var castObj = obj as Dispositivo;

            return castObj != null && Id == castObj.Id && Id != 0;
        }

        /// <summary>
        /// Gets the hascode of the device based on its id.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return 27 * 57 * Id.GetHashCode(); }

        /// <summary>
        /// Compares the current device with the givenn object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual int CompareTo(object obj)
        {
            var dev = obj as Dispositivo;

            return dev == null ? -1 : Codigo.CompareTo(dev.Codigo);
        }

        /// <summary>
        /// Gets the DetalleDispositivo that matches with the parameter ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual DetalleDispositivo FindDetailById(int id) { return DetallesDispositivo.Cast<DetalleDispositivo>().FirstOrDefault(detail => detail.Id.Equals(id)); }

        /// <summary>
        /// Gets the DetalleDispositivo that matches with the parameter name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //public virtual DetalleDispositivo FindDetailByName(string name)
        //{
        //    var lName = name.ToLower().Trim();
        //    var t = new TimeElapsed();
        //    var detalles = DetallesDispositivo.Cast<DetalleDispositivo>();
        //    if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", Id, String.Format("Dispositivo - FindDetailByName -  Cast Detalles ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
        //    t.Restart();
        //    var filtrado =
        //        detalles.FirstOrDefault(
        //            w =>
        //            w.TipoParametro.DispositivoTipo.Equals(w.Dispositivo.TipoDispositivo) &&
        //            w.TipoParametro.Nombre.ToLower().Trim().Equals(lName));
        //    if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", Id, String.Format("Dispositivo - FindDetailByName - filtrado Detalle ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
        //    return filtrado;
        //}

        public virtual IEnumerable<DetalleDispositivo> FindDetailsStartWith(string name)
        {
            var lName = name.ToLower().Trim();
            var t = new TimeElapsed();            
            var detalles = DetallesDispositivo.Cast<DetalleDispositivo>();
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", Id, String.Format("Dispositivo - FindDetailsStartWith - Cast Detalles ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
            t.Restart();
            var filtrado =
                detalles.Where(
                    w =>
                    w.TipoParametro.DispositivoTipo.Equals(w.Dispositivo.TipoDispositivo) &&
                    w.TipoParametro.Nombre.ToLower().Trim().StartsWith(lName));
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("DispatcherLock", Id, String.Format("Dispositivo - FindDetailsStartWith - filtrado Detalles ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
            return filtrado;
        }

        /// <summary>
        /// Gets max revision value from the configuration details list.
        /// </summary>
        /// <returns></returns>
        public virtual int GetMaxRevision() { return DetallesDispositivo.Count.Equals(0) ? 0 : (from DetalleDispositivo detalle in DetallesDispositivo select detalle.Revision).Max(); }

        /// <summary>
        /// Adds the givenn parameter to the device configuration details.
        /// </summary>
        /// <param name="parametro"></param>
        public virtual void AddDetalleDispositivo(TipoParametroDispositivo parametro)
        {
            if (parametro == null) return;

            if (parametro.DispositivoTipo == null)
                parametro.DispositivoTipo = TipoDispositivo;

            var detalle = new DetalleDispositivo { Dispositivo = this, Revision = 1, TipoParametro = parametro, Valor = parametro.ValorInicial };

            if (DetallesDispositivo.Contains(detalle)) return;

            DetallesDispositivo.Add(detalle);
        }

        /// <summary>
        /// Adds the specified device detail.
        /// </summary>
        /// <param name="detalle"></param>
        public virtual void AddDetalleDispositivo(DetalleDispositivo detalle) { DetallesDispositivo.Add(detalle); }

        /// <summary>
        /// Clear all currently assigned parameters due to device type change.
        /// </summary>
        public virtual void ClearParameters() { DetallesDispositivo.Clear(); }

        /// <summary>
        /// Retrieves from cache the vehicle last position.
        /// </summary>
        /// <returns></returns>
        public virtual LogUltimaPosicionVo RetrieveLastPosition() { return this.Retrieve<LogUltimaPosicionVo>("lastPosition"); }

        /// <summary>
        /// Determines if the current vehicle has the last reported position in cache.
        /// </summary>
        /// <returns></returns>
        public virtual Boolean IsLastPositionInCache() { return this.KeyExists("lastPosition"); }

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
        /// Clears vehicles cached data.
        /// </summary>
        public virtual void ClearCache()
        {
            this.Delete("lastPosition");
            this.Delete("lastLogin");
        }

        /// <summary>
        /// Adds the specific configuration to the current device.
        /// </summary>
        /// <param name="configuration"></param>
        public virtual void AddConfiguration(ConfiguracionDispositivo configuration) { Configuraciones.Add(configuration); }

        /// <summary>
        /// Adds the specific configuration to the current device.
        /// </summary>
        /// <param name="configuration"></param>
        public virtual void DeleteConfiguration(ConfiguracionDispositivo configuration) { Configuraciones.Remove(configuration); }

        /// <summary>
        /// Gets the device current configuration.
        /// </summary>
        /// <returns></returns>
        public virtual String GetConfiguration()
        {
            if (Configuraciones.Count.Equals(0)) return TipoDispositivo.GetConfiguration();

            var configurationBuilder = new StringBuilder();

            foreach (var configuracion in Configuraciones.OfType<ConfiguracionDispositivo>().OrderBy(config => config.Orden).ToList())
            {
	            configurationBuilder.AppendLine(configuracion.Configuracion);
            }

            return configurationBuilder.ToString();
        }

        public virtual int MaxRevision { get { return DetallesDispositivo.Count > 0 ? DetallesDispositivo.Cast<DetalleDispositivo>().Max(d => d.Revision) : 0; } }

        public virtual bool HasGarmin
        {
            get
            {
                var det = DetallesDispositivo.Cast<DetalleDispositivo>().FirstOrDefault(detail => detail.TipoParametro.Nombre.Equals("GTE_MESSAGING_DEVICE"));
                // MessagingDevice ...    public const String Garmin = "GARMIN";
                return det != null && det.Valor.Equals("GARMIN");    
            }
        }

		#endregion
    }

	public static class DispositivoX
	{
		public static int GetId(this Dispositivo me)
		{
			return me == null ? 0 : me.Id;
		}
	}
}