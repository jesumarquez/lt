using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Unetel.v2.UnexLibs;
using Logictracker.Utils;

namespace Logictracker.Unetel.v2
{
	[FrameworkElement(XName = "UnetelV2Parser", IsContainer = false)]
	public class Parser : BaseCodec, IPowerBoot, IWorkflow, IShortMessage, IProvisioned, IFuelControl, IFoteable
	{
        public override NodeTypes NodeType { get { return NodeTypes.Unetelv2; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 4040)]
		public override int Port { get; set; }

		[ElementAttribute(XName = "LogToFile", IsRequired = false, DefaultValue = false)]
		public bool LogToFile { get; set; }

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
			var payload = frame.Payload;
			if (payload.Length < 2) return null;

			// HACK el equipo de sampe esta mandando una 'Y'
			if (payload[0] == 'H' && payload[1] == '0')
			{
				payload[2] = Convert.ToByte(';');
			}

			var partes = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length).Split('@')[0].Split(';');
			if (partes.Length < 3) return null;

			if (payload[0] == 'H' && payload[1] == '0')
			{
				return DataProvider.FindByIMEI(partes[1], this);
			}

			if (payload[0] == 'T' && payload[1] == '0')
			{
				return DataProvider.FindByIMEI(partes[4], this);
			}

			if (payload[0] == 'A' && payload[1] == '0')
			{
				return DataProvider.FindByIMEI(partes[4], this);
			}

			if (payload[0] == 'V' && payload[1] == '0')
			{
				return DataProvider.FindByIMEI(partes[4], this);
			}

			if ((payload[0] == 'E') && (payload[1] != 'V'))
			{
				return DataProvider.FindByIMEI(partes[2], this);
			}

