using System.Collections.Generic;
using System.Collections.Specialized;
using log4net;
using Logictracker.AVL.Messages;
using Logictracker.Tracker.Parser.Spi;
using Logictracker.Utils;
using IMessage = Logictracker.Model.IMessage;

namespace Logictracker.Tracker.Parser.Caesat.Comm
{
    class Translator : ITranslator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Translator));

        private object VehiclesIdCollection { get; set; }

        public IMessage ToIMessage(CaesatFrame frame)
        {
            Logger.DebugFormat("ToIMessage : {0}, the frame was {1}", frame.LicensePlate, frame.Raw);

            var deviceId = FindDeviceId(frame.LicensePlate);

            if (deviceId > 0)
            {
                var pos = new GPSPoint()
                {
                    Lat = frame.Coordinate.Latitude,
                    Lon = frame.Coordinate.Longitude,
                    Date = frame.DateTime,
                    Speed = new Speed(frame.Speed),
                    Course = new Course(frame.Meaning)
                };

                var message = pos.ToPosition(deviceId, 0);
                Logger.DebugFormat("ToIMessage success: {0} for {1}", message.DeviceId, frame.LicensePlate);

                return message;
            }

            Logger.WarnFormat("ToIMessage : LicensePlate not found {0}, the frame was {1}", frame.LicensePlate, frame.Raw);
            return null;
        }

        private int FindDeviceId(string licensePlate)
        {
            var vehiclesIdCollection = (NameValueCollection) VehiclesIdCollection;
            var vehicleId= 0;
            int.TryParse(vehiclesIdCollection.Get(licensePlate),out vehicleId);
            return vehicleId;
        }

        //public static int FindDeviceId(string licensePlate)
        //{
        //    var cocheDao = new DAOFactory().CocheDAO;
        //    var coche = cocheDao.FindByPatente(-1, licensePlate);
        //    if (coche != null && coche.Dispositivo != null)
        //    {
        //        Logger.DebugFormat("FindDeviceId : {0}, for {1} success", coche.Dispositivo.Id, licensePlate);

        //        return coche.Dispositivo.Id;
        //    }

        //    Logger.ErrorFormat("FindDeviceId : Device or vehicle not found {0}", licensePlate);

        //    return -1;
        //}

       public IList<IMessage> Translate(IList<IFrame> frames)
        {
            var messageList = new List<IMessage>();
            foreach (var frame in frames)
            {
                var msg = ToIMessage(frame as CaesatFrame);
                if (msg != null)
                    messageList.Add(msg);
            }
            return messageList;
        }
    }

}
