using System.Collections.Generic;

namespace Logictracker.Routing.Client
{
    public class Solution
    {
        public decimal Cost { get; set; }
        public List<Route> Routes { get; set; }
        public List<string> UnassignedJobs { get; set; }

    }
}