#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Urbetrack.Backbone;
using Urbetrack.Comm.Core.TransactionUsers;
using Urbetrack.Comm.Core.Transport;
using Urbetrack.Configuration;
using Urbetrack.Messaging;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Model;
using Urbetrack.MsmqMessaging;
using Urbetrack.Toolkit;
using Urbetrack.Torino;
using Urbetrack.Types.BusinessObjects.Dispositivos;
using Urbetrack.Types.BusinessObjects.Messages;

#endregion

namespace Urbetrack.Comm.Core.Fleet
{
	public class Devices : IDataProvider
	{
		private static readonly object SingletonLocker = new object();
		private static Devices _instance;

		public readonly Base64MessageQueue Cmd = new Base64MessageQueue();
		public static SpineClientWrap NetworkSpine;

		internal readonly List<Device> Consoles = new List<Device>();
		private readonly Dictionary<String, Device.Rider> Riders = new Dictionary<String, Device.Rider>();
		private readonly Dictionary<int, Device> DevicesDictionary = new Dictionary<int, Device>();
		private readonly Dictionary<String, Device> DevicesByImei = new Dictionary<String, Device>();
		private readonly Dictionary<String, Device> DevicesByXbeeaddr = new Dictionary<String, Device>();

		private readonly object _locker = new object();

		public readonly Qtree.Qtree QuadTree = new Qtree.Qtree();

		private bool _areUptodate;
		private bool _running;
		private Timer _ticker;

		private readonly Thread _updater;
		private readonly Thread _comanderq;
		private readonly Thread _qtreemgr;
		private readonly String _spineName;

		public static SpineClientWrap.States GetSpineConnectionState()
		{
			return NetworkSpine == null ? SpineClientWrap.States.DISCONNECTED : NetworkSpine.State;
		}

		private Devices()
		{
			try
			{
				_spineName = String.Format("{0}@{1}", Guid.NewGuid(), Environment.MachineName);
				NetworkSpine = new SpineClientWrap(Config.Torino.SpineUrl, _spineName);
				NetworkSpine.SpineCommand += Spine_SpineCommand;
				NetworkSpine.SpineStateChanged += Spine_SpineStateChanged;
				// por defecto actualizo la base de datos cada 30 minutos.
				RefreshFreqSeconds = Config.Torino.DBRefreshPeriod;
				DevicesQueuePrefix = Config.Torino.PrivateQueuePrefix;
				Cmd.Nombre = Config.Torino.CommanderMQ;
				_qtreemgr = new Thread(QTreeAndSpineProc);
				_qtreemgr.Start();
				_running = true;
				WaitForQtreeLoad();
				_updater = new Thread(UpdateDevicesProc);
				_updater.Start();
				_comanderq = new Thread(CommanderProc);
				_comanderq.Start();
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName,e);
			}
		}

		private bool _qtreeLoaded;

		public static String DevicesQueuePrefix;

		private void WaitForQtreeLoad()
		{
			while (!_qtreeLoaded)
			{
				Thread.Sleep(1000);
			}
		}

