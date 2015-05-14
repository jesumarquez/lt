using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Logictracker.Cache;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Description.Attributes;
using Logictracker.Description.Runtime;
using Logictracker.Model;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Utils;

namespace Logictracker.Layers
{
    public abstract class BaseCodec : FrameworkElement, INode
    {
        private DeviceStatus _newestDeviceStatus;

        #region Attributes

        [ElementAttribute(XName = "DataTransportLayer", IsSmartProperty = true, IsRequired = true)]
        public IDataTransportLayer DataTransportLayer
        {
            get { return (IDataTransportLayer) GetValue("DataTransportLayer"); }
            set { SetValue("DataTransportLayer", value); }
        }

        [ElementAttribute(XName = "DataLinkLayer", IsSmartProperty = true, IsRequired = true)]
        public IDataLinkLayer DataLinkLayer
        {
            get { return (IDataLinkLayer) GetValue("DataLinkLayer"); }
            set { SetValue("DataLinkLayer", value); }
        }

        private static String MakeNewestDeviceStatusKey(int deviceId)
        {
            return "device_" + deviceId + "_newestDeviceStatus";
        }

        protected DeviceStatus NewestDeviceStatus
        {
            get 
            {
                if (_newestDeviceStatus == null)
                {
                    var key = MakeNewestDeviceStatusKey(Id);
                    _newestDeviceStatus = LogicCache.Retrieve<DeviceStatus>(typeof(DeviceStatus), key);
                }
                return _newestDeviceStatus;
            }
            set
            {
                _newestDeviceStatus = value;
                var key = MakeNewestDeviceStatusKey(Id);
                LogicCache.Store(typeof(DeviceStatus), key, _newestDeviceStatus);
            }
        }

        private String makeLastPacketReceivedAtKey(int deviceId)
        {
            return "device_" + deviceId + "_lastPacketReceivedAt";
        }

        public DateTime LastPacketReceivedAt
        {
            get
            {
                var key = makeLastPacketReceivedAtKey(Id);
                var dt = LogicCache.Retrieve<string>(key);
                return DateTime.ParseExact(dt, "O", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            }
            set
            {
                var key = makeLastPacketReceivedAtKey(Id);
                LogicCache.Store(key, value.ToString("O"));
            }
        }

        [ElementAttribute(XName = "DataProvider", IsSmartProperty = true, IsRequired = true)]
        public IDataProvider DataProvider
        {
            get { return (IDataProvider) GetValue("DataProvider"); }
            set { SetValue("DataProvider", value); }
        }

        #endregion

        #region INode

        public abstract NodeTypes NodeType { get; }

        public int Id { get; set; }

        public string Imei { get; set; }

        public int? IdNum { get; set; }

        private UInt32? _sequence = null;

        protected abstract uint NextSequenceMin();

        protected abstract uint NextSequenceMax();

        protected String GetMessagingDevice()
        {
            return DataProvider.GetDetalleDispositivo(Id, "GTE_MESSAGING_DEVICE").As(String.Empty);
        }

        protected bool HasGarminDeviceAttached()
        {
            var md = GetMessagingDevice();
            return md == MessagingDevice.Garmin;
        }

        public UInt32 NextSequence
        {
            get
            {

                var min = NextSequenceMin();
                var max = NextSequenceMax();

                try {
                    if (_sequence == null)
                    {
                        _sequence = Fota.ObtainLastSequence(this) ?? min;
                    }
                    else
                    {
                        _sequence++;
                        if (_sequence > max)
                            _sequence = min;
                    }
                } catch
                {
                    _sequence = min;
                }

                return (_sequence ?? min);
            }
            
        }

        public UInt32? Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }
        public abstract int Port { get; set; }

        /// <summary>
        /// Este metodo tiene la responsabilidad de detectar cual es el dispositivo que esta enviando el mensaje, y con este dato pedir la instacia a DataProvider.Find(Imei) o a DataProvider.Get(int32idparenti08), DataProvider devuelve siempre la misma instacia para cada dispositivo, asegurando tambien que solo un Factory o Decode es invocado al mismo tiempo para un dispositivo dado, con lo cual el parser no debe preocuparse por que halla procesamiento en paralelo
        /// contract
        /// if (DeviceId not in Payload)
        ///		DeviceId = ParserUtils.WithoutDeviceId;
        /// else if (DeviceId == 0)
        ///		DeviceId = ParserUtils.CeroDeviceId;
        /// else
        ///		Debug.Assert((DeviceId != ParserUtils.WithoutDeviceId) && (DeviceId != ParserUtils.CeroDeviceId));
        /// </summary>
        /// <param name="frame"></param>
        ///<param name="formerId"></param>
        public abstract INode Factory(IFrame frame, int formerId);

        /// <summary>
        /// Este metodo tiene la responsabilidad de traducir el reporte enviado por el dispositivo al protocolo de plataforma, que sera procesado por el dispatcher, nunca hay mas de una instacia para cada dispositivo corriendo en un momento dado, con lo cual el parser no debe preocuparse por que halla procesamiento en paralelo
        /// contract
        /// Debug.Assert(IMessage.UniqueIdentifier != 0);
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public abstract IMessage Decode(IFrame frame);

        public INode FactoryShallowCopy(int newId, int newIdNum, String newImei)
        {
            lock (DeviceList)
            {
                if (DeviceList.ContainsKey(newId)) return DeviceList[newId];

                var nuevo = (BaseCodec) MemberwiseClone();
                nuevo.OnMemberwiseClone();
                nuevo.Id = newId;
                nuevo.IdNum = newIdNum;
                nuevo.Imei = newImei;
                nuevo._deviceLock = new AutoResetEvent(true);
                DeviceList.Add(newId, nuevo);
                return nuevo;
            }
        }

