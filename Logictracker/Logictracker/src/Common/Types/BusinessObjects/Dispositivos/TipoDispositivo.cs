#region Usings

using System;
using System.Linq;
using System.Text;
using Iesi.Collections;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.BusinessObjects.Dispositivos
{   
    /// <summary>
    /// Class that represents a application device type.
    /// </summary>
    [Serializable]
    public class TipoDispositivo : IAuditable
    {
        #region Private Properties

        private ISet _dispositivos;
        private ISet _tiposParametro;
        private ISet _configuraciones;

        #endregion

        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual string Modelo { get; set; }
        public virtual string Fabricante { get; set; }
        public virtual string ColaDeComandos { get; set; }
        public virtual bool Baja { get; set; }

        public virtual TipoDispositivo TipoDispositivoPadre { get; set; }
        public virtual Firmware Firmware { get; set; }

        public virtual Firmware BaseFirmware { get { return TipoDispositivoPadre != null ? TipoDispositivoPadre.BaseFirmware : Firmware; } }
     
        public virtual ISet TiposParametro { get { return _tiposParametro ?? (_tiposParametro = new ListSet()); } }

        public virtual ISet Dispositivos { get { return _dispositivos ?? (_dispositivos = new ListSet()); } }

        /// <summary>
        /// A list of assigned configurations.
        /// </summary>
        public virtual ISet Configuraciones { get { return _configuraciones ?? (_configuraciones = new ListSet()); } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds the device parameter type associated to the givenn name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual TipoParametroDispositivo FindByName(string name)
        {
            var parameter = TiposParametro.Cast<TipoParametroDispositivo>().FirstOrDefault(parametro => parametro.Nombre.Equals(name));

            if (parameter != null || TipoDispositivoPadre == null) return parameter;

            return TipoDispositivoPadre.FindByName(name);
        }

        public override bool Equals(object obj)
        {
            var castObj = obj as TipoDispositivo;

            return castObj != null && castObj.Modelo.Equals(Modelo) && castObj.Fabricante.Equals(Fabricante);
        }
        
        public override int GetHashCode() { return Modelo.GetHashCode() + Fabricante.GetHashCode(); }

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
            if (Configuraciones.Count.Equals(0)) return String.Empty;

            var configurationBuilder = new StringBuilder();

            foreach (var configuracion in Configuraciones.OfType<ConfiguracionDispositivo>().OrderBy(config => config.Orden).ToList())
                configurationBuilder.AppendLine(configuracion.Configuracion);

            return configurationBuilder.ToString();
        }

        #endregion
    }
}
