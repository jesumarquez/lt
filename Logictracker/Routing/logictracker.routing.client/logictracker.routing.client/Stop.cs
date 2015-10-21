namespace Logictracker.Routing.Client
{
    public class Stop
    {
        public LocationType Location { get; set; }
        public string LocationId { get; set; }
        public CoordType Coord { get; set; }
        public decimal Duration { get; set; }
        public TimeWindowType TimeWindow { get; set; }

    }
}