using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class RouteList
    {
        public int CompanyId { get; set; }
        public DateTime DateTime { get; set; }
        public int LineId { get; set; }
        public RouteItem[] RouteItems { get; set; }
    }
}