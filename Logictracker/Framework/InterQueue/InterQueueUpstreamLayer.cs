#region Usings

using System;
using System.Xml.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.InterQueue.OpaqueMessage;
using Logictracker.Model;
using Logictracker.Model.EnumTypes;
using Logictracker.Model.IAgent;
using Logictracker.Model.Utils;
using Logictracker.Statistics;
using Logictracker.ZeroMQ;
using Exception = System.Exception;

#endregion

namespace Logictracker.InterQueue
{
	public class InterQueueUpstreamLayer : ILayer, ILoaderSettings
    {
        private IDispatcherLayer _dispatcher;
        private PipelineChannel _outChannel;
        private string _remoteUri = "";
        private string _localUri = "";
        private int _contextThreads = 5;

        /* [StalkedProperty(Label = "Mensajes Recibidos",
              Description = "Contador historico de mensajes Recibidos.")]*/
        public Gauge64 ReceivedMessages { get; private set; }
        
        /* [StalkedProperty(Label = "Bytes Enviados",
              Description = "Contador historico de datos enviados.")]*/
        public Gauge64 SentBytes { get; private set; }

        /* [StalkedProperty(Label = "Bytes Recibidos",
            Description = "Contador historico de datos recibidos.")] */
        public Gauge64 ReceivedBytes { get; private set; }
        

        /// <summary>
        /// Se notifica al servicio, que debe iniciar su operacion ya que 
        /// todas las dependencias estan iniciadas.
        /// </summary>
        /// <returns>True si pudo iniciar</returns>
        public bool ServiceStart()
        {
            SentBytes = new Gauge64();
            ReceivedMessages = new Gauge64();
            ReceivedBytes = new Gauge64();

            _outChannel = new PipelineChannel();
            _outChannel.Setup(_remoteUri, _localUri, _contextThreads);
            _outChannel.MessageReceived += InChannelMessageReceived;
            _outChannel.Start();
            return true;
        }

        private bool InChannelMessageReceived(byte[] data)
        {
            try
            {
                ReceivedMessages.Inc(1);
                ReceivedBytes.Inc((ulong)data.GetLength(0));
                var message = (OpaqueMessage.OpaqueMessage)GZip.DecompressAndDeserialize(data);
                if (message == null) return false;
                _dispatcher.Dispatch(message);
                var outbuff = GZip.SerializeAndCompress(new OpaqueMessageReply(message));
                _outChannel.Send(outbuff);
                SentBytes.Inc((ulong)outbuff.GetLength(0));
                return true;
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e);
            }
            return false;
        }

        /// <summary>
        /// Se notifica al servicio que termine su operacion.
        /// </summary>
        /// <returns>True si pudo detener.</returns>
        public bool ServiceStop()
        {
            _outChannel.Stop();
            return true;
        }

    	/// <summary>
    	/// Configura un parametro al ILayer.
    	/// </summary>
    	/// <param name="xElement">Instancia de <c>Setting</c> con la informacion del parametro.</param>
    	/// <param name="object"></param>
        /// <returns>Retorna el resultado de la carga segun la enumeracion <c>Logictracker.LoadResults</c></returns>
    	public LoadResults LoadSetting(XElement xElement, object @object)
        {
			if (xElement.Name.LocalName == "uri" && xElement.Attr("id") == "bind-to")
            {
                _localUri = xElement.Value;
                return LoadResults.LoadOk;
            }
			if (xElement.Name.LocalName == "uri" && xElement.Attr("id") == "connect-to")
            {
                _remoteUri = xElement.Value;
                return LoadResults.LoadOk;
            }
			if (xElement.Name.LocalName == "add" && xElement.Attr("id") == "threads")
            {
                _contextThreads = Convert.ToInt32(xElement.Value);
                return LoadResults.LoadOk;
            }
            return LoadResults.SettingUnknown;
        }

        public bool StackBind(ILayer bottom, ILayer top)
        {
            if (top is IDispatcherLayer)
            {
                _dispatcher = top as IDispatcherLayer;
				return true;
            }
			STrace.Error(GetType().FullName, "Falta IDispatcherLayer!");
			return false;
        }

		public String GetName() { return "NameNull"; }
		public String GetStaticKey() { return "KeyNull"; }
	}
}
