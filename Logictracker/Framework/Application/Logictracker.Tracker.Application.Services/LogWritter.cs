using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logictracker.Tracker.Application.Services
{
    public class LogWritter
    {
         public static void writeLog(Exception error, String path)
        {
            StringBuilder sb = new StringBuilder();           
            sb.AppendLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            sb.AppendLine(error.Message);
            if (error.StackTrace != null)
            {
               sb.AppendLine(error.StackTrace.ToString());
            }            
            sb.AppendLine("=======================================================================");
            string filePath = path;
            File.AppendAllText(filePath+"log.txt", sb.ToString());
            sb.Clear();
        }

    }
}
