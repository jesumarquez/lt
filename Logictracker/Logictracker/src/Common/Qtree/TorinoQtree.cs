#region Usings

using System.Collections.Generic;
using System.IO;
using Logictracker.QuadTree.Data;

#endregion

namespace Logictracker.Qtree
{
    public class TorinoQtree : BaseQtree
    {
        #region Overrides of BaseQtree
        public override double HorizontalResolution { get { return 2/3600.0; } }
        public override double VerticalResolution { get { return HorizontalResolution; } }

        public override QtreeFormats Format { get { return QtreeFormats.Torino; } }

        public override void Open(string directory)
        {
            var repo = new Repository();
            var so = new GridStructure();

            repo.Open<GR2>(directory, ref so); 

            Repository = repo;

            if (AlphaChannel == null) return;

            var lockDirectory = Path.Combine(directory, "locks");

            if (!Directory.Exists(directory))
            {
                AlphaChannel = new TorinoQtree();
                AlphaChannel.Create(lockDirectory, new GridStructure());
            }
            else AlphaChannel.Open(lockDirectory);
        }

        public void Create(string directory)
        {
            Create(directory, new GridStructure());
        }
        public override void Create(string directory, GridStructure structure)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            var repo = new Repository();
            repo.Init<GR2>(directory, structure);
            Repository = repo;
        }

        public override Dictionary<string, string> GetFileInfo()
        {
            return new Dictionary<string, string>
                       {
                           {"Tipo", "Torino"}
                       };
        }

        public override Dictionary<string, string> GetCellInfo(double latitud, double longitud)
        {
            return new Dictionary<string, string>
                       {
                           {"Latitud", latitud.ToString("0.0000000")},
                           {"Longitud", longitud.ToString("0.0000000")}
                       };
        }
        #endregion
    }
}
