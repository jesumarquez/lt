namespace LogicTracker.App.Web.Api.Models
{
    public class Job
    {
        public string ClientName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string EndDate { get; set; }
        public int Id { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int OrderNumber { get; set; }
        public int Quantity;
        public string StartDate { get; set; }
        public int State { get; set; }
        public float Value;
        public float Volumen;
        public float Weight;
        public string clienttype;
    }
}