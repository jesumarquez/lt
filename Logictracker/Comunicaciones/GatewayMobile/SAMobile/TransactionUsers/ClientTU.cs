using System;
using System.Collections.Generic;
using Urbetrack.Mobile.Comm.GEO;
using Urbetrack.Mobile.Comm.Messages;
using Urbetrack.Mobile.Comm.Transaccional;
using Urbetrack.Mobile.Comm.Transport;
using Urbetrack.Mobile.Toolkit;

namespace Urbetrack.Mobile.Comm.TransactionUsers
{
    public class ClientTU : TransactionUser
    {
        #region Mecanismos de Registracion
        public void LoginRequest(AbstractTransport t, Destination address, string IMEI, string password, short tableversion, string firmware)
        {
            var req = new LoginRequest
                          {
                              DeviceId = IdDispositivo,
                              IMEI = IMEI,
                              Password = password,
                              Firmware = firmware,
                              TableVersion = tableversion,
                              Destination = address
                          };
            var mrc = new MRC(req, t, this);
            // Seq, automatico aqui.
            t.NuevaTransaccion(mrc, req);
            mrc.Start();
        }

        public delegate void LoginSuccessHandler(object sender, short IdDispositivo);

        public event LoginSuccessHandler LoginSuccess;

        public delegate void LoginFailHandler(object sender, byte reason);

        public event LoginFailHandler LoginFail;
        #endregion

        #region Mecanismos de Posicionamiento
        public void AutoReport(AbstractTransport t, Destination address, List<GPSPoint> puntos, int idQueue)
        {
            var req = new Posicion
                          {
                              DeviceId = IdDispositivo, 
                              IdQueue = idQueue, 
                              Transport = t, 
                              Destination = address
                          };
            req.AddPoints(puntos);
            var mrc = new MRC(req, t, this);
            // Seq: automatico aqui.
            t.NuevaTransaccion(mrc, req);
            mrc.Start();
        }

        public delegate void AutoReportSuccessHandler(object sender, int idQueue);

        public event AutoReportSuccessHandler AutoReportSuccess;

        public delegate void AutoReportFailHandler(object sender, int idQueue);

        public event AutoReportFailHandler AutoReportFail;

        public delegate void GeneralFailHandler(object sender);

        public event GeneralFailHandler GeneralFail;
        #endregion

        public override bool NuevaSolicitud(PDU pdu, Transaction tr, AbstractTransport t)
        {

            // si llega hasta aqui, no se pudo procesar el mensaje.
            tr.ResponseCL = 0x01; // codigo interino de MensajeDesconocido.
            return false;
        }

        public override PDU RequieroRespuesta(PDU pdu, Transaction tr, AbstractTransport t)
        {
            throw new NotImplementedException();
        }

        public override void RespuestaEntregada(PDU pdu, Transaction tr, AbstractTransport t)
        {
            throw new NotImplementedException();
        }

        public override void RespuestaNoEntregada(PDU pdu, Transaction tr, AbstractTransport t)
        {
            throw new NotImplementedException();
        }

        public override PDU RequieroACK(PDU pdu, Transaction tr, AbstractTransport t)
        {
            throw new NotImplementedException();
        }

        public override void SolicitudNoEnviada(PDU pdu, Transaction tr, AbstractTransport t)
        {
            if (pdu.CH == (byte)Decoder.ComandoH.LoginRequest && LoginFail != null)
            {
                LoginFail(this, 0xFF);
            }
            else if (pdu.CH == (byte)Decoder.ComandoH.MsgPosicion && AutoReportFail != null)
            {
                var pos = pdu as Posicion;
                if (pos == null) throw new Exception("SA: fallo la especializacion del PDU");
                AutoReportFail(this, pos.IdQueue);
            }
            else if (pdu.CH == (byte)Decoder.ComandoH.KeepAlive)
            {
                GeneralFail(this);
            }
        }

        public override void SolicitudEntregada(PDU rta, PDU source, Transaction tr, AbstractTransport t)
        {
            if (rta.CH == (byte)Decoder.ComandoH.LoginAcepted && LoginSuccess != null)
            {
                var la = rta as LoginAcepted;
                if (la == null) throw new Exception("SA: fallo la especializacion del PDU");
                LoginSuccess(this, la.IdAsignado);
            }
            else if (rta.CH == (byte)Decoder.ComandoH.LoginReject && LoginFail != null)
            {
                var lr = rta as LoginReject;
                if (lr == null) throw new Exception("SA: fallo la especializacion del PDU");
                LoginFail(this, lr.CL);
            }
            else if (rta.CH == (byte)Decoder.ComandoH.OKACK)
            {
                if (source.CH == (byte)Decoder.ComandoH.MsgPosicion && AutoReportSuccess != null)
                {
                    var pos = source as Posicion;
                    if (pos == null) throw new Exception("SA: fallo la especializacion del PDU");
                    AutoReportSuccess(this, pos.IdQueue);
                }


            }
        }

        public void KeepAlive(AbstractTransport t, Destination address)
        {
            T.TRACE(0, "Solicitando keep alive.");
            var pdu = new PDU
            {
                CH = (byte)Decoder.ComandoH.KeepAlive,
                DeviceId = IdDispositivo,
                Transport = t,
                Destination = address,
            };
            var keep_alive_mrc = new MRC(pdu, pdu.Transport, this);
            pdu.Transport.NuevaTransaccion(keep_alive_mrc, pdu);
            keep_alive_mrc.Seq = pdu.Seq;
            keep_alive_mrc.Start();
        }

        #region Propiedades del Objeto
        public override byte LimiteSuperiorSeq
        {
            get { return 0x7F; }
        }

        public override byte LimiteInferiorSeq
        {
            get { return 0x00; }
        }

        public short IdDispositivo { get; set; }

        #endregion
    }
}