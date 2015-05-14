#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

#endregion

namespace Logictracker.Layers
{ 
    public static class Fota
	{
        private const string commandPattern = @"(?<command>\>[^\<]*\<)";
        private const string notSentPattern = @"\>[^\<]*(?<![^\>]*\b\;STATUS\=SENT\b[^\<]*)\<";
        
		#region Public Members
        public static String Peek(IFoteable parser)
        {
            return Peek(parser, false);
        }

        private static bool IsVirtualMessage(string message)
        {
            return message != null && message.StartsWith(VirtualMessagePrefix);
        }

        private static bool IsLastSequenceMessage(string message)
        {
            return message != null && message.StartsWith(LastSequence);
        }

       	public static String Peek(IFoteable parser, bool returnVMessages)
		{
			try
			{
				if (ParserUtils.IsInvalidDeviceId(parser.GetDeviceId())) return null;
				
				//GetInfo
			    var t = new TimeElapsed();

				var pending = PeekPrivate(parser);

                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", parser.Id, String.Format("Peek Private({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));

                while (IsVirtualMessage(pending) && !returnVMessages)
				{
					var d1 = pending.Split(';');
					var code = Convert.ToInt32(d1[0].Split('=')[1]);
					var mid = Convert.ToUInt64(d1[1].Split('=')[1]);

                    try
                    {
                        if (code != (int) MessageIdentifier.AckEvent)
                        {
                            t.Restart();
                            var message = ((MessageIdentifier) code).FactoryEvent(parser.Id, mid, null, DateTime.UtcNow,
                                                                                  null, null);
                            parser.DataTransportLayer.DispatchMessage(parser, message);
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", parser.Id, String.Format("DispatchMessage ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                        }
                        else
                        {
                            t.Restart();
                            STrace.Debug(typeof (Fota).FullName, parser.Id,
                                         String.Format("Fota Ack message id = {0}", mid));
                            var msg = new UserMessage(parser.Id, mid)
                                .SetUserSetting("user_message_code", "ACK")
                                .SetUserSetting("trackingId", mid.ToString(CultureInfo.InvariantCulture));
                            parser.DataTransportLayer.DispatchMessage(parser, msg);
                            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", parser.Id, String.Format("Fota Ack({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                        }
                        t.Restart();
                        Dequeue(parser, null);
                        if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", parser.Id, String.Format("Peek Dequeue ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                    } 
                    catch (Exception)
                    {
                        STrace.Error(typeof(Fota).FullName, parser.GetDeviceId(), "Error in DataTransportLayer.DispatchMessage?");
                    }
                    t.Restart();
				    pending = PeekPrivate(parser);
                    if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", parser.Id, String.Format("Peek Private({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
				}

			    return (pending == null ? null : CheckPending(parser, pending));

			}
			catch (Exception e)
			{
				STrace.Debug(typeof(Fota).FullName, parser.GetDeviceId(), String.Format("Error al Fotear {0}", e));
                return null;
			}			
		}

	    private static String CheckPending(IFoteable parser, String pending)
	    {
            var bydiff = (parser.LastSent.GetText(null) != pending);
	        var bytime = parser.LastSent.IsExpired();

	        var t = new TimeElapsed();
            var md = parser.DataProvider.GetDetalleDispositivo(parser.GetDeviceId(), "GTE_MESSAGING_DEVICE");

            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", parser.Id, String.Format("Peek DetalleDispositivo({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));


            var dc = BaseDeviceCommand.createFrom(pending, parser, null);
	        var isGMessage = dc.isGarminMessage();
	        var garminConnected = (md != null && md.Valor == MessagingDevice.Garmin) && (parser.IsGarminConnected == true);

            if (isGMessage && !garminConnected)
            {
                if (dc.getStatus() != BaseDeviceCommand.Attributes.Status_GarminNotConnected)
                    dc = UpdateFotaCommand(parser, null, BaseDeviceCommand.Attributes.Status_GarminNotConnected, UpdateFotaCommands.UpdateTries);
                parser.LastSent = null;
                return null;
            }

			if (bydiff || bytime)
			{                
                if (bytime)
                {
                    t.Restart();
                    dc = UpdateFotaCommand(parser, null, null, UpdateFotaCommands.UpdateTries);
                    if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", parser.Id, String.Format("Peek CheckPending UpdateFotaCommand ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                }
			    var result = (dc != null ? dc.ToString() : null);

			    parser.LastSent = (result == null ? null : new INodeMessage(dc.MessageId ?? 0, result, DateTime.UtcNow));
                
			    return result;
			}
			return null;
		}
					
		public static void Dequeue(IFoteable Device, ulong? msgid)
		{
			if (!Device.LastSent.IsOnTheFly())
			{
			    var t = new TimeElapsed();
				UpdateFotaCommand(Device, msgid, BaseDeviceCommand.Attributes.Status_Sent);
                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", Device.Id, String.Format("Dequeue ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
			}
			Device.LastSent = null;
		}

		public static void ResetStateMachine(IFoteable Device)
		{
			Device.LastSent = null;

			Archivar(Device, ".txt");
			Archivar(Device, ".txR");
			Archivar(Device, ".txR2");
			Archivar(Device, ".tx1");
			Archivar(Device, ".tx2");
		}

        public static void EnqueueGarmin(IFoteable Device, ulong MessageId, String Commands)
        {
            Enqueue(Device, MessageId, Commands);
        }

		public static void Enqueue(IFoteable Device, ulong MessageId, String Commands)
		{
            if (!Commands.EndsWith(Environment.NewLine)) Commands += Environment.NewLine;
			if (MessageId != 0) Commands = Commands + VirtualMessageFactory(MessageIdentifier.AckEvent, MessageId);
			EnqueuePrivate(Device, MessageId, Commands, ".txt");
		}

        public static void EnqueueOnTheFly(IFoteable Device, ulong MessageId, BaseDeviceCommand[] commands)
        {
            var zz = (IMessage) null;
            EnqueueOnTheFly(Device, MessageId, commands, ref zz);
        }

        public static void EnqueueOnTheFly(IFoteable Device, ulong MessageId, BaseDeviceCommand[] arrCmds, ref IMessage msg)
        {
            var reversed = arrCmds.Reverse();

            var commands = String.Join("", reversed.Select(bdc => bdc.ToString(false)).ToArray());

            var toSend = reversed.Last();
            var toSendStr = toSend.ToString(true);

            if (msg != null)
                msg.AddStringToSend(toSendStr);

            if (Device.LastSent == null)
            {
                if (msg != null)
                    Device.LastSent = new INodeMessage(toSend.MessageId ?? 0, toSendStr, DateTime.UtcNow);
                EnqueuePrivate(Device, MessageId, commands, ".txR");
            }
            else
            {
                InsertAfterLastFotaCommand(Device, commands);
            }
        }

        public static void EnqueueOnTheFly(IFoteable Device, ulong MessageId, string commands)
        {
            var zz = (IMessage)null;
            EnqueueOnTheFly(Device, MessageId, commands, ref zz);
        }

        public static void EnqueueOnTheFly(IFoteable Device, ulong MessageId, string command, ref IMessage msg)
        {
            var lines = command.SplitLines().Reverse();
            var reversed = lines.JoinLines();

            var toSendStr = lines.Last();

            if (msg != null)
                msg.AddStringToPostSend(toSendStr);

            if (Device.LastSent == null)
            {
                EnqueuePrivate(Device, MessageId, reversed, ".txR");
                if (msg != null)
                    Device.LastSent = new INodeMessage(MessageId, toSendStr, DateTime.UtcNow);
            }
            else
                InsertAfterLastFotaCommand(Device, reversed.ToString());
        }        

        public static void EnqueueLowPriority1(IFoteable Device, ulong MessageId, String Commands)
		{
			EnqueuePrivate(Device, MessageId, Commands, ".tx1");
		}

		public static void EnqueueLowPriority2(IFoteable Device, ulong MessageId, String Commands)
		{
			EnqueuePrivate(Device, MessageId, Commands, ".tx2");
		}

        public static void InsertAfterLastFotaCommand(IFoteable device, string commands)
        {
            var file = ObtainFirstExistantReversedFile(device);
            if (file == null) {
                EnqueuePrivate(device, 0, commands, ".txR");
                return;
            } 

            using (var sr = new BackwardReader(file))
            {
                String line;
                if ((line = sr.ReadLine()) != null)
                {

                    while (line != null && IsLastSequenceMessage(line))
                    {
                        line = sr.ReadLine();
                    }

                    if (line != null)
                    {
                        if (!commands.EndsWith(Environment.NewLine)) commands += Environment.NewLine;
                        sr.Write(commands);
                        sr.Write(line + Environment.NewLine);
                    }
                    if (device.Sequence != null)
                        sr.Write(LastSequenceFactory(device));
                }
                sr.TruncateHere();
            }
        }

        public enum UpdateFotaCommands
        {
            None,
            UpdateTries,
            RollbackTransaction
        }

        public static String UpdateFotaCommand(IFoteable device, ulong? msgid, string newStatus)
        {
            var replaced = UpdateFotaCommand(device, msgid, newStatus, null, UpdateFotaCommands.None);
            return (replaced != null?replaced.ToString():null);
        }

        public static BaseDeviceCommand UpdateFotaCommand(IFoteable device, ulong? msgid, string newStatus, UpdateFotaCommands action)
        {
            return UpdateFotaCommand(device, msgid, newStatus, null, action);
        }


        public static BaseDeviceCommand RollbackLastTransaction(IFoteable device, ulong? msgid)
        {
            return UpdateFotaCommand(device, msgid, BaseDeviceCommand.Attributes.Status_Rollbacked, null,
                                     UpdateFotaCommands.RollbackTransaction);
        }

        private static string ObtainFirstExistantReversedFile(INode device)
        {
            return
                new[] { ".txR", ".txR2", ".txG", ".txG2" }.Select(ext => GetFilePath(device, ext)).FirstOrDefault(IOUtils.FileExists);
        }

        private static string ObtainFirstExistantReversedFile(IFoteable device)
        {
            return
                new[] {".txR", ".txR2", ".txG", ".txG2"}.Select(ext => GetFilePath(device, ext)).FirstOrDefault(
                    IOUtils.FileExists);
        }

        public static BaseDeviceCommand UpdateFotaCommand(IFoteable device, ulong? msgid, string newStatus, BaseDeviceCommand newCommand, UpdateFotaCommands action)
		{

		    var result = (BaseDeviceCommand) null;

            var file = ObtainFirstExistantReversedFile(device);
			if (file == null) return null;

			using (var sr = new BackwardReader(file))
			{
			    var buffer = new StringBuilder();
			    var msgidfound = false;

				String line;
				while ((line = sr.ReadLine()) != null) // reads the lines from last to first
				{
                    var lineOriginal = line = line.Trim();

                    if (IsLastSequenceMessage(line)) continue;
                    if (IsVirtualMessage(line)) break;

                    var newLine = new StringBuilder();
                    
                    var isFirstCommand = true; // at begin of line evaluation, the next command in line is the first

                    MatchCollection commandMatches = getMatchesAllCommandsFrom(line);
				    var allSent = commandMatches.Count > 0; // the variable reflects if all commands in line were sent.
                    for (var i = 0; i < commandMatches.Count; i++)
                    {
                        var cM = commandMatches[i];
                        var command = cM.Groups[0].Value;                        

                        var dc = BaseDeviceCommand.createFrom(command, device, null);
                        if (!dc.isAlreadySent())
                        {                            
                            if (msgid == null || dc.MessageId == msgid)
                            {
                                allSent = allSent && (action != UpdateFotaCommands.RollbackTransaction && isFirstCommand && newStatus == BaseDeviceCommand.Attributes.Status_Sent);

                                msgidfound = true;
                                if (isFirstCommand)
                                {
                                    isFirstCommand = false;

                                    if (newCommand != null)
                                    {
                                        dc = newCommand;
                                        dc.IdNum = device.GetDeviceId();
                                    }

                                    if (action == UpdateFotaCommands.UpdateTries || action == UpdateFotaCommands.RollbackTransaction)
                                    {
                                        dc.Tries = (dc.Tries == null ? 0 : dc.Tries.Value) + 1;
                                        dc.LastTry = DateTime.Now;
                                    }

                                    if (action == UpdateFotaCommands.RollbackTransaction)
                                        dc.setStatus(BaseDeviceCommand.Attributes.Status_Rollbacked);
                                    else
                                        dc.setStatus(newStatus);
                                }
                                else if (action == UpdateFotaCommands.RollbackTransaction)
                                {
                                    dc.Tries = (dc.Tries == null ? 0 : dc.Tries.Value) + 1;
                                    dc.LastTry = DateTime.Now;
                                    dc.setStatus(BaseDeviceCommand.Attributes.Status_Rollbacked);
                                }



                                if (result == null) // just catch first not sent message
                                    result = dc;
                            }
                            else
                            {
                                allSent = false;
                            }
                        }
                        
                        newLine.Append(dc.ToString());
                    }
				    line = (allSent?null:newLine.ToString());
                    if (!String.IsNullOrEmpty(line))
                    {
                        if (lineOriginal != line)
                        {
                            STrace.Debug(typeof (Fota).FullName, device.Id, "FOTA> LINE ORIGINAL: " + lineOriginal);
                            STrace.Debug(typeof (Fota).FullName, device.Id, "FOTA> LINE REPLACED: " + line);
                        }
                        buffer.Insert(0, line + Environment.NewLine);
                    }
                    else
                    {
                        STrace.Debug(typeof(Fota).FullName, device.Id,      "FOTA> LINE DELETED : " + lineOriginal);
                    }

				    if (msgidfound) break;
				}
                if (buffer.Length != 0)
                    sr.Write(buffer.ToString());                
                if (device.Sequence != null)
                    sr.Write(LastSequenceFactory(device));
                sr.TruncateHere();
			}

            if (IOUtils.FileExists(file))
            {
                using (var sr = new BackwardReader(file))
                {
                    var allLastSequence = true;
                    String line;
                    while (allLastSequence && (line = sr.ReadLine()) != null)
                    {
                        allLastSequence = allLastSequence && IsLastSequenceMessage(line);
                    }
                    sr.Dispose();
                    if (allLastSequence)
                        File.Delete(file);
                }
            }
            return result;
		}

		public static String VirtualMessageFactory(MessageIdentifier eventCode, ulong messageId)
		{
            return String.Format("{0}{1};TrackingId={2};{3}", VirtualMessagePrefix, ((int)eventCode), messageId, Environment.NewLine);
		}

		public static String FindLast(IFoteable Device, String Text, bool lowPriorityFlag)
		{
            var file = ObtainFirstExistantReversedFile(Device);

			if (file == null) return null;

			using (var sr = new StreamReader(file))
			{
				String line;
				while ((line = sr.ReadLine()) != null)
				{
                    var value = FindLastInLineNotSent(line, Text);
                    if (value != null)
                        return value;
				}
			}
			return null;
		}

		#endregion

		#region Private Members

		private static void Archivar(IFoteable Device, String ext)
		{
			var src = GetFilePath(Device, ext);
			if (!IOUtils.FileExists(src)) return;
			var dst = src.Replace(ext, String.Format(" - {0:yyyy-MM-dd HH mm ss}{1}", DateTime.Now, ext));
			File.Move(src, dst);
		}

		private static String PeekPrivate(IFoteable device)
		{
			if (device.LastSent.IsOnTheFly())
			{
				return device.LastSent.Text;
			}
		    var t = new TimeElapsed();
            var fileR = GetFilePath(device, ".txR");
            var fileT = GetFilePath(device, ".txt");
			var fileR2 = GetFilePath(device, ".txR2");            
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private GetFilePath ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));

			//txR --- txt --- txR2 --- tx1 --- tx2
			//process				
			//        reverse
			//                process
			//                        reverse
			//                                 reverse
            
            t.Restart();
            if ((!IOUtils.FileExists(fileR)) && (IOUtils.FileExists(fileT) || !IOUtils.FileExists(fileR2)))
			{
                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private FileExists ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
                t.Restart();
                ProccessAnyPendingFileInternal(device);                
                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private ProcessAnyPending ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
/*				ThreadPool.QueueUserWorkItem(ProccessAnyPendingFile, device); return null;*/
			}

			//process
            t.Restart();
            var file = ObtainFirstExistantReversedFile(device);
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private ObtainFirstExistantReversedFile ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
			if (file == null) return null;            
    		String line;
		    var validLines = 0;

            t.Restart();
		    var lineNumber = 0;
		    using (var sr = new BackwardReader(file))
			{
                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private using BackwardReader ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
				while ((line = sr.ReadLine()) != null)
				{
				    lineNumber++;
                    if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private Readline {1} ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString(), lineNumber));
				    line = line.Trim();
				    if (line.Contains("/*"))
				        line = line.Substring(0, line.IndexOf("/*"));

                    if (IsLastSequenceMessage(line))
                    {                        
                        continue;
                    }
				    var t2 = new TimeElapsed();
                    if (IsVirtualMessage(line) || (line = GetNextInLineNotSentCommand(line)) != null)
                    {
                        validLines++;
                        break;
                    }                    
                    if (t2.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private IsVirtual+GetNextInLine {1} ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString(), lineNumber));
				}			
            }    
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private end of BackwardReader ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));            
            if (validLines == 0)
            {
                t.Restart();
                File.Delete(file);
                if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", device.Id, String.Format("Peek Private Delete File ({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
            }            
			return line;
        }
        

        private static String FindLastInLineNotSent(String line, String string2Find)
        {
            String result = null;

            MatchCollection matches = getMatchesCommandsNotSentFrom(line);

            foreach (Match m in matches)
            {
                if (m.Groups.Count > 0 && m.Groups[0].Success)
                {
                    if (m.Groups[0].Value.Contains(string2Find))
                        result = m.Groups[0].Value;
                }                
            }

            return result;
        }

        private static MatchCollection getMatchesAllCommandsFrom(String line)
        {
            return Regex.Matches(line, commandPattern);            
        }

        private static MatchCollection getMatchesCommandsNotSentFrom(String line)
        {
            return Regex.Matches(line, notSentPattern);
        }       

        private static String GetNextInLineNotSentCommand(String line)
        {

            String result = null;

            MatchCollection matches = getMatchesCommandsNotSentFrom(line);

            if (matches.Count > 0 && matches[0].Groups.Count > 0 && matches[0].Groups[0].Success)
            {
                result = matches[0].Groups[0].Value;
            }
            return result;
        }

        private static void ProccessAnyPendingFile(Object obj)
        {
			var baseCodec = obj as IFoteable;

            if (baseCodec != null)
            {
				baseCodec.ExecuteOnGuard(() => ProccessAnyPendingFileInternal(baseCodec), "ProccessAnyPendingFile", "");
            }
	    }

	    private static void ProccessAnyPendingFileInternal(IFoteable device)
		{
			//txR --- txt --- txR2 --- tx1 --- tx2
			//process				
			//        reverse
			//                process
			//                        reverse
			//                                 reverse

			var fileR = GetFilePath(device, ".txR");
			var fileT = fileR.Replace(".txR", ".txt");
		    var fileR2 = fileR.Replace(".txR", ".txR2");
			var file1 = fileR.Replace(".txR", ".tx1");
			var file2 = fileR.Replace(".txR", ".tx2");
			var fileBck = fileR.Replace(".txR", ".bck");

            if (IOUtils.FileExists(fileR)) return;
            if (!IOUtils.FileExists(fileT) && IOUtils.FileExists(fileR2)) return;


		    var fileAct = new List<String> { fileT, file1, file2 }
                .FirstOrDefault(IOUtils.FileExists);
			if (fileAct == null) return;

            if (IOUtils.FileExists(fileBck)) Archivar(device, ".bck");

			using (var sr = new BackwardReader(fileAct))
			{
				using (var sw = new StreamWriter(fileBck))
				{
					String line;
					while ((line = sr.ReadLine()) != null)
					{
						if (!String.IsNullOrEmpty(line) && !IsLastSequenceMessage(line))
							sw.WriteLine(line);
					}
                    if (device.Sequence != null)
                        sw.Write(LastSequenceFactory(device));
					sw.Close();
				}
			}
			File.Delete(fileAct);
			File.Move(fileBck, (fileAct == fileT) ? fileR : fileR2);
		}

		private static void EnqueuePrivate(IFoteable device, ulong messageId, String commands, String ext)
		{
			try
			{
                if (!commands.EndsWith(Environment.NewLine)) commands += Environment.NewLine;

				var file = GetFilePath(device, ext);
                var fexists = IOUtils.FileExists(file);
                if (fexists)
                {
                    using (var sr = new BackwardReader(file))
                    {
                        String line;
                        if ((line = sr.ReadLine()) != null)
                        {
                            while (line != null && IsLastSequenceMessage(line))
                            {
                                line = sr.ReadLine();
                            }
                            if (line != null)
                                sr.Write(line + Environment.NewLine);
                            sr.Write(commands);
                            if (messageId != 0)
                                sr.Write(VirtualMessageFactory(MessageIdentifier.AckEvent, messageId));
                            if (device.Sequence != null)
                                sr.Write(LastSequenceFactory(device));
                            sr.TruncateHere();
                        }
                    }
                } else
                {
                    using (var sw = new StreamWriter(file, true))
                    {
                        sw.Write(commands);
                        if (messageId != 0) sw.WriteLine(VirtualMessageFactory(MessageIdentifier.AckEvent, messageId));
                        if (device.Sequence != null)
                            sw.Write(LastSequenceFactory(device));
                        sw.Close();
                    }                    
                }
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(Fota).FullName, e, device.Id);
			}
		}

        private static string LastSequenceFactory(IFoteable device)
        {
            return LastSequence + "=" + device.Sequence + Environment.NewLine;
        }

        private static UInt32? ExtractLastSequence(string line)
        {
            var arrLine = line.Split(';');
            if (arrLine.Length > 0)
            {
                var arrParams = line.Split('=');
                if (arrParams.Length == 2 && arrParams[0] == LastSequence)
                {
                    var ns = UInt32.Parse(arrParams[1]);
                    return ns;
                }
            }
            return null;
        }

        private static String GetFilePath(INode Device, String ext)
        {
            var t = new TimeElapsed();
            var path = Process.GetApplicationFolder("FOTA");
            if (t.getTimeElapsed().TotalSeconds > 1) STrace.Debug("ParserLock", Device.Id, String.Format("Inside GetFilePath (GetApplicationFolder)({0} secs)", t.getTimeElapsed().TotalSeconds.ToString()));
            return GetFilePath(Device.Id, path, ext);
        }

        private static String GetFilePath(IFoteable Device, String ext)
        {
            return GetFilePath(Device.Id, Device.FotaFolder, ext);
        }

        private static String GetFilePath(int DeviceId, String Path, String ext)
        {
            return String.Format(@"{0}\{1:D4}{2}", Path, DeviceId, ext);
        }


