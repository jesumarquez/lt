using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.Entidades;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using NHibernate;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects.Dispositivos
{
    /// <summary>
    /// Device data access class.
    /// </summary>
    public class DispositivoDAO : GenericDAO<Dispositivo>
    {
//        public DispositivoDAO(ISession session) : base(session) { }

        #region Public Methods
        public Dispositivo FindByImei(string imei)
        {
            return Session.Query<Dispositivo>()
                .Where(d => d.Imei == imei)
                .Cacheable()
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the device associated to the givenn imei.
        /// </summary>
        /// <param name="imei"></param>
        /// <returns></returns>
        public Dispositivo GetByIMEI(string imei)
        {
			if (String.IsNullOrEmpty(imei) || imei == "000000000000000") return null;
			var _0_imei = "0" + imei;
			var _imei = imei.TrimStart('0');
			var Devices = Session.Query<Dispositivo>().Where(d => (d.Imei.ToUpperInvariant() == imei.ToUpperInvariant() || 
                                                                   d.Imei.ToUpperInvariant() == _imei.ToUpperInvariant() || 
                                                                   d.Imei.ToUpperInvariant() == _0_imei.ToUpperInvariant()) 
                                                                   && d.Estado < 2).Cacheable();
			if (Devices.Count() > 1)
			{
				STrace.Error(typeof(DispositivoDAO).FullName, String.Format("Error grave, el dispositivo con Imei '{0}' se encuentra dado de alta mas de una vez, IDs: '{1}'", imei, String.Join("', '", Devices.Select(dev =>  dev.Id.ToString("D4")).ToArray())));
				return null;
			}
        	return Devices.SingleOrDefault();
        }

        /// <summary>
        /// Gets the device associated to the givenn DeviceId.
        /// </summary>
        /// <param name="idNum"></param>
        /// <returns></returns>
        public Dispositivo GetByIdNum(int idNum)
        {
            var Devices = Session.Query<Dispositivo>().Where(d => d.IdNum == idNum && d.Estado < 2).Cacheable();
            if (Devices.Count() > 1)
            {
                STrace.Error(typeof(DispositivoDAO).FullName, String.Format("Error grave, el dispositivo con DeviceId '{0}' se encuentra dado de alta mas de una vez, IDs: '{1}'", idNum, String.Join("', '", Devices.Select(dev => dev.Id.ToString("D4")).ToArray())));
                return null;
            }
            return Devices.SingleOrDefault();
        }

        
        public Dispositivo GetGenericDevice()
        {
            const string imei = "No borrar.";

            var dispositivo = GetByIMEI(imei);

            if (dispositivo != null) return dispositivo;

            var tipoDao = new TipoDispositivoDAO();

            dispositivo = new Dispositivo
            {
                Codigo = imei,
                TipoDispositivo = tipoDao.FindAll().FirstOrDefault(),
                Imei = imei,
                Port = 2020,
                Clave = imei,
                Telefono = imei,
                Tablas = "",
            };

            SaveOrUpdate(dispositivo);

            return dispositivo;
        }
        public Dispositivo GetGenericDevice(Empresa empresa)
        {
            return GetGenericDevice(empresa, null);
        }
        public Dispositivo GetGenericDevice(Empresa empresa, Transportista transportista)
        {
            var descripcion = transportista != null
                            ? transportista.Descripcion.Length > 19
                                  ? transportista.Descripcion.Substring(0, 19)
                                  : transportista.Descripcion
                            : empresa != null ? empresa.Codigo : string.Empty;

            var imei = "No borrar." + descripcion;

            var dispositivo = GetByIMEI(imei);

            if (dispositivo != null) return dispositivo;

            var tipoDao = new TipoDispositivoDAO();

            dispositivo = new Dispositivo
            {
                Empresa =  empresa,
                Codigo = imei,
                TipoDispositivo = tipoDao.FindAll().FirstOrDefault(),
                Imei = imei,
                Port = 2020,
                Clave = imei,
                Telefono = imei,
                Tablas = ""
            };

            SaveOrUpdate(dispositivo);

            return dispositivo;
        }

        /// <summary>
        /// Gets all the devices for a specific TipoDispositivo.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public List<Dispositivo> GetByTipo(TipoDispositivo tipo)
        {
            return tipo == null ? FindAllActivos()
                : Session.Query<Dispositivo>().Where(device => device.Estado < 2 && device.TipoDispositivo != null && device.TipoDispositivo.Id == tipo.Id)
                      .OrderBy(device => device.Codigo).Cacheable().ToList();
        }

        public List<Dispositivo> GetByPrecinto(Precinto precinto)
        {
            return Session.Query<Dispositivo>()
                          .Where(device => device.Precinto != null 
                                        && device.Precinto.Id == precinto.Id)
                          .ToList();
        }

        public List<Dispositivo> GetByLineaTelefonica(int lineaTelefonica)
        {
            return Session.Query<Dispositivo>()
                          .Where(device => device.LineaTelefonica != null
                                        && device.LineaTelefonica.Id == lineaTelefonica)
                          .ToList();
        }

		/// <summary>
        /// Gets all devices assigned to the givenn device class.
        /// </summary>
        /// <param name="typeClass"></param>
        /// <returns></returns>
        public IEnumerable<Dispositivo> GetByTypeClass(string typeClass)
        {
            return string.IsNullOrEmpty(typeClass) ? FindAllActivos()
                : Session.Query<Dispositivo>().Where(device => device.Estado < 2 && device.TipoDispositivo != null && device.TipoDispositivo.Fabricante == typeClass)
                      .OrderBy(device => device.Codigo).ToList();
        }

        /// <summary>
        /// Sets the device as not active.
        /// </summary>
        /// <param name="dispositivo"></param>
        /// <returns></returns>
        public override void Delete(Dispositivo dispositivo)
        {
            if (dispositivo == null) return;

            dispositivo.Estado = Dispositivo.Estados.Inactivo;
            dispositivo.DtCambioEstado = DateTime.UtcNow;

            SaveOrUpdate(dispositivo);
        }

        /// <summary>
        /// Saves or update a device. If the device is null also saves the device details associated to the givenn device type.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="typeChanged"></param>
        public void SaveOrUpdateWithParametersUpdate(Dispositivo obj, Boolean typeChanged)
        {
            if (typeChanged) obj.ClearParameters();

            var parametroDAO = new TipoParametroDispositivoDAO();

            var parametros = parametroDAO.FindByTipoDispositivo(obj.TipoDispositivo.Id);

            if (parametros != null) 
				foreach (var parametro in parametros)
				{
            		obj.AddDetalleDispositivo(parametro);
				}

            SaveOrUpdate(obj);
        }

        /// <summary>
        /// Updates the specified parameter of the givenn device using the provided value.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public DetalleDispositivo UpdateDeviceParameter(Dispositivo device, String parameter, String value)
        {
            if (device == null) return null;

            var parametro = UpdateParameter(device, parameter, value);

            SaveOrUpdate(device);

            return parametro;
        }

        /// <summary>
        /// Increments in 500 all the device parameters revision in order to re configure all the device parameters on next reboot.
        /// </summary>
        /// <param name="device"></param>
        public void PurgeConfiguration(Dispositivo device)
        {
            var maxRevision = device.GetMaxRevision() + 500;

            foreach (DetalleDispositivo detail in device.DetallesDispositivo) detail.Revision = ++maxRevision;

            SaveOrUpdateWithParametersUpdate(device, false);
        }

        /// <summary>
        /// Reconfigures all device parameters to its default value.
        /// </summary>
        /// <param name="device"></param>
        public void ResetConfiguration(Dispositivo device)
        {
            foreach (TipoParametroDispositivo type in device.TipoDispositivo.TiposParametro) UpdateParameter(device, type.Nombre, type.ValorInicial);

            SaveOrUpdateWithParametersUpdate(device, false);
        }

        /// <summary>
        /// Getsthe device associated to the specified code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Dispositivo GetByCode(String code) { return Session.Query<Dispositivo>().FirstOrDefault(device => device.Codigo == code && device.Estado < 2); }

        public Dispositivo FindByCode(String code) { return Session.Query<Dispositivo>().FirstOrDefault(device => device.Codigo == code); }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the specified device parameter.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static DetalleDispositivo UpdateParameter(Dispositivo device, string parameter, string value)
        {
            var ddDAO = new DAOFactory().DetalleDispositivoDAO;
            var parametro = ddDAO.GetDeviceDetail(device.Id, parameter) ?? CreateDeviceParameter(device, parameter);

            parametro.Valor = value;
            parametro.Revision = device.GetMaxRevision() + 1;

            return parametro;
        }

        /// <summary>
        /// Finds all active devices.
        /// </summary>
        /// <returns></returns>
        private List<Dispositivo> FindAllActivos()
        {
            return Session.Query<Dispositivo>().Where(dispositivo => dispositivo.Estado < 2).OrderBy(dispositivo => dispositivo.Codigo).Cacheable().ToList();
        }

        /// <summary>
        /// Creates a device specific parameter for the givenn device.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static DetalleDispositivo CreateDeviceParameter(Dispositivo device, string parameter)
        {
            if (device == null) return null;

            device.AddDetalleDispositivo(device.TipoDispositivo.FindByName(parameter));

            var ddDAO = new DAOFactory().DetalleDispositivoDAO;
            return ddDAO.GetDeviceDetail(device.Id, parameter);
        }

        #endregion

		public Dispositivo ForceById(int id)
		{
			return Session.Query<Dispositivo>().First(x => x.Id == id && (x.Id != (-DateTime.UtcNow.Millisecond)));
		}

		public List<Dispositivo> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposDispositivo, bool onlyUnassigned)
        {
            var list = GetList(empresas, lineas, tiposDispositivo);
            if(onlyUnassigned)
            {
                var cocheDao = new CocheDAO();
                list = list.Where(d => cocheDao.FindMobileByDevice(d.Id) == null).ToList();
            }
            return list;
        }
        public List<Dispositivo> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposDispositivo)
        {
            return Query.FilterEmpresa(Session, empresas)
                .FilterLinea(Session, empresas, lineas)
                .FilterTipoDispositivo(Session, tiposDispositivo)
                .Where(d => d.Estado < Dispositivo.Estados.Inactivo)
                .Cacheable()
                .ToList();
        }
        public List<Dispositivo> GetUnassigned(IEnumerable<int> empresas, IEnumerable<int> lineas, IEnumerable<int> tiposDispositivo, string padre)
        {
            var list = GetList(empresas, lineas, tiposDispositivo);

            if (padre != "Sensor" && padre != "Entidad") //alguna cosa 'Luquística' (?)
            {
                var aCoches = new CocheDAO().FindAllAssigned().Select(c => c.Dispositivo.Id);
                var aEntidades = new EntidadDAO().FindAllAssigned().Select(e => e.Dispositivo.Id);

                list = list.Where(d => !aCoches.Contains(d.Id) && !aEntidades.Contains(d.Id)).ToList();
            }

            return list;
        }

        public bool IsUniqueIdNum(int idNum, int deviceId)
        {
            return Query.FirstOrDefault(d => d.IdNum == idNum && d.Id != deviceId) == null;
        }
    }
}