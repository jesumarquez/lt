#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.QuadTree.Data
{
    public class TransactionLog
    {
        private readonly object syncLock = new object();
        private readonly Repository Repository;
        private DateTime LastLoadTimestamp;
        private readonly Dictionary<string, Dictionary<int, int>> matrix = new Dictionary<string, Dictionary<int, int>>();

	    private string FileName { get { return String.Format("{0}\\transaction.log", Repository.BaseFolderPath); } }

        public TransactionLog(Repository Repository, int DeviceId)
        {
            //State = States.WF_INIT;
            this.Repository = Repository;
            LastLoadTimestamp = DateTime.MinValue;
            Load(DeviceId);
        }

        private void Load(int DeviceId)
        {
			STrace.Debug(GetType().FullName, DeviceId, String.Format("Loading Qtree: {0}", Repository.BaseFolderPath));
			lock (syncLock)
            {
                //State = States.LOADING;
                matrix.Clear();
                if (!File.Exists(FileName))
                {
                    STrace.Debug(GetType().FullName, "QTREE: Creando transaction log.");
                    using (var sw = File.CreateText(FileName))
                    {
                        sw.WriteLine("GR2-1.1:REV=000000000");
                        sw.Close();
                    }
                    //State = States.READY;
                    Repository.Revision = 0;
                    return;
                }

                StreamReader infile;
                try
                {
                    
                    var current_timestamp = File.GetLastWriteTime(FileName);
                    if (current_timestamp <= LastLoadTimestamp)
                    {
                        STrace.Debug(GetType().FullName, "QTREE: transacion log esta actualizado.");
                        //State = States.READY;
                        return;
                    }
                    infile = File.OpenText(FileName);
                    LastLoadTimestamp = current_timestamp;
                }
                catch (Exception e)
                {
                    STrace.Exception(GetType().FullName,e);
                    //State = States.BAD;
                    return;
                }
                var header = infile.ReadLine();
                //var bytes = 0;
                //var c = 0;
                if (header != "GR2-1.1:REV=000000000")
                {
                    //State = States.BAD;
                    throw new Exception("TRLOG/LOAD: el transaction.log no tiene la firma valida.");
                }
                while (!infile.EndOfStream)
                {
                    var line = infile.ReadLine();
                    if (line == null) continue;
                    //bytes += line.Length;
                    /*if (c % 10000 == 0)
                    {
                        STrace.Debug(GetType().FullName, "TRLOG/LOAD: c={0} progreso={1:0.0}% bytes={2} length={3}", c, (bytes * 100.0 / infileinfo.Length), bytes, infileinfo.Length);
                    }//*/
                    //c++;
                    var fields = line.Split(":".ToCharArray());
                    if (fields[1] != "SET-M2") continue;
                    var file = fields[2].Substring(5);
                    var sector = Convert.ToInt32(fields[3].Substring(7));
                    var rev = Convert.ToInt32(fields[4].Substring(4));
                    ProcessTransaccionLogLine(file, sector, rev);
                }
                infile.Dispose();
            }
            STrace.Debug(GetType().FullName, DeviceId, "QTREE Cargado satisfactoriamente.");
            //State = States.READY;
            
        }

        public void SectorTouch(string file, int sector)
        {
            /*if (revisionCounter++ > 16) {
                revisionCounter = 0;*/
                Repository.Revision++;
            //}
            using (var f = File.AppendText(FileName))
            {
                f.WriteLine("T:SET-M2:FILE={0}:SECTOR={1}:REV={2:D9}", Path.GetFileName(file), sector, Repository.Revision);
                f.Close();
            }
        }

        void ProcessTransaccionLogLine(string file, int sector, int rev)
        {
            if (Repository.Revision < rev) Repository.Revision = rev;

			//ya esta lockeado afuera
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
                matrix.Add(file, new Dictionary<int, int>());
                matrix[file].Add(sector, rev);
            }
        }

	    private Queue<QtreeAction> GetActionsQueue(int base_revision)
		{
			STrace.Debug(GetType().FullName, String.Format("QTREE/GETACTIONS: desde la revision={0}", base_revision));
			var actions = new List<QtreeAction>();
			foreach (var filename in matrix.Keys)
			{
				const int file_class = 0; //GetGR2DefaultClass(filename);
				STrace.Debug(GetType().FullName, String.Format("QTREE/FILE[{0}]: clase por defecto={1}", filename, file_class));
				if (!matrix[filename].ContainsKey(5000))
					matrix[filename].Add(5000, file_class);
				var max_file_revision = 0;
				var sent_file_sectors = 0;
				foreach (var sector in matrix[filename].Keys)
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
			foreach (var action in actions)
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
						Sector = last_action.Revision,
						Revision = last_action.Revision
					});
				}
				last_action = action;
				result.Enqueue(action);
			}
			// seteamos la version del qtree para evitar las locuras
			result.Enqueue(new QtreeAction
			{
				Action = QtreeAction.Actions.UPDATE_REVISION,
				Sector = Repository.Revision
			});

			return result;
		}

		public void Shrink()
        {
            var newFileName = FileName + ".new";
            var oldFileName = FileName + ".old";

            if (File.Exists(newFileName)) File.Delete(newFileName);
            if (File.Exists(oldFileName)) File.Delete(oldFileName);

            using (var newlog = File.CreateText(newFileName))
            {
                // cabecera
                newlog.WriteLine("GR2-1.1:REV=000000000");
                foreach (var action in GetActionsQueue(0))
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
            
            File.Move(FileName, oldFileName);
            File.Move(newFileName, FileName);
        }

    	public Dictionary<int, List<int>> GetChangedSectorsAndRevision(int base_revision, out int revision)
		{
			lock (syncLock)
			{
				//TODO: el resultado del parseo del transaction.log debe ser persistido junto con el timestamp del archivo, cuando se vuelve a recibir un enviar [full]qtree a menos que el timestamp halla cambiado utilizar el parseo ya persistido.
				var changedSectors = new Dictionary<int, List<int>>();
				var max_file_revision = 0;
				foreach (var filename in matrix.Keys)
				{
					var filenameint = Convert.ToInt32(filename.Split('-')[1].Split('.')[0]);
					foreach (var sector in matrix[filename].Keys)
					{
						var rev = matrix[filename][sector];
						if (rev > max_file_revision)
						{
							max_file_revision = rev;
						}
						if (rev <= base_revision) continue;
						if (!changedSectors.ContainsKey(filenameint))
							changedSectors.Add(filenameint, new List<int>());
						if (!changedSectors[filenameint].Contains(sector))
							changedSectors[filenameint].Add(sector);
					}
				}
				revision = max_file_revision;
				return changedSectors;
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
                if (filename.EndsWith(".DAT")) return;
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

	    private short FileLat { get; set; }

	    private short FileLon { get; set; }

        public int Sector { get; set; }

        public int Revision { get; set; }

        public int CompareTo(object obj)
        {
            var src = obj as QtreeAction;
            if (src != null) return Revision - src.Revision;
            throw new ArgumentException("El argumento no es del tipo QtreeAction.");
        }
    }
}
