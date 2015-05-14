#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Urbetrack.DAL.Factories;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Description;
using Urbetrack.Model;
using Urbetrack.Types.BusinessObjects.Dispositivos;
using Urbetrack.Types.BusinessObjects.Messages;

#endregion

namespace Urbetrack.SessionBorder
{
    [FrameworkElement(XName = "SQLDataProvider", IsContainer = false)]
    public class SqlServerDataProvider : FrameworkElement, IDataProvider
    {
		#region Attributes
		
		[ElementAttribute(XName = "Parser", IsSmartProperty = true, IsRequired = true, DefaultValue = null)]
		public ACParser Parser
		{
			get { return (ACParser)GetValue("Parser"); }
			set { SetValue("Parser", value); }
		}

		#endregion

		#region Properties

		[ThreadStatic]
		private static DAOFactory _daoFactory;
		private static DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } }
		
		#endregion

        #region IDataProvider

		public List<Mensaje> GetCannedMessagesTable(int device, int revision)
		{
			return DaoFactory.MensajeDAO.GetCannedMessagesTable(device, revision);
		}

		public DetalleDispositivo GetDetalleDispositivo(int DeviceId, String name)
		{
			return DaoFactory.DispositivoDAO.FindById(DeviceId).FindDetailByName(name);
		}

		public List<DetalleDispositivo> GetDetallesDispositivo(int DeviceId)
		{
			return DaoFactory.DispositivoDAO.FindById(DeviceId).DetallesDispositivo.Cast<DetalleDispositivo>().ToList();
		}

		public Boolean SetDetalleDispositivo(int DeviceId, String name, String value, String type)
		{
			var device = DaoFactory.DispositivoDAO.FindById(DeviceId);

			var details = new[]
			              	{
			              		name,
			              		value,
			              		"S",
			              		type,
			              		"0",
			              		"false"
			              	};

            GenerateDeviceParameterType(device, details, DaoFactory);

            GenerateDeviceParameter(device, details, DaoFactory, Parser);

			return true;
		}

		public string GetConfiguration(int DeviceId)
    	{
			return DaoFactory.DispositivoDAO.FindById(DeviceId).GetConfiguration();
    	}

		public INode Get(int DeviceId)
		{
			try
			{
				if (DeviceId == 0) return null;
				var dev = DaoFactory.DispositivoDAO.FindById(DeviceId);
				return dev == null ? null : Parser.Clone(dev.Id, dev.Imei);
			}
			catch (Exception e)
			{
				STrace.Trace(GetType().FullName, @"No existe el dispositivo con id ""{0}"" en la base; Error: {1}", DeviceId, e);
				return null;
			}
		}

		public INode Find(String imei)
        {
			var dev = DaoFactory.DispositivoDAO.GetByImei(imei);
			return dev == null ? null : Parser.Clone(dev.Id, dev.Imei);
        }

    	#endregion

        #region Private Methods

        /// <summary>
        /// Generates a new device parameter type with the givenn values.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="param"></param>
        /// <param name="daoFactory"></param>
        private static void GenerateDeviceParameterType(Dispositivo device, IList<object> param, DAOFactory daoFactory)
        {
            var parameter = (string)param[0];

            if (device.TipoDispositivo.FindByName(parameter) != null) return;

            var type = new TipoParametroDispositivo
            {
                Consumidor = (string)param[2],
                Descripcion = parameter,
                DispositivoTipo = device.TipoDispositivo,
                Editable = Convert.ToBoolean(param[5]),
                Nombre = parameter,
                Metadata = string.Empty,
                TipoDato = (string)param[3],
                ValorInicial = (string)param[4]
            };

            daoFactory.TipoParametroDispositivoDAO.SaveOrUpdate(type);
        }

    	/// <summary>
    	/// Creates or updates a new device parameter detail with the specified values.
    	/// </summary>
    	/// <param name="device"></param>
    	/// <param name="param"></param>
    	/// <param name="daoFactory"></param>
    	/// <param name="Parser"></param>
    	private static void GenerateDeviceParameter(Dispositivo device, IList<Object> param, DAOFactory daoFactory, ACParser Parser)
        {
            var parameter = (string)param[0];
            var value = (string)param[1];

            var parametro = daoFactory.DispositivoDAO.UpdateDeviceParameter(device, parameter, value);
    		if (parametro == null || !parametro.TipoParametro.RequiereReset) return;
    		//MessageSender.CreateReboot(device, new LogMensajeSaver()).Send();
    		var dev = (IPowerBoot)Parser.Clone(device.Id, device.Imei);
    		if (dev != null)
    			dev.Reboot(0);
        }

        #endregion
    }
}