		private void QTreeAndSpineProc()
		{
			try
			{
				if (!Config.Torino.DisableQtree)
				{
					QuadTree.Initialize(QueueSerializer.GetQtreeApplicationFolder());
				}

				_qtreeLoaded = true;
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e, "QTreeProc");
			}
			try
			{
				NetworkSpine.MainLoop();
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e, "SpineProc");
			}
		}

		private int RefreshFreqSeconds { get; set; }

		private void Spine_SpineStateChanged(SpineStates oldState)
		{
			if (NetworkSpine.State != SpineClientWrap.States.CONNECTED) return;

			foreach (var d in DevicesDictionary.Values)
			{
				d.RemoteDeviceState.Refresh();
			}
		}

		private void Spine_SpineCommand(Commands sig, object data)
		{
			try
			{
				STrace.Debug(GetType().FullName, String.Format("SPINE/SEÑAL RECIBIDA: signal={0} data={1} ", sig, data));
				var dummy = new byte[2];
				switch (sig)
				{
					case Commands.TEMPORARILY_DISABLE_DEVICE:
						{
							var d = FindById((int)data);
							if (d == null)
							{
								STrace.Debug(GetType().FullName, String.Format("SPINE/SEÑAL RECIBIDA: no se encontro el dispositivo {0} para deshabilitarlo temporalmente.", (int)data));
								return;
							}
							d.DisabledUntil = DateTime.Now.AddHours(1);
							d.State = DeviceTypes.States.OFFLINE;
							d.CancelAllTasks();
							d.RemoteDeviceState.State = DeviceState.States.MAINT;
							break;
						}
					case Commands.GATEWAY_REFRESH:
						foreach (var d in DevicesDictionary.Values)
						{
							if (d.RemoteDeviceState != null)
							{
								d.RemoteDeviceState.Refresh();
							}
							else
							{
								STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: no se pudo refrescar.", d.LogId));
							}
						}
						break;
					case Commands.CONFIGURE_DEVICE:
						Cmd.Push("CONFIGURE:" + (int)data, dummy);
						break;
					case Commands.FOTA_DEVICE:
						Cmd.Push("FOTA:" + (int)data, dummy);
						break;
					case Commands.FULL_QTREE_DEVICE:
						Cmd.Push("FULLQTREE:" + (int)data, dummy);
						break;
					case Commands.QTREE_DEVICE:
						Cmd.Push("QTREE:" + (int)data, dummy);
						break;
					case Commands.REBOOT_DEVICE:
						Cmd.Push("REBOOT:" + (int)data, dummy);
						break;
					case Commands.HEARTBEAT:
						break;
				}
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName,e);
			}
		}

		private void CommanderProc()
		{
			try
			{
				while (_running)
				{
					var myData = Thread.GetNamedDataSlot("device");

					String commandText;

					try
					{
						commandText = Cmd.PeekLabel();
					}
					catch (ExceptionMessageQueueInvalid)
					{
						STrace.Debug(GetType().FullName,"COMMANDER: ExceptionMessageQueueInvalid");
						Thread.Sleep(1000);
						continue;
					}

					if (String.IsNullOrEmpty(commandText))
					{
						if (Cmd.HaveMessages)
						{
							STrace.Debug(GetType().FullName,"COMMANDER: String.IsNullOrEmpty(commandText)");
							Cmd.JustPop();
						}
						Thread.Sleep(250);
						continue;
					}

					commandText = Cmd.JustPop();

					STrace.Debug(GetType().FullName, String.Format("COMMANDER: CMD[{0}]", commandText));

					// parseo el comando
					var cmdParts = commandText.Split(":".ToCharArray());

					try
					{
						if (cmdParts.GetLength(0) < 2)
						{
							if (commandText.StartsWith("CLOSE"))
							{
								STrace.Debug(GetType().FullName,"COMMANDER: cerrando commander.");

								return;
							}

							continue;
						}

						var args = cmdParts[1].Split(";".ToCharArray());
						var argc = args.GetLength(0);

						//  resumo en 3 variables.
						var command = cmdParts[0];
						var pointCode = Convert.ToInt32(args[0]);
						var iPoint = FindById(pointCode);

						#region Comandos de IPowerBoot

						// Request Soft Reboot RRS:(NodeCode);
						if (command.StartsWith("RSR"))
						{
							iPoint.Reboot();
							continue;
						}

						#endregion Comandos de IPowerBoot

						#region Comandos de IWorkflow

						// Set Workflow State SWS:(NodeCode);(State);
						if (command.StartsWith("SWS"))
						{
							iPoint.AutoEvento(MessageIdentifier.UnsuportedCmd, 1);
							continue;
						}

						#endregion Comandos de IWorkflow

						#region Comandos de IShortMessage

						// Submit Canned Message SCM:(NodeCode);(Code);(ReCode1,ReCode2,ReCodeN);
						if (command.StartsWith("SCM") && argc == 4)
						{
							var code = Convert.ToInt16(args[1]);
							var responses = new List<int>();

							if (!String.IsNullOrEmpty(args[2])) responses.AddRange(args[2].Split(",".ToCharArray()).Select(i => Convert.ToInt32(i)));

							iPoint.SendMessage(code, "", 345345, args[2]);

							continue;
						}

						// Submit Text Message STM:(NodeCode);(Text);(ReCode1,ReCode2,ReCodeN);
						if (command.StartsWith("STM") && argc == 4)
						{
							var textMessage = args[1];
							iPoint.SendMessage(99, textMessage, 345345, args[2]);
							continue;
						}

						// Delete Canned Message DCM:(NodeCode);(Code);(Rev);
						if (command.StartsWith("DCM") && argc == 4)
						{
							iPoint.DeleteAllMessages();
							continue;
						}

						/*/// Update Canned Message
						/// UCM:(NodeCode);(Code);(Text);(Rev);
						if (Command.StartsWith("UCM") && argc == 5 && iPoint is IShortMessage)
						{
							var code = Convert.ToInt32(args[1]);
							var text_message = args[2];
							var rev = Convert.ToInt32(args[3]);
							(iPoint as IShortMessage).SetCannedMessage(code, text_message, rev);
							return 1;
						}

						/// Update Canned Response
						/// UCR:(NodeCode);(Code);(Text);(Rev);
						if (Command.StartsWith("UCR") && argc == 5 && iPoint is IShortMessage)
						{
							var code = Convert.ToInt32(args[1]);
							var text_message = args[2];
							var rev = Convert.ToInt32(args[3]);
							(iPoint as IShortMessage).SetCannedResponse(code, text_message, rev);
							return 1;
						}*/

						#endregion Comandos de IShortMessage

						#region Comandos de IProvisioned

						/*/// Update System Parameter
                        /// UCR:(NodeCode);(Parameter);(NewValue);(Rev);
                        if (Command.StartsWith("USP") && argc == 4 && iPoint is IProvisioned)
                        {
                            var parameter = args[1];
                            var value = args[2];
                            var rev = Convert.ToInt32(args[3]);
                            (iPoint as IProvisioned).SetParameter(parameter, value, rev);
                            return 1;
                        }*/

						#endregion Comandos de IProvisioned

						var alreadyUpdated = false;

						if (commandText.StartsWith("@"))
						{
							commandText = commandText.Substring(1);
							alreadyUpdated = true;
						}

						if (commandText.StartsWith("CONFIGURE:"))
						{
							var data = commandText.Split(":".ToCharArray());
							var devid = Convert.ToUInt16(data[1]);
							var d = FindById(devid);
							if (d == null)
							{
								STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: no se configura por que no esta en el mapa.", devid));
							}
							else
							{
								STrace.Debug(GetType().FullName, String.Format("COMMANDER: Configurando dispositivo a {0}", devid));
								if (!alreadyUpdated) d.UpdateFrom(DeviceUpdate(this, d));
								d.Configure();
							}
						}
						else if (commandText.StartsWith("SBLC:"))
						{
							var partes = commandText.Split(":".ToCharArray());
							var data = partes[1].Split(";".ToCharArray());
							var devid = Convert.ToUInt16(data[0]);
							if (data.GetLength(0) >= 2)
							{
								var action = Convert.ToByte(data[1][0]);
								var d = FindById(devid);
								STrace.Debug(GetType().FullName, String.Format("COMMANDER: Configuracion accion de bootloader DEVICE[{0}] action={1}", d.LogId, action));
								d.SetBootloaderAction(action);
							}
						}
						else if (commandText.StartsWith("MSG:"))
						{
							var partes = commandText.Split(":".ToCharArray());
							var data = partes[1].Split(";".ToCharArray());
							var devid = Convert.ToUInt16(data[0]);
							var code = Convert.ToInt16(data[1]);
							var texto = "";
							var sessionid = 0;
							var rfilter = "";

							// MSG:766;29;;7532;25,26
							if (data.GetLength(0) >= 3)
							{
								texto = data[2];
							}
							if (data.GetLength(0) >= 4)
							{
								sessionid = Convert.ToInt32(data[3]);
							}
							if (data.GetLength(0) >= 5)
							{
								rfilter = data[4];
							}
							var d = FindById(devid);
							STrace.Debug(GetType().FullName, String.Format("COMMANDER: Enviando mensaje DEVICE[{0}] code={1} text={2}", d.LogId, code, texto));
							d.SendMessage(code, texto, sessionid, rfilter);
						}
						else if (commandText.StartsWith("DCM:"))
						{
							var partes = commandText.Split(":".ToCharArray());
							var devid = Convert.ToUInt16(partes[1]);
							var d = FindById(devid);
							STrace.Debug(GetType().FullName, String.Format("COMMANDER: Borrando tabla de mensajes en DEVICE[{0}]", d.LogId));
							d.DeleteAllMessages();
						}
						else if (commandText.StartsWith("SETPS:"))
						{
							var data = commandText.Split(":".ToCharArray());
							var devid = Convert.ToUInt16(data[1]);
							var param = data[2];
							var value = data[3];
							var d = FindById(devid);
							STrace.Debug(GetType().FullName, String.Format("COMMANDER: Enviando parametro {0} a {1} valor={2}", param, devid, value));
							var p = new Device.Parameter
							{
								Nombre = param,
								Revision = d.GetServerParameterInt("known_config_revision", 0),
								Valor = value
							};
							d.SendParameter(p);
						}
						else if (commandText.StartsWith("FULLQTREE:"))
						{
							var data = commandText.Split(":".ToCharArray());
							var devid = Convert.ToUInt16(data[1]);
							var d = FindById(devid);
							STrace.Debug(GetType().FullName, String.Format("COMMANDER: Enviando QTree Completo a {0}", devid));
							if (!alreadyUpdated) d.UpdateFrom(DeviceUpdate(this, d));
							d.UpdateQtree(QuadTree, 0);
						}
						else if (commandText.StartsWith("QTREE:"))
						{
							var data = commandText.Split(":".ToCharArray());
							var devid = Convert.ToUInt16(data[1]);
							var d = FindById(devid);

                            if (d == null)
                            {
                                STrace.Debug(GetType().FullName, String.Format("Node not find: {0}", devid));
                            }
                            else
                            {
                                STrace.Debug(GetType().FullName, String.Format("COMMANDER: Enviando QTree a dispositivo id='{0}'", devid));

                                if (!alreadyUpdated) d.UpdateFrom(DeviceUpdate(this, d));

                                d.UpdateQtree(QuadTree);
                            }
						}
						else if (commandText.StartsWith("RSH:"))
						{
							var daio = commandText.Split(":".ToCharArray());
							var data = daio[1].Split(";".ToCharArray());
							if (data.GetLength(0) >= 2)
							{
								var devid = Convert.ToUInt16(data[0]);
								var commandLine = data[1];
								var d = FindById(devid);
								d.RemoteShell(commandLine);
							}
						}
						else if (commandText.StartsWith("REBOOT:"))
						{
							var data = commandText.Split(":".ToCharArray());
							var devid = Convert.ToUInt16(data[1]);
							var d = FindById(devid);
							d.Reboot();
						}
						else if (commandText.StartsWith("FOTA:"))
						{
							var data = commandText.Split(":".ToCharArray());
							var devid = Convert.ToUInt16(data[1]);
							var d = FindById(devid);
							if (!alreadyUpdated) d.UpdateFrom(DeviceUpdate(this, d));
							if (d.FirmwareId != 0)
							{
								var sut = d.ut as ServerTU;
								if (sut == null)
									throw new NullReferenceException("COMMANDER: El dispositivo id=" + devid +
																	 " no tiene usario de transaccion para FOTA.");
								if (FirmwareRequest != null)
								{
									var firmwareName = FirmwareRequest(this, d.FirmwareId);
									STrace.Debug(GetType().FullName, String.Format("COMMANDER: Iniciando fota para el dispositivo id='{0}' file='{1}'", devid, firmwareName));
									d.FlashOverTheAir(firmwareName);
								}
								else
								{
									STrace.Debug(GetType().FullName, String.Format("COMMANDER: NO ESTA INSTALADO EL HANDLER DE OBTENCION DE FIRMWARE"));
								}
							}
							else
							{
								STrace.Debug(GetType().FullName, String.Format("COMMANDER: El dispositivo {0} no tiene firmware asignado.", devid));
							}
						}
					}
					catch (Exception e)
					{
						STrace.Exception(GetType().FullName, e);
					}
					finally
					{
						Thread.SetData(myData, null);
					}
				}
			}
			catch (ThreadAbortException)
			{
			}
		}

		public void Close()
		{
			NetworkSpine.Running = false;
			_running = false;
			var dummy = new byte[2];
			Cmd.Push("CLOSE", dummy);
			if (!_comanderq.Join(10000))
			{
				_comanderq.Abort();
				STrace.Debug(GetType().FullName, "DEVICES: el hilo de comandos se detubo forzadamente.");
			}
			if (!_updater.Join(10000))
			{
				_updater.Abort();
				STrace.Debug(GetType().FullName, "DEVICES: el hilo de base de datos se detubo forzadamente.");
			}
			_ticker.Change(Timeout.Infinite, Timeout.Infinite);
			_ticker = null;
		}

		public Device DoDeviceUpdate(Device d)
		{
			return DeviceUpdate(this, d);
		}

		public IEnumerable<Device.Message> DoRetrieveMessages(Device d)
		{
			return RetrieveMessages(d);
		}

		public Dictionary<String, Device.Rider> DoUpdateRiders()
		{
			return RetrieveRiders();
		}

		DateTime _retrieveTimeout;
		private bool _updatingDevice;

		public void LockUpdateDevices()
		{
			//BUG: usar un semaforo!
			while (_updatingDevice) Thread.Sleep(100);

			_retrieveTimeout = DateTime.Now.AddDays(1);
		}

		public void ReleaseUpdateDevices()
		{
			_retrieveTimeout = DateTime.Now.AddSeconds(-1);
		}

		private void UpdateDevicesProc()
		{
			try
			{
				_retrieveTimeout = DateTime.Now;

				Thread.Sleep(1000);

				while (true)
				{
					var sleepRequired = true;

					if (_retrieveTimeout < DateTime.Now)
					{
						if (RetrieveRiders != null)
						{
							var retrieveRiders = RetrieveRiders();

							foreach (var r in retrieveRiders.Values)
							{
								if (!Riders.ContainsKey(r.Identifier))
								{
									Riders.Add(r.Identifier, r);
								}
								else
								{
									Riders[r.Identifier] = r;
								}
							}
						}
						if (RetrieveDevices != null)
						{
                            lock (_locker)
                            {
                                _updatingDevice = true;
                                var devicesSrc = RetrieveDevices();
                                // Agrego los que no estan y actualizo los existentes.
                                foreach (var d in devicesSrc.Values)
                                {
                                    STrace.Debug(GetType().FullName, String.Format("DEVICES: procesando {0}", d.Imei));
                                    if (!DevicesDictionary.ContainsKey(d.Id_short))
                                    {
                                        STrace.Debug(GetType().FullName, String.Format("DEVICES: Add {0}", d.Imei));
                                        Add(d);
                                    }
                                    else
                                    {
                                        STrace.Debug(GetType().FullName, String.Format("DEVICES: Update {0}", d.Imei));
                                        DevicesDictionary[d.Id_short].UpdateFrom(d);
                                    }
                                }
                                STrace.Debug(GetType().FullName, String.Format("DEVICES: Total de dispositivos conocidos: {0}", DevicesDictionary.Count));
                                _areUptodate = true;
                                _retrieveTimeout = DateTime.Now.AddSeconds(RefreshFreqSeconds);
                                _updatingDevice = false;
                            }
						}
						else
						{
							STrace.Debug(GetType().FullName, "DEVICES: no hay handler de actualizacion. Espero 5s.");
						}
					}
					else
					{
						lock (_locker)
						{
							var updatesDone = 5;
							// update phase.
							foreach (var d in DevicesDictionary.Values)
							{
								if (d.Expired())
								{
									d.KeepAlive();
									d.Touch();
								}
								else
								{
									if (d.Type == DeviceTypes.Types.UNKNOW_DEVICE ||
										d.Type == DeviceTypes.Types.NGN_DEVICE)
									{
										continue;
									}

									if (d.Type == DeviceTypes.Types.URBMOBILE_v0_1)
									{
										d.ProccessQueue();
									}

									if (d.UpdateRequired && DeviceUpdate != null)
									{
										DeviceUpdate(this, d);
										d.UpdateRequired = false;
										updatesDone--;
									}

									if (updatesDone == 0)
									{
										STrace.Debug(GetType().FullName,"DEVICES: reciclando por update.");
										sleepRequired = false;
										break;
									}
								}
							}
						}
					}
					if (!_running)
					{
						STrace.Debug(GetType().FullName,"DEVICE: terminado hilo de actualizador.");
					}
					Thread.Sleep(sleepRequired ? 1000 : 100);
				}
			}
			catch (ThreadAbortException)
			{
				// la tarea termino por la fuerza.
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e);
				Thread.Sleep(10000);
			}
		}

		private void Add(Device d)
		{
			if (DevicesDictionary.ContainsKey(d.Id_short)) return;

			// al agregarlo al mapa, creamos la envoltura.
			d.RemoteDeviceState = new DeviceStateWrap(d.Id_short);

			DevicesDictionary.Add(d.Id_short, d);

			var imei = d.Imei;
			DevicesByImei.Add(imei, d);
			var xbeeaddr = d.GetServerParameter("xbee_address", "unknown");
			if (DevicesByXbeeaddr.ContainsKey(xbeeaddr))
			{
				// cualquiera duplicado se elimina
				DevicesByXbeeaddr[xbeeaddr] = null;
			}
			else
			{
				DevicesByXbeeaddr.Add(xbeeaddr, d);
			}
		}

		internal void Remove(Device d)
		{
			if (!DevicesDictionary.ContainsKey(d.Id_short)) return;
			DevicesDictionary.Remove(d.Id_short);
			var imei = d.Imei;
			DevicesByImei.Remove(imei);

			var xbeeaddr = d.GetServerParameter("xbee_address", "unknown");
			if (xbeeaddr != "unknow")
			{
				DevicesByXbeeaddr.Remove(xbeeaddr);
			}
		}

		public Device FindById(int id)
		{
			if (id == 0) return null;

			lock (_locker)
			{
				if (DevicesDictionary.ContainsKey(id)) return DevicesDictionary[id];
			}

			STrace.Error(GetType().FullName, String.Format("Error: Dispositivo no encontrado: {0}", id));
			return null;
		}

		public Device FindByImei(String imei)
		{
			lock (_locker)
			{
				return DevicesByImei.ContainsKey(imei) ? DevicesByImei[imei] : null;
			}
		}

		public Device FindByXbeeAddr(String addr)
		{
			lock (_locker)
			{
				return DevicesByXbeeaddr.ContainsKey(addr) ? DevicesByXbeeaddr[addr] : null;
			}
		}

		public static Devices I() { lock (SingletonLocker) { return _instance ?? (_instance = new Devices()); } }

		public void UpdateDestino(int id, Destino d) { DevicesDictionary[id].Destino = d; }

		public void WaitForUpToDate() { while (!_areUptodate) { Thread.Sleep(500); } }

		public delegate Device DeviceUpdateHandler(object sender, Device d);
		public delegate String FirmwareRequestHandler(object sender, int deviceId);
		public delegate Dictionary<int, Device> RetrieveDevicesHandler();
		public delegate List<Device.Message> RetrieveMessagesHandler(Device d);
		public delegate Dictionary<String, Device.Rider> RetrieveRidersHandler();

		public event RetrieveDevicesHandler RetrieveDevices;
		public event DeviceUpdateHandler DeviceUpdate;
		public event FirmwareRequestHandler FirmwareRequest;
		public event RetrieveMessagesHandler RetrieveMessages;
		public event RetrieveRidersHandler RetrieveRiders;

		public Device.Rider GetRider(String identifier, int revision)
		{
			if (Riders.ContainsKey(identifier))
			{
				var rider = Riders[identifier];
				if (rider.Revision > revision) return rider;
			}
			return null;
		}

		#region IDataProvider

		public INode Get(int DeviceId, INode parser) { return FindById(DeviceId); }
		public INode Find(String imei, INode parser) { return FindByImei(imei); }
		public List<Mensaje> GetCannedMessagesTable(int DeviceId, int revision) { return null; }
		public DetalleDispositivo GetDetalleDispositivo(int DeviceId, String name) { return null; }
		public List<DetalleDispositivo> GetDetallesDispositivo(int DeviceId) { return null; }
		public void SetDetalleDispositivo(int DeviceId, String name, String value, String type) { }
		public String GetConfiguration(int DeviceId) { return null; }
		public byte[] GetFirmware(int DeviceId) { return null; }

		#endregion
	}
}