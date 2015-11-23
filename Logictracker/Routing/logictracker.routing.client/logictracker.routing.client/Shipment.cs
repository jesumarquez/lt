using System.Collections.Generic;

namespace Logictracker.Routing.Client
{
    public class Shipment  
    {
        public Stop Pickup { get; set; }
        public Stop Delivery { get; set; }
        public int CapacityDemand { get; set; }
        public List<Dimension> CapacityDimensions { get; set; }
        public string RequiredSkills { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}