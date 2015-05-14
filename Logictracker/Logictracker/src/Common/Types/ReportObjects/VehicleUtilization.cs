#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Logictracker.Types.ReportObjects
{
    public class VehicleUtilization
    {
        public DateTime Fecha { get; set; }
        public List<Double> HsTurnos { get; set; }
        public List<Double> HsReales { get; set; }
    }
}
