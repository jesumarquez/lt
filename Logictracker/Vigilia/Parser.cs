using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Logictracker.AVL.Messages;
using Logictracker.Cache;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Layers;
using Logictracker.Layers.DeviceCommandCodecs;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;
using Logictracker.Utils;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Vigilia
{
    [FrameworkElement(XName = "VigiliaParser", IsContainer = false)]
    public partial class Parser : BaseCodec
    {
        private static DAOFactory _daoFactory = new DAOFactory();
        public override NodeTypes NodeType
        {
            get { return NodeTypes.Vigilia; }
        }

        private static readonly Dictionary<int, object> LocksByDevice = new Dictionary<int, object>();

        #region Code Rels

        private static Dictionary<int, MessageIdentifier> codeRels = new Dictionary<int, MessageIdentifier>()
                                                                         {
                                                                             {
                                                                                 EventCodes.MotorOn,
                                                                                 MessageIdentifier.EngineOnInternal
                                                                             },
                                                                             {
                                                                                 EventCodes.MotorOff,
                                                                                 MessageIdentifier.EngineOffInternal
                                                                             },
                                                                             {
                                                                                 EventCodes.GarminOn,
                                                                                 MessageIdentifier.GarminOn
                                                                             },
                                                                             {
                                                                                 EventCodes.GarminOff,
                                                                                 MessageIdentifier.GarminOff
                                                                             },
                                                                             {
                                                                                 EventCodes.PrivacyOff,
                                                                                 MessageIdentifier.PrivacyOff
                                                                             },
                                                                             {
                                                                                 EventCodes.PrivacyOn,
                                                                                 MessageIdentifier.PrivacyOn
                                                                             },
                                                                             {
                                                                                 EventCodes.PanicButtonOn,
                                                                                 MessageIdentifier.PanicButtonOn
                                                                             },
                                                                             {
                                                                                 EventCodes.PanicButtonOff,
                                                                                 MessageIdentifier.PanicButtonOff
                                                                             },
                                                                             {
                                                                                 EventCodes.AmbulanciaSirenaOn,
                                                                                 MessageIdentifier.SirenOn
                                                                             },
                                                                             {
                                                                                 EventCodes.AmbulanciaSirenaOff,
                                                                                 MessageIdentifier.SirenOff
                                                                             },
                                                                             {
                                                                                 EventCodes.AmbulanciaBalizaOn,
                                                                                 MessageIdentifier.BeaconOn
                                                                             },
                                                                             {
                                                                                 EventCodes.AmbulanciaBalizaOff,
                                                                                 MessageIdentifier.BeaconOff
                                                                             },
                                                                             {
                                                                                 EventCodes.GsmOn,
                                                                                 MessageIdentifier.GsmSignalOn
                                                                             },
                                                                             {
                                                                                 EventCodes.GsmOff,
                                                                                 MessageIdentifier.GsmSignalOff
                                                                             },
                                                                             {
                                                                                 EventCodes.GpsSignalOn,
                                                                                 MessageIdentifier.GpsSignalOn
                                                                             },
                                                                             {
                                                                                 EventCodes.GpsSignalOff,
                                                                                 MessageIdentifier.GpsSignalOff
                                                                             },
                                                                             {
                                                                                 EventCodes.Gps3DSignalOff,
                                                                                 MessageIdentifier.GpsSignal3DOff
                                                                             },
                                                                             {
                                                                                 EventCodes.Gps2DSignalOn,
                                                                                 MessageIdentifier.GpsSignal2DOn
                                                                             },
                                                                             {
                                                                                 EventCodes.PistonOff,
                                                                                 MessageIdentifier.PistonOff
                                                                             },
                                                                             {
                                                                                 EventCodes.PistonOn,
                                                                                 MessageIdentifier.PistonOn
                                                                             },
                                                                             {
                                                                                 EventCodes.DistanciaSinChofer,
                                                                                 MessageIdentifier.NoDriverMovement
                                                                             },
                                                                             {
                                                                                 EventCodes.DistanciaMotorOff,
                                                                                 MessageIdentifier.NoEngineMovement
                                                                             },
                                                                             {
                                                                                 EventCodes.PowerReconnected,
                                                                                 MessageIdentifier.PowerReconnected
                                                                             },
                                                                             {
                                                                                 EventCodes.PowerDisconnected,
                                                                                 MessageIdentifier.PowerDisconnected
                                                                             },
                                                                             {
                                                                                 EventCodes.DoorOn,
                                                                                 MessageIdentifier.DoorOpenned
                                                                             },
                                                                             {
                                                                                 EventCodes.DoorOff,
                                                                                 MessageIdentifier.DoorClosed
                                                                             },
                                                                             {
                                                                                 EventCodes.FrenadaAbrupta,
                                                                                 MessageIdentifier.DesaccelerationEvent
                                                                             },
                                                                             {
                                                                                 EventCodes.SensorPowerDisconnected,
                                                                                 MessageIdentifier.SensorPowerDisconected
                                                                             },
                                                                             {
                                                                                 EventCodes.ResetLostGeogrilla,
                                                                                 MessageIdentifier.ResetLostGeogrilla
                                                                             },
                                                                             {
                                                                                 EventCodes.RegainGeogrilla,
                                                                                 MessageIdentifier.RegainGeogrilla
                                                                             },
                                                                             {
                                                                                 EventCodes.CustomMsg1On,
                                                                                 MessageIdentifier.CustomMsg1On
                                                                             },
                                                                             {
                                                                                 EventCodes.CustomMsg1Off,
                                                                                 MessageIdentifier.CustomMsg1Off
                                                                             },
                                                                             {
                                                                                 EventCodes.CustomMsg2On,
                                                                                 MessageIdentifier.CustomMsg2On
                                                                             },
                                                                             {
                                                                                 EventCodes.CustomMsg2Off,
                                                                                 MessageIdentifier.CustomMsg2Off
                                                                             },
                                                                             {
                                                                                 EventCodes.CustomMsg3On,
                                                                                 MessageIdentifier.CustomMsg3On
                                                                             },
                                                                             {
                                                                                 EventCodes.CustomMsg3Off,
                                                                                 MessageIdentifier.CustomMsg3Off
                                                                             },
                                                                             {
                                                                                 EventCodes.JammingOn,
                                                                                 MessageIdentifier.JammingOn
                                                                             },
                                                                             {
                                                                                 EventCodes.JammingOff,
                                                                                 MessageIdentifier.JammingOff
                                                                             },
                                                                             {
                                                                                 EventCodes.DoorApCabinActive,
                                                                                 MessageIdentifier.DoorApCabinActive
                                                                             },
                                                                             {
                                                                                 EventCodes.DisengageActive,
                                                                                 MessageIdentifier.DisengageActive
                                                                             },
                                                                             {
                                                                                 EventCodes.SubstituteViolation,
                                                                                 MessageIdentifier.SubstituteViolation
                                                                             },
                                                                             {
                                                                                 EventCodes.DoorApVanActive,
                                                                                 MessageIdentifier.DoorApVanActive
                                                                             },
                                                                             {
                                                                                 EventCodes.DoorApCabinPassive,
                                                                                 MessageIdentifier.DoorApCabinPassive
                                                                             },
                                                                             {
                                                                                 EventCodes.DisengagePassive,
                                                                                 MessageIdentifier.DisengagePassive
                                                                             },
                                                                             {
                                                                                 EventCodes.DoorApVanPassive,
                                                                                 MessageIdentifier.DoorApVanPassive
                                                                             },
                                                                             {
                                                                                 EventCodes.ActiveAlertsInhibitorButtonOn,
                                                                                 MessageIdentifier.
                                                                                 ActiveAlertsInhibitorButtonOn
                                                                             },
                                                                             {
                                                                                 EventCodes.PowerOn,
                                                                                 MessageIdentifier.DeviceTurnedOn
                                                                             }
                                                                         };

        #endregion
        
        #region Attributes

        private Boolean? _isGarminConnected;

        [ElementAttribute(XName = "UseIdAsImei", IsRequired = false, DefaultValue = false)]
        public bool UseIdAsImei { get; set; }

        [ElementAttribute(XName = "Port", IsRequired = false, DefaultValue = 6065)]
        public override int Port { get; set; }

        [ElementAttribute(XName = "CheckIMEI", IsRequired = false, DefaultValue = true)]
        public bool CheckIMEI { get; set; }


        #endregion

        #region private

        private static object GetDeviceLock(int deviceId)
        {
            lock (LocksByDevice)
            {
                if (!LocksByDevice.ContainsKey(deviceId)) LocksByDevice.Add(deviceId, new object());
                return LocksByDevice[deviceId];
            }
        }

        #endregion private

        #region BaseCodec

        protected override UInt32 NextSequenceMin()
        {
            return 0x0000;
        }

        protected override UInt32 NextSequenceMax()
        {
            return 0xFFFF;
        }

        private String extractIMEIFromRGRI(String buffer)
        {
            return buffer.Replace(Reporte.IMEIResponsePrefix, "").Split(';')[0].Trim();
        }

        public override INode Factory(IFrame frame, int formerId)
        {
            string buffer = AsString(frame);
            if (String.IsNullOrEmpty(buffer)) return null;

//            if (UseIdAsImei) return DataProvider.FindByIMEI(GetDeviceIdAsString(buffer), this);

            if (buffer.StartsWith(Reporte.SetIdResponsePrefix))
            {
                return
                    DataProvider.Get(
                        Convert.ToInt32(
                            buffer.Replace(Reporte.SetIdResponsePrefix, "").Replace(",SHOW", "").Split(';')[0].Split('<')
                                [0]), this);
            }

            if (buffer.StartsWith(Reporte.IMEIResponsePrefix))
            {
                var imei = extractIMEIFromRGRI(buffer);
                _askedIMEI = null;
                return DataProvider.FindByIMEI(imei, this);
            }

            int devId = ParserUtils.GetDeviceIdTaip(buffer);
            if (!ParserUtils.IsInvalidDeviceId(devId))
                return DataProvider.FindByIdNum(devId, this);

            return DataProvider.FindByIMEI(GetImei(buffer), this);
        }

        private DateTime _dandFeaturesLastTimeAsked = DateTime.MinValue;

        private DateTime _garminSetupLastTimeSent = DateTime.MinValue;

        private String _askedIMEI = null;

        private void AskIMEI(ref IMessage msg)
        {
            if (msg == null)
                msg = new UserMessage(Id, 0);

            var result = BaseDeviceCommand.createFrom(String.Format(">{0}<", Mensaje.IMEIReq), this, null).ToString(true);
            msg.AddStringToSend(result);

        }

        private void AskIDandFeatures(ref IMessage msg)
        {
            if (msg == null)
                msg = new UserMessage(Id, 0);

            var result =
                BaseDeviceCommand.createFrom(String.Format(">{0}<", Mensaje.ForceIdReq), this, null).ToString(
                    true);
            msg.AddStringToSend(result);
            _dandFeaturesLastTimeAsked = DateTime.UtcNow;
        }

        public override IMessage Decode(IFrame frame)
        {
            return Decode(frame, true);
        }

        private static string QueryPartition(Parser dev)
        {
            var sb = new StringBuilder();
            var query = dev.DataProvider.GetDetalleDispositivo(dev.Id, "QuerySdOnHandshake").As("");

            STrace.Trace(typeof(Mensaje).FullName, dev.Id, string.Format("{0}", query));

            switch (query)
            {
                case "GeogrillaPartition":
                    sb.AppendFormat("{0}{1}", Mensaje.Factory<String>(dev, "QSDG1,R0000008004"), Environment.NewLine);
                    sb.AppendFormat("{0}{1}", Mensaje.Factory<String>(dev, "QSDG1,R0000000036"), Environment.NewLine);
                    sb.AppendFormat("{0}{1}", Mensaje.Factory<String>(dev, "QSDG1,R21DE09F050"), Environment.NewLine);
                    break;
                case "ImagePartition":
                    sb.AppendFormat("{0}{1}", Mensaje.Factory<String>(dev, "QSDI,S"), Environment.NewLine);
                    //responde: >RSDI,S,imageCount,usedMemory,lastImageOffset
                    break;
            }
            return sb.ToString();
        }


        
        public override String AsString(IFrame frame)
        {
            if (frame.PayloadAsString == null)
            {
                frame.PayloadAsString = Encoding.ASCII.GetString(frame.Payload, 0, frame.Payload.Length).Replace("<QP>", "");
                var ini = frame.PayloadAsString.IndexOf('>');
                if (ini >= 0)
                {
                    var len = (frame.PayloadAsString.IndexOf('<', ini) - ini) + 1;
                    if (len >= 2)
                    {
                        frame.PayloadAsString = frame.PayloadAsString.Substring(ini, len);
                        ParserUtils.CheckChecksumOk(frame.PayloadAsString, ";*", "<", Mensaje.GetCheckSum);
                    }
                }
            }
            return frame.PayloadAsString;
        }

        #endregion


        public IMessage Decode(IFrame frame, bool online)
        {
            IMessage salida = null;
            string buffer = AsString(frame);
            if (buffer == null) return null;
            string[] parser = buffer.Split(new string[] { "|" }, StringSplitOptions.None);
            /*
             patente|Fecha|latitud|longitud|velocidad|sentido|evento 
            Ejemplo : XXX111|2015-12-29 10:01:32|-33.95115|-59.40005|90|150|01|            
             
             */
            ulong msgId = ulong.Parse(buffer.Substring(58,2));
            GPSPoint pos;
            var code = EventCodes.Position;
            var time = DateTime.ParseExact(buffer.Substring(9,19), "yyyy-MM-dd HH:mm:ss",
                           System.Globalization.CultureInfo.InvariantCulture);
            var lat = float.Parse(buffer.Substring(29, 9), CultureInfo.InvariantCulture);
            var lon = float.Parse(buffer.Substring(39,10),CultureInfo.InvariantCulture);
            var vel = float.Parse(buffer.Substring(50,3));
            var dir = float.Parse(buffer.Substring(54,3));
            var patente = buffer.Substring(2,6);
            
            short codeevent = (short)0;
            switch (buffer.Substring(58, 2))
            {
                case "1":
                    {
                        codeevent = 5001;
                    }
                    break;
                case "3":
                    {
                        codeevent = 5003;
                    }
                    break;
                case "4":
                    {
                        codeevent = 5004;
                    }
                    break;
                case "5":
                    {
                        codeevent = 5005;
                    }
                    break;
                case "50":
                    {
                        codeevent = 5050;
                    }
                    break;
                case "51":
                    {
                        codeevent = 5051;
                    }
                    break;
                case "53":
                    {
                        codeevent = 5053;
                    }
                    break;
                default:
                    break;
            }

            var hdop = 0;
            pos = GPSPoint.Factory(time, lat, lon, vel, dir, 0, hdop);
            var device = DataProvider.FindByIMEI(patente, this);
            var deviceid = 0;
            if (device == null)
            {
                var empresa = _daoFactory.EmpresaDAO.FindByCodigo("LA");
                var tipodispositivo = _daoFactory.TipoDispositivoDAO.FindByModelo("VIGILIA");
                

                Dispositivo newdispo = new Dispositivo();
                newdispo.Empresa = empresa;
                newdispo.TipoDispositivo = tipodispositivo;
                newdispo.Clave = patente;
                newdispo.Tablas = "";
                newdispo.Port = 6068;
                newdispo.Imei = patente;
                newdispo.Codigo = patente;
                _daoFactory.DispositivoDAO.Save(newdispo);

                if (_daoFactory.CocheDAO.FindByPatente(empresa.Id, parser[0].ToString()) == null)
                {
                    var modeloDao = new ModeloDAO();
                    var marcaDao = new MarcaDAO();
                    var modelo = modeloDao.FindByCodigo(empresa.Id,-1,"Generico");
                    var marca = marcaDao.GetByDescripcion(empresa.Id, -1, "Generica");

                    var tipoVehiculoDao = new TipoCocheDAO();

                    Coche newcoche = new Coche();
                    newcoche.Patente = patente;

                    var interno = patente + "-VIGILIA"; 
                    newcoche.Interno = interno;
                    newcoche.Empresa = empresa;
                    newcoche.Dispositivo = newdispo;
                    newcoche.Marca = marca;
                    newcoche.Modelo = modelo;
                    newcoche.ModeloDescripcion = modelo.Descripcion;
                    newcoche.Poliza = patente;
                    newcoche.TipoCoche = tipoVehiculoDao.FindByCodigo(empresa.Id, -1, "CM");
                    
                    _daoFactory.CocheDAO.Save(newcoche);
                }
                else
                {
                    Coche coche = _daoFactory.CocheDAO.FindByPatente(empresa.Id, patente);
                    coche.Dispositivo = newdispo;
                    _daoFactory.CocheDAO.Save(coche);
                }
                deviceid = newdispo.Id;
            }
            else
            {
                deviceid = DataProvider.FindByIMEI(patente, this).Id;
            }

            salida = new Event(codeevent, -1, deviceid, msgId, pos, pos.GetDate(), "", new List<long>(), true);
                      
            return salida;
        }



        #region config

        private DateTime _lastConfigSentTimestamp = DateTime.MinValue;

        private IMessage GetSalida(GTEDeviceCommand dc, DeviceStatus ds)
        {
            if (ds == null) return null;
            if (ds.Position == null) return null;

            var ev2Resolve = Translate(ds.FiredEventNumber); //TODO: COMPLETE EVENT PROCESSING
//            MessageIdentifier evento = codeRels.ContainsKey(ev2Resolve) ? codeRels[ev2Resolve] : MessageIdentifier.NoMessage;

            MessageIdentifier evento = codeRels.ContainsKey(ds.FiredEventNumber)
                                           ? codeRels[ds.FiredEventNumber]
                                           : MessageIdentifier.NoMessage;

            switch (ds.FiredEventNumber)
            {
                case EventCodes.GarminOn:                    
                    IsGarminConnected = true;
                    break;
                case EventCodes.GarminOff:
                    IsGarminConnected = false;
                    break;                
            }

            if (evento != MessageIdentifier.NoMessage)
                return evento.FactoryEvent(Id, dc.MessageId ?? ParserUtils.MsgIdNotSet, ds.Position, ds.Position.GetDate(), null, null);

            return ds.Position.ToPosition(Id, dc.MessageId ?? ParserUtils.MsgIdNotSet);
        }
        
        private IMessage GetSalida(short codEv, IList<String> data, ulong msgId, GPSPoint gpsPoint, char engineStatus, String receivedRFID, char driverStatus, char sensor)
        {
            String rfid = decodeRFID(receivedRFID, driverStatus);

            MessageIdentifier codigo = codeRels.ContainsKey(codEv) ? codeRels[codEv] : MessageIdentifier.NoMessage;

            switch (codEv) // codigo del Evento generador del reporte
            {
                case EventCodes.Mpx01Sms:
                    if (data.Count < 9) goto default;
                    try
                    {
                        int codeN = Convert.ToInt32(data[8]);
                        var code = (MessageIdentifier)codeN;

                        /*if (!MessageIdentifierX.IsEstadoLogistico(code_n))
						{
							//controlo que este bien seteado el Detalle GTE_MESSAGING_DEVICE
							var md = GetMessagingDevice();
							if (md != MessagingDevice.Sms) DataProvider.SetDetalleDispositivo(Id, "GTE_MESSAGING_DEVICE", MessagingDevice.Sms, "string");
						}//*/

                        return new Event(Event.GenericMessage, (short)code, Id, msgId, gpsPoint, gpsPoint.GetDate(),
                                         rfid, null, true);
                    }
                    catch
                    {
                        goto default;
                    }
                case EventCodes.Tolva:
                    codigo = TolvaCode(engineStatus, DataProvider.GetDetalleDispositivo(Id, "GTE_MODEL").As("TS6-SG") == "TS15-SG");
                    break;
                case EventCodes.Trompo:
                    codigo = GiroCode(sensor);
                    break;
                case EventCodes.ChoferLoggedOn:
                    return MessageIdentifierX.FactoryRfid(Id, msgId, gpsPoint, gpsPoint.GetDate(), rfid, 0);
                case EventCodes.ChoferLoggedOff:
                    return MessageIdentifierX.FactoryRfid(Id, msgId, gpsPoint, gpsPoint.GetDate(),
                                                          decodeRFID(receivedRFID, 'M'), 1);
                case EventCodes.EmployeeLoggedOn:
                    return MessageIdentifierX.FactoryRfid(Id, msgId, gpsPoint, gpsPoint.GetDate(), rfid, 3);
                case EventCodes.EmployeeLoggedOff:
                    return MessageIdentifierX.FactoryRfid(Id, msgId, gpsPoint, gpsPoint.GetDate(), rfid, 4);
                case EventCodes.PowerOn:
                    if (gpsPoint != null) gpsPoint.Date = DateTime.UtcNow;
                    break;
                case EventCodes.Infraccion:
                case EventCodes.Infraccion2:
                    float maxVel = Convert.ToSingle(data[8]);
                    float maxPermitida = ((data.Count > 10) && (!String.IsNullOrEmpty(data[10])))
                                             ? Convert.ToSingle(data[10])
                                             : (float)80.0;
                    GPSPoint posIni = ((data.Count > 11) && !String.IsNullOrEmpty(data[11]))
                                          ? Posicion.ParseCompact(data[11], false)
                                          : null;
                    if ((posIni != null) && (posIni.Velocidad > maxVel)) maxVel = posIni.Velocidad;
                    if ((gpsPoint != null) && (gpsPoint.Velocidad > maxVel)) maxVel = gpsPoint.Velocidad;

                    return new SpeedingTicket(Id, msgId, posIni, gpsPoint, maxVel, maxPermitida, rfid);
                case EventCodes.InfraccionInicio:
                case EventCodes.InfraccionInicio2:
                    goto default;
                default:
                    if (codigo == MessageIdentifier.NoMessage)
                        return gpsPoint.ToPosition(Id, msgId);
                    break;
            }
            return codigo.FactoryEvent(Id, msgId, gpsPoint, gpsPoint.GetDate(), rfid, null);
        }

        private static short GetCodEvent(String data)
        {
            try
            {
                return Int16.Parse(data);
            }
            catch
            {
                return Convert.ToInt16(data, 16);
            }
        }

        private static short Translate(Int32 code)
        {
            switch (code)
            {
                case EventCodesRpi.Panico:
                    return EventCodes.PanicButtonOn;
                case EventCodesRpi.ApPuertaCabinaActiva:
                    return EventCodes.DoorApCabinActive;
                case EventCodesRpi.DesengancheActiva:
                    return EventCodes.DisengageActive;
                case EventCodesRpi.ViolacionSustituto:
                    return EventCodes.SubstituteViolation;
                case EventCodesRpi.ContactoOn:
                    return EventCodes.MotorOn;
                case EventCodesRpi.ConexionBateriaPrincipal:
                    return EventCodes.PowerReconnected;
                case EventCodesRpi.ApPuertaFurgonActiva:
                    return EventCodes.DoorApVanActive;
                case EventCodesRpi.ContactoOff:
                    return EventCodes.MotorOff;
                case EventCodesRpi.DesconexionBateriaPrincipal:
                    return EventCodes.PowerDisconnected;
                case EventCodesRpi.ReportePeriodicoEnContacto:
                    return EventCodes.Position;
                case EventCodesRpi.ReportePeriodicoSinContacto:
                    return EventCodes.Position;
                case EventCodesRpi.ResetEq:
                    return EventCodes.PowerOn;
                case EventCodesRpi.ApPuertaCabinaPasiva:
                    return EventCodes.DoorApCabinPassive;
                case EventCodesRpi.DesenganchePasiva:
                    return EventCodes.DisengagePassive;
                case EventCodesRpi.ApPuertaFurgonPasiva:
                    return EventCodes.DoorApVanPassive;
                case EventCodesRpi.BotonInhibidorDeAlertasActivasPresionado:
                    return EventCodes.ActiveAlertsInhibitorButtonOn;
                case EventCodes.GarminOn:
                    return EventCodes.GarminOn;
                case EventCodes.GarminOff:
                    return EventCodes.GarminOff;
                default:
                    return EventCodes.Position;
            }
        }

        private static MessageIdentifier GiroCode(char data)
        {
            int onFlag = StringUtils.AreBitsSet(data, 0x1) ? 1 : 0;
            onFlag += StringUtils.AreBitsSet(data, 0x2) ? 2 : 0;
            onFlag += StringUtils.AreBitsSet(data, 0x4) ? 4 : 0;
            switch (onFlag)
            {
                case 7:
                    return MessageIdentifier.SpinStop2; //EVT_SPIN_NO_SENSOR
                case 6:
                    return MessageIdentifier.MixerClockwiseFast;
                case 5:
                    return MessageIdentifier.MixerCounterClockwiseFast;
                case 4:
                case 3:
                    return MessageIdentifier.SpinStop2; //EVT_SPIN_FAIL
                case 2:
                    return MessageIdentifier.MixerClockwiseSlow;
                case 1:
                    return MessageIdentifier.MixerCounterClockwiseSlow;
                default:
                    return MessageIdentifier.MixerStopped;
            }
        }

        private static MessageIdentifier TolvaCode(char data, bool isS15Flag)
        {
            return StringUtils.AreBitsSet(data, isS15Flag ? 0x1 : 0x2)
                       ? MessageIdentifier.TolvaActivated
                       : MessageIdentifier.TolvaDeactivated;
        }

        private static IMessage ProcessPictureData(String data, ulong msgId, INode node)
        {
            string dataCount = data.Substring(27, 4);
            if (dataCount != "0000")
            {
                //guardar data jpeg en archivo
                string basePath = Path.Combine(Config.Directory.PicturesDirectory, node.GetDeviceId().ToString("D4"));
                if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

                string fn = Path.Combine(basePath,
                                         String.Format("{0:D4}-{1}.jpeg", node.GetDeviceId(), data.Substring(9, 12)));
                List<byte> realData = StringUtils.HexStringToByteList(data, 31);
                ByteArrayUtils.BytesToFile(fn, realData.ToArray(), Convert.ToInt32(data.Substring(23, 4), 16));
                node.Store(CacheVar.PicturesSession, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            }
            var cmd = Mensaje.Factory<UserMessage>(msgId, node, "SPC{0}", data.Substring(4, 23));
            return cmd;
        }

        private static String GetImei(String buffer)
        {
            string[] datos = buffer.Split(',');
            return datos[0] == Reporte.IdReq ? datos[1] : null;
        }

        private static String EncodeMpx01Msg(String message, ulong trackingId, INode node)
        {
            string texto = message.ToUpper().PadRight(32);
            return Mensaje.Factory(trackingId, node, @"STMTRM#D1{0}@#D2{1}@#B50\2C100\2C0\2C0@",
                                   texto.Substring(0, 16), texto.Substring(16, 16));
        }

        private static String EncodeSmsMsg(String message, ulong trackingId, INode node)
        {
            return Mensaje.Factory(trackingId, node, "SSMS {0:ddMMyyHHmmss} {1}", DateTime.UtcNow, message);
        }

        private IMessage ParserMpx01BigKeyboardData(String[] data, int id, ulong msgId)
        {
            GPSPoint posic = Posicion.ParseCompact(data[1], false);
            string[] tyd = data[2].Split(" ".ToCharArray(), 2);
            string t = tyd[0];
            string d = tyd.Length == 1 ? null : tyd[1].Trim();
            switch (t)
            {
                case "SMS":
                    return new TextEvent(id, msgId, posic.GetDate())
                    {
                        Text = d,
                        GeoPoint = posic,
                    };
                case "LOGIN_RFID":
                    {
                        string rfid = decodeRFID(d, 'm');
                        return MessageIdentifierX.FactoryRfid(id, msgId, posic, posic.GetDate(), rfid, 3);
                    }
                case "LOGIN_LEGAJO":
                    {
                        Event msg = MessageIdentifierX.FactoryRfid(id, msgId, posic, posic.GetDate(), null, 3);
                        msg.SensorsDataString = d; //legajo
                        return msg;
                    }
                case "LOGOUT_RFID":
                    {
                        string rfid = decodeRFID(d, 'm');
                        return MessageIdentifierX.FactoryRfid(id, msgId, posic, posic.GetDate(), rfid, 4);
                    }
                case "LOGOUT_LEGAJO":
                    {
                        Event msg = MessageIdentifierX.FactoryRfid(id, msgId, posic, posic.GetDate(), null, 4);
                        msg.SensorsDataString = d; //legajo
                        return msg;
                    }
                default:
                    return new UserMessage(id, msgId);
            }
        }

        private static BaseDeviceCommand[] GetGarminDeviceCommands4Setup(Parser dev)
        {
            return new[]
                       {
                           GarminFmi.EncodeUserInterfaceText("Despacho Logictracker").ToTraxFM(dev),
                           GarminFmi.EncodeAutoArrival(null, null).ToTraxFM(dev),
                           GarminFmi.EncodeMessageThrottlingControl(FmiPacketId.CsRequestCannedResponseListRefresh, false).ToTraxFM(dev),
                           GarminFmi.EncodeMessageThrottlingControl(FmiPacketId.CsTextMessageStatus, false).ToTraxFM(dev)
                       };

        }

        private static void GarminSetup(Parser dev)
        {
            Fota.EnqueueOnTheFly(dev, 0, GetGarminDeviceCommands4Setup(dev));
        }

        private static void GarminSetup(ref StringBuilder s, Parser dev)
        {            
            foreach (var dc in GetGarminDeviceCommands4Setup(dev))
            {
                s.AppendFormat("{0}{1}", dc.ToString(), Environment.NewLine);
            }
        }

        

        private static IMessage ProcessHandshake(Parser dev, int formerDeviceId, String[] data, ulong msgId)
        {
            if (ParserUtils.IsInvalidDeviceId(dev.Id)) return null;

            var sb = new StringBuilder(); // to concatenate all fota commands to be queued

            sb.AppendFormat("{0}{1}", BaseDeviceCommand.createFrom(String.Format(">{0}<", Mensaje.QueryVersion), dev, null).ToString(true), Environment.NewLine);
            sb.AppendFormat("{0}{1}", BaseDeviceCommand.createFrom(String.Format(">{0}<", Mensaje.QueryPort), dev, null).ToString(true), Environment.NewLine);

            var msg = (IMessage) new ConfigRequest(dev.Id, msgId);

            STrace.Debug(typeof(Mensaje).FullName, dev.Id, string.Format("formerDeviceId {0}", formerDeviceId));

            if (ParserUtils.IsInvalidDeviceId(formerDeviceId))
            {                
                //check config
                string receivedconfighashRev = data.Length > 4 ? data[4] : "";
                bool receivedconfighashValid = receivedconfighashRev.StartsWith("Rev=") &&
                                                !receivedconfighashRev.Contains("$revision");
                string receivedconfighash = receivedconfighashRev.Split('=').Last();
                DetalleDispositivo replaceUnknownConfigurationS = dev.DataProvider.GetDetalleDispositivo(dev.Id,"ReplaceUnknownConfiguration");
                bool replaceUnknownConfiguration = replaceUnknownConfigurationS.As(false);

                string calculatedconfighash;
                var conf = dev.GetConfig(out calculatedconfighash);
                STrace.Debug(typeof(Mensaje).FullName, dev.Id, string.Format("Comparing hashes:{2}received={0}{2}calculated={1}{2}", receivedconfighash, calculatedconfighash, Environment.NewLine));


          

                if (dev._lastConfigSentTimestamp.AddMinutes(30) <= DateTime.UtcNow && replaceUnknownConfiguration)
                {
                    STrace.Trace(typeof(Mensaje).FullName, dev.Id,
                                 string.Format("Updating Config, hashes:{2}received={0}{2}calculated={1}{2}",
                                               receivedconfighash, calculatedconfighash, Environment.NewLine));
                    dev._lastConfigSentTimestamp = DateTime.UtcNow;
                    Fota.Enqueue(dev, 0, conf);
                }

                var result = String.Format("{0}{1}", BaseDeviceCommand.createFrom(String.Format(">{0}<", String.Format(Mensaje.SetId, dev.GetDeviceId())), dev, null).ToString(true), Environment.NewLine);
                msg.AddStringToSend(result);
                //Fota.EnqueueOnTheFly(dev, 0, result, ref msg);
            }
            else
            {
                var phone = (data.Length > 2) ? (data[2] ?? "").Trim(@"""+".ToCharArray()) : "";
                Int64 number;
                if (!string.IsNullOrEmpty(phone) && Int64.TryParse(phone, out number) && number > 0)
                {
                    var oldnumber = dev.DataProvider.GetDetalleDispositivo(dev.Id, "Telephone").As("");
                    if (oldnumber != phone)
                    {
                        STrace.Debug(typeof(Mensaje).FullName, dev.Id, string.Format("Setting Telephone: {0}", phone));
                        dev.DataProvider.SetDetalleDispositivo(dev.Id, "Telephone", phone, "string");
                    }
                }
            }

            if (data.Length > 5)
                dev.IsGarminConnected = data[5] == "1";

            Fota.EnqueueOnTheFly(dev, 0, sb.ToString());

            return msg;
        }

        private static String GetDeviceIdAsString(String buffer)
        {
            try
            {
                var ini = buffer.IndexOf(";ID=", StringComparison.Ordinal);

                if (ini == -1) return null;

                ini += 4;

                var len = buffer.IndexOf(';', ini) - ini;

                return buffer.Substring(ini, len);
            }
            catch
            {
                return null;
            }
        }
        #endregion


        #region Nested type: CacheVar

        private abstract class CacheVar
        {
            public const String PicturesSession = "PicturesSession";
        }

        #endregion

        //si viene un código que no esta en este listado se interpreta como posición

        #region Nested type: EventCodes

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

        #endregion

        #region Nested type: EventCodes_Rpi

        private static class EventCodesRpi
        {
            public const int Panico = 1;
            public const int ApPuertaCabinaActiva = 2;
            public const int DesengancheActiva = 3;
            public const int ViolacionSustituto = 4;
            public const int ContactoOn = 7;
            public const int ConexionBateriaPrincipal = 8;
            public const int ApPuertaFurgonActiva = 12;
            public const int ContactoOff = 15;
            public const int DesconexionBateriaPrincipal = 16;
            public const int ReportePeriodicoEnContacto = 17;
            public const int ReportePeriodicoSinContacto = 18;
            public const int ResetEq = 23;
            public const int ApPuertaCabinaPasiva = 92;
            public const int DesenganchePasiva = 93;
            public const int ApPuertaFurgonPasiva = 94;
            public const int BotonInhibidorDeAlertasActivasPresionado = 96;
        }

        #endregion

        #region Nested type: Reporte

        private static class Reporte
        {
            // constantes para mensajeria sms
            private const String Sm = ">RSM";
            public const String Sms = ">RSMS";
            public const String Sm0 = ">RSM0";
            public const String Sm1 = ">RSM1";
            public const String Saliente = ">RMT";

            // constantes de mensajes personalizados
            public const String Rpi = ">RPI";
            public const String Rph = ">RPH";

            // constantes de mensajes varios
            public const String Rdu = ">RDU";
            public const String Rsr = ">RSR"; // respuesta a un reseteo voluntario del equipo

            // constantes de mensajes personalizados
            private const String Personalizado = ">RUS";
            public const String IdReq = ">RUS00";
            public const String OldEvento = ">RUS01";
            public const String TemperatureInfo = ">RUS03";
            public const String Evento = ">RUS04";
            public const String Evento2 = ">RUS08";
            public const String FinInfraccion = ">RUS05";
            public const String Mpx01BigKeyboardData = ">RUS06";
            public const String TelemetricData = ">RUS07";

            // constantes de garmin
            public const String MensajeGarmin = ">RFM";
            public const String MensajeGarminReady = ">RFM.,READY";


            // constantes de serial
            public const String RTMTRM = ">RTMTRM";
            public const String RTMTR0 = ">RTMTR0";
            public const String RTMTR1 = ">RTMTR1";

            // constantes para el manejo de la sd
            private const String SdCommandAck = ">RSD";
            public const String PicturesRetrieveFinish = ">RSDI,F";
            public const String PictureRequestPrefix1 = ">QSDI,P";
            // constantes para la particion de geogrillas (QuadTree) de la memoria SD
            public const String GeoGrillaRevision = ">RSDG1,R0000008004";
            public const String GeoGrillaGeometry = ">RSDG1,R0000000036";
            public const String GeoGrillaData = ">RSDG1,R21DE09F050";
            public const String GeoGrillaFile = ">RSDG1,R0000008432";
            public const String SdGgAck = ">RSDG1,W";
            public const String SdPassword = ">RSDG1,U";
            public const String SdLocked = ">RSDG1,LOCKED";
            // constantes para la sesion de escritura en la particion de geogrillas (QuadTree) de la memoria SD
            public const String WriteGgPrefix = ">SSDG1,W";
            public const String QuerySdSessionPassword = ">QSDG1,U";
            public const String StartGgWriteSession = ">SSDG1,U";
            public const String SdImageState = ">RSDI,S";

            //demas constantes
            public const String SetIdPrefix = ">SID";
            public const String Error = ">E";
            public const String Picture = ">RPC";
            public const String SetIdResponsePrefix = ">RID";
            public const String IMEIResponsePrefix = ">RGRI";
            public const String FirmwareVersion = ">RVR";

            // constantes auxiliares
            public const String SinNodo = "SinNodo";
            public const String SinIMEI = "SinIMEI";
            public const String GetTime = ">GETTIME";
            public const String AnalogicInputs = ">RAD";

            public static String GetTipoReporte(String buffer)
            {
                if (buffer.Contains(Error)) return Error;
                if (buffer.Contains(GeoGrillaGeometry)) return GeoGrillaGeometry;
                if (buffer.Contains(GeoGrillaData)) return GeoGrillaData;
                if (buffer.Contains(GetTime)) return GetTime;
                if (buffer.Contains(MensajeGarmin)) return MensajeGarmin;
                if (buffer.Contains(MensajeGarminReady)) return MensajeGarminReady;
                if (buffer.Contains(SdGgAck)) return SdGgAck;
                if (buffer.Contains(SdLocked)) return SdLocked;
                if (buffer.Contains(SdPassword)) return SdPassword;
                if (buffer.Contains(GeoGrillaRevision)) return GeoGrillaRevision;
                if (buffer.Contains(FirmwareVersion)) return FirmwareVersion;
                if (buffer.Contains(GeoGrillaFile)) return GeoGrillaFile;
                if (buffer.Contains(AnalogicInputs)) return AnalogicInputs;
                if (buffer.Contains(SdImageState)) return SdImageState;
                if (buffer.Contains(Sm)) return buffer.Substring(0, 5);
                if (buffer.Contains(Personalizado)) return buffer.Substring(0, 6);
                if (buffer.Contains(SdCommandAck)) return buffer.Substring(0, 7);
                if (buffer.Contains(IMEIResponsePrefix)) return IMEIResponsePrefix;

                return buffer.Substring(0, 4);
            }
        }

        #endregion
    }
}