        public INode Get(int id)
        {
            lock (DeviceList)
            {
                return DeviceList.ContainsKey(id) ? DeviceList[id] : null;
            }
        }

        public INode FindByIMEI(String imei)
        {
            lock (DeviceList)
            {
                return DeviceList.Where(d => d.Value.Imei == imei).Select(d => d.Value).SingleOrDefault();
            }
        }

        public INode FindByIdNum(int idNum)
        {
            lock (DeviceList)
            {
                return DeviceList.Where(d => d.Value.IdNum == idNum).Select(d => d.Value).SingleOrDefault();
            }
        }

        /// <summary>
        /// Metodo responsable de indicarle a la capa de transporte si la trama actual contiene un paquete completo,
        /// comenzando desde "Start" hasta "Start"+"Count" exclusive, indicando los limites del mismo.
        /// 
        /// Si se detecta ruido informarlo como un paquete indicando verdadero en IgnoreNoise.
        /// </summary>
        /// <param name="payload">Trama</param>
        /// <param name="start">Indice donde comienzan los datos pendientes a analizar</param>
        /// <param name="count">Cantidad de datos pendientes por analizar</param>
        /// <param name="detectedCount">Cantidad: indicar cuanto ocupa el paquete o el ruido detectado</param>
        /// <param name="ignoreNoise">Indica si se detecto ruido en la comunicacion en lugar de un paquete</param>
        /// <returns></returns>
        public virtual bool IsPacketCompleted(
            byte[] payload, int start, int count, out int detectedCount, out bool ignoreNoise)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Bandera que indica que se debe chequear que el dispositivo reporta con el id que le fue asignado y no otro
        /// </summary>
        public virtual bool ChecksCorrectIdFlag
        {
            get { return false; }
        }

        /// <summary>
        /// Ejecuta dentro de un lock por codec
        /// </summary>
        /// <param name="execute"> action a ejecutar dentro del lock</param>
        /// <param name="callerName"> action a ejecutat en el timeout </param>
        /// <param name="detailForOnFail"> actions a ejecutar en el fail </param>
        /// <returns> true si se pudo ejecutar dentro de lock</returns>
        public bool ExecuteOnGuard(Action execute, String callerName, String detailForOnFail)
        {
            bool rv = false;

            if (_deviceLock.WaitOne(TimeSpan.FromMinutes(1)))
            {
                try
                {
                    SessionHelper.CreateSession();
                    execute();
                    SessionHelper.CloseSession();
                    rv = true;
                }
                catch (Exception e)
                {
                    var context = new Dictionary<String, String> {{"Detalle", detailForOnFail}};
                    STrace.Debug(GetType().FullName, Id, detailForOnFail);
                    STrace.Exception(GetType().FullName, e, Id, context,
                                     String.Format("Exception during '{0}'", callerName));
                }
                finally
                {
                    _deviceLock.Set();
                }
            }
            else
            {
                STrace.Error(GetType().FullName, Id, String.Format("Lock timeout: {0}", callerName));
            }

            return rv;
        }

        #endregion

        #region Members

        private static readonly Dictionary<int, BaseCodec> DeviceList = new Dictionary<int, BaseCodec>();
        private AutoResetEvent _deviceLock = new AutoResetEvent(true);
        protected ulong Lastsentmessageid;

        protected virtual int MessageSequenceMin
        {
            get { return 1; }
        }

        protected virtual int MessageSequenceMax
        {
            get { return 99; }
        }

        public virtual String AsString(IFrame frame)
        {
            return frame.PayloadAsString ?? (frame.PayloadAsString = StringUtils.MakeString(frame.Payload));
        }

        /// <summary>
        /// Este metodo es el encargado de Inicializar la instancia del dispositivo ya que las mismas se crean con MemberwiseClone de otra instancia
        /// </summary>
        protected virtual void OnMemberwiseClone()
        {
        }

        #endregion

        #region rfid reader
        protected string decodeRFID(string receivedRFID, char driverStatus) // returns HEXA 
        {

            if (String.IsNullOrEmpty(receivedRFID) || (driverStatus == '*') || (receivedRFID.Trim() == String.Empty))
                return "0000000000";

            string rfidFormat = DataProvider.GetDetalleDispositivo(Id, "RFID_READER_FORMAT").As(String.Empty);
            if (String.IsNullOrEmpty(rfidFormat)) rfidFormat = "BASE36"; // (UBOX)

            STrace.Debug(typeof(BaseCodec).FullName, Id, String.Format("RFID (Format={0}) Received from device {1}: {2}", rfidFormat, Id, receivedRFID));

            return decodeRFID(receivedRFID, rfidFormat);
        }

        protected string decodeRFID(string receivedRFID, string RFIDFormat) // returns HEXA
        {
            var result = String.Empty;
            switch (RFIDFormat)
            {
                case "HEX":
                    result = receivedRFID;
                    break;
                case "INT":
                    try
                    {
                        var base10 = Convert.ToInt64(receivedRFID);
                        result = base10.ToString("X10");
                    }
                    catch { }
                    break;
                case "BASE36":
                default:
                    result = StringUtils.RfidHexaFromBase36(receivedRFID);
                    break;
            }
            return result;
        }
        #endregion rfid reader
    }
}