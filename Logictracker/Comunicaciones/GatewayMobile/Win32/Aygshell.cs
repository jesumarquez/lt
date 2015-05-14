using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Win32
{
    public static class Aygshell
    {
        [DllImport("aygshell")]
        public static extern int Vibrate(int cvn,             
            IntPtr rgvn, 
            uint Repeat, 
            uint Timeout);

        [DllImport("aygshell.dll")]   
       public  static extern uint SndPlaySync(string pszSoundFile, uint dwFlags);

        public static void PlaySound(string fileName)
        {
            SndPlaySync(fileName,0);
        }
    }
}
