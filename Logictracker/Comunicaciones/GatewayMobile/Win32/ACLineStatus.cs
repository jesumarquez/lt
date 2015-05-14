using System;

using System.Collections.Generic;
using System.Text;

namespace Win32
{
    // http://msdn.microsoft.com/en-us/library/aa926903.aspx
    public enum ACLineStatus : byte
    {
        AC_LINE_OFFLINE     = 0, // Offline
        AC_LINE_ONLINE      = 1, // Online
        AC_LINE_BACKUP_POWER    = 2, // Backup Power
        AC_LINE_UNKNOWN    = 0xFF, //
        Unknown = 0xFF, //status
    }
}
