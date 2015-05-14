using System;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Utils;

namespace Logictracker.Orbcomm
{
    [FrameworkElement(XName = "OrbcommParser", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.Orbcomm; } }

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 0)]
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

		public override IMessage Decode(IFrame frame)
        {
            try
            {
                var type = frame.Payload[0];
                var time = GetTiempo(BitConverter_BigEndian.ToInt32(frame.Payload, 1));
                switch (type)
                {
                    case 1: //posicion
                        var lat = (float)(BitConverter_BigEndian.ToUInt24(frame.Payload, 5) / 100000.0);
                        var lon = (float)(BitConverter_BigEndian.ToUInt24(frame.Payload, 8) / 100000.0);
                        var signs = frame.Payload[11];
                        if (BitHelper.IsBitSet(signs, 2)) lat *= -1;
                        if (BitHelper.IsBitSet(signs, 1)) lon *= -1;
                        STrace.Debug(GetType().FullName, String.Format("posicion: {0}:{1}:{2} === {3}", time, lat, lon, frame.Payload));
						return GPSPoint.Factory(time, lat, lon).ToPosition(Id, 0);
                    case 2: //login
                        return GetLogEvent(frame.Payload, Id, time, 3); //quote dispatcher code: "case 3: return MessageCode.RfidEmployeeLogin.GetMessageCode();"
					case 3: //logout
						return GetLogEvent(frame.Payload, Id, time, 4); //quote dispatcher code: "case 4: return MessageCode.RfidEmployeeLogout.GetMessageCode();"
					case 6: //teclado: SMS, LOGIN_RFID, LOGOUT_RFID, LOGIN_LEGAJO, LOGOUT_LEGAJO
                		var strs = Encoding.ASCII.GetString(frame.Payload, 5, frame.Payload.Length - 5).Split(" ".ToCharArray(), 2);
                		var tipo = strs[0];
                		var texto = strs[1].Trim();
						switch (tipo)
						{
							case MensajeTeclado.Sms:
								return new TextEvent(Id, 0, time)
								       	{
								       		Text = texto,
								       	};
							case MensajeTeclado.LoginRfid:
								{
									var rfid = decodeRFID(texto, 'm');
									return MessageIdentifierX.FactoryRfid(Id, 0, null, time, rfid, 3);
								}
							case MensajeTeclado.LoginLegajo:
								{
									var msg = MessageIdentifierX.FactoryRfid(Id, 0, null, time, null, 3);
									msg.SensorsDataString = texto; //legajo
									return msg;
								}
							case MensajeTeclado.LogoutRfid:
								{
                                    var rfid = decodeRFID(texto, 'm');
									return MessageIdentifierX.FactoryRfid(Id, 0, null, time, rfid, 4);
								}
							case MensajeTeclado.LogoutLegajo:
								{
									var msg = MessageIdentifierX.FactoryRfid(Id, 0, null, time, null, 4);
									msg.SensorsDataString = texto; //legajo
									return msg;
								}
							default:
								return null;
						}
					default:
						return null;
				}
            }
            catch (Exception e)
            {
				STrace.Exception(GetType().FullName, e, Id, String.Format("Parser Orbcomm message: {0}", frame.Payload));
                return null;
            }
        }

		public override INode Factory(IFrame frame, int formerId)
		{
			return null;
        }

        #endregion

        #region Members

        private IMessage GetLogEvent(byte[] payload, int device, DateTime time, int data)
        {
            var rfid = decodeRFID(Encoding.ASCII.GetString(payload, 5, 8), 'm');
			STrace.Debug(typeof(Parser).FullName, String.Format("login/logout: {0}:{1} === {2}", time, rfid, payload));
            return MessageIdentifierX.FactoryRfid(device, 0, null, time, rfid, data);
        }

        private static DateTime GetTiempo(int dt)
        {
            if (((uint)dt) == 0x86444400) return ((GPSPoint)null).GetDate();

            var yy = dt / 32140800; //reemplazar por la centuria mas cercana a el mes de la fecha
            var mm = dt % 32140800 / 2678400;
            var dd = dt % 2678400 / 86400;
            var hh = dt % 86400 / 3600;
            var m = dt % 3600 / 60;
            var ss = dt % 60;
            if (dd == 0)
            {
                mm--;
                dd = 31;
            }
            if (mm == 0)
            {
                yy--;
                mm = 12;
            }

			try
			{
				return new DateTime(((DateTime.UtcNow.Year / 100) * 100) + yy, mm, dd, hh, m, ss, DateTimeKind.Utc);
			}
			catch
			{
				var dtdt = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, m, ss, DateTimeKind.Utc);
				if (dtdt.AddHours(-2) > DateTime.UtcNow) dtdt = dtdt.AddDays(-1);
				return dtdt;
			}
        }

        #endregion
	}

	public static class MensajeTeclado
	{
		public const String Sms = "SMS";
		public const String LoginRfid = "LOGIN_RFID";
		public const String LogoutRfid = "LOGOUT_RFID";
		public const String LoginLegajo = "LOGIN_LEGAJO";
		public const String LogoutLegajo = "LOGOUT_LEGAJO";
	}
}
