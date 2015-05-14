#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Urbetrack.Comm.Core.Codecs;
using Urbetrack.Compresion;
using Urbetrack.DatabaseTracer.Core;
using Urbetrack.Hacking;

#endregion

namespace Urbetrack.Comm.Core.Qtree
{
    public class Qtree
    {
        public class QtreeM2
        {
            public int Revision { get; set; }
            public int Sector { get; set; }

            private string filename;
            public string Filename
            {
                get { return filename; }
                set
                {
                    filename = value;
                    var paso1 = filename.Split(".".ToCharArray());
                    var paso2 = paso1[0].Split("-+".ToCharArray());
                    FileLat = Convert.ToInt16(paso2[1]);
                    FileLon = Convert.ToInt16(paso2[2]);
                    if (paso1[0][2] == '-')
                        FileLat *= -1;
                    if (paso1[0].Substring(3).Contains("-"))
                        FileLon *= -1;
                }
            }

            public short FileLat { get; set; }

            public short FileLon { get; set; }

            public string Key { 
                get {
                    return String.Format("{0}-{1}", Filename, Sector);
                } 
            }
        }

        public class QtreeAction : IComparable
        {
            public enum Actions
            {
                START_UPDATE,
                ADD_GR2,
                SET_GR2_DEFAULTS,
                SET_M2,
                UPDATE_REVISION
            };

            public Actions Action { get; set; }
            private string filename;
            public string Filename
            {
                get { return filename; }
                set
                {
                    filename = value;
                    var paso1 = filename.Split(".".ToCharArray());
                    var paso2 = paso1[0].Split("-".ToCharArray());
                    FileLat = Convert.ToInt16(paso2[1]);
                    FileLon = Convert.ToInt16(paso2[2]);
                    if (paso1[0][2] == '-')
                        FileLat *= -1;
                    if (paso1[0].Substring(3).Contains("-"))
                        FileLon *= -1;
                }
            }

            public short FileLat { get; set; }

            public short FileLon { get; set; }

            public int Sector { get; set; }

            public int Revision { get; set; }
            
            public int CompareTo(object obj)
            {
                var src = obj as QtreeAction;
                if (src != null) return Revision - src.Revision;
                throw new ArgumentException("El argumento no es del tipo QtreeAction.");
            }
        }

        internal string RepoDir { get; set; }
        private DateTime last_timestamp;
        internal readonly Dictionary<int, QtreeRevision> tree = new Dictionary<int, QtreeRevision>();
        internal readonly Dictionary<string, QtreeM2> sectors = new Dictionary<string, QtreeM2>();
        
        internal static int ParseRev(string str)
        {
            if (!str.StartsWith("REV=")) throw new Exception("QTREE: no se puede parsear la revision.");
            return Convert.ToInt32(str.Substring(4));
        }
        private readonly object tree_lock = new object();
        public void Initialize(string repoDir)
        {
            RepoDir = repoDir;
            if (!Directory.Exists(RepoDir))
            {
                STrace.Debug(GetType().FullName,"QTREE: Creando directorio del repositorio.");
                Directory.CreateDirectory(RepoDir);
            }
            if (!File.Exists(String.Format("{0}\\transaction.log", RepoDir)))
            {
                STrace.Debug(GetType().FullName,"QTREE: Creando transaction log.");
                using (var sw = File.CreateText(String.Format("{0}\\transaction.log", RepoDir)))
                {
                    sw.WriteLine("GR2-1.1:REV=000000000");    
                    sw.Close();
                }
            }
            last_timestamp = DateTime.MinValue;
            LoadEx();
        }

