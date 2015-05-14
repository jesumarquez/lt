#region Usings

using System;
using Logictracker.Metrics.Enums;

#endregion

namespace Logictracker.Metrics.Core.AuxClasses
{
    /// <summary>
    /// Auxiliar class that represents a particular occurence of a metric.
    /// </summary>
    [Serializable]
    public class MetricInstance
    {
        public DateTime DateTime { get; set; }
        public String Name { get; set; }
        public String Module { get; set; }
        public String Component { get; set; }
        public Double Value { get; set; }
        public Int32? VehicleType { get; set; }
        public Int32? DeviceType { get; set; }

        public MetricType Type { get; set; }
        public Int32 Retries { get; set; }
    }
}