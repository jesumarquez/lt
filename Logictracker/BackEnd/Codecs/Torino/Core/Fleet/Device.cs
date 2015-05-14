#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using Urbetrack.Backbone;
using Urbetrack.Cache;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Qtree;
using Urbetrack.Comm.Core.Scheduler;
using Urbetrack.Comm.Core.Transaction;
using Urbetrack.Comm.Core.TransactionUsers;
using Urbetrack.Comm.Core.Transport;
using Urbetrack.Comm.Core.Transport.FileTransfer;
using Urbetrack.Comm.Core.Transport.XBeeRLP;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Hacking;
using Urbetrack.Messaging;
using Urbetrack.Model;
using Urbetrack.MsmqMessaging;
using Urbetrack.Toolkit;

#endregion

namespace Urbetrack.Torino
{
	[Serializable]
	public class Device : INode, IPowerBoot, IQuadtree, IShortMessage, IProvisioned
	{
		#region Device

        public DateTime LastPacketReceivedAt
        {
            get
            {
                var dt = UrbeCache.Retrieve<string>("device_" + Id + "_lastPacketReceivedAt");
                return DateTime.Parse(dt);
            }
            set
            {
                if (Id > 0) UrbeCache.Store("device_" + Id + "_lastPacketReceivedAt", value.ToString());
            }
        }


		public const int DefaultFlushTimeout = 60;

		public const int DefaultMaxPDUSamples = 6;
		public const int DefaultXbeeMaxPDUSamples = 4;

		public const int DefaultRetrieveFlags = 1;
		public const int DefaultXbeeRetrieveFlags = 1;

		[NonSerialized] internal readonly Queue<PDU> fota_q = new Queue<PDU>();
		[NonSerialized] private readonly Queue<Message> messages_q = new Queue<Message>();
		[NonSerialized] private readonly Queue<Parameter> settings_q = new Queue<Parameter>();
		private int autoevento_ultima_data;
		private short autoevento_ultimo_evento;
		private Destino destino;
		private bool enable_keepalive_pooling;
		public int ExpirationTime = 300;
		private int fota_pending;
		private short _idShort; //!< ID de dispositivo
		private bool keep_alive_in_progress;
		private MRC keep_alive_mrc;
		private int keep_alive_timer_id = -1;
		public Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
		[NonSerialized] private Queue<byte[]> qtree_q;
		private DeviceTypes.QTreeStates qtree_state;

		private short qtreePagina;
		private Qtree quad_tree;
		[NonSerialized] private Base64MessageQueue queue;
		public DeviceStateWrap RemoteDeviceState;
		private bool sending_files;
		private DeviceTypes.States state;
		[NonSerialized] public Transporte Transporte;
		private DeviceTypes.Types type;

		private DateTime ultima_actividad;
		[NonSerialized] internal readonly TransactionUser ut;

		//Agego el constructor vacio ya que es necesario para el UpdateNodesTree
		public Device()
		{
		}

		public Device(TransactionUser _ut)
		{
			sending_files = false;
			ut = _ut;
			state = DeviceTypes.States.UNLOADED;
			XBeeSession = new XBeeSession(this);
			QTreeState = DeviceTypes.QTreeStates.UNKNOWN;
		}

		public DeviceTypes.Types Type
		{
			get { return type; }
			set
			{
				if (value == type) return;
				type = value;

				RemoteDeviceState.Type = value;
			}
		}

		public short Id_short
		{
			get { return _idShort; }
			set
			{
				var newid = value;
				_idShort = newid;
			}
		}

		public string Imei { get; set; }

		public int FirmwareId { get; set; }

		public string FirmaRequerida { get; set; }

		public readonly XBeeSession XBeeSession;
		public bool HackBugXBEEv99;

		public bool StorageMediaFailure { get; set; }

		public string Vehicle { get; set; }

		public string Base { get; set; }

		public Uri BuildUri(string UserInfo)
		{
			var riderid = string.IsNullOrEmpty(UserInfo) ? "" : UserInfo + "@";
			return new Uri(String.Format("utn.device://{0}{1}/?did={2}&transport={3}", riderid, Destino.GetAddress(), Id_short, Destino.GetTransport()));
		}

		public bool SupportsChecksum
		{
			get { return Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.URBETRACK_v0_8; }
		}

		public bool SupportsGPSPointEx
		{
			get
			{
				return (Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.URB_v0_7 || Type == DeviceTypes.Types.URBETRACK_v0_8n ||
				        Type == DeviceTypes.Types.URBETRACK_v0_8 || Type == DeviceTypes.Types.GTE_TRAX_S6SG_v1_0);
			}
		}

		private bool SupportsQuadTree
		{
			get { return Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.URBETRACK_v0_8n || Type == DeviceTypes.Types.URBETRACK_v0_8; }
		}

		public byte LoginAcceptedQueryType
		{
			get
			{
				switch (Type)
				{
					case DeviceTypes.Types.URBETRACK_v1_0:
						return 0x01;
					case DeviceTypes.Types.URBETRACK_v0_8:
					case DeviceTypes.Types.URBETRACK_v0_8n:
						return 0x01;
					default:
						return 0x00;
				}
			}
		}

		private bool SupportsParameters
		{
			get
			{
				return Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.URBETRACK_v0_8n || Type == DeviceTypes.Types.UNETEL_v2 ||
				       Type == DeviceTypes.Types.URBETRACK_v0_8;
			}
		}

		public bool SupportsMessages
		{
			get { return Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.UNETEL_v2; }
		}

		public bool SupportsRemoteShell
		{
			get { return Type == DeviceTypes.Types.URBETRACK_v1_0; }
		}

		private bool SupportsKeepAlive
		{
			get { return Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.URBETRACK_v0_8n || Type == DeviceTypes.Types.URBETRACK_v0_8; }
		}

		public bool SupportsFota
		{
			get
			{
				return Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.URB_v0_5
				       || Type == DeviceTypes.Types.URB_v0_7 || Type == DeviceTypes.Types.URBETRACK_v0_8n ||
				       Type == DeviceTypes.Types.URBETRACK_v0_8;
			}
		}

