#region Usings

using System;

#endregion

namespace Logictracker.Metrics.Types
{
    /// <summary>
    /// Class that represents a processed system metric.
    /// </summary>
    [Serializable]
    public class Metric
    {
        public virtual Int32 Id { get; set; }
        public virtual DateTime Start { get; set; }
        public virtual DateTime End { get; set; }
        public virtual String Name { get; set; }
        public virtual String Module { get; set; }
        public virtual String Component { get; set; }
        public virtual Double Value { get; set; }
        public virtual Int32? VehicleType { get; set; }
        public virtual Int32? DeviceType { get; set; }
    }
}