namespace LogicTracker.App.Web.Api.Models
{
    public class JobState
    {
        public long JobId { get; set; }
        public short JobStatus { get; set; }
        public int MessageId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}