        static public void Upgrade2(string RepoDir)
        {
            var infile = File.OpenText(String.Format("{0}\\transaction.log", RepoDir));
            var outfile = File.CreateText(String.Format("{0}\\transaction.new", RepoDir));
            /*
              GR2-1.1:REV=000000000
              T:ADD-GR2:M2-44-65.GR2:REV=000000001
              T:SET-M2:FILE=M2-44-65.GR2:SECTOR=2862:REV=000000001
              T:SET-M2:FILE=M2-44-65.GR2:SECTOR=2922:REV=000000001
              T:SET-M2:FILE=M2-44-65.GR2:SECTOR=2862:REV=000000001
             */
            var header = infile.ReadLine();
            if (header != "GR2-1.1:REV=000000000") return;
            outfile.WriteLine("GR2-1.2:R=000000000");
            while (!infile.EndOfStream)
            {
            	var line = infile.ReadLine();
				Debug.Assert(line != null);
                var parts = line.Split(":".ToCharArray());
                if (parts[1] == "ADD-GR2")
                {
                    var arevs = parts[3].Split("=".ToCharArray());
                    outfile.WriteLine(":A:{0}:R={1}",parts[2],arevs[1]);
                    continue;
                }
                var fileparts = parts[2].Split("=".ToCharArray());
                var sector = parts[3].Split("=".ToCharArray());
                var revs = parts[4].Split("=".ToCharArray());
                outfile.WriteLine(":S:{0}:{1}:R={2}", fileparts[1], sector[1], revs[1]);
            }
            outfile.Close();
            
        }
        
        static public void Upgrade(string RepoDir)
        {
            var lines = File.ReadAllLines(String.Format("{0}\\transaction.log", RepoDir));
            if (lines[0] != "GR2-1.0:REV=000000") 
                return;
            File.Move(String.Format("{0}\\transaction.log", RepoDir), String.Format("{0}\\transaction.old", RepoDir));
            lines = File.ReadAllLines(String.Format("{0}\\transaction.old", RepoDir));
            var newlog = File.CreateText(String.Format("{0}\\transaction.log", RepoDir));
            var transaction = 0;
            var rev = 1;
            foreach (var line in lines)
            {
                var fields = line.Split(":".ToCharArray());
                if (fields[0] == "GR2-1.0")
                {
                    newlog.WriteLine("GR2-1.1:REV=000000000");
                    continue;
                }
                if (transaction++ >= 16)
                {
                    transaction = 0;
                    rev++;
                }

                switch (fields[1])
                {
                    case "ADD-GR2":
                        {
                            var file = fields[2];
                            newlog.WriteLine("T:ADD-GR2:{0}:REV={1:D9}",file,rev);
                        }
                        break;
                    case "SET-M2":
                        {
                            newlog.WriteLine("T:SET-M2:{0}:{1}:REV={2:D9}", fields[2], fields[3], rev);
                        }
                        break;
                    default:
                        Debug.Assert(true, "TIPO QTREE DESCONOCIDO.");
                        break;
                }
            }
            newlog.Flush();
            newlog.Close();
        }

        private readonly Dictionary<string, Dictionary<int, int>> matrix = new Dictionary<string, Dictionary<int, int>>();

        
        public byte GetGR2DefaultClass(string filename)
        {
            var ds = File.OpenRead(string.Format("{0}\\{1}", RepoDir, filename));
            var buffer = new byte[512];
            var ascii_table = new int[256];
            for (var i = 0; i < 256; ++i)
            {
                ascii_table[i] = 0;
            }

            for (var sector = 0; sector < 3600; ++sector)
            {
                long offset = (sector * 512) + 512;
                ds.Seek(offset, SeekOrigin.Begin);
                ds.Read(buffer, 0, 512);
                foreach (var b in buffer)
                {
                    ascii_table[b]++;
                }
            }

            var max_repeated = 0;
            byte byte_repeated = 0;
            for (var i = 0; i < byte.MaxValue; ++i)
            {
                if (ascii_table[i] <= max_repeated) continue;
                max_repeated = ascii_table[i];
                byte_repeated = (byte) i;
            }
            return byte_repeated;
        }

