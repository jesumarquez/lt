using System;
using System.Net.Mime;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Messages.Sender;
using Logictracker.Messages.Saver;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;
using Logictracker.DAL.Factories;


namespace Logictracker.Absolut
{
    namespace Logictracker.Absolut
    {
        [FrameworkElement(XName = "AbsolutParser", IsContainer = false)]
        public class Parser : BaseCodec
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

            [ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 6060)]
            public override int Port { get; set; }


            public override INode Factory(IFrame frame, int formerId)
            {
                var dc = new AbsolutDeviceCommand(frame.Payload);
                var parse = System.Text.Encoding.Default.GetString(frame.Payload).Split(',');
                var dev = DataProvider.FindByIMEI(parse[1], this);
                return dev;
            }

            public override IMessage Decode(IFrame frame)
            {
                var dc = new AbsolutDeviceCommand(frame.Payload);
                var parse = System.Text.Encoding.Default.GetString(frame.Payload).Split(',');
                var inode = DataProvider.FindByIMEI(parse[1], this);
                if (inode == null)
                {
                    inode = new Parser();
                    inode.Imei = parse[1];
                }
                var dispositivo = DataProvider.GetDispositivo(inode.Id);
                if (dispositivo == null)
                {
                    dispositivo = new Dispositivo();
                }
                //T1 Temperatura 25ºC
                var message =
                    M2mMessageSender.Create(dispositivo, new M2mMessageSaver(new DAOFactory())).AddCommand("2850");
                //0   Encabezado del paquete
                //1   IMEI del equipo
                //2   Número de paquete            string[] parse = _command.Split(new[] { "<", ">" }, StringSplitOptions.RemoveEmptyEntries); 
                //3   Evento que generó el paquete            var eventoLength = (_command.Length == 48 || _command.Length == 46 ? 3 : 2);
                //4   Nivel de señal GSM. (Entre 0 y 31)            var evento = (byte)0;
                //5   U1 en voltios            var hdop = (byte)0;
                //6   I1 en Ampere por 10. (10 equivale a 1.0 Amp)            var entradas = Convert.ToByte("0", 16);
                //7   U2 en voltios            var time = DateTimeUtils.SafeParseFormat(parse[10] + parse[2].Split('.')[0], "ddMMyyHHmmss");
                //8   I2 en Ampere por 10. (27 equivale a 2.7 Amp)
                //9   D1 Entrada DIN1 inactiva            /*
                //10  D2 Entrada D2 activa                Latitude: DD MM.MMMM
                //11  T1 Temperatura 25ºC               T1:valor,T2:valor
                //12  T2 Temperatura 23ºC              
                //13  T3 Sensor NTC3 abierto             
                //14  T4 Sensor NTC4 en cortocircuito            
                //15  Latitud              
                //16  Longitud              
                //17  Modelo y versión de firmware del equipo             096*42.858868             
                //18  Fecha y hora de la generación del evento GMT0             */
                //19  Fin de paquete

                //  T1:valor,T2:valor

                //11  T1 Temperatura 25ºC               T1:valor,T2:valor
                //12  T2 Temperatura 23ºC              
                //13  T3 Sensor NTC3 abierto             
                //14  T4 Sensor NTC4 en cortocircuito   

                message.AddParameter("T1", parse[11]);
                message.AddParameter("T2", parse[12]);
                message.AddParameter("T3", parse[13]);
                message.AddParameter("T4", parse[14]);
                // message.Send();
                // IMessage mensaje = (M2mMessageSender)message;
                // return new UserMessage(dev.Id, dev.IdNum);




                //Tn;msgid;idmovil;Temperatura;aa/dd/mm,hh:mm:ss; número de sensor @checksum 
                //??;parse[3];parse[1];parse[11];parse[18];

                var lowCmd = parse[11];
                var subcode = MessageIdentifier.TemperatureInfo;
                //switch (lowCmd)
                //{
                //    case '0': //login
                //        // return ParseTLogin(partes, node);
                //    case '1': //Evento de medición.
                //        subcode = MessageIdentifier.TemperatureInfo;
                //        break;
                //    case '2': //Evento de desconexión del sensor.
                //        subcode = MessageIdentifier.TemperatureDisconected;
                //        break;
                //    case '3': //Evento de desconexión de la alimentación principal del sensor
                //        subcode = MessageIdentifier.TemperaturePowerDisconected;
                //        break;
                //    case '4': //Evento de reconexión de la alimentación principal del sensor
                //        subcode = MessageIdentifier.TemperaturePowerReconected;
                //        break;
                //    case '5': //Evento descongelamiento de heladera (Botón oprimido)
                //        subcode = MessageIdentifier.TemperatureThawingButtonPressed;
                //        break;
                //    case '6': //Evento de fin de descongelamiento de heladera (Botón liberado)
                //        subcode = MessageIdentifier.TemperatureThawingButtonUnpressed;
                //        break;
                //        //apertura de puerta 2851
                //        //
                //    case '7': //Evento de puerta abierta
                //        subcode = MessageIdentifier.DoorOpenned;
                //        break;
                //    case '8': //Evento de puerta cerrada
                //        subcode = MessageIdentifier.DoorClosed;
                //        break;
                //}

                IMessage msg;
                var mid = Convert.ToUInt64(parse[2]);

                var dt = DateTimeUtils.SafeParseFormat(parse[18], "yyyy-MM-dd HH:mm:ss");
                MessageIdentifier code;
                switch (subcode)
                {
                    case MessageIdentifier.TemperatureInfo:
                        code = MessageIdentifier.TelemetricData;
                        break;
                        //debe enviarlos como M2M sino se descartan por dispositivo no asignado!
                        //case MessageIdentifier.DoorClosed:
                        //case MessageIdentifier.DoorOpenned:
                        //code = MessageIdentifier.GenericMessage;
                        //break;
                        //                        trama 
                        //2850
                        //TelemetricData = 2850,
                        //apertura de puerta
                        //2851
                        //TelemetricEvent = 2851,

                    default:
                        code = MessageIdentifier.TelemetricEvent;
                        break;
                }
                var msg_ = subcode.FactoryEvent(code, inode.Id, mid, null, dt, null, null);
                //5   U1 en voltios            var hdop = (byte)0;
                //6   I1 en Ampere por 10. (10 equivale a 1.0 Amp)            var entradas = Convert.ToByte("0", 16);
                //7   U2 en voltios            var time = DateTimeUtils.SafeParseFormat(parse[10] + parse[2].Split('.')[0], "ddMMyyHHmmss");
                //8   I2 en Ampere por 10. (27 equivale a 2.7 Amp)

                msg_.SensorsDataString = "U1:" + parse[5] + "I1:" + parse[6] + "U2:" + parse[7] + "I2:" + parse[8] + "T1:" + parse[11] + ",T2:" + parse[12] + ",T3:" + parse[13] + ",T4:" + parse[14];
                msg = msg_;
                var ackStr = dc.BuildAck().ToString(true);
                /*$B,353234020014377,ACK=12,$E*/
                return msg.AddStringToSend(parse[0] + "," + parse[1] + ",ACK=" + parse[2] + "," + parse[19]);

                /*return
                    msg.AddStringToSend(String.Format(@"RT{0};{1:D3};{2:D5};{3:yy/MM/dd,HH:mm:ss}", lowCmd, mid,
                        inode.Id, DateTime.UtcNow));*/
            }

            //private IMessage GetSalida(AbsolutDeviceCommand dc)
            //{
            //    var msgid = dc.MessageId ?? 0;
            //    var devStatus = dc.ParsePosition();
            //    var gpoint = devStatus.Position;
            //    return gpoint.ToPosition(Id, msgid);
            //}
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
}
