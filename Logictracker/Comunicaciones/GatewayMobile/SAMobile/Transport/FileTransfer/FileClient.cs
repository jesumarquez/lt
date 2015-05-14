using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Urbetrack.Mobile.Net.TCP;
using Urbetrack.Mobile.Toolkit;
using Decoder=Urbetrack.Mobile.Comm.Messages.Decoder;

namespace Urbetrack.Mobile.Comm.Transport.FileTransfer
{
    public class FileClient
    {
        public static int SuccesfulSentMessages { get; protected set; }
        public static int AbortedMessagesSend { get; protected set; }

        private static void TRACE(EndPoint endPoint, string format, params object[] args)
        {
            T.TRACE(Stream.StreamTraceLevel, "FileClient[" + T.ENDPOINT(endPoint) + "]: " + format, args);
        }

        public static bool SendMessage(IPEndPoint endPoint, short idDispositivo, byte[] mensaje, string filename)
        {
            const byte op = 0x01;
            var filesize = mensaje.GetLength(0);
            var result = false;
            try
            {
                TRACE(endPoint, "=== inciando cliente ===");
                var socket = new TcpClient();
                socket.Connect(endPoint);
                TRACE(endPoint, "Se ha conectado el socket TCP/IP");
                var buff = new byte[71];
                // ponemos a cero.
                for (var i = 0; i < 71; ++i)
                {
                    buff[i] = 0x00;
                }
                var pos = 0;
                Decoder.EncodeShort(ref buff, ref pos, idDispositivo);
                Decoder.EncodeByte(ref buff, ref pos, op);
                Decoder.EncodeInteger(ref buff, ref pos, filesize);
                var b = Encoding.ASCII.GetBytes(filename);
                Array.Copy(b, 0, buff, pos, b.GetLength(0));
                socket.Client.Send(buff);
                TRACE(endPoint, "Cabeceras enviadas");
                var total = 0;
                var r = socket.Client.Send(mensaje, mensaje.GetLength(0), SocketFlags.None);
                total += r;
                TRACE(endPoint, "mensaje enviado {0} bytes.", total);
                var ack = new byte[1];
                ack[0] = Convert.ToByte('N');
                try
                {
                    if (socket.Client.Receive(ack, 0, 1, SocketFlags.None) == 1)
                    {
                        try
                        {
                            socket.Client.Shutdown(SocketShutdown.Send);
                            socket.Client.Close();
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch
                        // ReSharper restore EmptyGeneralCatchClause
                        {
                        }
                        if (ack[0] == 'A')
                        {
                            TRACE(endPoint, "ACK Recibido", endPoint);
                            return result = true;
                        }
                        TRACE(endPoint, "NACK Recibido", endPoint);
                    }
                    TRACE(endPoint, "No se recibio respuesta valida");
                    return false;
                }
                catch (Exception e)
                {
                    T.EXCEPTION(e, "FileClient.Push WAIT FOR ACK/NACK PHASE");
                }
                return result = false;
            } catch (Exception e)
            {
                T.ERROR("Exception en FileClient: txt={0}", e.Message);
                return result = false;
            }
            finally
            {
                if (result) SuccesfulSentMessages++;
                else AbortedMessagesSend++;
            }
        }
    }
}