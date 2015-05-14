using System;

using System.Collections.Generic;
using System.Text;

namespace Win32
{
    /// <summary>
    /// 
    /// </summary>
    public enum CEDEVICE_POWER_STATE:int
    {
        PwrDeviceUnspecified = -1,
        //Full On: full power,  full functionality
        D0 = 0,        
        /// <summary>
        /// Low Power On: fully functional at low power/performance
        /// </summary>
        D1 = 1,
        /// <summary>
        /// Standby: partially powered with automatic wake
        /// </summary>
        D2 = 2,
        /// <summary>
        /// Sleep: partially powered with device initiated wake
        /// </summary>
        D3 = 3,
        /// <summary>
        /// Off: unpowered
        /// </summary>
        D4 = 4,
        PwrDeviceMaximum
    }
}
