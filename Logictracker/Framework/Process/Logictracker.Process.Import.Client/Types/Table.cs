using System;
using System.Collections.Generic;

namespace Logictracker.Process.Import.Client.Types
{
    public class Table
    {
        public string[] ColumnNames { get; private set; }
        public List<Record> Rows { get; set; }
        public bool HasHeaderRow { get; private set; }

        public Table():this(false, new string[0])
        {

        }
        public Table(bool hasHeaderRow, string[] columnNames)
        {
            HasHeaderRow = hasHeaderRow;
            ColumnNames = columnNames;
            Rows = new List<Record>();
        }

        public int IndexOf(string columnName)
        {
            if (!HasHeaderRow) return -1;

            for(var i = 0; i < ColumnNames.Length; i++)
                if(ColumnNames[i].ToLower() == columnName.ToLower()) return i;

            return -1;
        }

        public void Merge(Table otherTable)
        {
            if(ColumnNames.Length != otherTable.ColumnNames.Length)
                throw new ApplicationException("No se pueden unificar tablas que no tienen la misma estructura.");

            Rows.AddRange(otherTable.Rows);
        }
    }
}
