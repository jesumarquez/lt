using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class GarminEvent: IEvent
    {
        public static class StopStatus
        {
            public const short Active = 10;
            public const short Done = 11;
            public const short UnreadInactive = 12;
            public const short ReadInactive = 13;
            public const short Deleted = 14;
/*
        public static string GetLabelVariableName(short estado)
                        {
                            switch (estado)
                            {
                                case Cancelado: return "ENTREGA_STATE_CANCELADO";
                                case Pendiente: return "ENTREGA_STATE_PENDIENTE";
                                case EnCurso: return "ENTREGA_STATE_ENSITIO";
                                case Visitado: return "ENTREGA_STATE_VISITADO";
                                case SinVisitar: return "ENTREGA_STATE_SINVISITAR";
                                case NoCompletado: return "ENTREGA_STATE_NOCOMPLETADO";
                                case Completado: return "ENTREGA_STATE_COMPLETADO";

                                default: return "ENTREGA_STATE_PENDIENTE";
                            }
                        }
 */
        }

        public string EventType { get { return EventTypes.Garmin; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }
        public Empleado Chofer{ get; private set; }

        public Int64 DetailId { get; private set; }

        public short Estado { get; private set; }
        
        public GarminEvent(DateTime date, Int64 detailId, double latitud, double longitud, short estado, Empleado chofer)
        {
            Date = date;
            Latitud = latitud;
            Longitud = longitud;
            DetailId = detailId;
            Estado = estado;
            Chofer = chofer;
        }
    }
}
