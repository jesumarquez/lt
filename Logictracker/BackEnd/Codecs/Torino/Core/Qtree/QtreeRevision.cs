#region Usings

using System.Collections.Generic;

#endregion

namespace Urbetrack.Comm.Core.Qtree
{
    class QtreeRevision
    {
        internal int revision;
        internal readonly List<string> AddedFiles = new List<string>();
        internal readonly Dictionary<string, QtreeFile> Files = new Dictionary<string, QtreeFile>();

        public QtreeRevision(int rev)
        {
            revision = rev;
        }

        public void AddGR2(string filename)
        {
            AddedFiles.Add(filename);
        }

        public void SetM2(string filename, int sector, int m2_revision)
        {
            if (!Files.ContainsKey(filename))
            {
                Files.Add(filename, new QtreeFile(filename));
            }
            Files[filename].UpdateSector(sector, m2_revision);
        }
         

    }
}