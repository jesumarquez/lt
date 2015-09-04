﻿using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Reports.Messaging
{
    [Serializable]
    public class MobilesTimeReportCommand : IReportCommand
    {
        public DateTime InitialDate { get; set; }
        public DateTime FinalDate { get; set; }
        public string Email { get; set; }
        public ProgramacionReporte.FormatoReporte ReportFormat { get; set; }
        public string ReportName { get; set; }
        public int ReportId { get; set; }
        public int CustomerId { get; set; }
        public List<int> VehiclesId { get; set; }
    }
}
