using log4net;
using Logictracker.AVL.Messages;
using System.Collections.Generic;
using Logictracker.DAL.Factories;
using Logictracker.Model;
using Logictracker.Tracker.Services;
using Logictracker.Utils;
using Logitracker.Codecs.Sitrack;
using Spring.Messaging.Core;

namespace Logictracker.Tracker.Application.Services
{
    public class ReceptionService : IReceptionService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReceptionService));

        public MessageQueueTemplate TrackMessageQueueTemplate { get; set; }
        public DAOFactory DaoFactory { get; set; }

        public void ParseSitrackPositions(List<SitrackFrame> positions)
        {
            foreach (var pos in positions)
            {
                var ltframe = Translate(pos);
                if (ltframe != null)
                {
                    TrackMessageQueueTemplate.ConvertAndSend(ltframe);
                    Logger.InfoFormat("Message sent {0} ", ltframe.DeviceId);
                }
            }
        }

        private IMessage Translate(SitrackFrame frame)
        {
            Logger.DebugFormat("ToIMessage : {0}", frame.HolderDomain);

            var deviceId = FindDeviceId(frame.HolderDomain);

            if (deviceId > 0)
            {
                var pos = new GPSPoint
                {
                    Lat = (float) frame.Latitude,
                    Lon = (float) frame.Longitude,
                    Date = frame.ReportDate.ToUniversalTime(),
                    Speed = new Speed(frame.Speed),
                    Course = new Course(frame.Course)
                };

                var message = pos.ToPosition(deviceId, 0);
                Logger.InfoFormat("ToIMessage success: {0} for {1}", message.DeviceId, frame.HolderDomain);

                return message;
            }

            Logger.WarnFormat("ToIMessage : LicensePlate not found {0}", frame.HolderDomain);
            return null;
        }

        public int FindDeviceId(string licensePlate)
        {
            var cocheDao = DaoFactory.CocheDAO;
            var coche = cocheDao.FindByPatente(-1, licensePlate);
            if (coche != null && coche.Dispositivo != null)
            {
                Logger.DebugFormat("FindDeviceId : {0}, for {1} success", coche.Dispositivo.Id, licensePlate);

                return coche.Dispositivo.Id;
            }

            Logger.ErrorFormat("FindDeviceId : Device or vehicle not found {0}", licensePlate);

            return -1;
        }
    }
}
