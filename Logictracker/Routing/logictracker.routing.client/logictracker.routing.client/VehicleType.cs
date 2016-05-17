using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logictracker.Routing.Client
{
    public class VehicleType
    {
        public string Id { get; set; }
        public string Capacity { get; set; }
        public List<Dimension> CapacityDimensions { get; set; }
        public List<VehicleTypesCosts> Costs { get; set; }
        public string Type { get; set; }
        public string PenaltyFactor { get; set; }

        public VehicleType(string vehicleType)
        {
            Id = vehicleType;
            Capacity = "0";
            CapacityDimensions = new List<Dimension>();
            Costs = new List<VehicleTypesCosts>();
        }

        public void AddCapacityDimension(int index, string value)
        {
            var vtdimension = new Dimension(index, value);
            CapacityDimensions.Add(vtdimension);
        }
    }
}
