using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Layers;
using Logictracker.Model;

namespace Logictracker.Unetel.v2.UnexLibs
{
	public class Indication
	{
		public const String indicatedIMEI = "indicatedIMEI";
		public const String indicatedSecret = "indicatedSecret";
		public const String indicatedFirmwareSignature = "indicatedFirmwareSignature";
		public const String indicatedConfigurationRevision = "indicatedConfigurationRevision";
		public const String indicatedCannedMessagesTableRevision = "indicatedCannedMessagesTableRevision";
		public const String indicatedQuadTreeSignature = "indicatedQuadTreeSignature";
		public const String indicatedQuadTreeRevision = "indicatedQuadTreeRevision";
	}

	public static class UnexUtils
	{
		public static void ForceReplyCheckingFota(this IMessage msg, bool canappend, IFoteable dev)
		{
			if ((msg == null) || !canappend) return;

			var pending = Fota.Peek(dev);
			if (!String.IsNullOrEmpty(pending))
			{
				var lmid = dev.GetMessageId(pending);
				if (pending.Contains("///;#"))
				{
					pending = pending.Substring(0, pending.IndexOf("///;#"));
				}

				var lastmsg = dev.LastSent.GetText(null);
				STrace.Debug(typeof (UnexUtils).FullName, dev.GetDeviceId(), String.Format("ForceReplyCheckingFota 2: lastmsg={0} pending={1}", lastmsg, pending));
				if (lastmsg != pending)
				{
					dev.LastSent = new INodeMessage(lmid, pending, DateTime.UtcNow);
				}
				msg.AddStringToSend(pending);
			}
			else
			{
				//HACK: si no le respondo algo a estos equipos se cuelgan un rato y pasan a estar en amarillo
				if (String.IsNullOrEmpty(msg.GetPendingAsString())) 
					msg.AddStringToSend("B");
			}
		}
	}

	public class SafeConvert
	{
		public static Int32 ToInt32<T>(T source, Int32 @default)
		{
			try
			{
				return Convert.ToInt32(source);
			}
			catch
			{
				return @default;
			}
		}
	}

	public class Message
	{
		public readonly ulong Sequence;
		public readonly int MessageId;
		public readonly String CommandStr;

		private Message(ulong sequence, int messageId, String command)
		{
			Sequence = sequence;
			MessageId = messageId;
			CommandStr = command;
		}

		public static String BuildString(ulong sequence, int messageId, String command)
		{
			return String.Format("{0}///Sequence:{1};#MessageId:{2}", command, sequence, messageId);
		}

		public static Message BuildMessage(String msg)
		{
			if (String.IsNullOrEmpty(msg)) return null;
			var lim = msg.IndexOf("///");
			if (lim < 1) return null;
			var info = msg.Substring(lim + 3).Split(';');
			var cmd = msg.Substring(0, lim);
			return new Message(Convert.ToUInt32(info[0].Split(':')[1]), Convert.ToInt32(info[1].Split(':')[1]), cmd);
		}
	}
}