		public int DataPageWindowSize
		{
			get
			{
				switch (Type)
				{
					case DeviceTypes.Types.URB_v0_5:
						return Hacker.Device.Torino05_WindowSize;
					case DeviceTypes.Types.URBETRACK_v0_8n:
					case DeviceTypes.Types.URBETRACK_v0_8:
					case DeviceTypes.Types.URBETRACK_v1_0:
					case DeviceTypes.Types.URB_v0_7:
						return Hacker.Device.Torino10_WindowSize;
				}
				return 0;
			}
		}

		private Base64MessageQueue Queue
		{
			get
			{
				if (string.IsNullOrEmpty(Imei))
					throw new ApplicationException("No estaba definido el DeviceIdentifier cuando se intentaba construir la cola privada.");

				return queue ?? (queue = new Base64MessageQueue
					{
						Nombre = String.Format("{0}{1}",
						                       Devices.DevicesQueuePrefix,
						                       Imei)
					});
			}
		}

		public DeviceTypes.States State
		{
			get { return state; }

			set
			{
				var newstate = value;
				if (newstate == state) return;
				if (newstate == DeviceTypes.States.OFFLINE)
				{
					keep_alive_mrc = null;
					keep_alive_in_progress = false;
					EnableKeepAlivePooling = false;
					StorageMediaFailure = false;
					if (QTreeState == DeviceTypes.QTreeStates.TRANSFERING)
						QTreeState = DeviceTypes.QTreeStates.REQUIRES_UPDATE;
					if (RemoteDeviceState.State != DeviceState.States.SHUTDOWN)
						RemoteDeviceState.State = DeviceState.States.OFFLINE;
					AutoEvento(MessageIdentifier.DeviceOffLine_internal);
				}
				if (newstate == DeviceTypes.States.ONLINE)
				{
					EnableKeepAlivePooling = true;
					//RemoteDeviceState.QueryState = RemoteDeviceState.ServiceStates.ONLINE;
					AutoEvento(MessageIdentifier.DeviceOnLine_internal);
				}
				state = newstate;
			}
		}

		public Destino Destino
		{
			get { return destino ?? (destino = new Destino()); }
			set
			{
				if (destino == null) destino = new Destino();
				destino.TCP = value.TCP;
				destino.UDP = value.UDP;
				destino.XBee = value.XBee;
			}
		}

		public bool SupportsFixAtLogin
		{
			get { return (Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.URBETRACK_v0_8); }
		}

		public bool SupportExtendedDataPage
		{
			get
			{
				return (Type == DeviceTypes.Types.URBETRACK_v1_0 || Type == DeviceTypes.Types.URB_v0_7 || Type == DeviceTypes.Types.URBETRACK_v0_8n ||
				        Type == DeviceTypes.Types.URBETRACK_v0_8);
			}
		}

		public bool UpdateRequired { get; set; }

		private DeviceTypes.QTreeStates QTreeState
		{
			get { return qtree_state; }
			set
			{
				var new_value = value;
				if (qtree_state == new_value) return;
				switch (new_value)
				{
					case DeviceTypes.QTreeStates.REQUIRES_UPDATE:
						if (qtree_state == DeviceTypes.QTreeStates.TRANSFERING)
						{
							AutoEvento(MessageIdentifier.QtreePause); // QTREE START //pause
						}
						break;
					case DeviceTypes.QTreeStates.TRANSFERING:
						AutoEvento(MessageIdentifier.QtreeStart); // QTREE START
						break;
					case DeviceTypes.QTreeStates.UPTODATE:
						AutoEvento(MessageIdentifier.QtreeSuccess); // QTREE START
						break;
				}
				RemoteDeviceState.QTreeState = new_value;
				qtree_state = new_value;
			}
		}

		public short FotaCanceledAtPage { get; internal set; }

		public short TotalDePaginasFota { get; set; }

		private bool EnableKeepAlivePooling
		{
			set
			{
				if (!SupportsKeepAlive) return;
				enable_keepalive_pooling = value;
				if (enable_keepalive_pooling)
				{
					if (keep_alive_timer_id != -1) Scheduler.DelTimer(keep_alive_timer_id);
					var kai = GetServerParameterInt("shutdown_keepalive_interval", Hacker.Device.KeepAliveInterval);
					STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: programando keepalive dentro de {1}ms", LogId, kai));
					keep_alive_timer_id = Scheduler.AddTimer(RunKeepAlive, kai);
				}
				else
				{
					if (keep_alive_timer_id != -1)
					{
						Scheduler.DelTimer(keep_alive_timer_id);
						keep_alive_timer_id = -1;
					}
				}
			}
		}

		public string LegacyCode { get; set; }

		public string LogId
		{
			get { return String.Format("'{0}'/{1}", LegacyCode, Id_short); }
		}

		public DateTime DisabledUntil { get; set; }

		public void ProccessQueue()
		{
			if (State != DeviceTypes.States.ONLINE ||
			    Type != DeviceTypes.Types.URBMOBILE_v0_1) return;
			if (sending_files) return;
			var label = "";
			byte[] msg = null;
			try
			{
				msg = Queue.Peek(ref label);
			}
			catch (ExceptionMessageQueueInvalid e)
			{
				STrace.Exception(GetType().FullName, e);
			}

			if (msg == null) return;

			var fsep = new IPEndPoint(Destino.UDP.Address, Destino.UDP.Port + 1);
			if (FileClient.BlockDataTransferDeviceServerPort != 0)
			{
				fsep.Port = FileClient.BlockDataTransferDeviceServerPort;
			}
			STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FILECLIENT: enviando mensaje a ep='{1}'", LogId, fsep));
			var fc = new FileClient(fsep, this, 0x01, msg, label);
			fc.SendMessageResult += SendMessageResultHandler;
			sending_files = true;
		}

		public bool HackVersion99()
		{
			return Parameters.ContainsKey("known_firmware_signature") &&
			       Parameters["known_firmware_signature"].Valor == "RNR-OS-v0.80(99)Torino";
		}

		public int GetServerParameterInt(string param, int por_defecto)
		{
			try
			{
				if (Parameters.ContainsKey(param))
				{
					return Convert.ToInt32(Parameters[param].Valor);
				}
				SetServerParameter(param, por_defecto);
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e);
			}
			return por_defecto;
		}

