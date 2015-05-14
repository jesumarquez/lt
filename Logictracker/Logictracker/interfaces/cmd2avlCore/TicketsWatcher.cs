using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using parser_dll.command_data;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace command_data
{
    public class TicketFile {
        public String path;
        public int parsed_offset = 0;
        public int last_dquotebegin_offset = 0;
    }

    public class TicketsWatcher
    {
        List<TicketFile> ticket_files = new List<TicketFile>();
        StreamWriter logfile = null;
        public TicketsWatcher()
        {
            logfile = new StreamWriter(ConfigurationManager.AppSettings["log_file"],true);
            logfile.AutoFlush = true;
            logfile.WriteLine(String.Format("{0}-SERVICESTARTED", DateTime.Now));            
        }

        public void Initialize()
        {
            reload_dir();
            Create();
        }

        public void reload_dir()
        {
            try
            {
                logfile.WriteLine("RECONOCIENDO DIRECTORIO {0}", ConfigurationManager.AppSettings["watch_path"]);
                DirectoryInfo dir = new DirectoryInfo(ConfigurationManager.AppSettings["watch_path"]);
                foreach (FileInfo f in dir.GetFiles(ConfigurationManager.AppSettings["watch_filter"]))
                {
                    TicketFile tkt = new TicketFile();
                    tkt.path = f.Name;
                    ticket_files.Add(tkt);
                    logfile.WriteLine("RECONOCIENDO ARCHIVO {0}", f.Name);
                    updatefile(tkt);
                }
            }
            catch (Exception ex)
            {
                logfile.WriteLine(String.Format("EXCEPTION:RELOAD_DIR:{0}", ex.ToString()));
                throw;
            }
        }

        public void updatefile(TicketFile file)
        {
            try 
            {
                String FullName = ConfigurationManager.AppSettings["watch_path"] + file.path;
                FileStream fs = new FileStream(FullName, FileMode.Open, FileAccess.Read, FileShare.None);
                StreamReader sfile = new StreamReader(fs);
                String current_record = "";
                while (!sfile.EndOfStream)
                {
                    String line = sfile.ReadLine();
                    //Console.Write(line);
                    if (line[0] == '"')
                    {
                        if (line.LastIndexOf('"') > 0)
                        {
                            // registro completo en una linea.
                            current_record = line;
                            process_record(current_record, file.path);
                            current_record = "";
                        }
                        else
                        {
                            current_record += line;
                            current_record += "\n";
                        }
                    }
                    else
                    {
                        if (line.LastIndexOf('"') > 0)
                        {
                            current_record += line;
                            process_record(current_record, file.path);
                            current_record = "";
                        }
                        else
                        {
                            current_record += line;
                            current_record += "\n";
                        }
                    }
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                logfile.WriteLine(String.Format("EXCEPTION:UPDATE_FILE:{0}", ex.ToString()));
                throw;
            }
        }

        public void fill_parameters(SqlParameterCollection parms, ticket tkt, String filename)
        {
            List<int> _fds = new List<int>();
            //@Z1,@Z3,@Z10,@Z14,@Z19,@Z9,@Z75,@Z77,@Z152,@Z153,@Z154,@Z157
            _fds.Add(1); _fds.Add(3); _fds.Add(10); _fds.Add(14); _fds.Add(19); _fds.Add(9);
            _fds.Add(26); _fds.Add(27); _fds.Add(28); _fds.Add(29); _fds.Add(30); _fds.Add(31);
            _fds.Add(75); _fds.Add(77); _fds.Add(152); _fds.Add(153); _fds.Add(154); _fds.Add(157);
            parms.AddWithValue("@sourcehost", ConfigurationManager.AppSettings["hostid"]);
            parms.AddWithValue("@sourcefile", filename);
            parms.AddWithValue("@codigo", tkt.id);
            parms.AddWithValue("@canceled", tkt._canceled);
            foreach (int _fd in _fds)
            {
                if (!tkt.fields.ContainsKey(_fd))
                {
                    parms.AddWithValue("@Z" + _fd, "");
                }
                else
                {
                    parms.AddWithValue("@Z" + _fd, tkt.fields[_fd]);
                }
            }
        }

        public void process_record(String current_record, String filename){
            try 
            {
                //Console.Write("Procesando Registro:{0}", current_record);
                ticket_parser parser = new ticket_parser();
                ticket ticket = new ticket();
                ticket_parser._global_tracer = 0;
                ticket_parser._scan_tracer = 0;
                if (ticket_parser.parse_str(current_record, ticket))
                {
                    if (ticket.id != 0)
                    {
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["connstr"]))
                        {
                            conn.Open();
                            SqlCommand cmd = conn.CreateCommand();
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.CommandText = "SELECT count(*) AS count, ot01_estado FROM opetick01 WHERE opetick01_codigo=@id GROUP BY ot01_estado;";
                            cmd.Parameters.AddWithValue("@id", ticket.id);
                            int existe = 0;
                            bool cancelado = false;
                            bool finalizado = false;
                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    existe = dr.GetInt32(dr.GetOrdinal("count"));
                                    cancelado = dr.GetInt16(dr.GetOrdinal("ot01_estado")) == 9;
                                    finalizado = dr.GetInt16(dr.GetOrdinal("ot01_estado")) == 1;
                                }
                                dr.Close();
                            }

                            if (ticket._canceled)
                            {
                                Console.Write("Cancel de ticket id={0} en la base...\n", ticket.id);

                                if (existe > 0)
                                {
                                    SqlCommand updater = conn.CreateCommand();
                                    updater.CommandText =
                                        "UPDATE opetick01 SET ot01_estado = 9 WHERE opetick01_codigo = @id;";
                                    updater.Parameters.AddWithValue("@id", ticket.id);
                                    updater.ExecuteNonQuery();
                                }
                                else
                                {
                                    SqlCommand inserter = conn.CreateCommand();
                                    inserter.CommandText =
                                        "INSERT INTO opetick01 (opetick01_sourcestation, opetick01_sourcefile, opetick01_codigo, ot01_estado) VALUES (@sourcehost, @sourcefile, @id, 9);";
                                    inserter.Parameters.AddWithValue("@id", ticket.id);
                                    inserter.Parameters.AddWithValue("@sourcehost",
                                                                     ConfigurationManager.AppSettings["hostid"]);
                                    inserter.Parameters.AddWithValue("@sourcefile", filename);
                                    inserter.ExecuteNonQuery();
                                }

                                if (cancelado || finalizado) return;

                                SqlCommand cmd2 = conn.CreateCommand();
                                cmd2.CommandType = System.Data.CommandType.Text;
                                cmd2.CommandText =
                                    "SELECT parenti03_interno as interno, parenti02_descricorta AS planta FROM opetick01 o1, parenti03 p3, parenti02 p2 WHERE o1.opetick01_codigo= @id AND o1.rela_parenti08 = p3.rela_parenti08 AND o1.rela_parenti02 = p2.id_parenti02;";
                                cmd2.Parameters.AddWithValue("@id", ticket.id);

                                using (SqlDataReader dr = cmd2.ExecuteReader())
                                {
                                    if (dr.Read())
                                    {
                                        CicloLogistico(true, dr.GetValue(0).ToString(), dr.GetString(1));
                                    }
                                }
                            }
                            else
                            {
                                Console.Write("Insert/Update de ticket id={0} en la base...\n", ticket.id);
                                if (existe > 0)
                                {
                                    // no hace nada.. ya se importo.
                                }
                                else
                                {
                                    SqlCommand inserter = conn.CreateCommand();
                                    inserter.CommandText = "EXECUTE sp_urb_lomax_insert_ticket @sourcehost, @sourcefile, @codigo,@canceled,@Z1,@Z3,@Z10,@Z14,@Z19,@Z9,@Z26,@Z27,@Z28,@Z29,@Z30,@Z31,@Z75,@Z77,@Z152,@Z153,@Z154,@Z157;";
                                    fill_parameters(inserter.Parameters, ticket, filename);
                                    try
                                    {
                                        using (SqlDataReader dr = inserter.ExecuteReader())
                                        {
                                            while (dr.NextResult()) ;// dispara la exception de raiserror
                                        }
                                    }
                                    catch (SqlException sqle)
                                    {
                                        logfile.WriteLine(String.Format("{0},SQLTEXT,{1}", DateTime.Now, sqle.Message));
                                        if (sqle.State == 1)
                                        {
                                            logfile.WriteLine(String.Format("{0},FPLANTA,{1}", DateTime.Now, ticket.fields[2]));
                                        }
                                        if (sqle.State == 2)
                                        {
                                            logfile.WriteLine(String.Format("{0},FCLIENTE,{1}", DateTime.Now, ticket.fields[2]));
                                        }
                                        if (sqle.State == 3)
                                        {
                                            logfile.WriteLine(String.Format("{0},FOBRA,{1}", DateTime.Now, ticket.fields[2]));
                                        }
                                        if (sqle.State == 4)
                                        {
                                            logfile.WriteLine(String.Format("{0},FCAMION,{1}", DateTime.Now, ticket.fields[2]));
                                        }
                                        if (sqle.State > 4)
                                        {
                                            logfile.WriteLine(String.Format("{0},UNKNOW,{1}", DateTime.Now, ticket.fields[2]));
                                        }
                                        return;
                                    }
                                    CicloLogistico(false, ticket.fields[3], ticket.fields[1]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logfile.WriteLine(String.Format("EXCEPTION:PROCESS_RECORD:{0}", ex.ToString()));
                throw;
            }
        }

        public void CicloLogistico(bool cancela, string interno, string planta)
        {
            string msg = "X";
            if (cancela) msg += "2,";
            if (!cancela) msg += "1,";
            msg += interno + "," + planta + ", ";
            using (UdpClient cli = new UdpClient(ConfigurationManager.AppSettings["udp_host"],
                                  Convert.ToInt32(ConfigurationManager.AppSettings["udp_port"])))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(msg);
                int timer = 2000;
                int intento = 0;
                while (true)
                {
                    ++intento;
                    cli.Send(buffer, msg.Length - 1);
                    if (timer < 5000)
                    {
                        timer = timer * intento;
                    }
                    Thread.Sleep(timer);
                    if (cli.Available > 0)
                    {
                        try
                        {
                            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);
                            while (cli.Available > 0) cli.Receive(ref remoteEp);
                            cli.Close();
                        }
                        catch (Exception e)
                        {
                            Console.Write(e);
                        }
                        return;
                    }
                }
            }
        }

        public void Create()
        {
            // creo el monitor
            FileSystemWatcher watcher = new FileSystemWatcher();

            // seteo el filtro de extensiones.
            watcher.Filter = ConfigurationManager.AppSettings["watch_filter"];

            // creo los manejadores de eventos            
            watcher.Created += new FileSystemEventHandler(watcher_FileCreated);
            watcher.Changed += new FileSystemEventHandler(watcher_FileChanged);

            // configuro el directorio 
            watcher.Path = ConfigurationManager.AppSettings["watch_path"];

            // activo los eventos
            watcher.EnableRaisingEvents = true;
        }

        void watcher_FileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("UN NUEVO ARCHIVO {0}", e.FullPath);
            Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["before_parse_sleep"]));
            TicketFile file = new TicketFile();
            file.path = e.Name;
            updatefile(file);
        }

        void watcher_FileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("UN ARCHIVO CAMBIADO {0}", e.Name);
            Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["before_parse_sleep"]));
            TicketFile file = new TicketFile();
            file.path = e.Name;
            updatefile(file);
        }
    }
}

/**
 * 3,rela_parenti03 - parenti03_interno
 * 9,opetick01_fecha
 * 10,rela_parenti18 - parenti18_codigo
 * 14,rela_parenti09 - parenti09_legajo
 * 19,rela_parenti19 - parenti19_codigo (obra, todavia no exite)
 * 75,opetick01_hora
 * 82,opetick01_modificable (si es 1, el ticket puede recibir cambios)
 * 
 */


