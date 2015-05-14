using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Model;
using Logictracker.Utils;

namespace Logictracker.MaxTrack
{
    [FrameworkElement(XName = "MaxTrackParser", IsContainer = false)]
    public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Maxtrack; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2002)]
		public override int Port { get; set; }

		#endregion

		#region BaseCodec

        protected override UInt32 NextSequenceMin()
        {
            return 0x0000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

		public override INode Factory(IFrame frame, int formerId)
        {
			return DataProvider.Get(GetDeviceId(frame.Payload), this);
        }

		public override IMessage Decode(IFrame frame)
        {
            var salida = GetPosition(ByteStuffing(frame.Payload));
            if (salida == null) return null;
            // ACK
            salida.AddBytesToSend(MakeAck(frame.Payload));
            return salida;
        }

        #endregion
        
        #region Metodos especificos de MaxTrack 

    	private static int GetDeviceId(byte[] request) 
        {
            var did = 0;
            did |= request[3];
            did |= request[4]<<8;
            did |= request[5]<<16;
            did |= request[6]<<24;
            return did;
        }

        public static void PrintBufferAsCode(IEnumerable<byte> buffer)
        {
            Debug.Write("byte[] buffer = { ");
            var c = 0;
            foreach (var b in buffer)
            {
				Debug.Write(String.Format("0x{0}, ", b.ToString("X")));                
                c++;
            	if ((c%8) != 0) continue;
            	Debug.WriteLine("");
            	Debug.Write("                    ");
            }
            Debug.WriteLine(" };");
        }

        public static byte[] ByteStuffing(byte[] source)
        {
            int c = source.Count(b => b == 0x10);
        	if (c == 0) return source;
            int six = 0;
            var result = new byte[source.GetLength(0) - c];
            for (int i = 0; i < result.GetLength(0); ++i)
            {
                byte data = source[six++];
                if (data == 0x10) {
                    data = source[six++];
                    data -= 0x20;
                }
                result[i] = data;
            }
            return result;
        }
        // Convert four byte array elements to a float and display it.
    	private static float DecodeFloat(byte[] bytes, int index)
        {
            return BitConverter.ToInt32(bytes, index) / (float)1000000.0;
        }
        

        public static IMessage GetPosition(byte[] request)
        {
            var did = GetDeviceId(request);
            if (request[7] != 0x08) return null;

			var positionCount = BitConverter.ToUInt16(request, 9);
            var dt = GetDateTime(request, 11);
            var latitude = DecodeFloat(request, 15);
            var longitude = DecodeFloat(request, 19);
            var speed = new Speed(request[27]).Unpack();

			return GPSPoint.Factory(dt, latitude, longitude, speed).ToPosition(did, positionCount);
        }

    	private static DateTime GetDateTime(byte[] buffer, int ix)
        {
			var result = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dias = (buffer[ix+3] << 7) + ((buffer[ix+2]&0xFE)>>1);
            result = result.AddDays(dias);
            var horas = ((buffer[ix + 2] & 0x1) << 4) + ((buffer[ix + 1] & 0xF0) >> 4);
            result = result.AddHours(horas);
            var minutos = ((buffer[ix + 1] & 0x0F) << 2) + ((buffer[ix] & 0xC0) >> 6);
            result = result.AddMinutes(minutos);
            var segundos = (buffer[ix] & 0x3F);
            result = result.AddSeconds(segundos);
            return result;
        }

        public static byte[] MakeAck(byte[] request)
        { 
            var reply = new byte[12];
            reply[0] = 0x01;
            reply[1] = request[1];
            reply[2] = 0x02;
            reply[3] = request[3];
            reply[4] = request[4];
            reply[5] = request[5];
            reply[6] = request[6];
            reply[7] = request[request.GetLength(0) - 3];
            reply[8] = request[request.GetLength(0) - 2];
            ushort crc = Crc16Ccitt(reply);
            reply[9] = (byte)((crc & 0x00FF));
            reply[10] = (byte)((crc & 0xFF00) >> 8);
            reply[11] = 0x04;
            return reply;        
        }

        public static ushort Crc16Ccitt(byte[] buffer)
        {
            ushort crc = 0;
            for (int i = 1; i < (buffer.GetLength(0) - 3); ++i)
            {
                crc = Crc16Ccitt(crc, buffer[i]);
            }
            return crc;
        }

        public static ushort Crc16Ccitt(ushort crc, byte b)
        {
            const ushort polynomial = 0x1021; // 0001 0000 0010 0001  (0, 5, 12) 
            for (int i = 0; i < 8; i++) {
                bool bit = ((b   >> (7-i) & 1) == 1);
                bool c15 = ((crc >> 15    & 1) == 1);
                crc <<= 1;
                if (c15 ^ bit) crc ^= polynomial;
             }
            return crc;
        }

        #endregion
    }
}
