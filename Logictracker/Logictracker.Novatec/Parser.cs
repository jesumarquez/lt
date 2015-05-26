using System;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

namespace Logictracker.Novatel
{
    [FrameworkElement(XName = "NovatelParser", IsContainer = false)]
    public partial class Parser : BaseCodec
    {
        public override NodeTypes NodeType { get { return NodeTypes.Novatel; } }

        protected override UInt32 NextSequenceMin()
        {
            return 0x0000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

        [ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 6062)]
        public override int Port { get; set; }


        public override INode Factory(IFrame frame, int formerId)
        {
            var dc = new NovatelDeviceCommand(frame.Payload);
            var modemID = dc.ModemID; //"ModemID" en la documentacion de Novatel
            var dev = DataProvider.FindByIMEI(modemID, this);
            return dev;
        }

        public override IMessage Decode(IFrame frame)
        {
            IMessage salida = null;
            var msgId = ParserUtils.MsgIdNotSet;

            var dc = new NovatelDeviceCommand(frame.Payload, this);


            if (dc.hasMessasgeId())
                msgId = dc.MessageId ?? 0;

            salida = GetSalida(dc);

            // Replying with ACK if needed
           /* if (dc.hasIdNum() && dc.hasMessasgeId() && !salida.IsPending())
            {
                if (salida == null)
                    salida = new UserMessage(Id, msgId);
                var ackStr = dc.BuildAck().ToString(true);
                salida.AddStringToSend(ackStr);
            }*/

            /* CheckLastSentAndDequeueIt(dc);

           if (LastSent == null)  //&& !(new String[] {Reporte.IdReq}.Any(r=> tipoReporte == r)))
                SendPendingFota(ref salida);*/

            return salida;
        }

        private IMessage GetSalida(NovatelDeviceCommand dc)        
        {
            var msgid = dc.MessageId ?? 0;
            DeviceStatus devStatus = dc.ParsePosition();
            var gpoint = devStatus.Position;
            return gpoint.ToPosition(Id, msgid);
        }
        private abstract class Reporte
        {
            public const String Nada = "Nada";
            public const String SinNodo = "SinNodo";
            public const String EventoGP = "RGP";
            public const String EventoGL = "RGL";
            public const String IdReq = "RID";
            public const String IMEIReq = "REN11";
        }

        private static String GetTipoReporte(BaseDeviceCommand c)
        {
            var cmd = c.getCommand();
            if (cmd.StartsWith(Reporte.IMEIReq)) return Reporte.IMEIReq;
            if (cmd.StartsWith(Reporte.IdReq)) return Reporte.IdReq;
            if (cmd.StartsWith(Reporte.EventoGP)) return Reporte.EventoGP;
            if (cmd.StartsWith(Reporte.EventoGL)) return Reporte.EventoGL;
            return cmd.Length < 3 ? Reporte.Nada : cmd.Substring(0, 3);
        }
    }
}