        public Queue<QtreeAction> GetActionsQueue(int base_revision)
        {
            STrace.Debug(GetType().FullName, String.Format("QTREE/GETACTIONS: desde la revision={0}", base_revision));
            var actions = new List<QtreeAction>();
            foreach(var filename in matrix.Keys)
            {
                var file_class = GetGR2DefaultClass(filename);
                STrace.Debug(GetType().FullName, String.Format("QTREE/FILE[{0}]: clase por defecto={1}",filename, file_class));
                if (!matrix[filename].ContainsKey(5000))
                    matrix[filename].Add(5000, file_class);
                var max_file_revision = 0;
                var sent_file_sectors = 0;
                foreach(var sector in matrix[filename].Keys)
                {
                    if (sector == 5000 && sent_file_sectors > 0)
                    {
                        actions.Add(new QtreeAction
                                        {
                                            Action = QtreeAction.Actions.SET_GR2_DEFAULTS,
                                            Filename = filename,
                                            Sector = file_class,
                                            Revision = max_file_revision
                                        });
                        continue;
                    }
                    var rev = matrix[filename][sector];
                    if (rev > max_file_revision)
                    {
                        max_file_revision = rev; // almaceno el numero maximo de revision.
                    }
                    if (rev <= base_revision) continue; // salto lo ya procesado.
                    sent_file_sectors++;
                    var action = new QtreeAction
                                     {
                                         Action = QtreeAction.Actions.SET_M2,
                                         Filename = filename,
                                         Sector = sector,
                                         Revision = rev
                                     };
                    actions.Add(action);
                }
            }
            actions.Sort();
            var result = new Queue<QtreeAction>();
            result.Enqueue(new QtreeAction
                               {
                                   Action = QtreeAction.Actions.START_UPDATE,
                                   Sector = (base_revision == 0 ? 0x12345678 : 0x55AA5A5A)
                               }
                );

            QtreeAction last_action = null;
            foreach(var action in actions)
            {
                if (last_action == null)
                {
                    result.Enqueue(action);
                    last_action = action;
                    continue;
                }

                if (last_action.Revision != action.Revision && 
                    last_action.Action != QtreeAction.Actions.UPDATE_REVISION)
                {
                    result.Enqueue(new QtreeAction
                                       {
                                           Action = QtreeAction.Actions.UPDATE_REVISION,
                                           Sector = last_action.Revision
                                       });
                } 
                last_action = action;
                result.Enqueue(action);
            }
            // seteamos la version del qtree para evitar las locuras
            result.Enqueue(new QtreeAction
                               {
                                   Action = QtreeAction.Actions.UPDATE_REVISION,
                                   Sector = Revision
                               });

            return result;
        }

        void ProcessTransaccionLogLine(string file, int sector, int rev)
        {
            if (Revision < rev) Revision = rev;

            if (matrix.ContainsKey(file))
            {
                if (matrix[file].ContainsKey(sector))
                {
                    matrix[file][sector] = rev;
                }
                else
                {
                    matrix[file].Add(sector, rev);
                }
            }
            else
            {
                matrix.Add(file, new Dictionary<int,int>());
                matrix[file].Add(sector, rev);
            }
        }

        public int Revision { get; set; }

        public void DumpAsTransactionLog(Queue<QtreeAction> data, string filename)
        {
            var newlog = File.CreateText(String.Format("{0}\\{1}", RepoDir, filename));
            // cabecera
            newlog.WriteLine("GR2-1.1:REV=000000000");
            foreach (var action in data)
            {
                switch (action.Action)
                {
                    case QtreeAction.Actions.SET_M2:
                        newlog.WriteLine("T:SET-M2:FILE={0}:SECTOR={1}:REV={2:D9}", action.Filename, action.Sector, action.Revision);
                        break;
                }
            }
            newlog.Flush();
            newlog.Close();

        }

