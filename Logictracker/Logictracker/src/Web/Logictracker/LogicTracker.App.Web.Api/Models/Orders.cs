using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogicTracker.App.Web.Api.Models
{
    public class Orders
    {
        public String Code { get; set; }
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? RealStartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public OrderModel[] OrderModels { get; set; }
    }
}