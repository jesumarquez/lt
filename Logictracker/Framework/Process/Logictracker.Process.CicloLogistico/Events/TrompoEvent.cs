using System;

namespace Logictracker.Process.CicloLogistico.Events
{
    public class TrompoEvent:IEvent
    {
        public enum VelocidadTrompo
        {
            Undefined = 0,
            Slow = 1,
            Fast = 2
        }
        public enum SentidoTrompo
        {
            Detenido = 0,
            HorarioDerecha = 1,
            AntihorarioIzquierda = 2
        }
        public string EventType { get { return EventTypes.Trompo; } }

        public DateTime Date { get; private set; }
        public double Latitud { get; private set; }
        public double Longitud { get; private set; }
 
        public VelocidadTrompo Speed { get; private set; }
        public SentidoTrompo Sentido { get; private set; }

        public TrompoEvent(DateTime date, SentidoTrompo sentido, VelocidadTrompo speed, double latitud, double longitud)
        {
            Date = date;
            Speed = speed;
            Sentido = sentido;
            Latitud = latitud;
            Longitud = longitud;
        }
    }
}