        internal void LoadEx()
        {
            lock (tree_lock)
            {
                StreamReader infile;
                FileInfo infileinfo;
                try
                {
                    var filename = String.Format("{0}\\transaction.log", RepoDir);
                    var current_timestamp = File.GetLastWriteTime(filename);
                    if (current_timestamp <= last_timestamp)
                    {
                        STrace.Debug(GetType().FullName,"QTREE: transacion log esta actualizado.");
                        return;
                    }
                    infile = File.OpenText(filename);
                    infileinfo = new FileInfo(filename);
                    last_timestamp = current_timestamp;
                }
                catch (Exception e)
                {
                    STrace.Exception(GetType().FullName,e);
                    return;
                }
                tree.Clear();
                sectors.Clear();
                var header = infile.ReadLine();
                var bytes = 0;
                var c = 0;
                if (header != "GR2-1.1:REV=000000000") throw new Exception("QTREE: el transaction.log no tiene la firma valida.");
                while (!infile.EndOfStream)
                {
                    var line = infile.ReadLine();
					Debug.Assert(line != null);
                    bytes += line.Length;
                    if (c % 10000 == 0)
                    {
                        STrace.Debug(GetType().FullName, String.Format("QTREE/LOAD: c={0} progreso={1:0.0}% bytes={2} length={3}", c, (bytes * 100.0 / infileinfo.Length), bytes, infileinfo.Length));
                    }
                    c++;
                    var fields = line.Split(":".ToCharArray());
                    if (fields[1] != "SET-M2") continue;
                    var file = fields[2].Substring(5);
                    var sector = Convert.ToInt32(fields[3].Substring(7));
                    var rev = Convert.ToInt32(fields[4].Substring(4));
                    ProcessTransaccionLogLine(file, sector, rev);
                }
            }
            STrace.Debug(GetType().FullName,"QTREE: Cargado satisfactoriamente.");
        }
     
        internal int GetLastRev()
        {
            lock (tree_lock)
            {
                var last_rev = 0;
                foreach (var c in tree.Keys)
                {
                    if (c > last_rev) last_rev = c;
                }
                return last_rev;
            }
        }


        public byte[] GetSector(string name, int sector, ref byte repeated_byte)
        {
            var ds = File.OpenRead(string.Format("{0}\\{1}", RepoDir, name));
            long offset = (sector * 512) + 512;
            ds.Seek(offset, SeekOrigin.Begin);
            var buffer = new byte[512];
            ds.Read(buffer, 0, 512);
            repeated_byte = buffer[5];
            var counter = 0;
            for(var i=5; i < 455; ++i)
                if (buffer[i] == repeated_byte) counter++;
            if (counter != 450)
                repeated_byte = 0;
            return buffer;
        }

        //public Queue<byte[]> GetQTreeCompressed(int frame_limit, int from_revision)

