#region Usings

using System.Collections.Generic;

#endregion

namespace Urbetrack.Comm.Core.Qtree
{
    class QtreeFile
    {
        internal readonly string FileName;
        internal readonly Dictionary<int, int> Sectors = new Dictionary<int, int>();

        public QtreeFile(string filename)
        {
            FileName = filename;
        }

        public void UpdateSector(int sect, int revision)
        {
            if (!Sectors.ContainsKey(sect))
            {
                Sectors.Add(sect, revision);
                return;
            }
            Sectors[sect] = revision;
        }
    }
}