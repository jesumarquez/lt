using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Logictracker.Process.Import.Client.Types;

namespace Logictracker.Process.Import.Client.DataStrategy
{
    public class FileDataStrategy: IDataStrategy
    {
        protected Dictionary<string, string> Parameters;
        protected string Folder { get; set;}
        protected string FileName { get; set; }
        protected bool HeaderRow { get; set; }
        protected string EncodingName { get; set; }
        protected Dictionary<string,string> BackupFileNames { get; set; }

        public FileDataStrategy(params IDataSourceParameter[] parameters)
        {
            Parameters = parameters.ToDictionary(k => k.Name.ToLower(), v => v.Value);
            if (!Parameters.ContainsKey("folder")) throw new ApplicationException("No se encontro el parametro folder");
            if (!Parameters.ContainsKey("filename")) throw new ApplicationException("No se encontro el parametro filename");
            Folder = Parameters["folder"].Trim();
            FileName = Parameters["filename"].Trim();
            
            HeaderRow = !Parameters.ContainsKey("hasheaderrow") || Parameters["hasheaderrow"] == "1" || Parameters["hasheaderrow"] == "true";
            EncodingName = Parameters.ContainsKey("encoding") ? Parameters["encoding"].Trim() : "utf-8";

            BackupFileNames = new Dictionary<string, string>();
        }

        #region Implementation of IDataStrategy

        public Table GetNewData()
        {
            if(!Directory.Exists(Folder)) throw new ApplicationException(string.Format("El directorio {0} no existe", Folder));
            var files = Directory.GetFiles(Folder, FileName);

            if(files.Length == 0) return new Table();

            Table table = null;

            foreach (var fileTable in files.Select(file => ParseFile(file)))
            {
                if (table == null) table = fileTable;
                else table.Merge(fileTable);
            }

            BackupFileNames.Clear();

            var backupFolder = Path.Combine(Path.Combine(Folder, "Backup"), DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(backupFolder)) Directory.CreateDirectory(backupFolder);
            foreach (var file in files)
            {
                var newFileName = Path.Combine(backupFolder, Path.GetFileName(file));
                var count = 2;
                while(File.Exists(newFileName))
                {
                    var counter = string.Concat("(", count++, ")");
                    newFileName = Path.Combine(backupFolder, Path.GetFileNameWithoutExtension(file) + counter + Path.GetExtension(file));
                }

                BackupFileNames.Add(file, newFileName);
                File.Move(file, newFileName);
                Logger.Info("Archivo procesado. " + file);
            }
            
            return table;
        }

        public void Revert()
        {
            foreach(var oldFile in BackupFileNames.Keys)
            {
                File.Move(BackupFileNames[oldFile], oldFile);
                Logger.Info("Revert: " + oldFile);
            }
            BackupFileNames.Clear();
        }

        #endregion

        protected Table ParseFile(string file)
        {
            Table table;
            var csv = new StreamReader(file, Encoding.GetEncoding(EncodingName));

            if (HeaderRow)
            {
                var header = csv.ReadLine();
                var headerCells = Split(header);
                if (headerCells.Length == 1 && headerCells[0].Trim() == string.Empty) return null; //linea en blanco
                table = new Table(true, headerCells);
            }
            else
            {
                table = new Table();
            }

            while(!csv.EndOfStream)
            {
                var record = ParseLine(csv.ReadLine(), table);
                if(record != null) table.Rows.Add(record);
            }
            csv.Close();
            return table;
        }

        protected virtual Record ParseLine(string line, Table table)
        {
            try
            {
                var cells = Split(line);
                if (cells.Length == 1 && cells[0].Trim() == string.Empty) return null; //linea en blanco
                RecordRead(cells);
                
                return new Record(table, cells);
            }
            catch
            {
                Logger.Error("Error parseando registro: " + line);
                return null;
            }
        }

        public virtual void RecordRead(string[] cells) { }

        public virtual string[] Split(string line)
        {
            return new[] {line};
        }
    }
}
