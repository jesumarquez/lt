using System;

using System.Collections.Generic;
using System.Text;

namespace Win32
{
    [Flags()]
    public enum  DevicePowerFlags
    {
        None = 0,
        /// <summary>
        /// Specifies the name of the device whose power should be maintained at or above the DeviceState level.
        /// </summary>
        POWER_NAME = 0x00000001,
        /// <summary>
        /// Indicates that the requirement should be enforced even during a system suspend.
        /// </summary>
        POWER_FORCE = 0x00001000,
        POWER_DUMPDW = 0x00002000
    }
}
