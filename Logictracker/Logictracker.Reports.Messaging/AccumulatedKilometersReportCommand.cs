﻿using System;
using System.Collections.Generic;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class AccumulatedKilometersReportCommand : IReportCommand
    {
        public int ReportId { get; set; }
        public int BaseId { get; set; }
        public int CustomerId { get; set; }
        public List<int> VehiclesId { get; set; }
        public string ReportName { get; set; }
        public bool InCicle { get; set; }
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public string Email { get; set; }
    }
}