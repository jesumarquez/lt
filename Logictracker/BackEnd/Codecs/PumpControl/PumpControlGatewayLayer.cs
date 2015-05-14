#region Usings

using System.Threading;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using PumpControl.Properties;

#endregion

namespace PumpControl
{
    /// <summary>
    /// Gateway layer for handling pump control messages.
    /// </summary>
    [FrameworkElement(XName = "PumpControlGatewayLayer", XNamespace = ConfigPumpControl.PumpControlNamespaceUri, IsContainer = false)]
    public class PumpControlGatewayLayer : FrameworkElement, ILayer
    {
		#region Attributes

		/// <summary>
        /// Data pooling interval.
        /// </summary>
        [ElementAttribute(XName = "PoolInterval", IsSmartProperty = true, IsRequired = true)]
        public int PoolInterval { get; set; }

        /// <summary>
        /// Pump control data container host name.
        /// </summary>
        [ElementAttribute(XName = "HostName", IsSmartProperty = true, IsRequired = true)]
        public string HostName { get; set; }

        /// <summary>
        /// Pump control data container listening port.
        /// </summary>
        [ElementAttribute(XName = "Port", IsSmartProperty = true, IsRequired = true)]
        public int Port { get; set; }

        #endregion

        #region Public Methods

		/// <summary>
        /// Starts pump control service layer.
        /// </summary>
        /// <returns></returns>
        public bool ServiceStart()
        {
            _keepAliveTicks = OfflineKeepAliveTicks;

            _worker = new Thread(WorkerProc);

            _running = true;

            return true;
        }

        /// <summary>
        /// Stops pump control service layer.
        /// </summary>
        /// <returns></returns>
        public bool ServiceStop()
        {
            _worker.Interrupt();
            _worker.Join();

            _running = false;

            return true;
        }

        public bool StackBind(ILayer bottom, ILayer top)
        {
			if (top is IDispatcherLayer)
			{
				_dispatcherLayer = top as IDispatcherLayer;
				return true;
			}
			STrace.Error(GetType().FullName, "Falta IDispatcherLayer!");
			return false;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Dispatchs the processed message to the application top layer.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="data"></param>
        private void PushUp(string label, string data)
        {
            var message = new UserMessage(0, 0);

			message.SetUserSetting("label", label);
			message.SetUserSetting("body", data);

            _dispatcherLayer.Dispatch(message);
        }

        /// <summary>
        /// Service worker thread main tasks.
        /// </summary>
        private void WorkerProc()
        {
            PushUp("SERVICE", "STARTED");

            while (_running)
            {
                var logsState = PumpProtocol.QuerySignle(HostName, Port, "@M1.0.0.0|L|2@F");

                if (ProcessDcState(logsState)) goto WorkSleep;

                if (logsState.Contains("@D")) PushUp("FROMLOG", logsState);

                var reply = PumpProtocol.QuerySignle(HostName, Port, "@M1.0.0.0|E|2@F");

                if (ProcessDcState(reply)) goto WorkSleep;

                PushUp("SAMPLE", reply);

                WorkSleep: Thread.Sleep(PoolInterval);
            }

            PushUp("SERVICE", "STOPED");
        }

        /// <summary>
        /// Updates pump control data container state based on ints response.
        /// </summary>
        /// <param name="reply"></param>
        /// <returns></returns>
        private bool ProcessDcState(string reply)
        {
            if (string.IsNullOrEmpty(reply))
            {
                if (_dcActivo)
                {
                    PushUp("DC", "OFFLINE");

                    _dcActivo = false;
                }
                else
                {
                    _keepAliveTicks--;

                    if (_keepAliveTicks == 0)
                    {
                        PushUp("SERVICE", "KEEPALIVE");

                        _keepAliveTicks = OfflineKeepAliveTicks;
                    }
                }

                return true;
            }

            if (_dcActivo) return false;

            PushUp("DC", "ONLINE");

            _dcActivo = true;

            _keepAliveTicks = OfflineKeepAliveTicks;

            return false;
        }

        /// <summary>
        /// Offline ticks for checking service status.
        /// </summary>
        private const int OfflineKeepAliveTicks = 10;

        /// <summary>
        /// Main worker thread.
        /// </summary>
        private Thread _worker;

        /// <summary>
        /// Determines if the main worker thread is running.
        /// </summary>
        private bool _running;

        /// <summary>
        /// Ticks for sending the keep alive message.
        /// </summary>
        private int _keepAliveTicks;

        /// <summary>
        /// Determines pump control's data container status.
        /// </summary>
        private bool _dcActivo;

        /// <summary>
        /// Layer for dispatching processed messages.
        /// </summary>
        private IDispatcherLayer _dispatcherLayer;

        #endregion
    }
}