		public string GetServerParameter(string param, string por_defecto)
		{
			try
			{
				if (Parameters.ContainsKey(param))
				{
					return Parameters[param].Valor;
				}
				SetServerParameter(param, por_defecto);
			}
			catch (Exception e)
			{
				STrace.Exception(GetType().FullName, e);
			}
			// cualquier excepcion sale por defecto.
			return por_defecto;
		}

		public void SetServerParameter(string param, int value)
		{
			SetParameter(param, value.ToString(CultureInfo.InvariantCulture), 'S', "int", "0");
		}

		public void SetServerParameter(string param, string value)
		{
			SetParameter(param, value, 'S', "varchar(96)", "(unset)");
		}

		public void SetInfoParameter(string param, int value)
		{
			SetParameter(param, value.ToString(CultureInfo.InvariantCulture), 'I', "int", "0");
		}

		public void SetInfoParameter(string param, string value)
		{
			SetParameter(param, value, 'I', "varchar(96)", "(unset)");
		}

		private void SetParameter(string param, string value, char consumidor, string tipo, string valor_inicial)
		{
			if (string.IsNullOrEmpty(value))
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/SET_PARAMETER: parametro='{1}' es nulo o esta vacio.", LogId, param));
				return;
			}
			if (string.IsNullOrEmpty(valor_inicial))
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/SET_PARAMETER: parametro='{1}' valor inicial no asignando.", LogId, param));
				valor_inicial = "0";
			}
			if (!Parameters.ContainsKey(param))
			{
				var p = new Parameter
					{
						Consumidor = consumidor,
						TipoDato = tipo,
						Nombre = param,
						Valor = value,
						Revision = 0,
						UpdateRequired = true,
						ValorInicial = valor_inicial
					};
				Parameters.Add(param, p);
				UpdateRequired = true;
			}
			else
			{
				if (Parameters[param].Valor != value)
				{
					Parameters[param].Valor = value;
					Parameters[param].UpdateRequired = true;
					UpdateRequired = true;
				}
			}
		}

		public void SendMessage(short code, string text, int session_id, string rfilter)
		{
			var pdu = new ShortMessage
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					CL = (byte) (code == 99 ? 0x05 : 0x04),
					Code = code,
					Session = session_id,
					ReplyFilter = rfilter,
					Message = text
				};
			var mrc = new MRC(pdu, pdu.Transporte, ut);
			pdu.Transporte.NuevaTransaccion(mrc, pdu);
			mrc.Seq = pdu.Seq;
			mrc.Start();
		}

		private void SendMessageProgress(byte progress, short code, int session_id)
		{
			var pdu = new ShortMessage
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					CL = progress,
					Code = code,
					Session = session_id
				};
			var mrc = new MRC(pdu, pdu.Transporte, ut);
			pdu.Transporte.NuevaTransaccion(mrc, pdu);
			mrc.Seq = pdu.Seq;
			mrc.Start();
		}

		public void SetLogisticState(short logstate)
		{
			var pdu = new ShortMessage
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					CL = 0x03,
					Code = logstate
				};
			var mrc = new MRC(pdu, pdu.Transporte, ut);
			pdu.Transporte.NuevaTransaccion(mrc, pdu);
			mrc.Seq = pdu.Seq;
			mrc.Start();
		}

		public void DeleteAllMessages()
		{
			var pdu = new ShortMessage
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					CL = 0x02
				};
			var mrc = new MRC(pdu, pdu.Transporte, ut);
			pdu.Transporte.NuevaTransaccion(mrc, pdu);
			mrc.Seq = pdu.Seq;
			mrc.Start();
		}

		public void Configure()
		{
			// borro todos los parametros pendientes de enviarse.
			settings_q.Clear();
			messages_q.Clear();
			// actualizo los datos desde la base.
			UpdateFrom(Devices.I().DoDeviceUpdate(this));
			// primero verifico si corresponde actualizar el firmware.
			if (Parameters.ContainsKey("update_firmware") && Parameters["update_firmware"].IsEnabled())
			{
				if (GetServerParameter("known_firmware_signature", "unknow") != FirmaRequerida)
				{
					Devices.I().Cmd.Push(String.Format("@FOTA:{0}", Id_short), new byte[2]);
					return;
				}
			}

			// busco los parametros del server

			if (SupportsParameters)
			{
				var device_params = from par in Parameters.Values
				                    where par.Consumidor == 'D'
				                    orderby par.Revision
				                    select par;
				var known_config_revision = GetServerParameterInt("known_config_revision", 0);
				var max_server_revision = 0;
				foreach (var p in device_params)
				{
					// seleccion de parametros con revision > a la del dispo.
					max_server_revision = Math.Max(max_server_revision, p.Revision);
					if (p.Revision <= known_config_revision) continue;
					STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: seteando parametro {1} ({2};{3})", Id_short, p.Nombre, p.Revision, known_config_revision));
					settings_q.Enqueue(p);
				}

				if (max_server_revision < known_config_revision)
				{
					if (Hacker.Device.FixMissmatchConfiguration)
					{
						STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: RECONFIGURANDO parametros desincronizados. server={1} device={2}", Id_short, max_server_revision, known_config_revision));
						settings_q.Clear();
						foreach (var p in device_params)
						{
							settings_q.Enqueue(p);
						}
					}
					else
					{
						STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: parametros desincronizados. server={1} device={2}", Id_short, max_server_revision, known_config_revision));
					}
				}
			}

			if (SupportsMessages)
			{
				var messages = Devices.I().DoRetrieveMessages(this);
				var known_messages_revision = GetServerParameterInt("known_messages_revision", 0);
				var device_msg = from msg in messages
				                 where msg.Revision > known_messages_revision
				                 orderby msg.Revision
				                 select msg;
				foreach (var p in device_msg)
				{
					STrace.Debug(GetType().FullName, String.Format("DEVEICE[{0}]: modificacion sobre mensaje code={1} rev={2} delete={4} text={3}", Id_short, p.Code, p.Revision, p.Text, p.Deleted));
					messages_q.Enqueue(p);
				}
			}

			var disable_qtree = false;
			if (settings_q.Count != 0)
			{
				RunParametersQueue();
				disable_qtree = true;
			}
			if (messages_q.Count != 0)
			{
				RunMessagesQueue();
				disable_qtree = true;
			}

			if (disable_qtree) return;

			// por ultimo, verifico si corresponde actualizar el qtree.
			if (!Parameters.ContainsKey("update_qtree") || !Parameters["update_qtree"].IsEnabled()) return;
			Devices.I().Cmd.Push(String.Format("@QTREE:{0}", Id_short), new byte[2]);
		}

		private void RunMessagesQueue()
		{
			if (!SupportsMessages)
			{
				messages_q.Clear();
				return;
			}
			if (messages_q.Count == 0)
				return;
			var m = messages_q.Peek();
			var pdu = new ShortMessage
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					CL = (byte) (m.Deleted ? 0x01 : 0x00),
					Code = m.Code,
					Revision = m.Revision,
					Message = m.Text,
					Source = (byte) m.Source
				};
			var mrc = new MRC(pdu, pdu.Transporte, ut);
			pdu.Transporte.NuevaTransaccion(mrc, pdu);
			mrc.Seq = pdu.Seq;
			mrc.Start();
		}

		private void RunParametersQueue()
		{
			if (!SupportsParameters)
			{
				settings_q.Clear();
				return;
			}
			var p = settings_q.Peek();
			var pdu = new SetParameter
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					Parameter = p.Nombre,
					Revision = p.Revision,
					Value = p.Valor
				};
			var mrc = new MRC(pdu, pdu.Transporte, ut);
			pdu.Transporte.NuevaTransaccion(mrc, pdu);
			mrc.Seq = pdu.Seq;
			mrc.Start();
		}

		private void SendMessageResultHandler(int idDispositivo, string filename, bool status)
		{
			if (status)
			{
				var label = "";
				Queue.Pop(ref label);
			}
			else
			{
				STrace.Debug(GetType().FullName, idDispositivo, String.Format("DEVICE[{0}]/FILECLIENT: un mensaje no se envio.", LogId));
				State = DeviceTypes.States.OFFLINE;
			}
			sending_files = false;
		}

		public bool Expired()
		{
			if (state == DeviceTypes.States.OFFLINE ||
			    state == DeviceTypes.States.UNLOADED) return false;
			var expire_lapse = GetServerParameterInt("device_expire_lapse", 180);
			// expire 0 significa que nunca expira.
			if (expire_lapse == 0) return false;
			var diferencia = DateTime.Now - ultima_actividad;
			return diferencia.TotalSeconds > expire_lapse;
		}

		public void Touch()
		{
			ultima_actividad = DateTime.Now;
		}

		public void Touch(Destino d)
		{
			ultima_actividad = DateTime.Now;
			Destino = d;
			Devices.I().UpdateDestino(Id_short, d);
		}

		public override string ToString()
		{
			var expire_lapse = GetServerParameterInt("device_expire_lapse", 600);
			return
				string.Format(
					"DEVICE id={0} state={1} Type={2} imei={3} addr={4} active={5}",
					_idShort, State, Type, Imei, destino,
					expire_lapse - (DateTime.Now - ultima_actividad).TotalSeconds);
		}

		public void Trace()
		{
			STrace.Debug(GetType().FullName, ToString());
		}

		public void KeepAlive()
		{
			if (keep_alive_in_progress || !SupportsKeepAlive) return;
			keep_alive_in_progress = true;
			STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: solicitando keep alive.", LogId));
			keep_alive_mrc = SendCommand(Codes.HighCommand.KeepAlive);
		}

		private MRC SendCommand(Codes.HighCommand CH)
		{
			return SendCommand(CH, 0);
		}

		private MRC SendCommand(Codes.HighCommand CH, byte CL)
		{
			var p = new PDU
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					CH = (byte) CH,
					CL = CL
				};
			var mrc = new MRC(p, Transporte, ut);
			p.Transporte.NuevaTransaccion(mrc, p);
			mrc.Seq = p.Seq;
			mrc.Start();
			return mrc;
		}

		public bool Reboot()
		{
			RemoteDeviceState.State = DeviceState.States.SHUTDOWN;
			SendCommand(Codes.HighCommand.Reboot);
			return true;
		}

		private void KeepAliveV1(PDU response)
		{
			STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/KEEPALIVE-V1: respuesta recibida CL={1}", LogId, response.CL));
			switch (response.CL)
			{
				case 0x5d:
					AutoEvento(MessageIdentifier.HardwareChange_internal, Evento.HardwareChanges.EVTDATA_SDCI_FAULT);
					// SD FAULT
					RemoteDeviceState.StorageMediaFailure = true;
					break;
				case 0x20:
					STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Shutdown demorado por Base64MessageQueue de posiciones.", LogId));
					AutoEvento(MessageIdentifier.ShutdownDelayed, 0x20);
					break;
				case 0x21:
					STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Shutdown demorado por MessageQueueFarming de Eventos.", LogId));
					AutoEvento(MessageIdentifier.ShutdownDelayed, 0x21);
					break;
				case 0x01:
					if (qtree_q == null && (fota_q == null || fota_q.Count == 0))
					{
						STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Apagando dispositivo.", LogId));
						Reboot();
					}
					else STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Shutdown demorado por FOTA o QTREE.", LogId));
					break;
				case 0x00:
					{
						RemoteDeviceState.State = State == DeviceTypes.States.ONLINE
							                          ? DeviceState.States.ONLINE
							                          : DeviceState.States.ONNET;
						return;
					}
			}
		}

		private void KeepAliveV2(PDU response)
		{
			if ((response.CL & 0x60) == 0x60)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Apagando dispositivo, con SD rota.", LogId));
				Reboot();
			}
			else if (((response.CL & 0x48) == 0x40) &&
			         (qtree_q == null) && (fota_q == null || fota_q.Count == 0))
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Apagando dispositivo.", LogId));
				Reboot();
			}

			if ((response.CL & 0x20) == 0x20)
			{
				RemoteDeviceState.StorageMediaFailure = true;
				AutoEvento(MessageIdentifier.HardwareChange_internal, Evento.HardwareChanges.EVTDATA_SDCI_FAULT);
				// SD FAULT
			}

			if ((response.CL & 0x10) == 0x10)
			{
				RemoteDeviceState.StorageMediaFailure = true;
				AutoEvento(MessageIdentifier.HardwareChange_internal, Evento.HardwareChanges.EVTDATA_FLASH_FAULT);
				// SPI FAULT
			}

			if ((response.CL & 0x08) == 0x00)
			{
				RemoteDeviceState.State = State == DeviceTypes.States.ONLINE
					                          ? DeviceState.States.ONLINE
					                          : DeviceState.States.ONNET;
			}

			if ((response.CL & 0x04) == 0x04)
			{
				if (!RemoteDeviceState.HaveDisplay)
					RemoteDeviceState.HaveDisplay = true;
			}
			else
			{
				if (RemoteDeviceState.HaveDisplay)
					RemoteDeviceState.HaveDisplay = false;
			}
		}

		public void SolicitudEntregada(PDU response, PDU source)
		{
			if (source.CH == (byte) Codes.HighCommand.KeepAlive)
			{
				keep_alive_in_progress = false;
				keep_alive_mrc = null;
				if ((response.CL & 0x80) == 0x80)
				{
					KeepAliveV2(response);
				}
				else
				{
					KeepAliveV1(response);
				}
				return;
			}

			if (source.CH == (byte) Codes.HighCommand.ShortMessage)
			{
				if (source.CL < 0x03)
				{
					if (messages_q.Count == 0)
					{
						// 0,1,2 son configuran el dispo. soo..
						// termine de configurar el dispositivo, lo reinico.
						AutoEvento(MessageIdentifier.MessageTableSuccess, 0); // MESSAGE SUCCESS
						Reboot();
						return;
					}
					messages_q.Dequeue();
				}
				RunMessagesQueue();
				return;
			}

			if (source.CH == (byte) Codes.HighCommand.SetParameter)
			{
				settings_q.Dequeue();
				if (settings_q.Count == 0)
				{
					// termine de configurar el dispositivo, lo reinico.
					AutoEvento(MessageIdentifier.ConfigSuccess, 0); // CONFIG SUCCESS
					Reboot();
					return;
				}
				RunParametersQueue();
				return;
			}

			if (source.CH == (byte) Codes.HighCommand.DATA_PAGE)
			{
				try
				{
					var dp = source as DataPage;
					if (dp == null)
						throw new NullReferenceException("DEVICE[" + Id_short +
						                                 "]/COMMAND: fallo la especializacion de la PDU");
					var sut = ut as ServerTU;
					if (sut == null)
						throw new NullReferenceException("DEVICE[" + Id_short + "]/FOTA: El TransactionUser no se ServerTU");

					if (source.CL == 0x01 || source.CL == 0x04)
					{
						STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: Error el dispositivo informo que no coincide la firma MD5, rebooteo.", LogId));
						var dummy = new byte[2];
						Devices.I().Cmd.Push(String.Format("REBOOT:{0}", Id_short), dummy);
						AutoEvento(MessageIdentifier.FotaFail, 0); // FOTA FALLO
						return;
					}

					if (source.CL == 0x02)
					{
						if (response.CL == 0x01)
						{
							if (dp.Intento >= 3)
							{
								STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: el dispositivo no puede recibir la pagina {1}", LogId, dp.Pagina));
								fota_q.Clear();
								fota_pending = 0;
								State = DeviceTypes.States.OFFLINE;
								return;
							}
							STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: el dispositivo solicito la retransmision de la pagina {1}", LogId, dp.Pagina));
							try
							{
								var d = Devices.I().FindById(dp.IdDispositivo);
								var pdu = new DataPage
									{
										CL = (byte) (d.SupportExtendedDataPage ? 0x02 : 0x00),
										IdDispositivo = d.Id_short,
										Transporte = d.Transporte,
										Destino = d.Destino,
										Pagina = dp.Pagina,
										Intento = (short) (dp.Intento + 1),
										Buffer = new byte[512]
									};
								Array.Copy(dp.Buffer, pdu.Buffer, 512);
								var mrc = new MRC(pdu, pdu.Transporte, ut);
								pdu.Transporte.NuevaTransaccion(mrc, pdu);
								mrc.Seq = pdu.Seq;
								mrc.Start();
								return;
							}
							catch (Exception e)
							{
								STrace.Exception(GetType().FullName, e);
							}
						}
						RunFotaQueue();
						return;
					}

					if (source.CL == 0x03)
					{
						STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/QTREE: Se entrego sector {1}", LogId, dp.Pagina));
						RunQtreeQueue();
						return;
					}

					if (source.CL == 0x00)
					{
						RunFotaQueue();
					}
				}
				catch (Exception e)
				{
					STrace.Exception(GetType().FullName, e);
				}
			}
		}

		public void SolicitudNoEnviada(PDU pdu)
		{
			if (pdu.CH == (byte) Codes.HighCommand.KeepAlive)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/KEEPALIVE: no hay respuesta.", LogId));
				State = DeviceTypes.States.OFFLINE;
				return;
			}

			if (pdu.CH == (byte) Codes.HighCommand.ShortMessage)
			{
				var sm = (ShortMessage) pdu;
				if (pdu.CL == 0x04 || pdu.CL == 0x05)
				{
					AutoEvento(MessageIdentifier.UndeliverableSm, sm.Code);
					return;
				}
				if (pdu.CL == 0x06 || pdu.CL == 0x07)
				{
					return;
				}
				messages_q.Clear();
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/MESSAGE: No se pudo enviar un mensaje, reseteo.", LogId));
				State = DeviceTypes.States.OFFLINE;
				return;
			}

			if (pdu.CH == (byte) Codes.HighCommand.SetParameter)
			{
				settings_q.Clear();
				AutoEvento(MessageIdentifier.ConfigFail, 0); // CONFIG FAIL
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/PARAMETER: No se pudo enviar un parametro, reseteo.", LogId));
				State = DeviceTypes.States.OFFLINE;
				return;
			}

			if (pdu.CH == (byte) Codes.HighCommand.DATA_PAGE && pdu.CL == 0x03)
			{
				var page = pdu as DataPage;
				if (page == null)
					throw new NullReferenceException("DEVICE[" + LogId + "]/COMMAND: fallo la especializacion del PDU");
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/QTREE: Fallo pagina={1}, cancelamos.", Id_short, page.Pagina));
				QTreeState = DeviceTypes.QTreeStates.REQUIRES_UPDATE;
				qtree_q = null;
				State = DeviceTypes.States.OFFLINE;
				return;
			}

			if (pdu.CH != (byte) Codes.HighCommand.DATA_PAGE || (((pdu.CL != 0x00 && pdu.CL != 0x01) && pdu.CL != 0x02) && pdu.CL != 0x04)) return;
			var fd = pdu as DataPage;
			if (fd != null)
			{
				if (fd.CL == 0x01 || fd.CL == 0x04)
				{
					STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: El dispositivo parece haberese reinicializado. OK.", LogId));
					FotaCanceledAtPage = 0;
					AutoEvento(MessageIdentifier.FotaSuccess, 0); // FOTA EXITO.
				}
				else
				{
					STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: la pagina {1} no se pudo entregar, postergando.", LogId, fd.Pagina));
					FotaCanceledAtPage = fd.Pagina;
					AutoEvento(MessageIdentifier.FotaPause, FotaCanceledAtPage); // FOTA SUSPENDIDO
				}
			}
			fota_q.Clear();
			fota_pending = 0;
			State = DeviceTypes.States.OFFLINE;
		}

		public void SolicitudCancelada(PDU pdu)
		{
			if (pdu.CH != (byte) Codes.HighCommand.KeepAlive) return;
			STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/KEEPALIVE: cancelado satisfactoriamente.", LogId));
			keep_alive_in_progress = false;
			keep_alive_mrc = null;
		}

		public void FotaEnqueue(PDU pdu)
		{
			fota_pending++;
			fota_q.Enqueue(pdu);
		}

		private void AutoEvento(MessageIdentifier evento, Evento.HardwareChanges data)
		{
			AutoEvento((short) evento, (short) data);
		}

		private void AutoEvento(MessageIdentifier evento)
		{
			AutoEvento((short) evento, 0);
		}

		public void AutoEvento(MessageIdentifier evento, int data)
		{
			AutoEvento((short) evento, data);
		}

		private void AutoEvento(short evento, int data)
		{
			if (autoevento_ultima_data == data && autoevento_ultimo_evento == evento) return;
			autoevento_ultima_data = data;
			autoevento_ultimo_evento = evento;
			var sut = ut as ServerTU;
			if (sut == null)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: evento={1} data={2} no se pudo entregar", LogId, evento, data));
				return;
			}
			var evt = new Evento
				{
					IdDispositivo = Id_short,
					CodigoEvento = evento,
					Datos = data
				};
			sut.RaiseEvento(evt);
		}

		public void RunFotaQueue()
		{
			PDU p = null;
			fota_pending--;
			STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: fota_pending={1} fota_q.Count={2}", LogId, fota_pending, fota_q.Count));
			if (fota_q.Count == 1 && fota_pending > 0)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: todavia falta alguna pieza de fota para mandar el MD5. fota_pending={1} fota_q.Count={2}", LogId, fota_pending, fota_q.Count));
				return;
			}
			try
			{
				if (fota_q.Count > 0) p = fota_q.Dequeue();
			}
			catch (Exception e)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: La cola esta vacia.", LogId));
				e.Data.Add("device", this);
				STrace.Exception(GetType().FullName, e);
				return;
			}
			if (p == null)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: La cola esta vacia.", LogId));
				return;
			}
			var fd = p as DataPage;
			if (fd == null)
				throw new NullReferenceException(String.Format("DEVICE[{0}]/FOTA: El mensaje en la cola no es DATA_PAGE.", Id_short));
			// actualizamos el destino de la pdu.
			fd.Destino.UDP = destino.UDP;
			fd.Destino.TCP = destino.TCP;
