using System;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Unetel.v2.UnexLibs;
using Logictracker.Utils;

namespace Logictracker.Unetel.v1
{
	[FrameworkElement(XName = "UnetelV1Parser", IsContainer = false)]
	public class Parser : BaseCodec, IPowerBoot, IWorkflow, IShortMessage, IProvisioned, IKeepAliveInfo, IFoteable
	{
        public override NodeTypes NodeType { get { return NodeTypes.Unetelv1; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2020)]
		public override int Port { get; set; }

        private String _fotaFolder;

        public String FotaFolder
        {
            get
            {
                if (_fotaFolder == null)
                    _fotaFolder = Process.GetApplicationFolder("FOTA");
                return _fotaFolder;
            }
        }

		#endregion

		#region BaseCodec

        protected override UInt32 NextSequenceMin()
        {
            return 0x0000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

		public override INode Factory(IFrame frame, int formerId)
		{
			if (frame.Payload.Length < 2) return null;
			var s = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length);
			if (s.StartsWith("G"))
			{
				STrace.Debug(typeof(Parser).FullName, formerId, "Ack Unex V1 sin dispositivo (GA)");
				return DataProvider.Get(formerId, this);
			}
			if (frame.Payload[0] == 'H' && frame.Payload[1] == '0') return DataProvider.FindByIMEI(s.Substring(24, 16), this);

			var did = "0";
			if (s.StartsWith("E")) did = s.Substring(3, 5);
			if (s.StartsWith("Q")) did = s.Substring(2, 5);
			if (s.StartsWith("K") && s.Length > 6) did = s.Substring(1, 5);
			return DataProvider.Get(SafeConvert.ToInt32(did, 0), this);
		}

		public override IMessage Decode(IFrame frame)
		{
			var buffer = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length);
			try
			{
				var highCommand = Convert.ToChar(frame.Payload[0]);
				IMessage msg;
				var canappend = true;
				switch (highCommand)
				{
					case 'E':
						msg = ParseEvento(buffer, this);
						break;
					case 'K':
						msg = ParseKeepAlive(this);
						canappend = false;
						break;
					case 'Q':
						msg = Posicion.Parse(buffer, this);
						break;
					case 'H':
						if (Convert.ToChar(frame.Payload[1]) == 'S')
						{
							msg = HardwareStatus.Parse(buffer);
						}
						else
						{
							msg = ParseConfigRequest(buffer);
							canappend = false;
						}
						break;
					case 'G': //solo es un ack
						var mid = Convert.ToUInt64(buffer.Substring(2, 5));
						CheckFota(mid);
						msg = new UserMessage(Id, mid);
						canappend = false;
						break;
					default:
						msg = null;
						break;
				}

				msg.ForceReplyCheckingFota(canappend, this);
				return msg;
			}
			catch
			{
				STrace.Trace(GetType().FullName, Id, String.Format("No se pudo parsear: {0}", buffer));
				return null;
			}
		}

		protected override int MessageSequenceMin { get { return 130; } }
		protected override int MessageSequenceMax { get { return 254; } }
		public override bool ChecksCorrectIdFlag { get { return true; } }

		private static UserMessage ParseKeepAlive(INode node)
		{
			return new UserMessage(node.GetDeviceId(), 0).AddStringToSend("A").SetUserSetting("user_message_code", "KEEPALIVE");
		}

		private static Event ParseEvento(String asString, INode node)
		{
			var idEquipo = Convert.ToInt32(asString.Substring(3, 5));
			if (node.Id != SafeConvert.ToInt32(idEquipo, -1))
			{
				STrace.Debug(typeof(Parser).FullName, node.Id, String.Format("ENTRANTE DESCARTAD0, NO COINCIDE. raw_data={0}", asString));
				return null;
			}

			//se cambian algunos codigos por que hay conflictos
			var code = Convert.ToInt16(asString.Substring(1, 2));
			switch (code)
			{
				case 90: code = (short)MessageIdentifier.BateryReConected; break;
				case 91: code = (short)MessageIdentifier.PanicButtonOff; break;
				case 92: code = (short)MessageIdentifier.PanicButtonOn; break;
				case 93: code = (short)MessageIdentifier.SlowingTicket; break;
				case 94: code = (short)MessageIdentifier.EngineOnInternal; break;
				case 95: code = (short)MessageIdentifier.EngineOffInternal; break;
				case 96: code = (short)MessageIdentifier.SpeedingTicketEnd; break;
				case 97: code = (short)MessageIdentifier.BateryDisconected; break;
				case 99: code = (short)MessageIdentifier.DisplayProblem; break;
			}

			var idMsg = Convert.ToUInt32(asString.Substring(8, 5));

			return ((MessageIdentifier)code)
				.FactoryEvent(node.Id, idMsg, null, DateTime.UtcNow, null, null)
				.AddStringToSend(String.Format("GA{0:D5}", idMsg));
		}

		private ConfigRequest ParseConfigRequest(String asString)
		{
			var crq = new ConfigRequest(Id, 0)
			{
				GeoPoint = null
			};

			const int keepalive = 3;
			const int retry = 3;
			crq.AddStringToSend(String.Format("H9{0:D5}{1:D5}{2:D2}{3:D2}", 10000, Id, keepalive, retry));
			crq.StringParameters.Add(Indication.indicatedIMEI, asString.Substring(24, 16));
			crq.StringParameters.Add(Indication.indicatedSecret, asString.Substring(59, 8));
			crq.StringParameters.Add(Indication.indicatedFirmwareSignature, asString.Substring(54, 4));
			crq.IntegerParameters.Add(Indication.indicatedConfigurationRevision, 0);
			crq.IntegerParameters.Add(Indication.indicatedCannedMessagesTableRevision, SafeConvert.ToInt32(asString.Substring(58, 1), -1));

			return crq;
		}

