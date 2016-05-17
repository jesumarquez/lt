using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.Messages
{
    [Serializable]
    public class Infraccion : IAuditable
    {
        public static class Codigos
        {
            public const short None = 0;
            public const short ExcesoVelocidad = 1;
            public const short ExcesoRpm = 2;
            public const short FrenadaBrusca = 3;
            public const short AceleracionBrusca = 4;
            public const short BateriaDesconectada = 5;
            public const short Panico = 6;

            public static string GetLabelVariableName(short codigo)
            {
                switch (codigo)
                {
                    case ExcesoVelocidad: return "EXCESO_VELOCIDAD";
                    case ExcesoRpm: return "EXCESO_RPM";
                    case FrenadaBrusca: return "FRENADA_BRUSCA";
                    case AceleracionBrusca: return "ACELERACION_BRUSCA";
                    case BateriaDesconectada: return "BATERIA_DESCONECTADA";
                    case Panico: return "PANICO";
                    default: return string.Empty;
                }
            }
        }
        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }
        public virtual Empleado Empleado { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Zona Zona { get; set; }
        public virtual short CodigoInfraccion { get; set; }
        public virtual double Permitido { get; set; }
        public virtual double Alcanzado { get; set; }

        public virtual DateTime Fecha { get; set; }
        public virtual double Latitud { get; set; }
        public virtual double Longitud { get; set; }
        public virtual DateTime? FechaFin { get; set; }
        public virtual double LatitudFin { get; set; }
        public virtual double LongitudFin { get; set; }

        public virtual DateTime FechaAlta { get; set; }

        public virtual bool HasValidLatitudes { get { return Math.Abs(Latitud) > 0.0 && Math.Abs(Longitud) > 0.0; } }
        public virtual bool HasDuration { get { return FechaFin.HasValue; } }
        public virtual TimeSpan Duracion { get { return FechaFin.HasValue ? FechaFin.Value.Subtract(Fecha) : TimeSpan.Zero; } }
    }
}