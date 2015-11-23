using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class RouteItem
    {
        public string patente;
        public string interno;
        public string Code { get; set; }
        public int DeliveriesNumber { get; set; }
        public int Id { get; set; }
        public string Places { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Status { get; set; }
    }
}