using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class Route
    {
        public String Code { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public Job[] Jobs { get; set; }
    }
}