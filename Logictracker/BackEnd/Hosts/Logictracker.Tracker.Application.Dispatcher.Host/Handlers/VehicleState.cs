using System;
using Logictracker.Types.ValueObject.Positions;
using Logictracker.Utils;

namespace Logictracker.Tracker.Application.Dispatcher.Host.Handlers
{

    class LastPosition
    {
        public static LastPosition Create(LogUltimaPosicionVo position)
        {
            var rv = new LastPosition()
            {
                Latitud = position.Latitud,
                Longitud = position.Longitud,
                Velocidad = position.Velocidad,
                Curso = position.Curso,
                Fecha = position.FechaMensaje
            };

            return rv;
        }
        public double Curso { get; private set; }
        public int Velocidad { get; private set; }
        public double Longitud { get; private set; }
        public double Latitud { get; private set; }
        public DateTime Fecha { get; set; }
    }

    class VehicleState
    {
        public int DeviceId { get; private set; }
        private LastPosition _position;

        private VehicleState()
        {    
        }
        
        private VehicleState(LogUltimaPosicionVo position)
        {
            DeviceId = position.IdDispositivo;
            _position = LastPosition.Create(position);
        }

        public static VehicleState Create(LogUltimaPosicionVo position)
        {
            return position == null ? null : new VehicleState(position);
        }

        public void Process(GPSPoint gpsPoint)
        {

          //  Console.WriteLine(gpsPoint.Date.ToShortTimeString());
        }

        class VechicleNormal
        {
            
        }

        class VehicleExeceso
        {
            
        }

        class VehicleInfraccion
        {
            
        }
    }
}
