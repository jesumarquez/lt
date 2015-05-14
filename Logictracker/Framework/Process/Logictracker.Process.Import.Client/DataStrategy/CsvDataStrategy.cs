using System.Collections.Generic;

namespace Logictracker.Process.Import.Client.DataStrategy
{
    public class CsvDataStrategy: FileDataStrategy
    {
        protected string Separator { get; set; }
        protected string Qualifier { get; set; }

        public CsvDataStrategy(params IDataSourceParameter[] parameters)
            :base(parameters)
        {
            Qualifier = Parameters.ContainsKey("textqualifier") ? Parameters["textqualifier"].Trim() : "\"";
            Separator = Parameters.ContainsKey("separator") ? Parameters["separator"].Trim() : ";";
            if (Separator == "\\t") 
                Separator = "\t";

            Logger.Info("CsvDataStrategy iniciado (Folder: " + Folder + " | FileName: " + FileName + " | HeaderRow: " + HeaderRow + " | Separator: " + Separator + " | TextQualifier: " + Qualifier);
        }

        public override void RecordRead(string[] cells)
        {
            for (var i = 0; i < cells.Length; i++)
            {
                var cell = cells[i];
                if (cell.Length > 1 && cell[0] == '"' && cell[cell.Length - 1] == '"')
                    cells[i] = cell.Substring(1, cell.Length - 2);
            }
        }

        public override string[] Split(string line)
        {
            // 0: starting col
            // 1: open col
            // 2: not open col
            // 3: closed col
            var state = 0;
            var list = new List<string>();
            var text = "";
            
            if (Separator.Equals("|") && line.Contains("\"")) line = line.Replace("\"", string.Empty);

            for(var i = 0; i < line.Length; i++)
            {
                var c = line[i].ToString();

                if (c == Separator)
                {
                    if (state == 1) text += c;
                    else
                    {
                        list.Add(text);
                        text = "";
                        state = 0;
                    }
                }
                else if (c == Qualifier)
                {
                    if(state == 0) state = 1;
                    else if(state == 1) state = 3;
                    else if(state == 2) text += c;
                }
                else
                {
                    if (state == 0) state = 2;
                    text += c;
                }
            }
            
            if(state != 0) list.Add(text);
            
            return list.ToArray();
        }
    }
}
