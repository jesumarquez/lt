#region Usings

using System;
using System.Collections.Generic;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Description;
using Urbetrack.Gateway.Joint.MessageQueue;
using Urbetrack.Model;
using Urbetrack.Types.BusinessObjects.Dispositivos;
using Urbetrack.Types.BusinessObjects.Messages;

#endregion

namespace Urbetrack.Torino
{
    [FrameworkElement(XName = "TorinoGatewayLayer", XNamespace = ConfigTorino.TorinoNamespaceUri, IsContainer = false)]
	public class TorinoGatewayLayer : FrameworkElement, IDataProvider, IDataTransportLayer
    {
        #region ILayer

        public bool StackBind(ILayer bottom, ILayer top)
        {
			if (top is IDispatcherLayer)
            {
				DispatcherLayer = top as IDispatcherLayer;
				return true;
            }
	        STrace.Error(GetType().FullName, "Falta IDispatcherLayer!");
            return false;
        }

        private IDispatcherLayer DispatcherLayer;

        #endregion

        #region IService

        public bool ServiceStart()
        {
            TorinoServer = new TorinoServer();
            TorinoServer.Start(DispatcherLayer);
            return true;
        }

		public bool ServiceStop()
        {
            TorinoServer.Stop();
            return true;
        }

        private TorinoServer TorinoServer;

        #endregion

	    #region IDataTransportLayer

	    public void DispatchMessage(INode device, IMessage message)
	    {
		    DispatcherLayer.Dispatch(message);
	    }

	    #endregion
	    
        #region IDataProvider

		public List<Mensaje> GetCannedMessagesTable(int DeviceId, int revision) { return Devices.I().GetCannedMessagesTable(DeviceId, revision); }
		public DetalleDispositivo GetDetalleDispositivo(int DeviceId, String name) { return Devices.I().GetDetalleDispositivo(DeviceId, name); }
		public List<DetalleDispositivo> GetDetallesDispositivo(int DeviceId) { return Devices.I().GetDetallesDispositivo(DeviceId); }
		public String GetConfiguration(int DeviceId) { return null; }
		public INode Get(int DeviceId, INode parser) { return Devices.I().Get(DeviceId, parser); }
		public INode Find(String imei, INode parser) { return Devices.I().Find(imei, parser); }
		public void SetDetalleDispositivo(int DeviceId, String name, String value, String type) { }
		public byte[] GetFirmware(int DeviceId) { return null; }

		#endregion
    }
}
