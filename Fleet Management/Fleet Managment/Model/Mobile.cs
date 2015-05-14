using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Urbetrack.FleetManagment.Model
{
    public class Mobile
    {
        public int VehicleId { get; set; }
        public int DeviceId { get; set; }
        public String Device { get; set; }
        public String Vehicle { get; set; }
        public String IMEI { get; set; }
        public String District { get; set; }
        public String Base { get; set; }
        public String Queue { get; set; }
    }
}
