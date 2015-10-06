namespace LogicTracker.App.Web.Api.Models
{
    public class SitrackReceptionModel
    {
        public uint id { get; set; }
        public string reportDate { get; set; }
        public int device_id { get; set; }
        public int holder_id { get; set; }
        public string holder_domain { get; set; }
        public string holder_name { get; set; }
        public int event_id { get; set; }
        public string event_desc { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string location { get; set; }
        public int course { get; set; }
        public double speed { get; set; }
        public double odometer { get; set; }
        public int ignition { get; set; }
        public string ignitionDate { get; set; }
    }
}
