using System.Collections.Generic;

namespace Logictracker.Routing.Client
{
    public class Route
    {
        public string DriverId { get; set; }
        public string VehicleId { get; set; }
        public double Start { get; set; }
        public List<ActGroup> ActGroup { get; set; }
        public string End { get; set; }
    }
}