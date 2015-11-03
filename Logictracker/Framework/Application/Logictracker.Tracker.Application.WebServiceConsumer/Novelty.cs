namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public class Novelty
    {
        public string ServiceNumber { get; set; }
        public string MobileId { get; set; }

        public Novelty(string serviceNumber, string mobileId)
        {
            ServiceNumber = serviceNumber;
            MobileId = mobileId;
        }
    }
}