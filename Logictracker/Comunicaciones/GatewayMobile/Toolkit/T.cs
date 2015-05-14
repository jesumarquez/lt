using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Urbetrack.Mobile.Toolkit;

[assembly: CLSCompliant(true)]  

namespace Urbetrack.Mobile.Toolkit
{
    public class T
    {
        private static string FileName;
        public static FileStream fs;
        public static bool ResetFileOnStartup { get; set; }

        static T ()
        {
            ResetFileOnStartup = true;
            CurrentLevel = 0;
        }

        public static void Initialize(string file)
        {
            FileName = file;
            INFO("--T.Initialize({0})--", file);
        }

        private static string last_message = "";
        private static int last_message_cnt;
        public static int CurrentLevel;

        private static void WriteAhead(string txt)
        {
            txt += "\r\n";
            if (fs == null)
            {
                var filename = @"\Temp\" + FileName + ".log";
                var back_filename = @"\Temp\" + FileName + ".txt";
                try
                {
                    if (ResetFileOnStartup)
                    {
                        if (File.Exists(filename))
                        {
                            if (File.Exists(back_filename))
                            {
                                File.Delete(back_filename);
                            }
                            File.Move(filename, back_filename);
                        }
                    }
                } catch
                {
                    Debug.WriteLine("IMPOSIBLE HACER BACKUP DEL LOG.");
                }
                
                fs = File.Open(filename, (ResetFileOnStartup ? FileMode.Create : FileMode.Append), FileAccess.Write, FileShare.Read);
            }
            if (txt != last_message)
            {
                if (last_message_cnt > 1)
                {
                    var cnt_line = String.Format("{0:HHmmss}: Ultimo mensaje se repitio {1} veces.", DateTime.Now, last_message_cnt);
                    var cnt_buff = Encoding.ASCII.GetBytes(cnt_line);
                    fs.Write(cnt_buff, 0, cnt_buff.GetLength(0));
                }
                last_message_cnt = 0;
                last_message = txt;
            } else
            {
                last_message_cnt++;
                if (last_message_cnt > 1) return;
            }
            var line = String.Format("{0:HHmmss}:{1}", DateTime.Now, txt);
            var buff = Encoding.ASCII.GetBytes(line);
            fs.Write(buff, 0, buff.GetLength(0));
            fs.Flush();
        }

        public static void EXCEPTION(Exception e, string module)
        {
            var list = IEXCEPTION(e, module);
            foreach(var line in list)
            {
                ERROR(line);
            }
        }

        /// <summary>
        /// Registra en el trazador contextual el detalle de la excepcion dada.
        /// y adicionalmente agrega la informacion adicional en module.
        /// </summary>
        /// <param name="e">Excepcion a detallar</param>
        /// <param name="module">Informacion adional a incluir en el registro.</param>
        public static List<string> IEXCEPTION(Exception e, string module)
        {
            if (string.IsNullOrEmpty(module))
            {
                module = "";
            }
            var lines = new List<string>();
            IEXCEPTION(e, 0, lines, module);
            return lines;
        }

        private static void IEXCEPTION(Exception e, int ident, ICollection<string> lines, string module)
        {
            lines.Add(Format.Ident(ident == 0 ? "EXCEPTION.BEGIN('" + module + "')" : "INNER_EXCEPTION.BEGIN", ident));
            lines.Add(Format.Ident(String.Format("Exception : '{0}'", e.GetType()), ident));
            lines.Add(Format.Ident(String.Format("Message   : '{0}'", e.Message), ident));
            lines.Add(Format.Ident("StackTrace:", ident));
            foreach(var line in Regex.Split(e.StackTrace, "\r\n"))
            {
                lines.Add(Format.Ident(line, ident, 4));
            }
            if (e.InnerException != null)
            {
                IEXCEPTION(e.InnerException, ident + 1, lines, "");
            }
            lines.Add(Format.Ident(ident == 0 ? "EXCEPTION.END" : "INNER_EXCEPTION.END", ident));
        }

        public static void DEAD_REPORT(Exception exception)
        {
            var filename = @"\Temp\" + FileName + "_deadreport.txt";
            var output = String.Format(" ==== {0} ====  \r\n", DateTime.Now);
            output += Format.Join(IEXCEPTION(exception, "DEAD_REPORT"), "\r\n");
            try
            {
                fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
                var buff = Encoding.ASCII.GetBytes(output);
                fs.Write(buff, 0, buff.GetLength(0));
                fs.Flush();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch  { }
// ReSharper restore EmptyGeneralCatchClause
        }

        //[Conditional("DEBUG")]
        public static void TRACE(string txt)
        {
            TRACE(0, txt);
        }

        public static void TRACE(int level, string txt)
        {
            if (CurrentLevel < level) return;
            Debug.WriteLine(string.Format("XXXXDBG:{0}",txt));
            WriteAhead(txt);
        }

        public static void TRACE(int level, string txt, params object[] args)
        {
            if (CurrentLevel < level) return;
            Debug.WriteLine(string.Format("XXXXDBG:" + txt, args));
            WriteAhead(string.Format(txt, args));
        }

        public static void INFO(string txt)
        {
            Debug.WriteLine(string.Format("***INFO:{0}", txt));
            WriteAhead(txt);
        }

        public static void INFO(string txt, params object[] args)
        {
            INFO(string.Format("XXXXDBG:" + txt, args));
        }

        public static void NOTICE(string txt)
        {
            Debug.WriteLine(string.Format("*NOTICE:{0}", txt));
            WriteAhead(txt);
        }

        public static void NOTICE(string txt, params object[] args)
        {
            NOTICE(string.Format("XXXXDBG:" + txt, args));
        }

        public static void ERROR(string txt)
        {
            Debug.WriteLine(string.Format(">>>>ERR:{0}", txt));
            WriteAhead(txt);
        }

        public static void ERROR(string txt, params object[] args)
        {
            ERROR(string.Format("XXXXDBG:" + txt, args));
        }

        public static string ENDPOINT(EndPoint ep)
        {
            return ep == null ? "(null)" : ep.ToString();
        }

        public static string ENDPOINT(Socket s, EndPoint ep)
        {
            if (s == null) return "(null socket)";
            return ep == null ? "(null)" : ep.ToString();
        }
    }
}