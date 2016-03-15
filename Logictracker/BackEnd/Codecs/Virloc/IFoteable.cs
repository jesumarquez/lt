using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Layers;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

namespace Logictracker.Virloc
{
    public partial class Parser : IFoteable
    {
        private const String VirtualMessagePrefix = "DispatchEvent=";

        #region IFoteable

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

        public bool ReloadFirmware(ulong messageId)
        {
            SendMessages(GetFirm(), messageId, null);
            return true;
        }

        public bool ReloadMessages(ulong messageId)
        {
            return true;
        }

        public bool ResetFMIOnGarmin(ulong messageId)
        {
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
            return !line.Contains(">SID");
        }        

        public INodeMessage LastSent { get; set; }

        public bool? IsGarminConnected { get { return false; } set { } }

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
				default:
					Fota.Enqueue(this, messageId, config);
			        break;
			}                    
        }

        #endregion

        private String GetFirm()
        {
            return null;
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
                    configParametersTable.Where(detalle => detalle.TipoParametro.Nombre.StartsWith("VIRLOC_PARAM_"));
                foreach (DetalleDispositivo s in configParameters)
                {                    
                    config.Replace("$" + s.TipoParametro.Nombre, s.As("--- PARAMETRO SIN VALOR! ---"));
                }
            }

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

        private void CheckLastSentAndDequeueIt(BaseDeviceCommand dc)
        {
            string LastCmd = LastSent.GetText(String.Empty);

            if (LastSent != null)
            {
                if (LastSent.IsExpired())
                {
                    LastSent = null;
                }
                else
                {
                    var lastDC = BaseDeviceCommand.createFrom(LastCmd, this, null);
                    var result = lastDC.isExpectedResponse(dc);
                    if (result == DeviceCommandResponseStatus.Valid)
                    {
                        Fota.Dequeue(this, lastDC.MessageId);
                    }
                }
            }
        }

        // TODO: unificar GTEDeviceCommand e INodeMessage si se puede
        private void SendPendingFota(ref IMessage msg)
        {
            string pending = Fota.Peek(this);

            if (String.IsNullOrEmpty(pending)) return;

            #region SendPending

            var gteDC = BaseDeviceCommand.createFrom(pending, this, null);
            pending = gteDC.ToString(true);

            msg = (msg ?? new UserMessage(Id, 0));
            msg.AddStringToPostSend(pending);

            if (LastSent == null || LastSent.GetText(null) != pending)
                LastSent = new INodeMessage((ulong) Id, pending, DateTime.UtcNow);

            #endregion SendPending
        }
    }
}