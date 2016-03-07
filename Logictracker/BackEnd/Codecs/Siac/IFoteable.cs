using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Cache;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Layers;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

namespace Logictracker.Siac
{
    public partial class Parser : IFoteable
    {
        private const String VirtualMessagePrefix = "DispatchEvent=";

        #region IFoteable

        private String _fotaFolder;

        private Object fotaFolder = new Object();

        public String FotaFolder
        {
            get
            {
                if (_fotaFolder == null)
                {
                    lock (fotaFolder)
                    {
                        var disp = DataProvider.GetDispositivo(Id);
                        var empresa = disp.Empresa;
                        var subFolder = IOUtils.CleanFileName(empresa.RazonSocial.ToLower() + "." + empresa.Id);

                        var fotaRootFolder = Process.GetApplicationFolder("FOTA");

                        _fotaFolder = fotaRootFolder + "\\" + subFolder + "\\";
                        STrace.Debug(typeof(Parser).FullName, Id, String.Format("Fota Folder: {0}", _fotaFolder));
                        if (!Directory.Exists(_fotaFolder))
                            Directory.CreateDirectory(_fotaFolder);

                        string filesToDelete = String.Format("{0}.*", Id);
                        string[] fileList = Directory.GetFiles(fotaRootFolder, filesToDelete);
                        foreach (string file in fileList)
                        {
                            string fileToMove = file;
                            string moveTo = _fotaFolder + Path.GetFileName(file);

                            try
                            {
                                //moving file
                                File.Move(fileToMove, moveTo);
                            }
                            catch (Exception ex)
                            {
                                STrace.Exception(typeof(Parser).FullName, ex, Id, String.Format("Error moving FOTA file from '{0}' to '{1}'", fileToMove, moveTo));
                            }
                        }
                    }
                }
                return _fotaFolder;
            }
        }

        public bool ReloadFirmware(ulong messageId)
        {
            SendMessages(GetFirm(), messageId, null);
            return true;
        }

        public bool ReloadMessages(ulong messageId)
        {
            string md = DataProvider.GetDetalleDispositivo(Id, "GTE_MESSAGING_DEVICE").As(String.Empty);

            var messages = new StringBuilder();
            GetMessages(ref messages);
            SendMessages(messages.ToString(), messageId, md);
            return true;
        }

        public bool ResetFMIOnGarmin(ulong messageId)
        {
            var config = new StringBuilder();
            config.Append(GarminFmi.EncodeDataDeletionFor(DataDeletionProtocolId.DeleteFleetManagementInterfaceOnTheClient).ToTraxFM(this, false));
            GarminSetup(ref config, this);
            GetMessages(ref config, false);
            SendMessages(config.ToString(), messageId, MessagingDevice.Garmin);
            return true;
        }

        public bool ReloadConfiguration(ulong messageId)
        {
            String dummyhash;
            var config = new StringBuilder();
            config.Append(GetConfig(out dummyhash));
            config.AppendFormat("{0}{1}", ">SSR55AA<", Environment.NewLine); //Reseteo el equipo luego de enviarle la nueva configuracion
            SendMessages(config.ToString(), messageId, null);
            return true;
        }

        public bool ClearDeviceQueue(ulong messageId)
        {
            var config = new StringBuilder();
            config.AppendFormat("{0}{1}", ">SDLE<", Environment.NewLine); //Borro la cola de eventos/posiciones del dispositivo
            SendMessages(config.ToString(), messageId, null);
            return true;
        }

        public Boolean ContainsMessage(String line)
        {
            int ini = line.IndexOf('>');
            if (ini < 0) return false;
            int len = (line.IndexOf('<', ini) - ini) + 1;
            if (len < 3) return false;
            return !line.Contains(Reporte.SetIdPrefix);
        }

        public INodeMessage LastSent { get; set; }

        public ulong GetMessageId(String line)
        {
            return ParserUtils.GetMsgIdTaip(line);
        }

        #endregion

        #region SendMessages

        private void SendMessages(BaseDeviceCommand[] cmd, String md)
        {
            var lines = cmd.Select(dc => dc.ToString() + Environment.NewLine);
            var cmds = lines.ToString();
            SendMessages(cmds, ParserUtils.MsgIdNotSet, md);
        }

        private void SendMessages(BaseDeviceCommand[] cmd, ulong msgid, String md)
        {
            var lines = cmd.Select(dc => dc.ToString() + Environment.NewLine);
            var cmds = lines.ToString();
            SendMessages(cmds, msgid, md);
        }

