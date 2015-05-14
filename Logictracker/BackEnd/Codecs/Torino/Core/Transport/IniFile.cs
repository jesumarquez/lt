#region Usings

using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Urbetrack.Comm.Core.Transport
{
    public class IniFile
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);


        public static void Set(string FileName, string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, FileName);
        }

        public static string Get(string FileName, string Section, string Key, string Default)
        {
            var result = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, Default, result, 255, FileName);
            return result.ToString();
        }
    }
}
