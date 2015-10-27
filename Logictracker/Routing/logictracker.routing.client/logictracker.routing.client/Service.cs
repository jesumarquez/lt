using System.Collections.Generic;

namespace Logictracker.Routing.Client
{
    public class Service
    {
        public LocationType Location { get; set; }
        public string LocationId { get; set; }
        public CoordType Coord { get; set; }
        public string Name { get; set; }
        public int CapacityDemand { get; set; }
        public List<Dimension> CapacityDimensions { get; set; }
        public decimal Duration { get; set; }
        public TimeWindowType TimeWindow { get; set; }
        public string RequiredSkills { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
    }
}