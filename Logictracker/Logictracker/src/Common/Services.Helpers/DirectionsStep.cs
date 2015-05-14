using System;
using System.Collections.Generic;

namespace Logictracker.Services.Helpers
{
    [Serializable]
    public class DirectionsStep
    {
        public LatLon StartLocation { get; set; }
        public LatLon EndLocation { get; set; }
        public TimeSpan Duration { get; set; }
        public string Instructions { get; set; }
        public double Distance { get; set; }
        public List<LatLon> Points { get; set; }
    }
}
