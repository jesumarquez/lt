using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LogicTracker.App.Web.Api.Providers
{
    public class LogWritter
    {
        public static void writeLog(Exception error)
        {
            StringBuilder sb = new StringBuilder();           
            sb.AppendLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            sb.AppendLine(error.Message);
            sb.AppendLine(error.StackTrace.ToString());
            sb.AppendLine("=======================================================================");
            string filePath = HttpContext.Current.Server.MapPath("~");
            File.AppendAllText(filePath+"log.txt", sb.ToString());
            sb.Clear();
        }
    }
}