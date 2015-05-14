namespace Logictracker.Process.Import.Client.Types
{
    public class Record
    {
        public Table Table { get; private set; }
        public object[] Cells { get; set; }
        public int Count { get { return Cells.Length; } }

        public Record(Table table, object[] cells)
        {
            Table = table;
            Cells = cells;
        }

        public object this[int id]
        {
            get { return Cells[id]; }
            set { Cells[id] = value; }
        }

        public object this[string columnName]
        {
            get
            {
                var idx = Table.IndexOf(columnName);
                if (idx > 0 && idx >= Cells.Length) return string.Empty;
                return Cells[idx];
            }
            set { Cells[Table.IndexOf(columnName)] = value; }
        }

        public bool Contains(string columnName)
        {
            return Table.IndexOf(columnName) >= 0;
        }
    }
}
