﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class VehicleActivityReportCommand : IReportCommand
    {
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public string Email { get; set; }
        public string ReportName { get; set; }
        public int ReportId { get; set; }
        public int CustomerId { get; set; }
        public List<int> VehiclesId { get; set; }
    }
}
