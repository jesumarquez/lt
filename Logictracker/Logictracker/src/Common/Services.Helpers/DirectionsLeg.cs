using System;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.Services.Helpers
{
    [Serializable]
    public class DirectionsLeg
    {
        public TimeSpan Duration { get { return Steps.Aggregate(TimeSpan.Zero, (w, i) => w.Add(i.Duration)); } }
        public double Distance { get { return Convert.ToInt32(Steps.Sum(s => s.Distance)); } }
        public List<DirectionsStep> Steps = new List<DirectionsStep>(); 
    }
}
