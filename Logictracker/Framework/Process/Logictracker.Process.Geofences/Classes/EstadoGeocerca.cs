using System;
using System.Collections.Generic;
using Logictracker.Types.ValueObject;
using Logictracker.Utils;

namespace Logictracker.Process.Geofences.Classes
{
    [Serializable]
    public enum EstadosGeocerca { Desconocido, Dentro, Fuera }

    [Serializable]
    public class EstadoGeocerca
    {
        public Geocerca Geocerca { get; set; }

        public bool EnExcesoVelocidad { get; set; }
        public int VelocidadPico { get; set; }
        public int VelocidadMaxima { get; set; }
        public GPSPoint PosicionInicioExceso { get; set; }
        public EstadosGeocerca Estado { get; set; }

        public EstadoGeocerca()
        {
            Estado = EstadosGeocerca.Desconocido;
        }

        public void InicioExceso(GPSPoint posicion)
        {
            EnExcesoVelocidad = true;
            PosicionInicioExceso = posicion;
            VelocidadPico = Math.Max(VelocidadPico, posicion.Velocidad);
        }

        public void FinExceso()
        {
            EnExcesoVelocidad = false;
            PosicionInicioExceso = null;
            VelocidadPico = 0;
        }

        public void UpdateVelocidadPico(int speed) { VelocidadPico = Math.Max(VelocidadPico, speed); }

        public EstadoGeocerca Clone()
        {
            return new EstadoGeocerca
                             {
                                 Geocerca = Geocerca,
                                 EnExcesoVelocidad = EnExcesoVelocidad,
                                 VelocidadPico = VelocidadPico,
                                 VelocidadMaxima = VelocidadMaxima,
                                 PosicionInicioExceso = PosicionInicioExceso,
                                 Estado = Estado
                             };
        }
    }
}