        private void SendMessages(String config, ulong messageId, String md)
        {
            if (string.IsNullOrEmpty(config))
                return;

            switch (md)
            {
                case MessagingDevice.Garmin:
                    Fota.EnqueueGarmin(this, messageId, config);
                    break;
                default:
                    Fota.Enqueue(this, messageId, config);
                    break;
            }
            /*            IMessage msg = null;
                        SendPendingFota(ref msg);
                        if (msg == null) return;

                        STrace.Debug(typeof (Parser).FullName, Id, String.Format("Enviando: {0}", msg.GetPendingAsString()));
                        DataLinkLayer.SendMessage(Id, msg); */
        }

        #endregion

        private bool _sdSession;

        private String GetFirm()
        {
            string[] fw =
                Encoding.ASCII.GetString(DataProvider.GetFirmware(Id)).Split(new[] { "\r\n", "\n", Environment.NewLine },
                                                                             StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < fw.Length; i++)
            {
                fw[i] = String.Format(">SFUS{0:X4}{1}<", i, fw[i]);
            }
            return String.Join(Environment.NewLine, fw) + fw[0].Replace(">SFUS0000", ">SFUSFFFF") + Environment.NewLine +
                   ">SSR55AA<" + Environment.NewLine +
                   Fota.VirtualMessageFactory(MessageIdentifier.FotaSuccess, NextSequence);
        }

        private String GetCannedForGarmin(Logictracker.Types.BusinessObjects.Messages.Mensaje[] messagesParameters, UInt16 setFmiPacketId, UInt16 deleteFmiPacketId)
        {
            var config = new StringBuilder();

            foreach (Logictracker.Types.BusinessObjects.Messages.Mensaje m in messagesParameters)
            {
                int cod = Convert.ToInt32(m.Codigo);
                config.Append(GarminFmi.EncodeSetCanned(cod, "code=" + cod + ";" + m.Texto, setFmiPacketId).ToTraxFM(this, false));
            }
            return config.ToString();
        }


        private String GetResponsesForGarmin(Logictracker.Types.BusinessObjects.Messages.Mensaje[] messagesParameters, UInt16 setFmiPacketId, UInt16 deleteFmiPacketId)
        {
            var config = new StringBuilder();

            foreach (Logictracker.Types.BusinessObjects.Messages.Mensaje m in messagesParameters)
            {
                int cod = Convert.ToInt32(m.Codigo);
                config.Append(GarminFmi.EncodeSetCanned(cod, m.Texto, setFmiPacketId).ToTraxFM(this, false));
            }

            return config.ToString();
        }

        private String GetCannedMessages4Garmin(Logictracker.Types.BusinessObjects.Messages.Mensaje[] messagesParameters)
        {
            return GetCannedForGarmin(messagesParameters, FmiPacketId.ScSetCannedMessage,
                                      FmiPacketId.ScDeleteCannedMessage);
        }

        private String GetCannedResponses4Garmin(Logictracker.Types.BusinessObjects.Messages.Mensaje[] messagesParameters)
        {
            return GetResponsesForGarmin(messagesParameters, FmiPacketId.ScSetCannedResponse,
                                      FmiPacketId.ScDeleteCannedResponse);
        }

        private void GetMessages(ref StringBuilder config)
        {
            GetMessages(ref config, true);
        }

