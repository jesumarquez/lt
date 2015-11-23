namespace Logictracker.Routing.Client
{
    public class VehicleTypesCosts
    {
        public decimal Fixed { get; set; }
        public decimal Distance { get; set; }
        public decimal Time { get; set; }

        public VehicleTypesCosts()
        {
            Fixed = 0.0m;
            Distance = 0.0m;
            Time = 0.0m;
        }
    }
}