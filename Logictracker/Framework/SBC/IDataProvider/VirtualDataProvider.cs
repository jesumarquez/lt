#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Model.IAgent;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Messages;
using Logictracker.Utils;

#endregion

namespace Logictracker.Layers
{
	[FrameworkElement(XName = "VirtualDataProvider", IsContainer = false)]
	public class VirtualDataProvider : FrameworkElement, Model.IDataProvider, IService
	{
		#region Attributes

		[ElementAttribute(XName = "Parser", IsSmartProperty = true, IsRequired = true, DefaultValue = null)]
		public INode Parser
		{
			get { return (INode)GetValue("Parser"); }
			set { SetValue("Parser", value); }
		}

		#endregion

		#region IDataProvider

		public INode Get(int deviceId, INode parser)
		{
			lock (_devicesSync)
			{
				return _devices.ContainsKey(deviceId) ? _devices[deviceId] : null;
			}
		}

        public INode FindByIdNum(int idNum, INode device)
        {
            lock (_devicesSync)
            {
                return (from node in _devices where node.Value.IdNum.Equals(idNum) select node.Value).SafeFirstOrDefault();
            }
        }

		public INode FindByIMEI(String imei, INode device)
		{
			lock (_devicesSync)
			{
				return (from node in _devices where node.Value.Imei.Equals(imei) select node.Value).SafeFirstOrDefault();
			}
		}

        public List<Mensaje> GetResponsesMessagesTable(int deviceId, int revision) { return null; }

		public List<Mensaje> GetCannedMessagesTable(int deviceId, int revision) { return null; }
		public DetalleDispositivo GetDetalleDispositivo(int deviceId, String name) { return null; }
        public Dispositivo GetDispositivo(int deviceId) { return null; }
		public List<DetalleDispositivo> GetDetallesDispositivo(int deviceId) { return null; }
		public String GetConfiguration(int deviceId) { return null; }
		public void SetDetalleDispositivo(int deviceId, string name, string value, string type) { }
		public byte[] GetFirmware(int deviceId) { return null; }

		#endregion

		#region IService

		public bool ServiceStart()
		{
			return true;
		}

		public bool ServiceStop()
		{
			return true;
		}

		#endregion

		#region Public Members

		public INode Add(int devId, int idNum, String Imei)
		{
			var dev = Parser.FactoryShallowCopy(devId, idNum, Imei);
			if (_devices.ContainsKey(devId))
			{
				STrace.Debug(typeof(VirtualDataProvider).FullName, devId, "Se intento dar de alta un dispositivo que ya existe");
			}
			else
			{
				lock (_devicesSync)
				{
					_devices.Add(devId, dev);
				}
			}
			return dev;
		}

		public void Del(int DeviceId)
		{
			if (!_devices.ContainsKey(DeviceId))
			{
				STrace.Debug(typeof(VirtualDataProvider).FullName, DeviceId, "Se intento dar de baja un dispositivo que no existe");
			}
			else
			{
				lock (_devicesSync)
				{
					_devices.Remove(DeviceId);
				}
			}
		}

		#endregion

		#region Private Members
		
		private readonly Object _devicesSync = new Object();
		private readonly Dictionary<int, INode> _devices = new Dictionary<int, INode>();

		#endregion
	}
}