        public Queue<byte[]> GetQTreeCompressed(int frame_limit, int from_revision)
        {
            frame_limit--; // reservo el espacio para el 0xAC (cierre)
            var result = new Queue<byte[]>();
            var dataset = GetActionsQueue(from_revision);
            var data_buffer = new byte[frame_limit+1];
            var db_cursor = 0;
            var pagina = 0;
            var huffman_sectors = 0;
            var rle_sectors = 0;
            var uncompresed_sectors = 0;
            var total_sectors = 0;
            const int adds = 0;
            byte operaciones = 0;
            var operaciones_en_pagina = 0;
            var source_size = 0;
            // preparo el espacio pata operaciones.
            UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0);
            var total_de_pasos = dataset.Count;
            var pasos = 0;
            foreach (var action in dataset)
            {
                pasos++;
                var completado = pasos * 100.0 / total_de_pasos;
                if (pasos % 5000 == 0)
                {
                    STrace.Debug(GetType().FullName, String.Format("QTREE/GETQTREECOMPRESSED: {0:0.0}% completado.", completado));
                }
                if (operaciones_en_pagina > Hacker.QTree.OperacionesPorPagina)
                {
                    UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0xAC); // Cierre del bloque
                    source_size++;
                    operaciones++;
                    operaciones_en_pagina = 0;
                    pagina++;
                    //Marshall.User("QTREE: un OPERACIONXPAGINA cerro msg {0} de {1} bytes y {2} operaciones", pagina, db_cursor, operaciones);

                    var transient = new byte[db_cursor];
                    var dummy = 0;
                    Array.Copy(data_buffer, transient, db_cursor);
                    // Actualizo las operaciones en el bloque.
                    UrbetrackCodec.EncodeByte(ref transient, ref dummy, operaciones);
                    result.Enqueue(transient);
                    db_cursor = 0;
                    operaciones = 0;
                    // preparo el espacio pata operaciones.
                    UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0);
    
                }
                switch (action.Action)
                {
                    case QtreeAction.Actions.START_UPDATE:
                        //Marshall.User(String.Format("QTREE:MSG={0}/START={1}/SIZE={2}: FULL FORMAT", pagina, db_cursor, 5));
                        source_size += 5;
                        operaciones_en_pagina++;
                        // 0xFF es Full Format.
                        UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0xFF);
                        // Patron de Seguridad.
                        UrbetrackCodec.EncodeInteger(ref data_buffer, ref db_cursor, action.Sector);
                        operaciones++;
                        break;
                    case QtreeAction.Actions.SET_GR2_DEFAULTS:
                        if (db_cursor + 8 > frame_limit)
                        {
                            //Marshall.User(String.Format("QTREE:MSG={0}/START={1}/SIZE={2}: CLOSE PAGE", pagina, db_cursor, 1));
                            UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0xAC); // Cierre del bloque
                            source_size++;
                            operaciones++;
                            operaciones_en_pagina = 0;
                            pagina++;
                            //Marshall.User("QTREE: un UPDATE_REVISION cerro msg {0} de {1} bytes y {2} operaciones", pagina, db_cursor, operaciones);
                            var transient = new byte[db_cursor];
                            var dummy = 0;
                            Array.Copy(data_buffer, transient, db_cursor);
                            // Actualizo las operaciones en el bloque.
                            UrbetrackCodec.EncodeByte(ref transient, ref dummy, operaciones);
                            result.Enqueue(transient);
                            db_cursor = 0;
                            operaciones = 0;
                            // preparo el espacio pata operaciones.
                            UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0);
                        }
                        // 0xDF es DeFaults
                        UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0xDF);
                        UrbetrackCodec.EncodeShort(ref data_buffer, ref db_cursor, action.FileLat);
                        UrbetrackCodec.EncodeShort(ref data_buffer, ref db_cursor, action.FileLon);
                        UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, (byte) action.Sector);
                        operaciones++;
                        break;
                    case QtreeAction.Actions.SET_M2:
                        {
                            byte repeated_byte = 0;
                            var sector = GetSector(action.Filename, action.Sector, ref repeated_byte);
                            var file_class = (byte) matrix[action.Filename][5000];
                            if (repeated_byte != 0 && repeated_byte == file_class)
                            {
                                STrace.Debug(GetType().FullName, String.Format("QTREE/FILE[{0}]: saltando sector x defecto {1} class={2}", action.Filename,
                                        action.Sector, file_class));
                                continue;
                            }
                            source_size += 512;
                            var page_buffer = new byte[512];
                            var page_cursor = 0;

                            UrbetrackCodec.EncodeByte(ref page_buffer, ref page_cursor, 0x01);
                            UrbetrackCodec.EncodeShort(ref page_buffer, ref page_cursor, action.FileLat);
                            UrbetrackCodec.EncodeShort(ref page_buffer, ref page_cursor, action.FileLon);
                            UrbetrackCodec.EncodeInteger(ref page_buffer, ref page_cursor, action.Sector);
                            

                            var huff = new Huffman();
                            var result_huffman = huff.Compress(sector, 0, 512);
                            var rle = new RLE();
                            var result_rle = rle.Compress(sector, 0, 512);
                            byte[] compressed_data;

                            if (sector.GetLength(0) <= result_huffman.GetLength(0) && sector.GetLength(0) <= result_rle.GetLength(0))
                            {
                                uncompresed_sectors++;
                                compressed_data = sector;
                                UrbetrackCodec.EncodeShort(ref page_buffer, ref page_cursor, (short)compressed_data.GetLength(0));
                                UrbetrackCodec.EncodeByte(ref page_buffer, ref page_cursor, 0x00); // indicador de algoritmo (Sin compresion)
                            } else
                                if (result_huffman.GetLength(0) <= sector.GetLength(0) && result_huffman.GetLength(0) <= result_rle.GetLength(0))
                                {
                                    huffman_sectors++;
                                    compressed_data = result_huffman;
                                    UrbetrackCodec.EncodeShort(ref page_buffer, ref page_cursor, (short)compressed_data.GetLength(0));
                                    UrbetrackCodec.EncodeByte(ref page_buffer, ref page_cursor, 0x02); // indicador de algoritmo (Huffman)
                                }
                                else
                                    if (result_rle.GetLength(0) <= result_huffman.GetLength(0) && result_rle.GetLength(0) <= sector.GetLength(0))
                                    {
                                        rle_sectors++;
                                        compressed_data = result_rle;
                                        UrbetrackCodec.EncodeShort(ref page_buffer, ref page_cursor, (short)compressed_data.GetLength(0));
                                        UrbetrackCodec.EncodeByte(ref page_buffer, ref page_cursor, 0x01); // indicador de algoritmo (RLE)
                                    }
                                    else
                                    {
                                        STrace.Debug(GetType().FullName,"QTREE: imposible elijir algoritmo.");
                                        return null;
                                    }
                            // agrego el buffer de datos...
                            UrbetrackCodec.EncodeBytes(ref page_buffer, ref page_cursor, compressed_data);
                            // valido si es posible meter el buffer en una sola pagina al menos.
                            if (page_cursor > frame_limit)
                            {
                                STrace.Debug(GetType().FullName,"QTREE: el resultado de una pagina no entra en el buffer completo.");
                                STrace.Debug(GetType().FullName,"QTREE: el resultado de una pagina no entra en el buffer completo.");
                                return null;
                            }
                            // ahora tengo que validar si me entra en esta pagina, o la cierro y empiezo otra.
                            if (db_cursor + page_cursor > frame_limit)
                            {
                                //Marshall.User(String.Format("QTREE:MSG={0}/START={1}/SIZE={2}: CLOSE PAGE", pagina, db_cursor, 1));
                                UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0xAC); // Cierre del bloque
                                source_size++;
                                operaciones++;
                                operaciones_en_pagina = 0;
                                pagina++;
                                //Marshall.User("QTREE: un SET_M2 cerro msg {0} de {1} bytes y {2} operaciones", pagina, db_cursor, operaciones);
                                
                                var transient = new byte[db_cursor];
                                var dummy = 0;
                                Array.Copy(data_buffer, transient, db_cursor);
                                // Actualizo las operaciones en el bloque.
                                UrbetrackCodec.EncodeByte(ref transient, ref dummy, operaciones);
                                result.Enqueue(transient);
                                db_cursor = 0;
                                operaciones = 0;
                                // preparo el espacio pata operaciones.
                                UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0);
                            }
                            Array.Copy(page_buffer, 0, data_buffer, db_cursor, page_cursor);
                            //Marshall.User(String.Format("QTREE:MSG={0}/START={1}/SIZE={2}: SET {3};{4} PAGE:{5}", pagina, db_cursor, page_cursor, action.FileLat, action.FileLon, action.Sector));
                            db_cursor += page_cursor;
                            total_sectors++;
                            operaciones++;
                            operaciones_en_pagina ++;
                        }
                        break;
                    case QtreeAction.Actions.UPDATE_REVISION:
                        if (db_cursor + 5 > frame_limit)
                        {
                            //Marshall.User(String.Format("QTREE:MSG={0}/START={1}/SIZE={2}: CLOSE PAGE", pagina, db_cursor, 1));
                            UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0xAC); // Cierre del bloque
                            source_size++;
                            operaciones++;
                            operaciones_en_pagina = 0;
                            pagina++;
                            //Marshall.User("QTREE: un UPDATE_REVISION cerro msg {0} de {1} bytes y {2} operaciones", pagina, db_cursor, operaciones);
                            var transient = new byte[db_cursor];
                            var dummy = 0;
                            Array.Copy(data_buffer, transient, db_cursor);
                            // Actualizo las operaciones en el bloque.
                            UrbetrackCodec.EncodeByte(ref transient, ref dummy, operaciones);
                            result.Enqueue(transient);
                            db_cursor = 0;
                            operaciones = 0;
                            // preparo el espacio pata operaciones.
                            UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0);
                        }
                        //Marshall.User(String.Format("QTREE:MSG={0}/START={1}/SIZE={2}: UPDATE REVISION {3}", pagina, db_cursor, 5, action.Sector));
                        UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0x02);
                        UrbetrackCodec.EncodeInteger(ref data_buffer, ref db_cursor, action.Sector);
                        source_size+=5;
                        operaciones++;
                        operaciones_en_pagina++;
                        break;
                }
            }

            {
                //Marshall.User(String.Format("QTREE:MSG={0}/START={1}/SIZE={2}: UPDATE FINISH", pagina, db_cursor, 1));
                UrbetrackCodec.EncodeByte(ref data_buffer, ref db_cursor, 0xDC); // Fin de la actualizacion.
                var transient = new byte[db_cursor];
                var dummy = 0;
                operaciones++;
                source_size += 1;
                //Marshall.User("QTREE: cerro la ultima pagina {0} de {1} bytes y {2} operaciones", pagina, db_cursor, operaciones);
                Array.Copy(data_buffer, transient, db_cursor);
                // Actualizo las operaciones en el bloque.
                UrbetrackCodec.EncodeByte(ref transient, ref dummy, operaciones);
                result.Enqueue(transient);
            }
        	if (pagina == 0)
            {
                STrace.Debug(GetType().FullName, String.Format("QRTEE NO SE REQUIERE UPDATE revision {0}", from_revision));
                return null;
            }
        	var suma_total = result.Sum(tmp => tmp.GetLength(0));
        	STrace.Debug(GetType().FullName, String.Format("QRTEE COMPRIMIDO entre revisiones {0} y {1}", from_revision, Revision));
            STrace.Debug(GetType().FullName,"===================================================================================");
            STrace.Debug(GetType().FullName, String.Format("  Tamaño Origninal:   {0} bytes en {1} paginas.", source_size, total_sectors));
            STrace.Debug(GetType().FullName, String.Format("  Tamaño Final:       {0} bytes en {1} paginas. ({2} bytes pormedio por pagina)", suma_total, pagina, (pagina > 0 ? suma_total / pagina : 0)));
            STrace.Debug(GetType().FullName, String.Format("  Tasa de Compresion: {0}%", suma_total * 100 / source_size));
            STrace.Debug(GetType().FullName, String.Format("  Paginas RAW:        {0} ({1}%)", uncompresed_sectors, uncompresed_sectors * 100 / total_sectors));
            STrace.Debug(GetType().FullName, String.Format("  Paginas RLE:        {0} ({1}%)", rle_sectors, rle_sectors * 100 / total_sectors));
            STrace.Debug(GetType().FullName, String.Format("  Paginas HUFFMAN:    {0} ({1}%)", huffman_sectors, huffman_sectors * 100 / total_sectors));
            STrace.Debug(GetType().FullName, String.Format("  GR2 Agreagados:     {0}", adds));

            return result;
        }

        public void TestUnique(Queue<QtreeAction> sumario)
        {
            var uniq = new Dictionary<string, QtreeAction>();
            foreach (var action in sumario)
            {
                switch (action.Action)
                {
                    case QtreeAction.Actions.SET_M2:
                        var key = String.Format("T:SET-M2:FILE={0}:SECTOR={1}:REV={2:D9}", action.Filename, action.Sector, action.Revision);
                        uniq.Add(key, action);
                        break;
                }
            }
            uniq.Clear();
            foreach (var action in sumario)
            {
                switch (action.Action)
                {
                    case QtreeAction.Actions.SET_M2:
                        var key = String.Format("T:SET-M2:FILE={0}:SECTOR={1}", action.Filename, action.Sector);
                        uniq.Add(key, action);
                        break;
                }
            }
        }
    }
}