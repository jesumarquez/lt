#region Usings

using System;
using System.Globalization;

#endregion

namespace Logictracker.Utils
{
    /// <summary>
    /// Objeto que representa una posicion geografica, altura, velocidad y rumbo de un
    /// objeto en movimiento, representado en el tiempo.
    /// </summary>
    [Serializable]
    public class GPSPoint
    {
        public enum SourceProviders
        {
            Unespecified = 0,
            Unknown = 1,
            GpsProvider = 2,
            AGpsProvider = 3,
            NetworkProvider = 4
        }

        #region Public Members

        public byte LcyStatus { get; set; }

        public DateTime Date { get; set; }

        public float Lat { get; set; }

        public float Lon { get; set; }

		/// <summary>
		/// Kilometros por Hora
		/// </summary>
        public Speed Speed { get; set; }

        public Course Course { get; set; }

        public Altitude Height { get; set; }

        public float HDOP { get; set; }

        public char Zona { get; set; }

        public Int32 DeviceId { get; set; }

        /// <summary>
        /// Almacena metodo por el cual se obtubo la posicion. 
        /// </summary>
        public SourceProviders SourceProvider { get; set; }

        public float HorizontalAccuracy { get; set; }

        public int Age { get; set; }

        public int Velocidad
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Speed.Unpack());
                }
                catch
                {
                    return 0;
                }
            }
        }

    	public IgnitionStatus IgnitionStatus;
		public void SetEngineStatus(bool enginestatus)
		{
			IgnitionStatus = enginestatus ? Utils.IgnitionStatus.On : Utils.IgnitionStatus.Off;
		}

    	public override string ToString()
        {
			return String.Format(CultureInfo.InvariantCulture
				,"Fecha Hora: {0:ddMMyy HHmmss} Posicion: {1:00.000000} {2:000.000000} Velocidad:{3} Curso:{4} Altitud: {5} HDOP: {6}"
				,Date
				,Lat 
				,Lon
				,Speed
				,Course
				,Height
				,HDOP
				);
        }
        // TODO: DEPRECATE
		public static GPSPoint ParseGp(String data, out byte entradas, out int evento, bool knotsFlag)
		{
			entradas = 0;
			evento = 0;
			if (!data.StartsWith(">RGP")) return null;

			var time = DateTimeUtils.SafeParseFormat(data.Substring(4, 12), "ddMMyyHHmmss");
			var lat = Convert.ToSingle(data.Substring(16, 8)) * (float)0.00001;
			var lon = Convert.ToSingle(data.Substring(24, 9)) * (float)0.00001;
			var vel = Convert.ToSingle(data.Substring(33, 3));
			if (knotsFlag) vel = Speed.KnotToKm(vel);
			var dir = Convert.ToSingle(data.Substring(36, 3));
			//var tipoDePos = Convert.ToSingle(data.Substring(37, 1));
			//var edad = Convert.ToSingle(data.Substring(38, 2));
			entradas = Convert.ToByte(data.Substring(40, 2), 16);
			evento = Convert.ToInt32(data.Substring(42, 2));

			return new GPSPoint(time, lat, lon, vel, SourceProviders.Unespecified, 0)
			{
				Course = new Course(dir),
			};
		}

		public static float ResampleAxis(String s)
		{
			var t = Convert.ToSingle(s, CultureInfo.InvariantCulture);
			var grados = (int)(t / 100);
			var fracciongrado = (((t / 100) - grados) * 100) / 60.0;
			return (float)(grados + fracciongrado);
		}

        #endregion

        #region Constructores

        public GPSPoint()
        {
            Date = DateTime.UtcNow;
            Speed = new Speed(0);
            Course = new Course(0);
            Height = new Altitude(0);
			IgnitionStatus = Utils.IgnitionStatus.Unknown;
            SourceProvider = SourceProviders.Unespecified;
            HorizontalAccuracy = 0;
        }

        public GPSPoint(DateTime date, float lat, float lon) : this(date, lat, lon, 0, SourceProviders.Unespecified, 0) { }

        public GPSPoint(DateTime date, float lat, float lon, float speed, SourceProviders source, float ha)
        {
			if (IsValidLat(lat)) throw new ArgumentOutOfRangeException("lat");
			if (IsValidLon(lon)) throw new ArgumentOutOfRangeException("lon");
			Date = date;
            Lat = lat;
            Lon = lon;
            Speed = new Speed(speed);
            Course = new Course(0);
            Height = new Altitude(0);
            IgnitionStatus = Utils.IgnitionStatus.Unknown; 
            SourceProvider = source;
            HorizontalAccuracy = ha;
		}

		public static GPSPoint Factory(DateTime date, float lat, float lon)
		{
			return Factory(date, lat, lon, 0, 0, 0, 0);
		}

		public static GPSPoint Factory(DateTime date, float lat, float lon, float speed)
		{
			return Factory(date, lat, lon, speed, 0, 0, 0);
		}

        public static GPSPoint Factory(DateTime date, float lat, float lon, float speed, float course, float altitude, float hdop)
        {
            return Factory(date, lat, lon, speed, course, altitude, hdop, SourceProviders.Unespecified, 0);
        }

		public static GPSPoint Factory(DateTime date, float lat, float lon, float speed, float course, float altitude, float hdop, SourceProviders source, float ha)
		{
			try
			{
				return new GPSPoint(date, lat, lon, speed, source, ha)
					{
						Course = new Course(course), 
						Height = new Altitude(altitude), 
						HDOP = hdop,

					};
			}
			catch
			{
				return new GPSPoint
					{
						Date = date,
						Lat = lat,
						Lon = lon,
						Speed = new Speed(speed),
						Course = new Course(course),
						Height = new Altitude(altitude),
						HDOP = hdop,
                        SourceProvider = source,
                        HorizontalAccuracy = ha
					};
			}
		}

		#endregion

	    #region Private Members

	    private static bool IsValidLat(float latitude)
	    {
		    return latitude == 0 || Math.Abs(latitude) > 90 || Math.Abs(latitude) < -90;
	    }

	    private static bool IsValidLon(float longitude)
	    {
		    return longitude == 0 || Math.Abs(longitude) > 180 || Math.Abs(longitude) < -180;
	    }

	    #endregion

        public bool Equalz(GPSPoint point2)
        {
            if (DeviceId != point2.DeviceId) return false;
            if (Date != point2.Date) return false;
            if (Lat != point2.Lat) return false;
            if (Lon != point2.Lon) return false;
            if (HDOP != point2.HDOP) return false;
            if (Speed.Unpack() != point2.Speed.Unpack()) return false;
            if (Height.Unpack() != point2.Height.Unpack()) return false;
            if (HorizontalAccuracy != point2.HorizontalAccuracy) return false;
            if (Velocidad != point2.Velocidad) return false;
            if (Course.Unpack() != point2.Course.Unpack()) return false;
            if (LcyStatus != point2.LcyStatus) return false;
            if (Zona != point2.Zona) return false;
            if (SourceProvider != point2.SourceProvider) return false;

            return true;
        }
	}

	public enum IgnitionStatus
	{
		Off = 0,
		On = 1,
		Unknown = 2,
	}

	public static class GPSPointX
	{
		public static DateTime GetDate(this GPSPoint me)
		{
			return me == null ? new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc) : me.Date;
		}

		public static DateTime GetDate(this GPSPoint me, DateTime def)
		{
			return me == null ? def : me.Date;
		}
        
		/*public static double ResampleBackAxis(double f)
        {
            var grados = (long)f;
            var minutos = (f - grados) * 60;
            grados *= 100;

            return grados + minutos;
        }//*/

		/*public static bool Equivalent(this GPSPoint me, GPSPoint it)
		{
			if ((me == null) || (it == null)) return false;
			if (Distancias.LoxodromicaConAltitud(me.Lat, me.Lon, me.Height.Unpack(), it.Lat, it.Lon, it.Height.Unpack()) > 20) return false;
			if (Math.Abs(me.Speed.Unpack() - it.Speed.Unpack()) > 2) return false;
			if (Math.Abs(me.Course.Unpack() - it.Course.Unpack()) > 2) return false;
			//if (me.Date.ToString("ddMMyyyyHHmmss") != it.Date.ToString("ddMMyyyyHHmmss")) return false;
			if (me.Date != it.Date) return false;
			return me.DeviceId == it.DeviceId;
		}//*/
	}
}
