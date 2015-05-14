#region Usings

using System;
using Logictracker.Metrics.Enums;

#endregion

namespace Logictracker.Metrics.Core.AuxClasses
{
    /// <summary>
    /// Class for remembering the current metric calculated value.
    /// </summary>
    [Serializable]
    public class MetricValue
    {
        public String Module { get; set; }
        public String Component { get; set; }
        public String Name { get; set; }
        public DateTime FirstInstance { get; set; }
        public DateTime LastInstance { get; set; }
        public Double CurrentValue { get; set; }
        public Int32 Ocurrences { get; set; }
        public Int32? DeviceType { get; set; }
        public Int32? VehicleType { get; set; }
        public MetricType Type { get; set; }
    }
}