#region Usings

using System;
using System.IO;
using Urbetrack.DatabaseTracer.Core;

#endregion

namespace Urbetrack.Comm.Core.Transport
{
    public class IO
    {
        public static bool PadFile(string source, string destination, int align, byte fill, bool overwrite)
        {
            if (!overwrite && File.Exists(destination)) return true;
            try
            {
                File.Copy(source, destination, true);
                using (var fs = File.OpenWrite(destination))
                {
                    var toadd = align - (fs.Length%align);
                    fs.Seek(0, SeekOrigin.End);
                    for (var i = 0; i < toadd; ++i) fs.WriteByte(fill);
                    fs.Close();
                }
                return true;
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(IO).FullName, e);
            }
            return false;
        }

        public static bool Bulk(string filename, string data)
        {
            try
            {   
                using (var fs = File.CreateText(filename))
                {
                    fs.Write(data);
                    fs.Close();
                }
                return true;
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(IO).FullName, e);
            }
            return false;
        }

        public static string Cat(string filename)
        {
            try
            {
                string data;
                using (var fs = File.OpenText(filename))
                {
                    data = fs.ReadToEnd();
                    fs.Close();
                }
                return data;
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(IO).FullName, e);
            }
            return "";
        }
    }
}