			var devid = SafeConvert.ToInt32(partes[2], 0);
			return devid == 0 ? null : DataProvider.Get(devid, this);
		}

		public override IMessage Decode(IFrame frame)
		{
			var buffer = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length);
			var res = DecodeInternal(frame);
			if (LogToFile) Log(buffer, res.GetPendingAsString(), res.GetPendingPostAsString(), frame.RemoteAddressAsString);
			return res;
		}

		public IMessage DecodeInternal(IFrame frame)
		{
			var buffer = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length);
			try
			{
				var parts = buffer.Split('@');
				if (parts.GetLength(0) > 1)
				{
					var sum = buffer.TakeWhile(b => b != '@').Aggregate<char, byte>(0, (current, b) => (byte) (current + b));

					if (parts[1] != String.Format("{0:X2}", sum))
					{
                        STrace.Debug(typeof(Parser).FullName, Id, "Reporte ignorado por error de checksum");

					    if (buffer[0] == 'H' || buffer[0] == 'N' || buffer[0] == 'R')
					    {
					        return new UserMessage(0, 0).AddStringToSend("B");
					    }

				        var ack = String.Format("R{0};{1}", buffer.Substring(0, 2), buffer.Split(';')[1]);
				        return new UserMessage(0, 0).AddStringToSend(ack);
					}
				}

				if (buffer.Contains("???"))
				{
				    if (buffer[0] == 'H' || buffer[0] == 'N' || buffer[0] == 'R')
				    {
				        STrace.Debug(typeof(Parser).FullName, Id, "Reporte ignorado por corrupto");
				        return new UserMessage(0, 0).AddStringToSend("B");
				    }

				    var ack = String.Format("R{0};{1}", buffer.Substring(0, 2), buffer.Split(';')[1]);
				    STrace.Debug(typeof(Parser).FullName, Id, "Reporte ignorado por corrupto");
				    return new UserMessage(0, 0).AddStringToSend(ack);
				}

			    var partes = parts[0].Split(';');

				if (ParserUtils.IsInvalidDeviceId(Id))
				{
					return (partes.Length > 1) ? new UserMessage(0, 0).AddStringToSend(String.Format("R{0};{1}", buffer.Substring(0, 2), partes[1])) : null;
				}

				var canappend = true;
				var highCommand = Convert.ToChar(frame.Payload[0]);
				IMessage msg;
				switch (highCommand)
				{
					case 'A':
						msg = EventoBotonera.Parse(partes, this, parts[0]);
						break;
					case 'T':
						msg = EventoTemperatura.Parse(partes, this);
						break;
					case 'Q':
						msg = Posicion.Parse(partes, this);
						break;
					case 'H':
						msg = ParseConfigRequest(partes);
						canappend = false;
						break;
					case 'E':
						msg = Convert.ToChar(frame.Payload[1]) == 'V' ? Entrante.Parse(partes, this) : Rondin.Parse(partes, this);
						break;
					case 'V':
						msg = Vending.Parse(partes, this);
						break;
					case 'N': //solo es un nack
					case 'R': //solo es un ack
						var msgid = Convert.ToUInt64(partes[1]);
						CheckFota(msgid);
						msg = new UserMessage(Id, msgid);
						canappend = false;
						break;
					default:
						msg = null;
						break;
				}

				msg.ForceReplyCheckingFota(canappend, this);

				//checksum deshabilitado por que trae problemas al appendear un comando
				/*if (msg != null)
				{
					if (parts.GetLength(0) > 1)
					{
						msg.AddStringToSend(String.Format("@{0:X2}", msg.GetPending().Aggregate<byte, byte>(0, (current, b) => (byte)(current + b))));
					}
				}//*/

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

		public override bool IsPacketCompleted(byte[] payload, int start, int count, out int detectedCount, out bool ignoreNoise)
		{
			ignoreNoise = false;
			detectedCount = 0;
			if (payload.All(b => b != '@')) return false;

			detectedCount = Array.IndexOf(payload, '@', start, count) + 2;
			return count >= detectedCount;
		}

		public override bool ChecksCorrectIdFlag { get { return true; } }

		private ConfigRequest ParseConfigRequest(String[] partes)
		{
			var crq = new ConfigRequest(Id, 0)
			{
				GeoPoint = null
			};

			var res = String.Format("H9;{0:D5}", Id);
			res += res.TakeWhile(b => b != '@').Aggregate<char, byte>(0, (current, b) => (byte) (current + b)).ToString("X2");

			crq.AddStringToSend(res);

			crq.StringParameters.Add(Indication.indicatedIMEI, partes[1]);
			crq.StringParameters.Add(Indication.indicatedSecret, partes[2]);
			crq.StringParameters.Add(Indication.indicatedFirmwareSignature, partes[3]);
			if (partes.Length > 4)
				crq.IntegerParameters.Add(Indication.indicatedConfigurationRevision, SafeConvert.ToInt32(partes[4], -1));
			if (partes.Length > 5)
				crq.IntegerParameters.Add(Indication.indicatedCannedMessagesTableRevision, SafeConvert.ToInt32(partes[5], -1));
			return crq;
		}

		#endregion

		#region SendMessages

		private void SendMessages(String config, ulong messageId)
		{
			Fota.Enqueue(this, messageId, config);
		}

		private static String BuildLine(String config, ulong sequence)
		{
			return String.Format("{0}///;#{1:X4}", config, sequence);
		}
		
		#endregion

		#region IPowerBoot

		public bool Reboot(ulong messageId)
		{
			var seq = NextSequence;
			var cmd = GetStrReboot(seq);
			SendMessages(BuildLine(cmd, seq), messageId);
			return true;
		}

		private String GetStrReboot(ulong seq)
		{
			return String.Format("RS;{0:D3};{1:D5}", seq, Id);
		}

		#endregion

		#region IWorkflow

		public bool SetWorkflowState(ulong messageId, int state, WorkflowMessage[] messages)
		{
			var seq = NextSequence;
			var cmd = String.Format("CL;{0:D3};{1:D5};{2:D2}", seq, Id, state);
			SendMessages(BuildLine(cmd, seq), messageId);
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
			var cmd = GetStrSetCannedMessage(code, seq, message, revision);
			SendMessages(BuildLine(cmd, seq), messageId);
			return true;
		}

		private String GetStrSetCannedMessage(int code, ulong seq, String message, int revision)
		{
			return String.Format("CM;{0:D3};{1:D5};{2:D2};{3:D1};{4};{5:D5}", seq, Id, code, 1, message, revision);
		}

		public bool SetCannedResponse(ulong messageId, int code, String response, int revision)
		{
			if (code > 88)
			{
				STrace.Debug(GetType().FullName, Id, String.Format("SetCannedResponse codigo demasiado grande: {0}", code));
				return false;
			}

			var seq = NextSequence;
			var cmd = String.Format("CM;{0:D3};{1:D5};{2:D2};{3:D1};{4};{5:D5}", seq, Id, code, 0, response, revision);
			SendMessages(BuildLine(cmd, seq), messageId);
			return true;
		}

		public bool DeleteCannedMessage(ulong messageId, int code, int revision)
		{
			if (code > 88)
			{
				STrace.Debug(GetType().FullName, Id, String.Format("DeleteCannedMessage codigo demasiado grande: {0}", code));
				return false;
			}

			var commands = new StringBuilder();
			var seq = NextSequence;

			// HACK: se usa SetCannedMessage primero para establecer el numero de revision.
			if (code == 0)
			{
				var cmdScm = GetStrSetCannedMessage(20, seq, "BORRANDO...", revision);
				commands.AppendLine(BuildLine(cmdScm, seq));
				seq = NextSequence;
			}

			var cmd = String.Format("DM;{0:D3};{1:D5};{2:D2}", seq, Id, code);
			commands.AppendLine(BuildLine(cmd, seq));
			if (code == 0)
			{
				seq = NextSequence;
				var cmdReboot = GetStrReboot(seq);
				commands.AppendLine(BuildLine(cmdReboot, seq));
			}
			SendMessages(commands.ToString(), messageId);
			return true;
		}

		public bool SubmitCannedMessage(ulong messageId, int code, int[] replies)
		{
			if (code > 88)
			{
				STrace.Debug(GetType().FullName, Id, String.Format("SubmitCannedMessage codigo demasiado grande: {0}", code));
				return false;
			}

			var seq = NextSequence;
			var cmd = String.Format("DD;{0:D3};{1:D5};{2:D2}", seq, Id, code);
			SendMessages(BuildLine(cmd, seq), messageId);
			return true;
		}

        public bool SubmitTextMessage(ulong messageId, uint textMessageId, string textMessage, uint[] replies, int ackEvent)
        {
            throw new NotImplementedException();
        }

		public bool SubmitTextMessage(ulong messageId, String textMessage, int[] replies)
		{
			var seq = NextSequence;
			var cmd = String.Format("DD;{0:D3};{1:D5};99;{2}", seq, Id, textMessage);
			SendMessages(BuildLine(cmd, seq), messageId);
			return true;
		}

		#endregion

		#region IProvisioned

		public bool SetParameter(ulong messageId, String parameter, String value, int revision, int hash)
		{
			var seq = NextSequence;
			String cmd;
			if (parameter == "DigitalOutput1")
			{
				cmd = String.Format("DM;{0:D3};{1:D5};{2:D2}", seq, Id, value == "true" ? 94 : 95);
			}
			else if (true)
			{
				cmd = String.Format("CP;{0:D3};{1:D5};{2};{3};{4:D5}", seq, Id, parameter, value, hash);
			}
			SendMessages(BuildLine(cmd, seq), messageId);
			return true;
		}

		#endregion

		#region IFuelControl

		public bool DisableFuel(ulong messageId, bool immediately)
		{
			var seq = NextSequence;
			var cmd = String.Format("DM;{0:D3};{1:D5};{2:D2}", seq, Id, immediately ? 90 : 91);
			SendMessages(BuildLine(cmd, seq), messageId);
			return true;
		}

		public bool EnableFuel(ulong messageId)
		{
			var seq = NextSequence;
			var cmd = String.Format("DM;{0:D3};{1:D5};{2:D2}", seq, Id, 92);
			SendMessages(BuildLine(cmd, seq), messageId);
			return true;
		} 

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

		private void CheckFota(ulong msgid)
		{
			if (LastSent.GetId() == msgid)
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

		#region LogToFile

		private void Log(String buffer, String reply, String post, String ip)
		{
			try
			{
				lock (Locker)
				{
					var filePath = String.Format("{0}{1:yyyy-MM-dd HH}.log", DirPath, DateTime.Now);
					using (var sw = new StreamWriter(filePath, true))
					{
						sw.WriteLine("{0:yyyy-MM-dd HH mm ss};ip={1};rx={2};tx={3};posttx={4}", DateTime.Now, ip, buffer, reply, post);
						sw.Close();
					}
				}
			}
			catch (Exception e)
			{
				STrace.Exception("LogToFile", e);
			}
		}

		private static readonly Object Locker = new Object();

		private String _dirPath;
		private String DirPath
		{
			get
			{
				if (_dirPath == null)
				{
					var myPath = Assembly.GetExecutingAssembly().Location;
					var i = myPath.LastIndexOf('\\');
					_dirPath = String.Format("{0}\\logs\\{1} ", myPath.Remove(i + 1), STrace.Module);
				}

				return _dirPath;
			}
		}

		#endregion

	}
}