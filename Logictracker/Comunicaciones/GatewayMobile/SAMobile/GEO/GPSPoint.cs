using System;
using System.Collections.Generic;
using System.Text;

namespace Urbetrack.Mobile.Comm.GEO
{
    public class GPSPoint
    {
        #region Miembros Privados
        private DateTime date;
        private float lat;
        private float lon;
        private float speed;
        private float course;
        private float height;
        private byte lcyStatus = 0x00;
        #endregion

        #region Propiedades Publicas
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float Lat
        {
            get { return lat; }
            set { lat = value; }
        }

        public float Lon
        {
            get { return lon; }
            set { lon = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public float Course
        {
            get { return course; }
            set { course = value; }
        }

        public byte LcyStatus
        {
            get { return lcyStatus; }
            set { lcyStatus = value;  }
        }
        #endregion

        #region Constructores
        public GPSPoint()
        {
            Date = DateTime.Now;
        }

        public GPSPoint(DateTime _date, float _lat, float _lon)
            : this(_date, _lat, _lon, 0)
        {
        }

        public GPSPoint(DateTime _date, float _lat, float _lon, int _speed)
        {
            Date = _date;
            Lat = _lat;
            Lon = _lon;
            Speed = _speed;
        }

        public override string ToString()
        {
            return
                String.Format(
                    "Fecha: {0:ddMMyy} Hora: {0:HHmmss} Posicion:{1} {2} - {3} {4} Velocidad:{5} State: {6}", Date,
                    Math.Abs(Lat).ToString("0000.0000").Replace(',', '.'), (Lat < 0 ? 'S' : 'N'),
                    Math.Abs(Lon).ToString("00000.0000").Replace(',', '.'), (Lon < 0 ? 'W' : 'E'), Speed, LcyStatus);
        }

        public string AsMessage(int distance)
        {
            return
                String.Format(
                    "P;{0:ddMMyyHHmmss};{1};{2};{3}", Date,
                    Lat.ToString("0000.0000").Replace(',', '.'),
                    Lon.ToString("00000.0000").Replace(',', '.'), distance);
        }
        #endregion
    }
}