		#endregion

		#region SendMessages

		private void SendMessages(String config, ulong messageId, ulong seq)
		{
			Fota.Enqueue(this, messageId, String.Format("{0}///;#{1:X4}", config, seq));
		}

		#endregion

		#region IPowerBoot

		public bool Reboot(ulong messageId)
		{
			var seq = NextSequence;
			var cmd = String.Format("CR{0:D5}TELEQUIP", seq);
			SendMessages(cmd, messageId, seq);
			return true;
		}

		#endregion

		#region IWorkflow

		public bool SetWorkflowState(ulong messageId, int state, WorkflowMessage[] messages)
		{
			var seq = NextSequence;
			var cmd = String.Format("C3{0:D5}{1:D2}", seq, state);
			SendMessages(cmd, messageId, seq);
			return true;
		}

		#endregion

		#region IShortMessage

		public bool SetCannedMessage(ulong messageId, int code, String message, int revision)
		{
			if (code > 88)
			{
				STrace.Debug(GetType().FullName, Id, String.Format("SetCannedMessage codigo demasiado grande: {0}", code));
				return false;
			}

			var seq = NextSequence;
			var cmd = String.Format("CM{0:D5}{1}1{2:D2}{3}{4:D2}{5}", seq, "0", code, 1, message.Length, message);
			SendMessages(cmd, messageId, seq);
			return true;
		}

		public bool SetCannedResponse(ulong messageId, int code, String response, int revision)
		{
			if (code > 88)
			{
				STrace.Debug(GetType().FullName, Id, String.Format("SetCannedResponse codigo demasiado grande: {0}", code));
				return false;
			}

			var seq = NextSequence;
			var cmd = String.Format("CM{0:D5}{1}1{2:D2}{3}{4:D2}{5}", seq, "0", code, 0, response.Length, response);
			SendMessages(cmd, messageId, seq);
			return true;
		}

		public bool DeleteCannedMessage(ulong messageId, int code, int revision)
		{
			if (code > 88)
			{
				STrace.Debug(GetType().FullName, Id, String.Format("DeleteCannedMessage codigo demasiado grande: {0}", code));
				return false;
			}

			return SetCannedMessage(messageId, code, "(vacio)", revision);
		}

		public bool SubmitCannedMessage(ulong messageId, int code, int[] replies)
		{
			if (code > 88)
			{
				STrace.Debug(GetType().FullName, Id, String.Format("SubmitCannedMessage codigo demasiado grande: {0}", code));
				return false;
			}

			var seq = NextSequence;
			var cmd = String.Format("P{0:D5}{1:D2}", seq, code);
			SendMessages(cmd, messageId, seq);
			return true;
		}

        public bool SubmitTextMessage(ulong messageId, uint textMessageId, string textMessage, uint[] replies, int ackEvent)
        {
            throw new NotImplementedException();
        }

		public bool SubmitTextMessage(ulong messageId, String textMessage, int[] replies)
		{
			var seq = NextSequence;
			var cmd = String.Format("P{0:D5}99{1}00", seq, textMessage.PadRight(32));
			SendMessages(cmd, messageId, seq);
			return true;
		}

		#endregion

		#region IProvisioned

		public bool SetParameter(ulong messageId, String parameter, String value, int revision, int hash)
		{
			var seq = NextSequence;
			var p = value.Split(',');
			String cmd;
			switch (parameter)
			{
				case "config_0": cmd = String.Format("C0{0:D5}{1} {2} {3:D4}{4:D2}{5:D2}", seq, p[0], p[1], Convert.ToInt32(p[2]), Convert.ToInt32(p[3]), Convert.ToInt32(p[4])); break;
				case "config_1": cmd = String.Format("C1{0:D5}{1:D3}{2:D2}{3:D3}{4:D2}", seq, Convert.ToInt32(p[0]), Convert.ToInt32(p[1]), Convert.ToInt32(p[2]), Convert.ToInt32(p[3])); break;
				case "config_2": cmd = String.Format("C2{0:D5}{1:D2}{2}", seq, p[0].Length, p[0]); break;
				case "P": cmd = String.Format("P{0:D5}{1:D2}0200", seq, Convert.ToInt32(value)); break;
				default: return false;
			}
			SendMessages(cmd, messageId, seq);
			return true;
		}

		#endregion

		#region IKeepAliveInfo

		public int KeepAliveLapse { get; set; }

		#endregion

		#region IFoteable

		public Boolean ReloadConfiguration(ulong messageId) { return true; }
        public Boolean ReloadMessages(ulong messageId) { return true; }

        public bool ResetFMIOnGarmin(ulong messageId)
        {
            throw new NotImplementedException();
        }

        public Boolean? IsGarminConnected { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

		public Boolean ReloadFirmware(ulong messageId) { return true; }
		public INodeMessage LastSent { get; set; }

		private void CheckFota(ulong mid)
		{
			STrace.Trace(GetType().FullName, Id, String.Format("CheckFota: LastSent.GetId()={0} mid={1}", LastSent.GetId(), mid));
			if (mid != 10000)
			//if (LastSent.GetId() == mid)
			{
				Fota.Dequeue(this, null);
			}
		}

		public Boolean ContainsMessage(String line)
		{
			return !String.IsNullOrEmpty(line);
			//return Line.IndexOf("///;#") >= 1;
		}

		public ulong GetMessageId(String line)
		{
			return line.Contains("///;#") ? Convert.ToUInt64(line.Substring(line.IndexOf("///;#") + 5), 16) : ParserUtils.MsgIdNotSet;
		}

		#endregion
	}
}