#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Torino;
using XbeeCore;

#endregion

namespace Urbetrack.Comm.Core.Transport.XBeeRLP
{
    public class XBeeRadioLinkProtocol
    {

        internal enum FrameType
        {
            LOOPBACK_SHORT = 0x7A,  // Loopback Request, respuesta reducida.
            LOOPBACK_LARGE = 0x7B,  // Loopback Request, respuesta agrandada.
            CHANNEL_FLOOD = 0x7F,  // Datos generados para saturar el canal.
            STATUS_REPORT = 0x8A,   // Reporte de estado del dispositivo.
            ENABLE_LINK = 0xB0,  // Activar la interface.
            ENABLE_LINK_RESPONSE = 0xB1, // Confirmacion de Activar la interface.
            DISABLE_LINK = 0xB2,  // Desactivar la interface.
            DISABLE_LINK_RESPONSE = 0xB3, // Confirmacion de Desactivar la interface.
            CHANGE_CHANNEL = 0xCC,  // Cambiar la radio al canal indicado, luego de este byte sigue el nuevo canal al que debe cambiarse la radio.
            CHANGE_CHANNEL_RESPONSE = 0xCA,  // Confirmacion de Cambio de canal.
            USER_PART = 0xDF   // Parte de usuario.
        };

        internal TransporteXBEE TransporteXBEE { get; set; }
        internal XBeeAPIPort XBeeAPIPort { get; set; }

        internal XBeeRadioLinkProtocol(TransporteXBEE txb, XBeeAPIPort xbap)
        {
            TransporteXBEE = txb;
            XBeeAPIPort = xbap;
            XBeeAPIPort.HardwareStatus += HardwareStatus;
            XBeeAPIPort.PDURecibida += PDURecibida;
            XBeeAPIPort.NodeComesUp += NodeComesUp;
            XBeeAPIPort.NodeComesDown += NodeComesDown;
        }

        internal void NodeComesUp(XBeeAPIPort sender, XBeeNode node)
        {
            var d = Devices.I().FindByImei(node.IMEI());
            if (d == null) return;
            d.XBeeSession.DeviceComesUp(this, TransporteXBEE);
            TransporteXBEE.DoReceiveReport(d);
        }

        internal void NodeComesDown(XBeeAPIPort sender, XBeeNode node)
        {
            var d = Devices.I().FindByImei(node.IMEI());
            if (d == null) return;
            d.XBeeSession.DeviceComesDown();
            TransporteXBEE.DoReceiveReport(d);
        }

        public void Stop()
        {
            XBeeAPIPort.Close();
            XBeeAPIPort = null;
        }

        internal void PDURecibida(XBeeAPIPort uart, XBeePDU pdu)
        {
            STrace.Debug(typeof(XBeeRadioLinkProtocol).FullName, String.Format("XBEE-PDU: code={0:X}", pdu.Data[0]));
            switch (pdu.Data[0])
            {
                case (byte)FrameType.STATUS_REPORT:
                    {
                        var pos = 1;
                        var node = XBeeAPIPort.FindNode(pdu.Address);
                        if (node == null)
                        {
                            var addr = String.Format("{0:X}",pdu.Address);
                            var nd = Devices.I().FindByXbeeAddr(addr);
                            if (nd == null) return;
                            var nn = new XBeeNode
                                         {
                                             Address = pdu.Address,
                                             Id = String.Format("D:{0}", nd.Imei)
                                         };
                            nn.Trace("NODO DETECTADO EN CALIENTE:");
                            node = nn;
                        }
                        XBeeAPIPort.UpdateNode(node);
                        var d = Devices.I().FindByImei(node.IMEI());
                        if (d == null)
                        {
                            STrace.Debug(typeof(XBeeRadioLinkProtocol).FullName, String.Format("XBEERLP: device imei={0} no encontrado.", node.IMEI()));
                            return;
                        }

                        d.XBeeSession.Report.RadioLinkState = (XBeeReport.DeviceXbeeMachineStates) UrbetrackCodec.DecodeByte(pdu.Data, ref pos);
                        d.XBeeSession.Report.CommCoreState = (XBeeReport.DeviceSessionMachineStates)UrbetrackCodec.DecodeByte(pdu.Data, ref pos);
                        d.XBeeSession.Report.NetworkConnections = UrbetrackCodec.DecodeByte(pdu.Data, ref pos);
                        d.XBeeSession.Report.QueryState = (XBeeReport.QueryStates) UrbetrackCodec.DecodeByte(pdu.Data, ref pos);

                        d.XBeeSession.Report.QueryStartSample = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.QueryEndSample = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.CursorSample = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.OldestSample = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        
                        d.XBeeSession.Report.OldestTrackingSample = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.Processed = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.Empty = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.Tracking = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        
                        d.XBeeSession.Report.Detailed = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.Sent = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.SessionSent = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        d.XBeeSession.Report.Pendings = UrbetrackCodec.DecodeInteger(pdu.Data, ref pos);
                        
                        d.Destino.XBee = new XBeeAddress { Addr = pdu.Address };
                        STrace.Debug(typeof(XBeeRadioLinkProtocol).FullName, String.Format("XBEERLP: device imei={0} actualizando. {1}/{2}/{3}", node.IMEI(), d.XBeeSession.Report.RadioLinkState, d.XBeeSession.Report.NetworkConnections, d.XBeeSession.Report.QueryState));
                        TransporteXBEE.DoReceiveReport(d);
                        return;
                    }
                case (byte)FrameType.USER_PART:
                    {
                        var data_size = pdu.Data.GetLength(0) - 1;
                        var instance_buffer = new byte[data_size];
                        Array.Copy(pdu.Data, 1, instance_buffer, 0, data_size);
                        var node = XBeeAPIPort.FindNode(pdu.Address);
                        if (node == null)
                        {
                            STrace.Debug(typeof(XBeeRadioLinkProtocol).FullName, String.Format("XBEERLP IGNORANDO NODO {0}", pdu.Address));
                            return;
                        }
                        XBeeAPIPort.UpdateNode(node);
                        var instance_addr = new XBeeAddress {Addr = pdu.Address};
                        TransporteXBEE.Receive(instance_buffer, data_size, instance_addr);
                        return;
                    }
                case (byte)FrameType.DISABLE_LINK_RESPONSE:
                    {
                        var node = XBeeAPIPort.FindNode(pdu.Address);
                        if (node == null)
                            return;
                        XBeeAPIPort.UpdateNode(node);
                        var d = Devices.I().FindByImei(node.IMEI());
                        d.XBeeSession.GoesInactive();
                        TransporteXBEE.DoReceiveReport(d);
                        RemoveTransaction(d);
                        break;
                    }
                case (byte)FrameType.ENABLE_LINK_RESPONSE:
                    {
                        var node = XBeeAPIPort.FindNode(pdu.Address);
                        if (node == null)
                            return;
                        XBeeAPIPort.UpdateNode(node);
                        var d = Devices.I().FindByImei(node.IMEI());
                        if (!FoundTransaction(d)) break;
                        d.XBeeSession.GoesActive();
                        TransporteXBEE.DoReceiveReport(d);
                        RemoveTransaction(d);
                        break;
                    }
                default:
                    STrace.Debug(typeof(XBeeRadioLinkProtocol).FullName,"@@@");
                    break;
            }
            return;
        }

