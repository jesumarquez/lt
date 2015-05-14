#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Logictracker.AVL.Messages;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Messages;

#endregion

namespace Logictracker.Layers
{
    [FrameworkElement(XName = "SQLDataProvider", IsContainer = false)]
    public class SQLDataProvider : FrameworkElement, Model.IDataProvider
    {
		#region Attributes

		[ElementAttribute(XName = "Dispatcher", IsSmartProperty = true, IsRequired = true, DefaultValue = null)]
		public IDispatcherLayer Dispatcher
		{
			get { return (IDispatcherLayer)GetValue("Dispatcher"); }
			set { SetValue("Dispatcher", value); }
		}

		#endregion

		#region Properties

		[ThreadStatic]
		private static DAOFactory _daoFactory;
		private static DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } }
		
		#endregion

        #region IDataProvider

		public List<Mensaje> GetCannedMessagesTable(int deviceId, int revision)
		{
			return DaoFactory.MensajeDAO.GetCannedMessagesTable(deviceId, revision);
		}


        public List<Mensaje> GetResponsesMessagesTable(int deviceId, int revision)
        {
            return DaoFactory.MensajeDAO.GetResponsesMessagesTable(deviceId, revision);
        }

        public DetalleDispositivo GetDetalleDispositivo(int deviceId, String name)
        {
            return DaoFactory.DetalleDispositivoDAO.GetDeviceDetail(deviceId, name);
		}

        public Dispositivo GetDispositivo(int deviceId)
        {
            return DaoFactory.DispositivoDAO.FindById(deviceId);
        }

		public List<DetalleDispositivo> GetDetallesDispositivo(int deviceId)
		{
			return DaoFactory.DetalleDispositivoDAO.GetDeviceDetails(deviceId).ToList();
		}

		public void SetDetalleDispositivo(int deviceId, string name, string value, string type)
		{
			var msg = new SetDetail(deviceId, 0);
			msg.SetUserSetting(SetDetail.Fields.Name, name);
			msg.SetUserSetting(SetDetail.Fields.Value, value);
			msg.SetUserSetting(SetDetail.Fields.TipoDato, type);
			msg.SetUserSetting(SetDetail.Fields.Consumidor, "S");
			msg.SetUserSetting(SetDetail.Fields.Editable, "false");
			msg.SetUserSetting(SetDetail.Fields.Metadata, "");
			msg.SetUserSetting(SetDetail.Fields.ValorInicial, "0");
			Dispatcher.Dispatch(msg);
		}

		public string GetConfiguration(int deviceId)
		{
			STrace.Debug(GetType().FullName, deviceId, String.Format("ForceById GetConfiguration"));
			return DaoFactory.DispositivoDAO.ForceById(deviceId).GetConfiguration();
    	}

		public byte[] GetFirmware(int deviceId)
		{
			var ss = DaoFactory.DispositivoDAO.ForceById(deviceId);
			return ss.Firmware.Binario;
    	}

		public INode Get(int deviceId, INode device)
		{
			try
			{
				if (deviceId == 0) return null;
				var dev1 = device.Get(deviceId);
				if (dev1 != null) return dev1;
				STrace.Debug(GetType().FullName, deviceId, String.Format("ForceById Get"));
				var dev = DaoFactory.DispositivoDAO.ForceById(deviceId);
				return dev == null ? null : device.FactoryShallowCopy(dev.Id, dev.IdNum, dev.Imei);
			}
			catch (Exception e)
			{
				STrace.Debug(GetType().FullName, deviceId, String.Format("No se encontro el dispositivo en la base; Error: {0}", e));
				return null;
			}
		}

		public INode FindByIdNum(int idNum, INode device)
        {
			var dev1 = device.FindByIdNum(idNum);
			if (dev1 != null) return dev1;

			var dev = DaoFactory.DispositivoDAO.GetByIdNum(idNum);
			return dev == null ? null : device.FactoryShallowCopy(dev.Id, dev.IdNum, dev.Imei);
        }

        public INode FindByIMEI(String imei, INode device)
        {
            var dev1 = device.FindByIMEI(imei);
            if (dev1 != null) return dev1;

            var dev = DaoFactory.DispositivoDAO.GetByIMEI(imei);
            return dev == null ? null : device.FactoryShallowCopy(dev.Id, dev.IdNum, dev.Imei);
        }


		public INode Find(String empresa, String linea, String patente, String interno, bool usarInternoFlag, INode device)
		{
			//imei de fantasia asi puedo buscar en el diccionario interno y no tengo que buscar en la base
			//el coche y dispositivo cada vez que llega un reporte.
			var imeiDummy = empresa + "+" + linea +"+" + patente + "+" + interno;

			//primero buscar en el diccionario interno
			var dev1 = device.FindByIMEI(imeiDummy);
			if (dev1 != null) return dev1;

			//buscar en la base el coche y su dispo
			var e1 = DaoFactory.EmpresaDAO.FindByCodigo(empresa);
			var emp = e1 == null ? null : new List<int> {e1.Id};
			var l1 = DaoFactory.LineaDAO.FindByCodigo(emp.First(), linea);
			var lin = l1 == null ? null : new List<int> {l1.Id};
			var d1 = usarInternoFlag
				? DaoFactory.CocheDAO.GetByInterno(emp, lin, interno) 
				: DaoFactory.CocheDAO.GetByPatente(emp, lin, patente);
			var dev = d1 == null ? null : d1.Dispositivo;
            return dev == null ? null : device.FactoryShallowCopy(dev.Id, dev.IdNum, imeiDummy);
		}

    	#endregion
    }
}