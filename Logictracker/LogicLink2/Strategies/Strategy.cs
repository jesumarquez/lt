using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using LinqToExcel;
using Logictracker.Process.Import.Client;
using Logictracker.Process.Import.Client.Types;
using Logictracker.Process.Import.EntityParser;

namespace Logictracker.Scheduler.Tasks.Logiclink2.Strategies
{
    public abstract class Strategy
    {
        protected static Table ParseFile(string file, char separator)
        {
            var table = new Table();
            var csv = new StreamReader(file, Encoding.GetEncoding("utf-8"));

            while (!csv.EndOfStream)
            {
                var record = ParseLine(csv.ReadLine(), separator, table);
                if (record != null) table.Rows.Add(record);
            }
            csv.Close();
            return table;
        }

        protected static Table ParseFile(string file, Dictionary<int, int> anchosFijos)
        {
            var table = new Table();
            var csv = new StreamReader(file, Encoding.GetEncoding("utf-8"));

            while (!csv.EndOfStream)
            {
                var record = ParseLine(csv.ReadLine(), anchosFijos, table);
                if (record != null) table.Rows.Add(record);
            }
            csv.Close();
            return table;
        }

        protected static Table ParseFile(string file)
        {
            var table = new Table();
            var csv = new StreamReader(file, Encoding.GetEncoding("utf-8"));

            while (!csv.EndOfStream)
            {
                var record = ParseLine(csv.ReadLine(), table);
                if (record != null) table.Rows.Add(record);
            }
            csv.Close();
            return table;
        }

        protected static List<Row> ParseExcelFile(string file, bool hasHeader)
        {
            var table = new Table();
            var excel = new ExcelQueryFactory(file);

            var rows = CreateRows(excel, hasHeader);

            return rows;
        }

        protected static List<Row> CreateRows(ExcelQueryFactory excel, bool hasHeader)
        {
            var currentWorkSheet = excel.GetWorksheetNames().First();
                        
            var sheet = hasHeader
                            ? excel.Worksheet(currentWorkSheet).Select(t => t as List<Cell>)
                            : excel.WorksheetNoHeader(currentWorkSheet).Select(t => t as List<Cell>);
            List<string> columns = null;

            var xls = new ExcelQueryFactory(excel.FileName);
            columns = hasHeader
                        ? xls.GetColumnNames(currentWorkSheet).ToList()
                        : xls.GetColumnNames(currentWorkSheet).Select((c, i) => "Column" + 1).ToList();

            var list = new List<Row>(sheet.Count());

            var emptyCount = 0;

            foreach (var row in sheet)
            {
                var importRow = new Row();
                for (var i = 0; i < columns.Count(); i++)
                {                    
                    importRow.Insert(i, row[i]);
                }

                var empty = row.All(v => string.IsNullOrEmpty(v.Value.ToString().Trim()));
                if (empty) emptyCount++;
                else emptyCount = 0;

                if (emptyCount > 1) break;

                if (!empty) list.Add(importRow);
            }
            return list;
        }

        protected static Record ParseLine(string line, Table table)
        {
            try
            {
                if (line == string.Empty) return null; //linea en blanco

                return new Record(table, new[] { line });
            }
            catch
            {
                Logger.Error("Error parseando registro: " + line);
                return null;
            }
        }

        protected static Record ParseLine(string line, char separator, Table table)
        {
            try
            {
                var cells = line.Split(separator);
                if (cells.Length == 1 && cells[0].Trim() == string.Empty) return null; //linea en blanco
                
                return new Record(table, cells);
            }
            catch
            {
                Logger.Error("Error parseando registro: " + line);
                return null;
            }
        }

        protected static Record ParseLine(string line, Dictionary<int, int> anchosFijos, Table table)
        {
            try
            {
                var cells = new string[anchosFijos.Count];

                foreach (var anchoFijo in anchosFijos)
                {
                    cells[anchoFijo.Key] = line.Substring(0, anchoFijo.Value);
                    line = line.Remove(0, anchoFijo.Value);
                }

                if (cells.Length == 1 && cells[0].Trim() == string.Empty) return null; //linea en blanco

                return new Record(table, cells);
            }
            catch
            {
                Logger.Error("Error parseando registro: " + line);
                return null;
            }
        }
        
        protected static void ThrowProperty(string property, string entity)
        {
            throw new EntityParserException("Valor inválido para el campo '" + property + "' del elemento de tipo " + entity);
        }
    }
}
