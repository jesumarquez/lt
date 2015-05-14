using System;

using System.Collections.Generic;
using System.Text;

namespace Win32
{
    public enum BatteryFlag : byte
    {
        BATTERY_FLAG_HIGH = 0x01,
        BATTERY_FLAG_CRITICAL = 0x04,
        BATTERY_FLAG_CHARGING = 0x08,
        BATTERY_FLAG_NO_BATTERY = 0x80,
        BATTERY_FLAG_UNKNOWN = 0xFF,
        BATTERY_FLAG_LOW = 0x02
    }
}
