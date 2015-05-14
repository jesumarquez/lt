#region Usings

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Urbetrack.Backbone;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Comm.Core.Mensajeria;
using Urbetrack.Comm.Core.Transaction;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Hacking;

#endregion

namespace Urbetrack.Comm.Core.Transport
{
    public class TransporteUDP : Transporte
    {
        private IPEndPoint _local_address;
        private EndPoint _remote_address = new IPEndPoint(IPAddress.Any,0);
        private byte[] _read_buffer;
        private int _read_bytes;
        private int _buffersize;
        private Socket _socket;

        public IPEndPoint ServerAddress { get; set; }

        public virtual void AbrirTransporte(IPEndPoint local_address, int buffersize, string _role)
        {
            ConnectionMode = ConnectionModes.DatagramOriented;
            _buffersize = buffersize;
            _read_buffer = new byte[_buffersize];
            _local_address = local_address;
            _socket = new Socket(AddressFamily.InterNetwork,
                                 SocketType.Dgram, ProtocolType.Udp);

            _socket.Bind(_local_address);
            _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback, _socket);
            STrace.Debug(GetType().FullName, String.Format("reactor udp iniciado, address={0}", _local_address));
        }

        public void CerrarTransporte() {
            STrace.Debug(GetType().FullName, "reactor udp cierre solicitado.");
            _socket.Close();
        }

        private int get_read_bytes() { return _read_bytes; }

        private byte[] get_buffer() { return _read_buffer; }

        public override int MTU { get { return 512; } }

        private bool sendTo(byte[] buffer, int buffer_size, EndPoint remote_ep)
        {
            if (Hacker.UDP.DisableSocketWrite)
            {
                STrace.Debug(GetType().FullName, String.Format("udp pdu hackeado al enviar, remote address={0}", remote_ep));
                return true;
            }
            try
            {
                var bytes = _socket.SendTo(buffer, buffer_size, SocketFlags.None, remote_ep);
                //Marshall.Debug(string.Format("udp pdu enviado, remote address={0}", remote_ep));
                return bytes == buffer_size;
            } catch {
                return false;
            }
        }

        private void _ReadCallback(IAsyncResult ar)
        {
            var myData = Thread.GetNamedDataSlot("device");
            try
            {
                _read_bytes = _socket.EndReceiveFrom(ar, ref _remote_address);

                if (Hacker.UDP.DisableSocketRead)
                {
                    STrace.Debug(GetType().FullName, String.Format("udp reactor, hackeado, tamaño={0}", _read_bytes));

                    _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback, _socket);

                    Thread.SetData(myData, null);

                    return;
                }
                
                try
                {                    
                    var ret = Codes.DecodeErrors.NoError;
                    var instance_buffer = new byte[get_read_bytes()];

                    Array.Copy(get_buffer(), instance_buffer, get_read_bytes());

                    PDU pdu = null;

                    try
                    {
                        pdu = CODEC.Decode(instance_buffer, ref ret);
                    }
                    catch(Exception)
                    {
                        STrace.Debug(GetType().FullName,"PDU malformada!");
                    }

                    if (pdu == null || ret != Codes.DecodeErrors.NoError)
                    {
                        STrace.Debug(GetType().FullName, String.Format("Error la PDU no se pudo decodificar reason={0}", ret));

                        Thread.SetData(myData, null);

                        _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback, _socket);

                        return;
                    }

                    pdu.Transporte = this;
                    pdu.Destino = new Destino {UDP = (_remote_address as IPEndPoint)};

                    var d = Devices.I().FindById(pdu.IdDispositivo);

                    Thread.SetData(myData, d);

                    if (d != null && CODEC.AllowOfflineMessages)
                    {
                        d.Transporte = this;
                        d.Touch(pdu.Destino);
                    } else
                    if (d != null && d.State == DeviceTypes.States.ONLINE)
                    {
                        STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: TOUCH por mensaje CH={1}", d.LogId, pdu.CH));
                        d.Transporte = this;
                        d.Touch(pdu.Destino);
                    } 
                    else if (d != null)
                    {
                        if ((pdu.CH < 0x80) && ((Codes.HighCommand)pdu.CH) != Codes.HighCommand.LoginRequest)
                        {
                            if (CODEC.DontReplayWhenOffline)
                            {
                                // ignoramos todos los mensajes excepto LRQ cuando esta offline.
                                STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Ignorando mensaje CH={1} por estar offline.", d.LogId, pdu.CH));
                                Thread.SetData(myData, null);
                                _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback, _socket);
                                return;
                            }

                            if (CODEC.SupportsLoginRequired) {
                                // si el dispositivo no esta registrado, entonces enviamos 
                                // mensaje de registracion requerida.
                                STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Enviando Login Requerido.", d.LogId));
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
                            }

                            Thread.SetData(myData, null);
                            _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback, _socket);
                            return;
                        }
                    }
                    var devlog = (d != null ? d.LogId : "?");
                    var t = ObtenerTransaccion(pdu.IdTransaccion);
                    if (t == null)
                    {
                        if (pdu.CH < 0x80)
                        {
                            var mrs = new MRS(pdu, this, TransactionUser);
                            NuevaTransaccion(mrs, pdu);
                            mrs.Start();
                        } else
                        {
                            STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: RTA HUERFANA CH={1}", devlog, pdu.CH));
                        }
                    }
                    else
                    {
                        STrace.Debug(GetType().FullName, String.Format("DEVICE[{0}]: Recivo Retransmision CH={1}", devlog, pdu.CH));
                        t.RecibePDU(pdu);
                    }
                }
				catch (ObjectDisposedException e)
                {
                    STrace.Debug(GetType().FullName, e.Message);
                }
                catch (Exception e)
                {
                    STrace.Exception(GetType().FullName, e);
                }
                Thread.SetData(myData, null);
                _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback, _socket);
            }
            catch (Exception e)
            {
				if (e.Message != "An existing connection was forcibly closed by the remote host")
				{
					if (e is ObjectDisposedException)
					{
						STrace.Debug(GetType().FullName, e.Message);
					}
					else
					{
						STrace.Exception(GetType().FullName, e);
					}
				}
                Thread.SetData(myData, null);
                _socket.BeginReceiveFrom(_read_buffer, 0, _buffersize, 0, ref _remote_address, _ReadCallback, _socket);
            }
        }

        public override void Close(Destino dst)
        {
            
        }

        public override bool Send(PDU pdu)
        {
            var d = Devices.I().FindById(pdu.IdDispositivo);
            if (d != null && d.State == DeviceTypes.States.ONLINE) d.Touch(pdu.Destino);
            if (d != null && d.SupportsChecksum)
            {
                pdu.Options = 0x80;
            }
            var size = 0;
            var instance_buffer = new byte[8192];
            CODEC.Encode(ref instance_buffer, ref size, pdu);
            return sendTo(instance_buffer, size, pdu.Destino.UDP);
        }
    }
}