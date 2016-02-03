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
using System.Linq;
using Logictracker.DAL.DAO.BusinessObjects.Dispositivos;
using Logictracker.DAL.DAO.BusinessObjects.Messages;
using Logictracker.DAL.DAO.BusinessObjects.Vehiculos;

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
            
            if (deviceId != 0 && deviceId != -1)
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
                    var dispositivoDao = new DispositivoDAO();
                    var dispositivo = dispositivoDao.FindById(deviceId);

                    var codeEvent = frame.EventId + 9000;
                    
                    var msgTraduccionDao = new MensajeTraducidoDAO();
                    var codeEventTraducido = msgTraduccionDao.GetCodigoFinal(dispositivo.Empresa.Id, codeEvent.ToString());
                    var evento = new Event(Convert.ToInt16(codeEventTraducido), -1, deviceId, (ulong)frame.EventId, pos, pos.GetDate(), "", null, true);

                    return evento;
                }
                
                var message = pos.ToPosition(deviceId, 0);         

                return message;
            }
            
            return null;
        }

        public int FindDeviceId(string licensePlate)
        {
            var cocheDao = DaoFactory.CocheDAO;
            var coche = cocheDao.FindByPatente(-1, licensePlate);

            var ignorarEstosVehiculos = new String[]
            {
                "IHC257",
                "IPA610",
                "KFA642",
                "KFA654",
                "KLZ351",
                "KLZ353",
                "KLZ356",
                "KLZ357",
                "KLZ358",
                "KLZ359",
                "KNR667",
                "KNR669",
                "KNR670",
                "KNR672",
                "OZW828",
                "OZW829",
                "NFV130",
                "IZU324",
                "KHN597",
                "KLV824",
                "NAD921",
                "FPX254",
                "GVF077",
                "GXF152",
                "HMZ248",
                "KTI081",
                "NQR543",
                "ODO689",
                "OWJ436",
                "JCT954",
                "MBV767",
                "GRO787",
                "GUJ319",
                "HEX296",
                "ITO924",
                "JNP852",
                "KGC770",
                "KRX246",
                "LJB399",
                "MZU653",
                "NPH607",
                "NPH752",
                "NSS381",
                "OJM894",
                "OMD050",
                "FQK895",
                "FTT510",
                "HQA702",
                "JAC956",
                "LOO445",
                "ENG697",
                "ENK059",
                "EXJ786",
                "FCA601",
                "FJY750",
                "FJY751",
                "FLZ229",
                "GEE691",
                "GEE692",
                "GGZ503",
                "GYS514",
                "IUS984",
                "IUS985",
                "JOD789",
                "KHC046",
                "KHN528",
                "KUM175",
                "MEX407",
                "NDR511",
                "NDR520",
                "FZY566",
                "HIV461",
                "FSF948",
                "FSF949",
                "GTG910",
                "MXX566",
                "NTT208",
                "NTT209",
                "ELB116",
                "GJY865",
                "GJY866",
                "GZM709",
                "HCI950",
                "HHL554",
                "JDU744",
                "JDU746",
                "KJN325",
                "KJN326",
                "KZE167",
                "KZE169",
                "LZJ923",
                "FFL452",
                "GEP485",
                "GEP486",
                "GEP506",
                "GGL918",
                "GIJ090",
                "HGE718",
                "HGE719",
                "IQH430",
                "JPR625",
                "JSB430",
                "KZJ068",
                "LKN815",
                "LLG679",
                "LLG681",
                "NRJ358",
                "NTE542",
                "NUW684",
                "NUW685"
            };

            var rta = ignorarEstosVehiculos.Contains(coche.Patente);

            if (rta)
            {
                return -1;
            }
            
            if (coche != null && coche.Dispositivo != null)
            {
                return coche.Dispositivo.Id;
            }

            LogWritter.writeLog(
                new Exception(
                    string.Format("FindDeviceId : El vehiculo no existe o no tiene dispositivo asociado --> {0}",
                        licensePlate)), _path);

            return -1;
        }
    }
}
