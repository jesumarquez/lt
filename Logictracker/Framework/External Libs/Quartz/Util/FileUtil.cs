#region Usings

using System;
using System.IO;

#endregion

namespace Quartz.Util
{
    /// <summary>
    /// Utility class for file handling related things.
    /// </summary>
    public class FileUtil
    {
        /// <summary>
        /// Resolves file to actual file if for example relative '~' used.
        /// </summary>
        /// <param name="fName">File name to check</param>
        /// <returns>Expanded file name or actual no resolving was done.</returns>
        protected internal static string ResolveFile(string fName)
        {
            if (fName != null && fName.StartsWith("~"))
            {
                // relative to run directory
                fName = fName.Substring(1);
                if (fName.StartsWith("/") || fName.StartsWith("\\"))
                {
                    fName = fName.Substring(1);
                }
                fName = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, fName);
            }

            return fName;
        }
    }
}