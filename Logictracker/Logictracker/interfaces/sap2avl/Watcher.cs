using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
/* programa basura por evecchio */

namespace etao.sap2avl
{
    public class Watcher
    {
        EventLog el;
        private bool reloading;

        private Timer batch_timer;
        public Watcher(EventLog _el)
        {
            el = _el;
            reloading = false;
            batch_timer = new Timer(new TimerCallback(reload_dir), null, 0, Convert.ToInt32(ConfigurationManager.AppSettings["timer"]));
            //reload_dir(null);
            // creamos el monitor
            //Create();
        }

        public void log(string s) {
            if (el != null) {
                el.WriteEntry(s);
            } else {
                Console.WriteLine(s);
            }
        }

        public void reload_dir(Object status)
        {
            if (reloading) return;
            reloading = true;
            string proj_filename = String.Format("{0}\\{1}", ConfigurationManager.AppSettings["watch_path"],
            ConfigurationManager.AppSettings["proj_file"]);
            string cust_filename = String.Format("{0}\\{1}", ConfigurationManager.AppSettings["watch_path"],
            ConfigurationManager.AppSettings["cust_file"]);

            DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["watch_path"]);
            foreach (FileInfo f in dir.GetFiles(ConfigurationManager.AppSettings["watch_filter"]))
            {
                if (f.Length == 0)
                {
                    log(String.Format("IGNORANDO ARCHIVO {0}, size={1}", f.FullName,f.Length));
                } 
                else if (f.FullName.ToLower() == proj_filename.ToLower())
                {
                    procesar(f.FullName);
                }
                else if (f.FullName.ToLower() == cust_filename.ToLower())
                {
                    procesar(f.FullName);
                }
                else
                {
                    log(String.Format("IGNORANDO ARCHIVO {0}.", f.FullName));
                }
            }
            reloading = false;
        }
        /* antiguo monitor de archivos
        public void Create()
        {
            // creo el monitor
            FileSystemWatcher watcher = new FileSystemWatcher();

            // seteo el filtro de extensiones.
            watcher.Filter = ConfigurationManager.AppSettings["watch_filter"];

            // creo el evento
            watcher.Created += new
            FileSystemEventHandler(watcher_FileCreated);

            // configuro el directorio 
            watcher.Path = ConfigurationManager.AppSettings["watch_path"];

            // activo los eventos
            watcher.EnableRaisingEvents = true;
        }

        void watcher_FileCreated(object sender, FileSystemEventArgs e)
        {
            procesar(e.FullPath);
        }
        */

        void procesar(string filename)
        {
            string proj_filename = String.Format("{0}\\{1}", ConfigurationManager.AppSettings["watch_path"],
                        ConfigurationManager.AppSettings["proj_file"]);
            string proj_log = String.Format("{0}\\{1}_proj.log", ConfigurationManager.AppSettings["watch_path"],
                        DateTime.Now.ToString("yyyyMMddHHmm"));
            string proj_back = String.Format("{0}\\{1}_proj.back", ConfigurationManager.AppSettings["watch_path"],
                        DateTime.Now.ToString("yyyyMMddHHmm"));
            string cust_filename = String.Format("{0}\\{1}", ConfigurationManager.AppSettings["watch_path"],
                        ConfigurationManager.AppSettings["cust_file"]);
            string cust_log = String.Format("{0}\\{1}_cust.log", ConfigurationManager.AppSettings["watch_path"],
                        DateTime.Now.ToString("yyyyMMddHHmm"));
            string cust_back = String.Format("{0}\\{1}_cust.back", ConfigurationManager.AppSettings["watch_path"],
                        DateTime.Now.ToString("yyyyMMddHHmm"));
            if (filename.ToLower() == proj_filename.ToLower())
            {
                log("PROCESANDO ARCHIVO DE PROYECTOS");
                ProjImport a = new ProjImport(el);
                string _format = "10,custcode;12,contract;40,name;12,qty;40,obs;14,telefono;30,ignore;8,setupdate;23,ignore;40,calle;30,altura;8,ignore;16,cp;12,ignore;5,uom;14,state;14,ciudad;14,localidad;1,porf;1,valid;44,ignore;8,expiredate;40,instructions;40,contactname";
                using (StreamReader file = OpenReader(proj_filename))
                {
                    using (StreamWriter flog = OpenWriter(proj_log))
                    {
                        a.ImportFixed(file, _format, flog, "sp_urb_lomax_insert_proj");
                        flog.Close();
                    }
                    file.Close();
                }
                MoveFile(proj_filename, proj_back);
            }
            else if (filename.ToLower() == cust_filename.ToLower())
            {
                log("PROCESANDO ARCHIVO DE CLIENTES");
                ProjImport a = new ProjImport(el);
                string _format = "10,custcode;40,name;8,shortname;40,calle;40,altura;40,ciudad;10,state;10,cp;3,pais;40,ignore;14,telefono;203,ignore;2,valid"; // ;40;14;14;14;14;3;12;8;8;40;30;40;20";
                using (StreamReader file = OpenReader(cust_filename))
                {
                    using (StreamWriter flog = OpenWriter(cust_log))
                    {
                        a.ImportFixed(file, _format, flog, "sp_urb_lomax_insert_cust");
                        flog.Close();
                    }
                    file.Close();
                }
                MoveFile(cust_filename, cust_back);
            } else {
                log(String.Format("IGNORANDO ARCHIVO {0}.", filename));
            }
        }

        public StreamWriter OpenWriter(string filename)
        {
            int intento = 0;
            while (true)
            {
                try
                {
                    StreamWriter file = new StreamWriter(filename);
                    file.AutoFlush = true;
                    return file;
                }
                catch (Exception e)
                {
                    log(String.Format("Excepcion {2} al crear {0}:\nVolcado de la Excepcion:\n{1}", filename, e, ++intento));
                    Thread.Sleep(2000);
                    continue;
                }
            }
        }

        public StreamReader OpenReader(string filename)
        {
            int intento = 0;
            while(true)
            {
                try
                {
                    StreamReader file = new StreamReader(filename);
                    return file;
                }
                catch (Exception e)
                {
                    log(String.Format("Excepcion {2} al abrir {0}:\nVolcado de la Excepcion:\n{1}", filename, e, ++intento));
                    Thread.Sleep(500);
                    continue;
                }
            }
        }

        public void MoveFile(string oldname, string newname)
        {
            int intento = 0;
            while (true)
            {
                try
                {
                    File.Move(oldname, newname);
                    return;
                }
                catch (Exception e)
                {
                    log(String.Format("Excepcion {2} al mover {0},{3}:\nVolcado de la Excepcion:\n{1}", oldname, e, ++intento, newname));
                    Thread.Sleep(2500);
                    continue;
                }
            }
        }

        public Timer BatchTimer
        {
            get { return batch_timer; }
        }
    }
}
