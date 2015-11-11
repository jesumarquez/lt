using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logictracker.Types.BusinessObjects.Ordenes;

namespace Logictracker.Web.Models
{
    public class OrderSelectionModel
    {
        public List<OrderModel> OrderList { get; set; }
        public string RouteCode { get; set; }
        public DateTime StartDateTime { get; set; }
        public int LogisticsCycleType { get; set; }
        public int IdVehicle { get; set; }
    }
}
