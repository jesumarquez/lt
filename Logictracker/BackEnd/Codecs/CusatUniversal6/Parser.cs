using System;
using Logictracker.AVL.Messages;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using System.Runtime.InteropServices;
using Logictracker.DAL.Factories;

namespace Logictracker.Backend.Codec.CusatUniversal
{
	[FrameworkElement(XName = "CusatUniversal6", IsContainer = false)]
	public class Parser : BaseCodec
	{
        public override NodeTypes NodeType { get { return NodeTypes.CusatUniv6; } }
        private static DAOFactory _daoFactory = new DAOFactory();

		#region Attributes

		[ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 6070)]
		public override int Port { get; set; }

		[ElementAttribute(XName = "Empresa", IsRequired = false, DefaultValue = "")]
		public String Empresa { get; set; }

		[ElementAttribute(XName = "UseIntern", IsRequired = false, DefaultValue = false)]
		public bool UseIntern { get; set; }

		[ElementAttribute(XName = "SwapInternAndPatent", IsRequired = false, DefaultValue = false)]
		public bool SwapInternAndPatent { get; set; }

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
            string buffer = AsString(frame);
            if (String.IsNullOrEmpty(buffer)) return null;


            CusatUniversalParser parser = new CusatUniversalParser(buffer);

            return DataProvider.FindByIMEI(parser.Matricula.Trim(), this);
        }

		public override IMessage Decode(IFrame frame)
        {
            IMessage salida = null;
            string buffer = AsString(frame);
            if (buffer == null) return null;

            CusatUniversalParser parser = new CusatUniversalParser(buffer);
          
            /*
          
             * 
             >RU1PBN177  110316154602-3292596-06883632 325 500 000 10 80 04 012|#1BDE4B<
                  patente 8 caracteres DDMMYYHHMMSS   8dig lat    8dig long  fix3d  age 3  vel    Sat  dop  enhex 2   evento 3
             >RU1 PBN177  ------------ 110316154602  -3292596   -0688363      2     325    500     80    04        012
              
             * 
             * 
             * 
            Universal 6, este es nuestro y hay algunas variantes mas ( ancho fijo ):
RU1XYZ12345DDMMYYHHMMSS-3400000-58000003000000000080280017|#1234
RU1 = OPCODE
XYZ12345 = MATRICULA ( 8)
DDMMYYHHMMSS = FECHA
-3400000 = LAT
-5800000 = LON
3 = Fix 3D
000 = Age
000 = Vel
000 = Dir
08 = Sat
02 = Dop
80 = ent en hexa ( 128 )
017 = evento
1234 = número de bloque
NOTA: entre “|” y “#” está pensado para poner data ( quizás variable ) en el futuro, nunca se usó.
             * 
             * 
             * 
             * 
             
             */
            ulong msgId = NextSequence;// ulong.Parse(parser[6]); 
            GPSPoint pos;
            var code = EventCodes.Position;
            var time = DateTime.ParseExact(parser.Fecha, "ddMMyyHHmmss",
                           System.Globalization.CultureInfo.InvariantCulture);
            var lat = float.Parse(parser.Latitud.Insert(3,","));// parser[2].Replace('.', ','));
            var lon = float.Parse(parser.Longitud.Insert(4,","));
            var vel = float.Parse(parser.Vel);
            var dir = float.Parse(parser.Dir);

            short codeevent = (short)0;

            /*
             0=PEDIDO DE POSICION
1=BOTON DE PANICO
8=CONTACTO ON
15=BAT. DESCONECTADA
16=CONTACTO OFF
17=POSICION EN CONTACTO
18=POSICION SIN CONTACTO
23=RESET DE EQUIPO
             
             * 0=PEDIDO DE POSICION
8001=BOTON DE PANICO
8008=CONTACTO ON
8015=BAT. DESCONECTADA
8016=CONTACTO OFF
8017=POSICION EN CONTACTO
8018=POSICION SIN CONTACTO
8023=RESET DE EQUIPO

             * 
             * 
             */

            switch (parser.Evento)
            {
                case "001":
                    {
                        codeevent = 8001; //8001=BOTON DE PANICO
                    }
                    break;
                case "008":
                    {
                        codeevent = 8008; //8008=CONTACTO ON
                    }
                    break;
                case "015":
                    {
                        codeevent = 8015; //8015=BAT. DESCONECTADA
                    }
                    break;
                case "016":
                    {
                        codeevent = 8016; //8016=CONTACTO OFF
                    }
                    break;
                case "017":
                    {
                        codeevent = 8017; //8017=POSICION EN CONTACTO
                    }
                    break;
                case "018":
                    {
                        codeevent = 8018; //8018=POSICION SIN CONTACTO
                    }
                    break;
                case "023":
                    {
                        codeevent = 8023; //8023=RESET DE EQUIPO
                    }
                    break;
                default:
                    break;
            }



            var hdop = 0;
            pos = GPSPoint.Factory(time, lat, lon, vel, dir, 0, hdop);
            var device = DataProvider.FindByIMEI(parser.Matricula, this);
            var deviceid = 0;
            if (device == null)
            {
                var empresa = _daoFactory.EmpresaDAO.FindByCodigo("QP");
                var tipodispositivo = _daoFactory.TipoDispositivoDAO.FindByModelo("CUSATUNIVERSAL6");
                Dispositivo newdispo = new Dispositivo();
                newdispo.Empresa = empresa;
                //  newdispo.Linea = _daoFactory.LineaDAO.FindByNombre(empresa.Id, "Generica");
                newdispo.TipoDispositivo = tipodispositivo;
                newdispo.Clave = parser.Matricula.ToString();
                newdispo.Tablas = "";
                newdispo.Port = 6070;
                //  newdispo.Firmware = _daoFactory.FirmwareDAO.FindById(5);
                newdispo.Imei = parser.Matricula.ToString();
                newdispo.Codigo = parser.Matricula.ToString();
                _daoFactory.DispositivoDAO.Save(newdispo);
                if (_daoFactory.CocheDAO.FindByPatente(empresa.Id, parser.Matricula.ToString()) == null)
                {
                    Coche newcoche = new Coche();
                    newcoche.Patente = parser.Matricula.ToString();
                    newcoche.Interno = parser.Matricula.ToString();
                    newcoche.ModeloDescripcion = "";
                    newcoche.Empresa = empresa;
                    newcoche.Dispositivo = newdispo;
                    _daoFactory.CocheDAO.Save(newcoche);
                }
                else
                {
                    Coche coche = _daoFactory.CocheDAO.FindByPatente(empresa.Id, parser.Matricula.ToString());
                    coche.Dispositivo = newdispo;
                    _daoFactory.CocheDAO.Save(coche);
                }
                deviceid = newdispo.Id;
            }
            else
            {
                deviceid = DataProvider.FindByIMEI(parser.Matricula.ToString(), this).Id;
            }

            if (codeevent == 0)
            {
                salida = pos.ToPosition(deviceid, msgId);
            }
            else
            {
                salida = new Event(codeevent, -1, deviceid, msgId, pos, pos.GetDate(), "", new List<long>(), true);
            }

            return salida;
            /*



			if (ParserUtils.IsInvalidDeviceId(Id)) return null;

			var buffer = AsString(frame);
            if (buffer == null || !buffer.Contains(">RU")) return null;

			var dt = DateTimeUtils.SafeParseFormat(buffer.Substring(28, 12), "ddMMyyHHmmss");
			var codEv = GetEventCode(buffer);

			//var msgId = GetMsgId(buffer); //siempre es 0001!!!
			var msgId = (ulong)((dt.Ticks << 8) + codEv);


			var gpsPoint = ParseRu2(buffer, dt);
			return GetSalida(gpsPoint, dt, "00000000", codEv, this, msgId);*/
        }

        public class CusatUniversalParser
        {
            public String Inicio = "";
            public String OpCode = "";
            public String Matricula = "";
            public String Fecha = "";
            public String Latitud = "";
            public String Longitud = "";
            public String Fix3d = "";
            public String Age = "";
            public String Vel = "";
            public String Dir = "";
            public String Sat = "";
            public String Dop = "";
            public String HexInt = "";
            public String Evento = "";
            public String Separador = "";
            public String Bloque = "";
            public String Final = "";

            public CusatUniversalParser(string cadena)
            {
                Inicio = cadena.Substring(0, 1);
				OpCode = 			cadena.Substring(1,  3);
				Matricula =			cadena.Substring(4,  8);
				Fecha = 			cadena.Substring(12,  12);
				Latitud =			cadena.Substring(24,  8);
				Longitud =			cadena.Substring(32,  8);
				Fix3d = 			cadena.Substring(40,  1);
				Age =   			cadena.Substring(41,  3);
				Vel =   			cadena.Substring(44,  3);
				Dir =   			cadena.Substring(47,  3);
				Sat =   			cadena.Substring(50,  2);
				Dop =   			cadena.Substring(52,  2);
				HexInt = 			cadena.Substring(54,  2);
				Evento = 			cadena.Substring(56,  3);
				Separador =			cadena.Substring(59,  2);
				Bloque = 			cadena.Substring(61,  6);
				Final = 			cadena.Substring(67,  1);
            }
        }
    
  


        private static class EventCodes
        {
            public const int Mpx01Sms = 1;

            public const int Position = 2;



            public const int PistonOn = 31;
            public const int PistonOff = 32;
            public const int DoorOn = 33;
            public const int DoorOff = 34;
            public const int PowerDisconnected = 35;
            public const int PowerReconnected = 36;
            public const int PanicButtonOn = 39;
            public const int PanicButtonOff = 40;
            public const int AmbulanciaSirenaOn = 41;
            public const int AmbulanciaSirenaOff = 42;
            public const int AmbulanciaBalizaOn = 43;
            public const int AmbulanciaBalizaOff = 44;

            public const int GarminOn = 48;
            public const int GarminOff = 47;

            public const int SensorPowerDisconnected = 51;

            public const int Trompo = 55;
            public const int Tolva = 56;

            public const int JammingOn = 57;
            public const int JammingOff = 58;
            public const int MotorOn = 61;
            public const int MotorOff = 62;
            public const int GsmOn = 63;
            public const int GsmOff = 64;
            public const int GpsSignalOn = 65;
            public const int Gps3DSignalOff = 66;
            public const int Gps2DSignalOn = 67;
            public const int GpsSignalOff = 68;
            public const int ChoferLoggedOn = 69;
            public const int ChoferLoggedOff = 70;
            public const int EmployeeLoggedOn = 71;
            public const int EmployeeLoggedOff = 72;

            public const int CustomMsg1On = 73;
            public const int CustomMsg1Off = 74;
            public const int CustomMsg2On = 75;
            public const int CustomMsg2Off = 76;
            public const int CustomMsg3On = 77;
            public const int CustomMsg3Off = 78;

            public const int PrivacyOn = 80;
            public const int PrivacyOff = 81;

            public const int InfraccionInicio = 97;
            public const int Infraccion = 99;
            public const int ResetLostGeogrilla = 0xA3;
            public const int RegainGeogrilla = 0xA4;
            public const int InfraccionInicio2 = 0xC0;
            public const int Infraccion2 = 0xBD;
            public const int DistanciaMotorOff = 0xCC;
            public const int DistanciaSinChofer = 0xCF;

            //public const int TemperatureAlarmLow = 0xEB;
            //public const int TemperatureAlarmHigh = 0xEC;

            /*public const int RpmDetenido = 0xF0;
			public const int RpmVelBaja = 0xF1;
			public const int RpmVelAlta = 0xF2;//*/

            public const int FrenadaAbrupta = 0xF4;
            //public const int AcceleracionAbrupta = 0xF5;
            public const int PowerOn = 0xFE;

            //Cusat
            public const int DoorApCabinActive = 800; //ap_puerta_cabina_activa
            public const int DisengageActive = 801; //desenganche_activa
            public const int SubstituteViolation = 802; //violacion_sustituto
            public const int DoorApVanActive = 803; //ap_puerta_furgon_activa
            public const int DoorApCabinPassive = 804; //ap_puerta_cabina_pasiva
            public const int DisengagePassive = 805; //desenganche_pasiva
            public const int DoorApVanPassive = 806; //ap_puerta_furgon_pasiva
            public const int ActiveAlertsInhibitorButtonOn = 807; //boton_inhibidor_de_alertas_activas_presionado
        }

		public override String AsString(IFrame frame)
		{
			if (frame.PayloadAsString == null)
			{
				frame.PayloadAsString = base.AsString(frame);
				var ini = frame.PayloadAsString.IndexOf('>');
				if (ini >= 0)
				{
					var len = (frame.PayloadAsString.LastIndexOf('<') - ini) + 1;
					if (len > 0)
					{
						frame.PayloadAsString = frame.PayloadAsString.Substring(ini, len);
						//ParserUtils.CheckChecksumOk(frame.PayloadAsString, ";*", "<", GetCheckSum);

						/*if (frame.PayloadAsString.Contains(">RU1"))
						{
							frame.PayloadAsString = ">RU2" + frame.PayloadAsString.Substring(4, 8) + "                " + frame.PayloadAsString.Substring(12);
						}//*/
					}
				}
			}
			return frame.PayloadAsString;
		}

        #endregion
        
        #region Members


		private static int GetEventCode(String buffer)
		{
			int codEv;
			try
			{
				codEv = Convert.ToInt32(buffer.Substring(73, 2));
			}
			catch (FormatException)
			{
				codEv = Convert.ToInt32(buffer.Substring(73, 2), 16);
			}
			return codEv;
		}

		private static GPSPoint ParseRu2(String buffer, DateTime dt)
		{
			var lat = Convert.ToSingle(buffer.Substring(40, 8)) * (float)0.00001;
			var lon = Convert.ToSingle(buffer.Substring(48, 9)) * (float)0.00001;
			var fix = Convert.ToInt32(buffer.Substring(57, 1)); //FIX 0=Err, 1=No fix, 2=2D, 3=3D y 4=OK
			if (fix < 2) return null;
			var age = Convert.ToInt32(buffer.Substring(58, 3));
			if ((age / 60) > 4) return null;
			var vel = Convert.ToSingle(buffer.Substring(61, 3));
			var dir = Convert.ToSingle(buffer.Substring(64, 3));
			//var cantSat = Convert.ToInt32(buffer.Substring(67, 2));
			//var pdop = Convert.ToInt32(buffer.Substring(69, 2));
			var entradas = Convert.ToByte(buffer.Substring(71, 2), 16);

			return new GPSPoint(dt, lat, lon, vel, GPSPoint.SourceProviders.Unespecified, 0)
			{
				Course = new Course(dir),
				IgnitionStatus = ((entradas & 0x80) == 0x80) ? IgnitionStatus.On : IgnitionStatus.Off,
			};
		}

		private static IMessage GetSalida(GPSPoint gpsPoint, DateTime dt, String rfid, int codEv, INode device, ulong msgId)
		{
			MessageIdentifier codigo;
			switch (codEv) // codigo del Evento generador del reporte
			{
				case Evento.Panico: codigo = MessageIdentifier.PanicButtonOn; break;
				case Evento.DoorOpenned: codigo = MessageIdentifier.DoorOpenned; break;
				case Evento.SemiTrailerUnhook: codigo = MessageIdentifier.TrailerUnHooked; break;
				case Evento.Substitute: codigo = MessageIdentifier.SubstituteViolation; break;
				case Evento.Generic01: codigo = MessageIdentifier.CustomMsg1On; break;
				case Evento.Generic02: codigo = MessageIdentifier.CustomMsg2On; break;
				case Evento.EngineOn: codigo = MessageIdentifier.EngineOn; break;
				case Evento.EngineOff: codigo = MessageIdentifier.EngineOff; break;
				//case Evento.Periodic01: goto default;
				//case Evento.Periodic02: goto default;
				case Evento.DeviceTurnedOn: codigo = MessageIdentifier.DeviceTurnedOn; break;
				default:
					var pos = gpsPoint.ToPosition(device.Id, msgId);
					pos.Tiempo = dt;
					return pos;
			}
			
			return codigo.FactoryEvent(device.Id, msgId, gpsPoint, dt, rfid, null);
		}

		//si no es uno de estos es una posicion
		private abstract class Evento
		{
			public const int Panico = 1;
			public const int DoorOpenned = 2;
			public const int SemiTrailerUnhook = 3;
			public const int Substitute = 4;
			public const int Generic01 = 5;
			public const int Generic02 = 6;
			public const int EngineOn = 8;
			public const int EngineOff = 16;
			//public const int Periodic01 = 17; //posicion
			//public const int Periodic02 = 18; //posicion
			public const int DeviceTurnedOn = 23;
		}

		#endregion
	}
}