        private void GetMessages(ref StringBuilder config, bool deleteMessagesPreviously)
        {
            string md = DataProvider.GetDetalleDispositivo(Id, "GTE_MESSAGING_DEVICE").As(String.Empty);

            if (md == MessagingDevice.Garmin || md == MessagingDevice.Mpx01)
            {

                // cargar todos los posibles responses
                switch (md)
                {
                    case MessagingDevice.Garmin:
                        List<Logictracker.Types.BusinessObjects.Messages.Mensaje> resp = DataProvider.GetResponsesMessagesTable(Id, 0);
                        if (deleteMessagesPreviously)
                            config.Append(GarminFmi.EncodeDataDeletionFor(DataDeletionProtocolId.DeleteAllCannedRepliesOnTheClient).ToTraxFM(this, false));
                        if (resp != null && resp.Any())
                        {
                            var respArr = resp.OrderBy(m => m.Codigo).ToArray();
                            config.Append(GetCannedResponses4Garmin(respArr));
                        }
                        break;
                    default:
                        break;
                }
                config.Append(Environment.NewLine);

                //cargar todos los canned messages
                List<Logictracker.Types.BusinessObjects.Messages.Mensaje> cmt = DataProvider.GetCannedMessagesTable(Id, 0);
                {
                    if ((cmt != null) && (cmt.Any()))
                    {
                        Logictracker.Types.BusinessObjects.Messages.Mensaje[] messagesParameters =
                            cmt.Where(m => !m.TipoMensaje.DeEstadoLogistico).OrderBy(m => m.Codigo).ToArray();

                        switch (md)
                        {
                            case MessagingDevice.Garmin:
                                if (deleteMessagesPreviously)
                                    config.Append(GarminFmi.EncodeDataDeletionFor(DataDeletionProtocolId.DeleteAllCannedMessagesOnTheClient).ToTraxFM(this, false));
                                config.Append(GetCannedMessages4Garmin(messagesParameters));
                                break;
                            case MessagingDevice.Mpx01:
                                int count = 0;
                                foreach (Logictracker.Types.BusinessObjects.Messages.Mensaje m in messagesParameters.Take(10))
                                {
                                    count++;
                                    string texto = m.Texto.ToUpper().PadRight(32);
                                    config.Replace(String.Format("$MENSAJE{0:D2}PARTE01", count), texto.Substring(0, 16));
                                    config.Replace(String.Format("$MENSAJE{0:D2}PARTE02", count),
                                                   texto.Substring(16, 16));
                                    config.Replace(String.Format("$MENSAJE{0:D2}CODIGO", count), m.Codigo);
                                }

                                for (int i = count; i < 11; i++)
                                {
                                    const String texto = "                ";
                                    config.Replace(String.Format("$MENSAJE0{0:D1}PARTE01", i), texto);
                                    config.Replace(String.Format("$MENSAJE0{0:D1}PARTE02", i), texto);
                                    config.Replace(String.Format("$MENSAJE{0:D2}CODIGO", count), "00");
                                }
                                break;
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(config.ToString().Trim()))
            {
                config.AppendFormat("{0}{0}Envio de Mensajes Generado Exitosamente{0}", Environment.NewLine);
                config.Insert(0, Fota.VirtualMessageFactory(MessageIdentifier.MessagesStart, 0));
                config.Append(Fota.VirtualMessageFactory(MessageIdentifier.MessagesSuccess, 0));
            }
            else config.AppendLine("No hay mensajes para Enviar");
        }

        private String GetConfig(out String hash)
        {
            var config =
                new StringBuilder(DataProvider.GetConfiguration(Id).Replace("\r", "\n").Replace("\n", Environment.NewLine));
            string md = DataProvider.GetDetalleDispositivo(Id, "GTE_MESSAGING_DEVICE").As(String.Empty);
            if (String.IsNullOrEmpty(config.ToString()) && (md != MessagingDevice.Garmin))
            {
                hash = null;
                return null;
            }

            //reemplazar cada "$ParameterName" con "ParameterValue"
            List<DetalleDispositivo> configParametersTable = DataProvider.GetDetallesDispositivo(Id);

            if (configParametersTable != null)
            {
                IEnumerable<DetalleDispositivo> configParameters =
                    configParametersTable.Where(detalle => detalle.TipoParametro.Nombre.StartsWith("GTE_PARAM_"));
                foreach (DetalleDispositivo s in configParameters)
                {
                    config.Replace("$" + s.TipoParametro.Nombre, s.As("--- PARAMETRO SIN VALOR! ---"));
                }
                config.Replace("$known_qtree_revision",
                               DataProvider.GetDetalleDispositivo(Id, "known_qtree_revision").As("0"));
            }


            GetMessages(ref config);

            if (!String.IsNullOrEmpty(config.ToString().Trim()))
            {
                config.AppendFormat("{0}{0}Configuracion Generada Exitosamente{0}", Environment.NewLine);
                config.Insert(0, Fota.VirtualMessageFactory(MessageIdentifier.ConfigStart, 0));
            }
            else config.AppendLine("Configuracion vacia.");

            config.Append(Fota.VirtualMessageFactory(MessageIdentifier.ConfigSuccess, 0));

            string conf = config.ToString();
            hash = conf.GetHashCode().ToString(CultureInfo.InvariantCulture);
            conf = conf.Replace("$revision", hash);
            return conf;
        }

        private bool CheckLastSentAndDequeueIt(String buffer, ulong msgId, ref IMessage salida)
        {
            var result = true;
            string LastCmd = LastSent.GetText(String.Empty);
            ulong LastId = LastSent.GetId();

            if (buffer.StartsWith(Reporte.SdLocked))
            {
                _sdSession = false;
                STrace.Debug(typeof(Parser).FullName, Id, "Sesion de escritura en sd BORRANDO_POR_LOCKED");
            }
            else if (buffer.StartsWith(Reporte.SdPassword))
            {
                //Debug.Assert(LastSent.IsOnTheFly() == true);
                if (LastCmd.StartsWith(Reporte.StartGgWriteSession) && (LastId == msgId))
                {
                    LastSent = null;
                    _sdSession = true;
                    STrace.Debug(typeof(Parser).FullName, Id, "Sesion de escritura en sd ABIERTA");
                }
                else if (_sdSession)
                {
                    STrace.Debug(typeof(Parser).FullName, Id, "Sesion de escritura en sd NO_MANEJADO");
                }
                else if (LastId == msgId)
                {
                    ulong mid = NextSequence;

                    
                    LastSent = new INodeMessage(mid,
                                                Mensaje.Factory(mid, this,
                                                                String.Format("{0}{1}",
                                                                              Reporte.StartGgWriteSession.TrimStart('>'),
                                                                              buffer.Substring(8, 8))),
            
                                    DateTime.MinValue) { IsOnTheFly = true };

                    salida.AddStringToSend(LastSent.Text);

                    STrace.Debug(typeof(Parser).FullName, Id, "Sesion de escritura en sd ABRIENDO");
                }
                else
                {
                    STrace.Debug(typeof(Parser).FullName, Id,
                                 String.Format("Sesion de escritura en sd NADA (LastSent.GetId()={0} msgId={1})",
                                               LastSent.GetId(), msgId));
                }
            }else

            if (LastSent != null)
            {
                if (LastSent.IsExpired())
                {
                    LastSent = null;
                }
                else
                {
                    var lastDC = BaseDeviceCommand.createFrom(LastCmd, this, null);
                    var respStatus = lastDC.isExpectedResponse(BaseDeviceCommand.createFrom(buffer, this, null));
                    switch (respStatus)
                    {
                        case DeviceCommandResponseStatus.Valid:
                            result = true;
                            Fota.Dequeue(this, lastDC.MessageId ?? null);
                            break;
                        case DeviceCommandResponseStatus.Invalid:
                            result = false;
                            break;
                        case DeviceCommandResponseStatus.Exception:
                            Fota.RollbackLastTransaction(this, lastDC.MessageId ?? null);
                            result = false;
                            break;
                    }
                }
            }
            return result;
        }

        // TODO: unificar GTEDeviceCommand e INodeMessage si se puede
        private void SendPendingFota(ref IMessage msg)
        {
            string pending = Fota.Peek(this);

            if (String.IsNullOrEmpty(pending)) return;
            if (CheckImageSession(pending)) return;

            //procesar primero el pendiente
            pending = CheckConditionsBeforeSendPending(pending);

            if (String.IsNullOrEmpty(pending)) return;

            #region SendPending

            var gteDC = BaseDeviceCommand.createFrom(pending, this, null);
            pending = gteDC.ToString(true);

            msg = (msg ?? new UserMessage(Id, 0));
            msg.AddStringToPostSend(pending);

            if (LastSent == null || LastSent.GetText(null) != pending)
                LastSent = new INodeMessage((ulong)Id, pending, DateTime.UtcNow);

            #endregion SendPending
        }

        private String CheckConditionsBeforeSendPending(String pending)
        {
            if (!LastSent.IsOnTheFly())
            {
                string md = GetMessagingDevice();
                if (md == MessagingDevice.Garmin && BaseDeviceCommand.createFrom(pending, this, null).isGarminMessage())
                {
                    if (IsGarminConnected == null || !IsGarminConnected.Value)
                    {
                        // var mid = NextSequence; 
                        // pending = GarminFmi.EncodePing().ToTraxFm<String>(mid, this);
                        // LastSent = new INodeMessage(mid, pending, DateTime.UtcNow) { IsOnTheFly = true };
                        // STrace.Debug(typeof(IFoteable).FullName, this.GetDeviceId(), "GARMIN PING Sent");
                        LastSent = null;
                        return null;
                    }
                }
                else
                {
                    ulong mid = ParserUtils.GetMsgIdTaip(pending);

                    if (pending.StartsWith(Reporte.WriteGgPrefix) &&
                        !_sdSession)
                    {
                        //si quiero escribir en la sd tengo que abrir sesion antes, y antes tengo que pedir la password para abrir sesion
                        STrace.Debug(typeof(Parser).FullName, Id, "Sesion de escritura en sd QUERING");
                        mid = NextSequence;
                        pending = Mensaje.Factory(mid, this, Reporte.QuerySdSessionPassword.TrimStart('>'));
                        LastSent = new INodeMessage(mid, pending, DateTime.UtcNow) { IsOnTheFly = true };
                    }
                }
            }
            return pending;
        }

        private bool CheckImageSession(string pending)
        {
            //para no cancelar pedidos anteriores mantengo una sesion de imagenes
            if (pending.StartsWith(Reporte.PictureRequestPrefix1))
            {
                //el limite de espera / time out de la sesion de pedido de fotos
                DateTime limit;
                var ps = this.Retrieve<String>(CacheVar.PicturesSession);
                limit = DateTime.TryParse(ps, out limit) ? limit.AddMinutes(15) : DateTime.MinValue;

                //espero que terminen de venir las fotos del anterior request antes de enviar el que esta en la cola.
                if (limit > DateTime.UtcNow)
                {
                    return true;
                }
                this.Store(CacheVar.PicturesSession, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            }

            return false;
        }
    }
}