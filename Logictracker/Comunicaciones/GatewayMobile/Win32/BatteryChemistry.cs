using System;

using System.Collections.Generic;
using System.Text;

namespace Win32
{
    public enum  BatteryChemistry:byte
    {
        BATTERY_CHEMISTRY_ALKALINE = 0x01,  // Alkaline battery.
        BATTERY_CHEMISTRY_NICD = 0x02, // Nickel Cadmium battery.
        BATTERY_CHEMISTRY_NIMH = 0x03, // Nickel Metal Hydride battery.
        BATTERY_CHEMISTRY_LION= 0x04, // Lithium Ion battery.
        BATTERY_CHEMISTRY_LIPOLY = 0x05, // Lithium Polymer battery.
        BATTERY_CHEMISTRY_ZINCAIR = 0x06, // Zinc Air battery.
        BATTERY_CHEMISTRY_UNKNOWN = 0xFF // Battery chemistry is unknown.
    }
}
