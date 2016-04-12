using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System;


namespace Logictracker.Utils.NHibernate
{
    public class NHSQLInterceptor : EmptyInterceptor, IInterceptor
    {
        SqlString
            IInterceptor.OnPrepareStatement
                (SqlString sql)
        {
            /* StackTrace trace = new StackTrace(Thread.CurrentThread, true);

             String AppName = System.AppDomain.CurrentDomain.ToString();


             try
             {
                 string stackTracer = trace.ToString();
                 string sqlQuery = sql.ToString();
                 using (StreamWriter sw = File.AppendText(Directory.GetCurrentDirectory() + @"\LOGSQL.txt"))
                 {
                     sw.WriteLine("Stack Trace");
                     sw.WriteLine("=======================================================");
                     sw.WriteLine(stackTracer);
                     sw.WriteLine("=======================================================");
                     sw.WriteLine("Consulta SQL");
                     sw.WriteLine(sqlQuery);
                     sw.WriteLine("=======================================================");
                 }
                 FileInfo fi = new FileInfo(Directory.GetCurrentDirectory() + @"\LOGSQL.txt");
                 if (fi.Length > 1048576)
                 {
                     fi.Delete();
                 }
             }
             catch (Exception)
             {
             }
             */
            return sql;
        }
    }
}
