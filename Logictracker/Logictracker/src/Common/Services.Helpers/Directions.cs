using System;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.Services.Helpers
{
    [Serializable]
    public class Directions
    {
        public string Descripcion { get; set; }
        public TimeSpan Duration { get { return Legs.Aggregate(TimeSpan.Zero, (w, i) => w.Add(i.Duration)); } }
        public double Distance { get { return Convert.ToInt32(Legs.Sum(s => s.Distance)); } }
        public string Mode { get; set; }
        public List<DirectionsLeg> Legs  = new List<DirectionsLeg>();
    }
}
