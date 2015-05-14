using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Win32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VibrateNote
    {
        public short wDuration;
        public byte bAmplitude;
        public byte bFrequency;
    }
}
