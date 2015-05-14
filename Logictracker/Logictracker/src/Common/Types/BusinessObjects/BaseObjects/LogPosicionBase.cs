using System;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Utils;

namespace Logictracker.Types.BusinessObjects.BaseObjects
{
    /// <summary>
    /// Base class for positions. It was separetad from LogPosicion to solve nhibernate query inherancy issues.
    /// </summary>
    public class LogPosicionBase : IAuditable
    {
        #region Public Properties

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual DateTime FechaRecepcion { get; set; }
        public virtual DateTime FechaMensaje { get; set; }
        public virtual double Longitud { get; set; }
        public virtual double Latitud { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
        public virtual Coche Coche { get; set; }
        public virtual int Velocidad { get; set; }
        public virtual bool VeloCalculada { get; set; }
        public virtual double Altitud { get; set; }
        public virtual float HDop { get; set; }
        public virtual double Curso { get; set; }
        public virtual byte Status { get; set; }
        public virtual bool? MotorOn { get; set; }
        public virtual int? Provider { get; set; }
        public virtual float? HorizontalAccuracy { get; set; }
        public virtual Zona Zona { get; set; }

        #endregion

        public LogPosicionBase() { }

        public LogPosicionBase(GPSPoint position, Dispositivo dispositivo, Coche coche)
        {
            Coche = coche;
            Dispositivo = dispositivo;
            FechaMensaje = position.Date;
            FechaRecepcion = DateTime.UtcNow;
            Latitud = position.Lat;
            Longitud = position.Lon;
            Altitud = position.Height.Unpack();
            Curso = position.Course.Unpack();
            Status = position.LcyStatus;
            VeloCalculada = false;
            Velocidad = position.Velocidad;
            MotorOn = position.IgnitionStatus == IgnitionStatus.On ? true
                        : position.IgnitionStatus == IgnitionStatus.Off ? false
                        : new bool?();
            Provider = (int) position.SourceProvider;
            HorizontalAccuracy = position.HorizontalAccuracy;
            HDop = position.HDOP;
        }

        public LogPosicionBase(GPSPoint position, Coche coche)
            : this(position, coche != null ? coche.Dispositivo : null, coche) { }

        public virtual GPSPoint ToGpsPoint()
        {
            return new GPSPoint
            {
                Date = FechaMensaje,
                Lat = (float)Latitud,
                Lon = (float)Longitud,
                Speed = new Speed(Velocidad),
                Height = new Altitude((float)Altitud),
                DeviceId = Dispositivo.Id,
                HDOP = HDop,
                IgnitionStatus = MotorOn.HasValue ? (MotorOn.Value ? IgnitionStatus.On : IgnitionStatus.Off) : IgnitionStatus.Unknown,
                SourceProvider = Provider.HasValue ? (GPSPoint.SourceProviders)Enum.ToObject(typeof(GPSPoint.SourceProviders), Provider.Value) : GPSPoint.SourceProviders.Unespecified,
                HorizontalAccuracy = HorizontalAccuracy.HasValue ? HorizontalAccuracy.Value : 0
            };
        }
    }
}