#if XBEE_HABILITADO
            fd.Destino.XBee = destino.XBee;
#endif
			fd.TotalDePaginas = TotalDePaginasFota;

			STrace.Debug(GetType().FullName, String.Format(fd.Buffer.GetLength(0) == 16 ? "DEVICE[{0}]/FOTA: Intento enviar hashcode MD5 total de paginas={1}" : "DEVICE[{0}]/FOTA: Intento enviar pagina={1}", LogId, fd.Pagina));
			try
			{
				var mrc = new MRC(p, p.Transporte, ut);
				p.Transporte.NuevaTransaccion(mrc, p);
				mrc.Seq = p.Seq;
				mrc.Start();
			}
			catch (Exception e)
			{
				e.Data.Add("device", this);
				STrace.Exception(GetType().FullName, e);
			}
		}

		public void UpdateFrom(Device d)
		{
			if (d == null) return;
			Imei = d.Imei;
			LegacyCode = d.LegacyCode;
			FirmaRequerida = d.FirmaRequerida;
			FirmwareId = d.FirmwareId;
			Vehicle = d.Vehicle;
			Base = d.Base;
			Parameters.Clear();
			Parameters = d.Parameters;
			// actualizo datos remotos...
			if (RemoteDeviceState == null)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: no pudo actualizar ningun estado remoto por que falta la envolruta.", d.LogId));
				return;
			}
			if (!Parameters.ContainsKey("known_qtree_revision"))
			{
				RemoteDeviceState.QTreeState = DeviceTypes.QTreeStates.UNKNOWN;
				return;
			}
			var qtver = Convert.ToInt32(Parameters["known_qtree_revision"].Valor);
			RemoteDeviceState.QTreeState = qtver == int.MaxValue
				                               ? DeviceTypes.QTreeStates.UNKNOWN
				                               : (qtver >= Devices.I().QuadTree.Revision
					                                  ? DeviceTypes.QTreeStates.UPTODATE
					                                  : DeviceTypes.QTreeStates.REQUIRES_UPDATE);
		}

		public string GetManufacturer()
		{
			switch (Type)
			{
				case DeviceTypes.Types.URB_v0_5:
				case DeviceTypes.Types.URB_v0_7:
				case DeviceTypes.Types.URBMOBILE_v0_1:
				case DeviceTypes.Types.URBETRACK_v0_8n:
				case DeviceTypes.Types.URBETRACK_v0_8:
				case DeviceTypes.Types.URBETRACK_v1_0:
					return "PAGUSTECH";
				case DeviceTypes.Types.SISTELCOM_v1:
				case DeviceTypes.Types.SISTELCOM_v2:
					return "SISTELCOM";
				case DeviceTypes.Types.UNETEL_v1:
				case DeviceTypes.Types.UNETEL_v2:
					return "UNETEL";
				default:
					return "UNKNOW";
			}
		}

		public void UpdateQtree(Qtree _quad_tree)
		{
			UpdateQtree(_quad_tree, GetServerParameterInt("known_qtree_revision", 0));
		}

		public void UpdateQtree(Qtree _quad_tree, int from_revision)
		{
			if (state != DeviceTypes.States.ONLINE)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/QTREE: UPDATE QTREE no procesado el dispositivo esta offline.", LogId));
				Trace();
				return;
			}
			if (!SupportsQuadTree)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/QTREE: este dispositivo no soporta QTREE Type='{1}'", LogId, Type));
				return;
			}
			if (qtree_q != null)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/QTREE: ya hay otro upgrade de qtree en progreso.", LogId));
				return;
			}
			quad_tree = _quad_tree;
			quad_tree.LoadEx();

			var cache_name = String.Format("from_{0}", from_revision);

			qtree_q = QueueSerializer.Fetch(cache_name);

			if (qtree_q == null)
			{
				qtree_q = quad_tree.GetQTreeCompressed(512, from_revision);
				QueueSerializer.Store(cache_name, qtree_q);
			}

			if (qtree_q == null) return;

			qtreePagina = (short) qtree_q.Count;
			QTreeState = DeviceTypes.QTreeStates.TRANSFERING;
			var lapse = Hacker.Network.DATA_PAGE_INITIAL_LAPSE;
			for (var i = 0; i < DataPageWindowSize; ++i)
			{
				// ReSharper disable ConditionIsAlwaysTrueOrFalse
				// ReSharper disable HeuristicUnreachableCode
				if (qtree_q == null) return;
				// ReSharper restore HeuristicUnreachableCode
				// ReSharper restore ConditionIsAlwaysTrueOrFalse
				RunQtreeQueue();
				Thread.Sleep(lapse);
				lapse = Hacker.Network.DATA_PAGE_LAPSE;
			}
		}

		private void RunQtreeQueue()
		{
			Thread.Sleep(Hacker.Network.DATA_INTER_PAGE_LAPSE);
			if (qtree_q == null)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/RUN_QTREE_QUEUE: la cola es nula.", LogId));
				return;
			}
			if (quad_tree == null)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/RUN_QTREE_QUEUE: no tiene asignado un QuadTree.", LogId));
				return;
			}
			if (qtree_q.Count == 0)
			{
				SetServerParameter("known_qtree_revision", quad_tree.Revision);
				QTreeState = DeviceTypes.QTreeStates.UPTODATE;
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/RUN_QTREE_QUEUE: No hay mas acciones a enviar, nueva revision {1}.", LogId, GetServerParameterInt("known_qtree_revision", 0)));
				qtree_q = null;
				return;
			}
			var buffer = qtree_q.Dequeue();
			var sut = ut as ServerTU;
			if (sut == null)
				throw new NullReferenceException(String.Format("DEVICE[{0}]/QTREE: El TransactionUser no es ServerTU.", Id_short));
			var p = new DataPage
				{
					CL = 0x03,
					// firma MD5 de fota.
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					Pagina = qtreePagina--,
					Buffer = buffer
				};
			var mrc = new MRC(p, Transporte, ut);
			Transporte.NuevaTransaccion(mrc, p);
			mrc.Seq = p.Seq;
			mrc.Start();
		}

		public void Query(int sensor, DateTime start_time, DateTime end_time)
		{
			if (state != DeviceTypes.States.ONLINE)
			{
				STrace.Debug(GetType().FullName, "DEVICE: Cancelo Query, el dispositivo esta offline.");
				Trace();
				return;
			}

			var sut = ut as ServerTU;
			if (sut == null)
				throw new NullReferenceException(
					"El UT del dispositivo retorna null cuando se lo transforma a ServerTU.");
			var query = new Query
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					Sensor = sensor,
					StartTime = start_time,
					EndTime = end_time
				};

			var mrc = new MRC(query, Transporte, ut);
			Transporte.NuevaTransaccion(mrc, query);
			mrc.Seq = query.Seq;
			mrc.Start();
		}

		public void FlashOverTheAir(string firmware_name)
		{
			if (state != DeviceTypes.States.ONLINE)
			{
				STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/FOTA: Cancelo FOTA, el dispositivo esta offline.", LogId));
				Trace();
				return;
			}

			var sut = ut as ServerTU;
			if (sut == null)
				throw new NullReferenceException(String.Format("DEVICE[{0}]/FOTA: El TransactionUser no es ServerTU.", Id_short));
			RemoteDeviceState.FlashCounter++;
			sut.FlashOverTheAir(Id_short, firmware_name);
		}

		private void RunKeepAlive(int timer_id)
		{
			if (!SupportsKeepAlive) return;
			STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Keep Temporizado", LogId));
			keep_alive_timer_id = -1;
			KeepAlive();
			keep_alive_timer_id = Scheduler.AddTimer(RunKeepAlive, GetServerParameterInt("shutdown_keepalive_interval", Hacker.Device.KeepAliveInterval));
		}

		public void CancelAllTasks()
		{
			if (keep_alive_mrc == null) return;
			STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Cancelando Keep Alive, se reconecto.", LogId));
			keep_alive_mrc.Cancelar();
		}

		public void DeviceEvent(Evento evt)
		{
			if (evt.CodigoEvento == (short) MessageIdentifier.SubmitSm)
			{
				SendMessageProgress(0x06, (short) evt.Datos, evt.Extra);
			}
			if (evt.RiderRevision == -1) return;
			var updateRider = Devices.I().GetRider(evt.RiderIdentifier, evt.RiderRevision);
			if (updateRider == null) return;
			var dummy = new byte[2];
			Devices.I().Cmd.Push(String.Format("SETRIDERNAME:{0};", Id_short), dummy);

			var sut = ut as ServerTU;
			if (sut == null) return;
			STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Enviando datos de Chofer id='{1}' description='{2}'", LogId, updateRider.Identifier, updateRider.Description));
			var setRider = new SetRider
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					Identifier = updateRider.Identifier,
					Description = updateRider.Description,
					File = updateRider.File,
					Revision = updateRider.Revision,
					Flags = 0
				};
			var mrc = new MRC(setRider, Transporte, ut);
			Transporte.NuevaTransaccion(mrc, setRider);
			mrc.Seq = setRider.Seq;
			mrc.Start();
		}

		#region Nested type: Rider

		[Serializable]
		public class Rider
		{
			public string Identifier { get; set; }

			public string Description { get; set; }

			public string File { get; set; }

			public int Revision { get; set; }
		}

		#endregion Nested type: Rider

		#region Nested type: Message

		[Serializable]
		public class Message
		{
			public int Revision { get; set; }

			public short Code { get; set; }

			public string Text { get; set; }

			public char Source { get; set; }

// ReSharper disable UnusedAutoPropertyAccessor.Global
			public int Destination { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

			public bool Deleted { get; set; }
		}

		#endregion Nested type: Message

		#region Nested type: Parameter

		[Serializable]
		public class Parameter
		{
			public int Revision { get; set; }

			public string Nombre { get; set; }

			public string Valor { get; set; }

			public char Consumidor { get; set; }

			public string TipoDato { get; set; }

			public bool UpdateRequired { get; set; }

			public string ValorInicial { get; set; }

			public bool IsEnabled()
			{
				if (string.IsNullOrEmpty(Valor)) return false;
				return Valor == "enabled" || Valor == "1" || Valor == "true";
			}
		}

		#endregion Nested type: Parameter

		public void RemoteShell(string command_line)
		{
			var pdu = new RemoteShell
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					CommandLine = command_line,
				};
			var mrc = new MRC(pdu, pdu.Transporte, ut);
			pdu.Transporte.NuevaTransaccion(mrc, pdu);
			mrc.Seq = pdu.Seq;
			mrc.Start();
		}

		public void SetBootloaderAction(byte action)
		{
			SendCommand(Codes.HighCommand.SetBootloaderAction, action);
		}

		public void SendParameter(Parameter p)
		{
			var pdu = new SetParameter
				{
					IdDispositivo = Id_short,
					Transporte = Transporte,
					Destino = Destino,
					Parameter = p.Nombre,
					Revision = p.Revision,
					Value = p.Valor
				};
			var mrc = new MRC(pdu, pdu.Transporte, ut);
			pdu.Transporte.NuevaTransaccion(mrc, pdu);
			mrc.Seq = pdu.Seq;
			mrc.Start();
		}

		/*
		private Queue<string> outgoing_queue = new Queue<string>();

		public void EnqueueOutgoing(string a)
		{
			outgoing_queue.Enqueue(a);
		}

		public string DequeueOutgoing()
		{
			if (outgoing_queue.Count == 0) return null;
			return outgoing_queue.Dequeue();
		}
		*/

		#endregion

		#region Implementation of INode

		/// <summary>
		/// Codigo unico en la red. (ex DeviceID)
		/// </summary>
		public int Id
		{
			get { return Id_short; }
			set { Id_short = (short)value; }
		}

		// ReSharper disable UnusedAutoPropertyAccessor.Local
		public ulong Sequence { get; private set; }
		// ReSharper restore UnusedAutoPropertyAccessor.Local

		// ReSharper disable UnusedAutoPropertyAccessor.Local
		public ulong NextSequence { get; private set; }
		// ReSharper restore UnusedAutoPropertyAccessor.Local

		/// <summary>
		/// Contraseña del dispositivo.
		/// </summary>
		public String Password { get; set; }

		public IDataProvider DataProvider { get; set; }
		public int Port { get; set; }
		public INode Factory(IFrame frame, int formerId) { throw new NotImplementedException(); }
		public IMessage Decode(IFrame frame) { throw new NotImplementedException(); }
		public INode FactoryShallowCopy(int newDeviceId, string newImei) { throw new NotImplementedException(); }
		public INode Get(int deviceId) { throw new NotImplementedException(); }
		public INode Find(string imei) { throw new NotImplementedException(); }
		public bool IsPacketCompleted(byte[] Payload, int Start, int Count, out int DetectedCount, out bool IgnoreNoise) { throw new NotImplementedException(); }
		public bool ChecksCorrectIdFlag { get; private set; }
		public bool ExecuteOnGuard(Action execute, String callerName, String detailForOnFail) { throw new NotImplementedException(); }

		#endregion

		#region IPowerBoot Members

		public bool Reboot(int messageId)
		{
			return Reboot();
		}

		#endregion IPowerBoot Members

		#region IQuadtree Members

		public bool SyncronizeQuadtree(int messageId, bool full, int baserevision)
		{
			if (full) UpdateQtree(Devices.I().QuadTree, 0);
			else UpdateQtree(Devices.I().QuadTree);
			return true;
		}

		#endregion IQuadtree Members

		#region IShortMessage Members

		public bool SetCannedMessage(int id_message, int code, string message, int revision)
		{
			return true;
		}

		public bool SetCannedResponse(int id_message, int code, string response, int revision)
		{
			return SetCannedMessage(id_message, code, response, revision);
		}

		public bool DeleteCannedMessage(int id_message, int code, int revision)
		{
			DeleteAllMessages();
			return true;
		}

		public bool SubmitCannedMessage(int id_message, int code, int[] replies)
		{
			SendMessage((short)code, "", id_message, "");
			return true;
		}

        public bool SubmitTextMessage(int MessageId, int textMessageId, string textMessage, int[] replies)
        {
            throw new NotImplementedException();
        }


		public bool SubmitTextMessage(int id_message, string textMessage, int[] replies)
		{
			SendMessage(99, textMessage, id_message, "");
			return true;
		}

		#endregion IShortMessage Members

		#region IProvisioned Members

		public bool SetParameter(int MessageId, string parameter, string value, int revision, int hash)
		{
			var p = new Parameter
			{
				Nombre = parameter,
				Revision = revision,
				Valor = value
			};
			SendParameter(p);
			return true;
		}

		#endregion
	}
}