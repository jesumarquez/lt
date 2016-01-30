using log4net;
using Logictracker.AVL.Messages;
using System.Collections.Generic;
using Logictracker.DAL.Factories;
using Logictracker.Model;
using Logictracker.Tracker.Services;
using Logictracker.Utils;
using Logitracker.Codecs.Sitrack;
using Spring.Messaging.Core;
using System;

namespace Logictracker.Tracker.Application.Services
{
    public class ReceptionService : IReceptionService
    {

        public MessageQueueTemplate TrackMessageQueueTemplate { get; set; }
        public DAOFactory DaoFactory { get; set; }
        public String _path { get; set; }


        public void ParseSitrackPositions(List<SitrackFrame> positions)
        {
            foreach (var pos in positions)
            {
                var ltframe = Translate(pos);
                if (ltframe != null)
                {
                    TrackMessageQueueTemplate.ConvertAndSend(ltframe);
                }
            }
        }

        public void ParseSitrackPositions(List<SitrackFrame> positions, String path)
        {
            _path = path;
            ParseSitrackPositions(positions);
        }

        private IMessage Translate(SitrackFrame frame)
        {          
            var deviceId = 0;

            if (frame.HolderDomain != null)
            {
               deviceId = FindDeviceId(frame.HolderDomain);
            }
            
            if (deviceId != 0)
            {
                var pos = new GPSPoint
                {
                    Lat = (float) frame.Latitude,
                    Lon = (float) frame.Longitude,
                    Date = frame.ReportDate.ToUniversalTime(),
                    Speed = new Speed(frame.Speed),
                    Course = new Course(frame.Course)
                };
                if (frame.EventDesc != null)
                {
                    var codeEvent = frame.EventId + 9000;
                    var evento = new Event((short)codeEvent, -1, deviceId, (ulong)frame.EventId, pos, pos.GetDate(), "", null, true);

                    return evento;
                }
                
                var message = pos.ToPosition(deviceId, 0);         

                return message;
            }
            if (frame.HolderDomain != null) 
            {
                LogWritter.writeLog(new Exception(string.Format("Patente inexistente {0}", frame.HolderDomain)), _path);
            }
            return null;
        }

        public int FindDeviceId(string licensePlate)
        {
            var cocheDao = DaoFactory.CocheDAO;
            var coche = cocheDao.FindByPatente(-1, licensePlate);
            if (coche != null && coche.Dispositivo != null)
            {
                return coche.Dispositivo.Id;
            }

            LogWritter.writeLog(new Exception(string.Format("FindDeviceId : El vehiculo no existe o no tiene dispositivo asociado --> {0}", licensePlate)), _path);

            return -1;
        }
    }
}
