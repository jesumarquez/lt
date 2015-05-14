#region Usings

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Utils
{

    public static class IOUtils
    {

        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static bool PathFileExists(StringBuilder path);

        public static bool FileExists(String path)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(path);            
            var exists = PathFileExists(builder);
            return exists;

//            return (new FileInfo(path)).Exists;
        }

        public static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}