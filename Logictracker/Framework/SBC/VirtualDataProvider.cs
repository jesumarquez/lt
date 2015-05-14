#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Urbetrack.Description;
using Urbetrack.Model;
using Urbetrack.Types.BusinessObjects.Dispositivos;
using Urbetrack.Types.BusinessObjects.Messages;

#endregion

namespace Urbetrack.SessionBorder
{
	[FrameworkElement(XName = "VirtualDataProvider", IsContainer = false)]
	public class VirtualDataProvider : FrameworkElement, IDataProvider
	{
		#region Attributes

		[ElementAttribute(XName = "Parser", IsSmartProperty = true, IsRequired = true, DefaultValue = null)]
		public ACParser Parser
		{
			get { return (ACParser)GetValue("Parser"); }
			set { SetValue("Parser", value); }
		}

		#endregion

		#region IDataProvider

		public INode Get(int DeviceId)
		{
			lock (_devicesSync)
			{
				return _devices.ContainsKey(DeviceId) ? _devices[DeviceId] : null;
			}
		}

		public INode Find(String imei)
		{
			lock (_devicesSync)
			{
				return (from node in _devices where node.Value.Identifier.Equals(imei) select node.Value).FirstOrDefault();
			}
		}

		public List<Mensaje> GetCannedMessagesTable(int DeviceId, int revision) { return null; }
		public DetalleDispositivo GetDetalleDispositivo(int DeviceId, String name) { return null; }
		public List<DetalleDispositivo> GetDetallesDispositivo(int DeviceId) { return null; }
		public String GetConfiguration(int DeviceId) { return null; }
		public Boolean SetDetalleDispositivo(int DeviceId, String name, String value, String type) { return true; }

		#endregion

		#region Public Members

		public ACParser Add(int DeviceId, String Imei)
		{
			var dev = Parser.Clone(DeviceId, Imei);
			lock (_devicesSync)
			{
				_devices.Add(DeviceId, dev);
			}
			return dev;
		}

		public void Del(int NodeCode)
		{
			lock (_devicesSync)
			{
				_devices.Remove(NodeCode);
			}
		}

		#endregion

		#region Private Members
		
		private readonly Object _devicesSync = new Object();
		private readonly Dictionary<int, INode> _devices = new Dictionary<int, INode>();

		#endregion
	}
}