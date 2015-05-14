#region Usings

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Urbetrack.Backbone;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transaction;
using Urbetrack.Comm.Core.Transport.XBeeRLP;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Torino;
using XbeeCore;

#endregion

namespace Urbetrack.Comm.Core.Transport
{
    public class TransporteXBEE : Transporte
    {
        public readonly Dictionary<int, XBeeRadioLinkProtocol> links = new Dictionary<int, XBeeRadioLinkProtocol>();
        public int CongestionThreshold { get; set; }

        public void Open()
        {
        }

        public XBeeAPIPort GetUart(int id)
        {
            return !links.ContainsKey(id) ? null : links[id].XBeeAPIPort;
        }

        public void AddUart(int id, string serialPort, int baudRate)
        {
            var uart = new XBeeAPIPort
                           {
                               Id = id,
                               SerialPort = serialPort,
                               Rate = baudRate,
                               Handshake = Handshake.RequestToSend
                           };

            var rlp = new XBeeRadioLinkProtocol(this,uart);
            links.Add(id, rlp);
        }

        public void ActivateNode(Device d)
        {
            links[0].ActivateNode(d, d.XBeeSession.ActivationRetrieveFlags);        
        }

        public void DeactivateNode(Device d)
        {
            links[0].ActivateNode(d, d.XBeeSession.ActivationRetrieveFlags);
        }

        public override bool Send(PDU pdu)
        {
            var d = Devices.I().FindById(pdu.IdDispositivo);
            STrace.Debug(GetType().FullName, String.Format("XBEE/SEND: d={0}", (d == null ? "(nil)" : d.LogId)));
            if (d != null && d.State == DeviceTypes.States.ONLINE) d.Touch(pdu.Destino);
            var size = 0;
            var instance_buffer = new byte[8192];
            var bytes = CODEC.Encode(ref instance_buffer, ref size, pdu);
            STrace.Debug(GetType().FullName, String.Format("---XBEE SENDING {0}/{1} bytes TO {2}----------\n{3}", bytes, size, pdu.Destino.XBee, pdu.Trace("")));

            /*if (pdu.SourcePdu != null && pdu.SourcePdu.CH == (byte)Codes.HighCommand.MsgPosicion)
            {
                STrace.Trace(GetType().FullName,0, "--- PAUSA XBEE DE POSICION ---");
                Thread.Sleep(1000);
            }*/
            return links[0].Send(instance_buffer, size, pdu.Destino.XBee);
        }

        public override bool DeviceGoesONNET { get { return true; } }

        public override int MTU { get { return 96; } }


        public override void Close(Destino dst)
        {
            
        }

        public override bool UseSeqCache
        {
            get { return true; }
        }

        public override int SeqCache
        {
            get { return 60; }
        }

        public void Shutdown()
        {
            links[0].Stop();
        }

        public delegate void ReceiveReportHandler(Device device);

        public event ReceiveReportHandler ReceiveReport;

        public void DoReceiveReport(Device device)
        {
            if (ReceiveReport == null)
            {
                STrace.Debug(GetType().FullName,"TRANSPORT_XBEE: no hay receptor de reportes.");
                return;
            }
            ReceiveReport(device);
        }


        public void Receive(byte[] source_buffer, int size, XBeeAddress addr)
        {
            try
            {
                var ret = Codes.DecodeErrors.NoError;
                var instance_buffer = new byte[size];
                Array.Copy(source_buffer, instance_buffer, size);
                var pdu = CODEC.Decode(instance_buffer, ref ret);

                if (pdu == null || ret != Codes.DecodeErrors.NoError)
                {
                    STrace.Debug(GetType().FullName, String.Format("XBEE: Error la PDU no se pudo decodificar reason={0}", ret));
                    return;
                }

                pdu.Transporte = this;
                pdu.Destino = new Destino {XBee = addr};
                var d = Devices.I().FindById(pdu.IdDispositivo);
                var devlog = (d != null ? d.LogId : "?");
                var myData = Thread.GetNamedDataSlot("device");
                Thread.SetData(myData, d); 

                STrace.Debug(GetType().FullName, pdu.Trace(""));
                if (d != null) d.Trace();

                if (d != null && d.State == DeviceTypes.States.ONNET)
                {
                    d.Transporte = this;
                    d.Touch(pdu.Destino);
                }
                else if (d != null)
                {
                    if ((Codes.HighCommand) pdu.CH != Codes.HighCommand.LoginRequest)
                    {
                        // si el dispositivo no esta registrado, entonces enviamos 
                        // mensaje de registracion requerida.
                        STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/XBEE: Enviando Login requerido", devlog));
                        var rta = new PDU
                                      {
                                          IdDispositivo = pdu.IdDispositivo,
                                          Seq = pdu.Seq,
                                          Options = pdu.Options,
                                          Destino = pdu.Destino,
                                          CH = ((byte) Codes.HighCommand.LoginRequerido),
                                          CL = 0x00
                                      };
                        Send(rta);
                        Thread.SetData(myData, null);
                        return;
                    }
                }

                var t = ObtenerTransaccion(pdu.IdTransaccion);
                if (t == null)
                {
                    if (pdu.CH < 0x80)
                    {
                        STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/XBEE: Nueva Transaccion Entrante CH={1} Seq={2}", devlog, pdu.CH, pdu.Seq));
                        
                        var mrs = new MRS(pdu, this, TransactionUser);
                        if (d != null && d.HackBugXBEEv99)
                        {
                            d.Transporte = this;    
                        }
                        NuevaTransaccion(mrs, pdu);                        
                        mrs.Start();
                    }
                    else
                    {
                        STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/XBEE: RTA HUERFANA CH={1}", devlog, pdu.CH));
                    }
                }
                else
                {
                    STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]/XBEE: Recivo Retransmision CH={1} Seq={2}", devlog, pdu.CH, pdu.Seq));
                    t.RecibePDU(pdu);
                }
            } 
			catch (Exception e)
            {
                STrace.Exception(GetType().FullName, e);
            }
        }

    }
}