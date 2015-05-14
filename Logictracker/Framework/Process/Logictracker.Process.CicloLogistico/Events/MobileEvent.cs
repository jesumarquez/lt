using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class MobileEvent: IEvent
    {
        //public static class StopStatus
        //{
        //    public const short Active = 10;
        //    public const short Done = 11;
        //    public const short UnreadInactive = 12;
        //    public const short ReadInactive = 13;
        //    public const short Deleted = 14;
        //}

        public string EventType { get { return EventTypes.Mobile; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }
        public string DeviceId{ get; private set; }
        public int MessageId { get; private set; }
        public Int64 DetailId { get; private set; }

        public short Estado { get; private set; }
        
        public MobileEvent(DateTime date, Int64 detailId, double latitud, double longitud, short estado, int messageId, string deviceId)
        {
            Date = date;
            Latitud = latitud;
            Longitud = longitud;
            DetailId = detailId;
            Estado = estado;
            MessageId = messageId;
            DeviceId = deviceId;
        }
    }
}
