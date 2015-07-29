using System;
using Logictracker.Tracker.Parser.Spi;
using Logictracker.Types.BusinessObjects.Positions;

namespace Logictracker.Tracker.Parser.Caesat
{
    public class CaesatFrame : IFrame
    {
        public string MessageType { get; set; }
        public DateTime DateTime { get; set; }
        public string LicensePlate { get; set; }
        public Coordinate Coordinate { get; set; }
        public int Speed { get; set; }
        public int Meaning { get; set; }
        public int EventCode {get; set; }
        public int Temperature1 { get; set; }
        public int Temperature2 { get; set; }
        public int Temperature3 { get; set; }
        public string Raw { get; set; }
    }
}