namespace LogicTracker.App.Web.Api.Models
{
    public class RouteState
    {
        public int RouteId { get; set; }
        public string RouteCode { get; set; }
        public int RouteStatus { get; set; }
        public JobState[] JobStates { get; set; }
    }
}