#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

#endregion

namespace Logictracker.Types.ReportObjects.Datamart
{
    /// <summary>
    /// Class that represents a position processed for datamart generation.
    /// </summary>
    public class DatamartPosition
    {
        #region Public Properties

        public DateTime Fecha { get; set; }
        public Empleado Chofer { get; set; }
        public double KmRecorridos { get; set; }
        public double HsMarcha { get; set; }
        public double HsDetencion { get; set; }
        public double MinutosExceso { get; set; }
        public double SegundosSinInformar { get; set; }
        public int ExcesosVelocidad { get; set; }
        public int Velocidad { get; set; }
        public string EstadoDeMovimiento { get; set; }
        public ReferenciaGeografica Geocerca { get; set; }
        public string EstadoMotor { get; set; }
        public int Secuencia { get; set; }

        #endregion
    }
}
