namespace Logictracker.Routing.Client
{
    public class Vehicle
    {
        public string Id { get; set; }
        public LocationType Location { get; set; }
        public LocationType StartLocation { get; set; }
        public LocationType EndLocation { get; set; }
        public string TypeId { get; set; }
        public TimeWindowType TimeSchedule { get; set; }
        public bool ReturnToDepot { get; set; }
        public string Skills { get; set; }
        public string Type { get; set; }
    }
}