using System;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Model;
using Logictracker.Utils;

namespace Logictracker.RedSos
{
    [FrameworkElement(XName = "RedSosParser", IsContainer = false)]
    public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Redsos; } }

		#region Attributes
		
		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 2000)]
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
			var s = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length);
            if (!s.StartsWith("$D,")) return null;
            var nodecode = Convert.ToInt32(s[1]);
            //var msgid = Convert.ToUInt32(s[2]);
			return DataProvider.Get(nodecode, this);
        }

		public override IMessage Decode(IFrame frame)
        {
            /*
             * $D         = Packet header
               Campo 1	   = ID de equipo 6 dígitos
               Campo 2	   = Numero de paquete
               Campo 3	   = Numero de Evento
               Campo 4	   = Latitud
               Campo 5	   = Longitud
               Campo 6	   = Velocidad (nudos)
               Campo 7	   = Rumbo
               Campo 8	   = Edad de posición
               Campo 9	   = Estado de entradas
               Campo 10   = Estado de salidas
               Campo 11   = Fecha
               Campo 12   = Hora
               Campo 13   = Tensión de alimentación (x 0.07 = Volt)
               Campo 14   = Tensión de batería (x0.07 = Volt)
               Campo 15   = Corriente de antena GPS (x1.21 = mA)
               Campo 16   = Tiempo de reporte en minutos
               Campo 17   = Tiempo de reporte 2 en minutos
               Campo 18   = Clave de acceso
               Campo 19   = Nivel de señal GSM
               Campo 20   = Tipo de paquete de datos (significado de Dato 1 – 4)
               Campo 21   = Dato 1
               Campo 22   = Dato 2
               Campo 23   = Dato 3
               Campo 24   = Dato 4
               $E	   = Packet end
             */
			var s = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length).Split(',');
            var nodecode = Convert.ToInt32(s[1]);
            var msgid = Convert.ToUInt32(s[2]);
            var dt = DateTime.Parse(s[11] + " " + s[12]);
			var speed = Convert.ToSingle(s[6]);
        	var lat = Convert.ToSingle(s[4]);
        	var lon = Convert.ToSingle(s[5]);
        	var ack = String.Format("$B,{0},ACK={1},$E", s[1], s[2]);
			return GPSPoint.Factory(dt, lat, lon, speed)
				.ToPosition(nodecode, msgid)
				.AddStringToSend(ack);
        }

        #endregion
    }
}