        internal static void HardwareStatus(XBeeAPIPort uart, XBeePDU pdu)
        {
            STrace.Debug(typeof(XBeeRadioLinkProtocol).FullName,"XBEE RLP: Hardware Status Received.");
        }
        
        public bool Send(byte[] instance_buffer, int size, XBeeAddress destination)
        {
            var tx_buffer = new byte[size + 1];
            tx_buffer[0] = (byte)FrameType.USER_PART;
            Array.Copy(instance_buffer, 0, tx_buffer, 1, size);
            XBeeAPIPort.Send(tx_buffer, destination.Addr);

            return true;
        }

        public AutoResetEvent rlptr_syncro = new AutoResetEvent(true);
        public Dictionary<string, XBeeRLPTransaction> rlptr = new Dictionary<string, XBeeRLPTransaction>();

        public void AddTransaction(XBeeRLPTransaction tr, Device d)
        {
            rlptr_syncro.WaitOne();
            if (rlptr.ContainsKey(d.Imei))
            {
                rlptr_syncro.Set();
                return;
            }
            rlptr.Add(d.Imei, tr);
            rlptr_syncro.Set();
        }

        private bool FoundTransaction(Device d)
        {
            rlptr_syncro.WaitOne();
            var result = rlptr.ContainsKey(d.Imei);
            rlptr_syncro.Set();
            return result;
        }

        public void RemoveTransaction(Device d)
        {
            rlptr_syncro.WaitOne();
            if (!rlptr.ContainsKey(d.Imei))
            {
                rlptr_syncro.Set();
                return;
            }
            rlptr[d.Imei].Remove();
            rlptr.Remove(d.Imei);
            rlptr_syncro.Set();
        }

        public void ActivateNode(Device d, byte retrieve_flags)
        {
            d.XBeeSession.ActivationRetrieveFlags = retrieve_flags;
            var tx_buffer = new byte[1];
            tx_buffer[0] = (byte)FrameType.ENABLE_LINK;
            STrace.Debug(typeof(XBeeRadioLinkProtocol).FullName, "XBEETRL: Enviando ENABLE_LINK");
            XBeeAPIPort.Send(tx_buffer, d.Destino.XBee.Addr);
            AddTransaction(new XBeeRLPTransaction(d, tx_buffer, this), d);
        }

        public void DeactivateNode(Device d)
        {
            var tx_buffer = new byte[1];
            tx_buffer[0] = (byte)FrameType.DISABLE_LINK;
            STrace.Debug(typeof(XBeeRadioLinkProtocol).FullName, "XBEETRL: Enviando DISABLE_LINK");
            XBeeAPIPort.Send(tx_buffer, d.Destino.XBee.Addr);
            AddTransaction(new XBeeRLPTransaction(d, tx_buffer, this), d);
        }

        public void ShortLoopback(XBeeAddress destination, int size)
        {
            var tx_buffer = new byte[size];
            tx_buffer[0] = (byte)FrameType.LOOPBACK_SHORT;
            XBeeAPIPort.Send(tx_buffer, destination.Addr);
        }
    };
}