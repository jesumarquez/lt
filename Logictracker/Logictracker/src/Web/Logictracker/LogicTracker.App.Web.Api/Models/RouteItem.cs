using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class RouteItem
    {
        public string Code { get; set; }
        public int DeliveriesNumber { get; set; }
        public int Id { get; set; }
        public string Places { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Status { get; set; }
        public string interno { get; set; }
        public string patente { get; set; }
    }
}