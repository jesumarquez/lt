using System;

using System.Collections.Generic;
using System.Text;

namespace Win32
{
    public enum PowerState
    {
        POWER_STATE_ON = (0x00010000),          // on state
        POWER_STATE_OFF = (0x00020000),         // no power, full off
        POWER_STATE_CRITICAL = (0x00040000),    // critical off
        POWER_STATE_BOOT = (0x00080000),        // boot state
        POWER_STATE_IDLE = (0x00100000),        // idle state
        POWER_STATE_SUSPEND = (0x00200000),     // suspend state
        POWER_STATE_UNATTENDED = (0x00400000),  // Unattended state.
        POWER_STATE_RESET = (0x00800000),       // reset state
        POWER_STATE_USERIDLE = (0x01000000),    // user idle state
        POWER_STATE_PASSWORD = (0x10000000)     // This state is password protected.
    };
}
