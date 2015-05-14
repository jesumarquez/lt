
namespace Logictracker.Process.Geofences.Classes
{
    public enum GeocercaEventState { Entra, Sale, ExcesoVelocidad, TimeTrackingEntrada, TimeTrackingSalida, ExcesoPermanencia, ExcesoPermanenciaEntrega }
    public class GeocercaEvent
    {
        public EstadoGeocerca Estado { get; set; }
        public GeocercaEventState Evento { get; set; }
    }
}
