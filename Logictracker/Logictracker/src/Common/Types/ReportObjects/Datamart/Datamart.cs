#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

#endregion

namespace Logictracker.Types.ReportObjects.Datamart
{
    /// <summary>
    /// Represents consilidated information about significant route segments.
    /// </summary>
    [Serializable]
    public class Datamart : IAuditable
    {
        public static class TiposCiclo
        {
            public const int Hormigon = 0;
            public const int Distribucion = 1;
        }

        public static class EstadosMotor
        {
            public const string Encendido = "Encendido";
            public const string Apagado = "Apagado";
            public const string Indefinido = "Indefinido";
        }

        public static class Estadosvehiculo
        {
            public const string SinReportar = "Sin Reportar";
            public const string EnMovimiento = "En Movimiento";
            public const string Detenido = "Detenido";
        }

        #region Public Properties

        public virtual Int32 Id { get; set; }

        public virtual Coche Vehicle { get; set; }
        public virtual Empleado Employee { get; set; }
        public virtual ReferenciaGeografica GeograficRefference { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual Zona Zona { get; set; }

        public virtual DateTime Begin { get; set; }
        public virtual DateTime End { get; set; }
        public virtual Double Kilometers { get; set; }
        public virtual Double MovementHours { get; set; }
        public virtual Double StoppedHours { get; set; }
        public virtual Double NoReportHours { get; set; }
        public virtual Int32 Infractions { get; set; }
        public virtual Double InfractionMinutes { get; set; }
        public virtual Int32 MinSpeed { get; set; }
        public virtual Int32 AverageSpeed { get; set; }
        public virtual Int32 MaxSpeed { get; set; }
        public virtual String EngineStatus { get; set; }
        public virtual String VehicleStatus { get; set; }
        public virtual Double HorasMarcha { get; set; }
        public virtual Double Consumo { get; set; }
        public virtual int? TipoCiclo { get; set; }
        public virtual int? IdCiclo { get; set; }
        public virtual bool? EnTimeTracking { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the type of the current object.
        /// </summary>
        /// <returns></returns>
        public virtual Type TypeOf() { return GetType(); }

        #endregion
    }
}
