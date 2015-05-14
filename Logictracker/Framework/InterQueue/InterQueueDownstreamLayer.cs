#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
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
	public class InterQueueDownstreamLayer : IAsyncDispatcherLayer, ILoaderSettings
    {
        private readonly AutoResetEvent _syncro = new AutoResetEvent(true);
        private readonly Dictionary<ulong, IAsyncResult> _activeMessages = new Dictionary<ulong, IAsyncResult>();

        private PipelineChannel _outChannel; 
        private string _remoteUri = "";
        private string _localUri = "";
        private int _contextThreads = 5;

        public Gauge64 SentMessages { get; private set; }
        public Gauge64 SentBytes { get; private set; }
        public Gauge64 ReceivedBytes { get; private set; }

        public bool ServiceStart()
        {
            SentBytes = new Gauge64();
            SentMessages = new Gauge64();
            ReceivedBytes = new Gauge64();

            _outChannel = new PipelineChannel();
            _outChannel.Setup(_remoteUri, _localUri, _contextThreads);
            _outChannel.MessageReceived += OutChannelMessageReceived;
            _outChannel.Start();
            return true;
        }

        private bool OutChannelMessageReceived(byte[] data)
        {   
            try
            {
                ReceivedBytes.Inc((ulong)data.GetLength(0));
                var reply = (OpaqueMessageReply)GZip.DecompressAndDeserialize(data);
                if (reply == null) return false;
                _syncro.WaitOne();
                if (!_activeMessages.ContainsKey(reply.RepliedUniqueIdentifier))
                {
                    _syncro.Set();
                    return false;    
                }
                var result = (GenericAsyncResult)_activeMessages[reply.RepliedUniqueIdentifier];
                _syncro.Set();

                result.SetCompletedAsynchronously();
                if (result.Callback != null)
                {
                    result.Callback(result);
                }
                return true;
            }
            catch (Exception e)
            {
                STrace.Exception(GetType().FullName,e);
            }
            return false;
        }

        public bool ServiceStop()
        {
            _outChannel.Stop();
            return true;
        }

    	public LoadResults LoadSetting(XElement setting, object @object)
        {
			if (setting.Name.LocalName == "uri" && setting.Attr("id") == "bind-to")
            {
                _localUri = setting.Value;
                return LoadResults.LoadOk;
            }
			if (setting.Name.LocalName == "uri" && setting.Attr("id") == "connect-to")
            {
                _remoteUri = setting.Value;
                return LoadResults.LoadOk;
            }
			if (setting.Name.LocalName == "add" && setting.Attr("id") == "threads")
            {
                _contextThreads = Convert.ToInt32(setting.Value);
                return LoadResults.LoadOk;
            }
            return LoadResults.SettingUnknown;
        }

        public bool StackBind(ILayer bottom, ILayer top)
        {
            return true;
        }

        public IAsyncResult BeginDispatch(TimeSpan timeout, IMessage msg, object state, AsyncCallback callback)
        {
            var r = new GenericAsyncResult(state)
                        {
                            Callback = callback,
                            UniqueIdentifier = msg.UniqueIdentifier
                        };
            _syncro.WaitOne();
            _activeMessages.Add(msg.UniqueIdentifier, r);
            _syncro.Set();

            SentMessages.Inc(1);
            var obuff = GZip.SerializeAndCompress(msg);
            SentBytes.Inc((ulong)obuff.GetLength(0));
            _outChannel.Send(obuff);
            return r;
        }

        public BackwardReply EndDispatch(IAsyncResult result)
        {
            if (!(result is GenericAsyncResult)) return null;
            _syncro.WaitOne();
            if (_activeMessages.ContainsKey((result as GenericAsyncResult).UniqueIdentifier))
            {
                _activeMessages.Remove((result as GenericAsyncResult).UniqueIdentifier);
                _syncro.Set();
                return BackwardReply.None;
            }
            _syncro.Set();
            return BackwardReply.Release;
        }

		public String GetName() { return "NameNull"; }
		public String GetStaticKey() { return "KeyNull"; }
	}
}