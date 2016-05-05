using System;
using Logictracker.Types.BusinessObjects.Positions;
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
                Fecha = position.FechaMensaje,
                Hdop =  position.HDop??0
            };

            return rv;
        }
        public double Curso { get; private set; }
        public int Velocidad { get; private set; }
        public double Longitud { get; private set; }
        public double Latitud { get; private set; }
        public DateTime Fecha { get; set; }

        internal static LastPosition Create(GPSPoint point)
        {
            return new LastPosition
            {
                Latitud = point.Lat,
                Longitud = point.Lon,
                Velocidad = point.Velocidad,
                Curso = point.Course.Unpack(),
                Fecha = point.Date,
                Hdop = point.HDOP
            };
        }

        public float Hdop { get; set; }

        internal static LastPosition Create(LastPosition lastPosition)
        {
            return new LastPosition()
            {
                Latitud = lastPosition.Latitud,
                Longitud = lastPosition.Longitud,
                Velocidad = lastPosition.Velocidad,
                Curso = lastPosition.Curso,
                Fecha = lastPosition.Fecha,
                Hdop =  lastPosition.Hdop
            };
        }

        internal GPSPoint ToGpsPoint()
        {
            return new GPSPoint(Fecha,(float) Latitud,(float) Longitud,Velocidad,GPSPoint.SourceProviders.GpsProvider,Hdop);
        }

        internal static LastPosition Create(LogPosicion position)
        {
            if (position == null)
                throw new ArgumentNullException("position");

            return new LastPosition()
            {
                Latitud = position.Latitud,
                Longitud = position.Longitud,
                Velocidad = position.Velocidad,
                Curso = position.Curso,
                Fecha = position.FechaMensaje
            };
        }
    }
}