/*
        private static String GetFilePath(int DeviceId, String ext)
		{
			return GetFilePath(DeviceId, Process.GetApplicationFolder("FOTA"), ext);
		}
*/
		/*private static void MessageSupervisorCallback(IAsyncResult ar)
		{
			var result = ar as MessageSupervisionResult;
			Debug.Assert(result != null);
			var msg = ar.AsyncState as IMessage;
			Debug.Assert(msg != null);
			//STrace.Trace(GetType().FullName,"MessageSupervisorCallback: did={0};mid={1}", msg.DeviceId, msg.UniqueIdentifier);

			if (result.IsCompleted)
			{
				//SelfEvent(MessageIdentifier.RebootTimeout);
				RemoveSupervisor(msg.UniqueIdentifier, msg.DeviceId);
			}
			else
			{
				//SelfEvent(MessageIdentifier.RebootSuccess);
				MessageSupervisor.Inquired(result);
				DevCache.Set<String>(msg.DeviceId, "LastMessageTime", null);

				var node = FindNode(msg.DeviceId);

				if (DataTransportLayer != null)
					DataTransportLayer.SendMessage(msg.DeviceId, msg);
			}
		}//*/

		/*private static void RemoveSupervisor(ulong uid, int nodecode)
		{
			var Supervisors = LogicCache.Retrieve<Dictionary<ulong, MessageSupervisionResult>>(typeof(Dictionary<ulong, MessageSupervisionResult>), "Supervisors:" + nodecode);
			Supervisors.Remove(uid);
			LogicCache.Store(typeof(Dictionary<ulong, MessageSupervisionResult>), "Supervisors:" + nodecode, Supervisors);
		}//*/

		/*private TimeSpan[] GetTimeSpans()
		{
			return (new[] {32, 16, 8, 8}).Select(j => new TimeSpan(0, 0, j)).ToArray();
		}//*/

		private const String VirtualMessagePrefix = "DispatchEvent=";
        private const String LastSequence = "LastSequence";

        #endregion

        public static UInt32? ObtainLastSequence(INode device)
        {
            UInt32? result = null;
            var file = ObtainFirstExistantReversedFile(device);
            if (file == null)
                return null;

            using (var sr = new BackwardReader(file))
            {
                var line = String.Empty;
                if ((line = sr.ReadLine()) != null)
                {
                   result = ExtractLastSequence(line);
                }
            }
            return result;
        }
	}
}