using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Logictracker.DAL.Factories;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.IAgent;
using Logictracker.Utils;

namespace Logictracker.Interfaces.AccessControl
{
	[FrameworkElement(XName = "AccessInterface", IsContainer = true)]
	public class AccessInterface : FrameworkElement, IService
	{
		//para administrar la puerta se entra en http://192.168.10.66/ user y password "admin"

		#region Attributes

		[ElementAttribute(XName = "Dispatcher", IsSmartProperty = true, IsRequired = true)]
		public IDispatcherLayer Dispatcher
        {
            get { return (IDispatcherLayer)GetValue("Dispatcher"); }
			set { SetValue("Dispatcher", value); }
        }

		[ElementAttribute(XName = "Port", DefaultValue = 3040)]
		public int Port { get; set; }

		#endregion

        #region Public Methods

        public bool ServiceStart()
        {
            try
            {
                _worker = new Thread(DoWork);
                _worker.Start();
                return true;
            }
            catch (Exception e)
            {
				STrace.Exception(GetType().FullName, e, "Exception during startup");
                return false;
            }
        }

        public bool ServiceStop()
        {
            try
            {
				if (_conection != null) _conection.Close();
				if (_socket != null)
				{
					try
					{
						_socket.Close();
					}
					catch {}
				}
				_worker.Interrupt();
				_worker.Abort();
				return true;
            }
            catch (Exception e)
            {
				STrace.Exception(GetType().FullName, e, "Exception during stop");
                return false;
            }
        }

        #endregion

        #region Private Members

		private void DoWork()
		{
			var daoFactory = new DAOFactory();
			var buffer = new Byte[420];
			Connect();

			var receive = true;
			while (receive)
			{
				try
				{
					_socket.ReceiveFrom(buffer, 0, 419, SocketFlags.None, ref _remoteEndPoint);
					if (buffer[9] != 0x51) continue;

					var mac = BitConverter.ToString(buffer, 10, 6).Replace("-", String.Empty);
					var dev = daoFactory.DispositivoDAO.GetByIMEI(mac);
					if (dev == null)
					{
						STrace.Error(GetType().FullName, String.Format("Access no esta dado de alta, dar de alta un Dispositivo con el Imei: {0}", mac));
						continue;
					}

					var rfid = BitConverter.ToString(buffer, 31, 4).Replace("-", String.Empty);
					var entrada = StringUtils.AreBitsSet(buffer[27], 0x01);
					var dt = new DateTime(buffer[26] + (DateTime.UtcNow.Year/100)*100, buffer[25], buffer[24], buffer[23], buffer[22], buffer[21]);
					var msg = MessageIdentifierX.FactoryRfid(dev.Id, 0, null, dt, rfid, entrada ? 3 : 4);
					if (msg == null)
					{
						STrace.Debug(GetType().FullName, dev.Id, String.Format("Se descarta: DateTime={0} UserId={1} EsEntrada={2}", dt, rfid, entrada));
						continue;
					}

					STrace.Debug(GetType().FullName, String.Format("Llego: DateTime={0} UserId={1} EsEntrada={2}", dt, rfid, entrada));
					Dispatcher.Dispatch(msg);
				}
				catch (ThreadAbortException)
				{
					receive = false;
				}
				catch (Exception e)
				{
					STrace.Exception(GetType().FullName, e, "Stopping Access Main Loop due to exception");
				}
			}
		}

		private void Connect()
		{
			_remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
			_conection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_conection.Bind(new IPEndPoint(IPAddress.Any, Port));
			_conection.Listen(1);
			_socket = _conection.Accept();
		}

		private Thread _worker;
        private Socket _conection;
        private Socket _socket;
        private EndPoint _remoteEndPoint;

        #endregion
    }
}
