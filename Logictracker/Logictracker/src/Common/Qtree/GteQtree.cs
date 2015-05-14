#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Logictracker.QuadTree.Data;

#endregion

namespace Logictracker.Qtree
{
    public class GteQtree : BaseQtree
    {
        public virtual ulong Height { get { return Repository.Headers.GridStructure.Lat_GridCount; } }
        public virtual ulong Width { get { return Repository.Headers.GridStructure.Lon_GridCount; } }
        public virtual double FileSectorCount { get { return Repository.Headers.GridStructure.FileSectorCount; } }

        #region Overrides of BaseQtree

        public override QtreeFormats Format { get { return QtreeFormats.Gte; } }

        public override double HorizontalResolution { get { return Repository.HorizontalResolution; } }
        public override double VerticalResolution { get { return Repository.VerticalResolution; } }
        public override double Top { get { return Repository.Headers.GridStructure.Lat_OffSet / 10000000.0; } }
        public override double Left { get { return Repository.Headers.GridStructure.Lon_OffSet / 10000000.0; } }
        public override double Bottom { get { return Top - (Height * VerticalResolution); } }
        public override double Right { get { return Left + (Width * HorizontalResolution); } }

        

        public override void Open(string directory)
        {
            var repo = new Repository();
            var so = new GridStructure();

            repo.Open<GeoGrillas>(directory, ref so);

            Repository = repo;

            if (AlphaChannel == null) return;

            var lockDirectory = Path.Combine(directory, "locks");

            if (!Directory.Exists(directory))
            {
                AlphaChannel = new GteQtree();
                AlphaChannel.Create(lockDirectory, so);
            }
            else AlphaChannel.Open(lockDirectory);
        }

        public void Create(string dir, double top, double left, double bottom, double right, ulong cellWidth, ulong cellHeight)
        {
            var latGrid = cellWidth;
            var lonGrid = cellHeight;
            var latOffset = Convert.ToInt64(top * 10000000);
            var lonOffset = Convert.ToInt64(left * 10000000);
            var latGridCount = Convert.ToUInt64(Math.Ceiling((latOffset - (bottom * 10000000)) / latGrid));
            var lonGridCount = Convert.ToUInt64(Math.Ceiling(((right * 10000000) - lonOffset) / lonGrid));
            var sectorCount = Convert.ToUInt64(Math.Ceiling(lonGridCount / 32.0));
            Create(dir, new GridStructure
            {
                Signature = "GeoGrillas",
                Lat_Grid = latGrid,
                Lon_Grid = lonGrid,
                Lat_OffSet = latOffset,
                Lon_OffSet = lonOffset,
                Lat_GridCount = latGridCount,
                Lon_GridCount = lonGridCount,
                FileSectorCount = sectorCount,
            });
        }
        public override void Create(string dir, GridStructure so)
        {
            var repo = new Repository();
            repo.Init<GeoGrillas>(dir, so);
            Repository = repo;
        }

        public override Dictionary<string, string> GetFileInfo()
        {
            return new Dictionary<string, string>
                       {
                           {"Tipo", "Gte"}
                       };
        }

        public override Dictionary<string, string> GetCellInfo(double latitud, double longitud)
        {
            var dic = new Dictionary<string, string>();
            long nSector = 0;
            long nByte = 0;
            var lowBits = true;
            var fileName = string.Empty;
            if (IsInsideQtree(latitud, longitud))
            {
                var file = (Repository.IndexFile as GeoGrillas);
                if (file != null)
                    fileName = file.GetFileNameAndIndexes((float)latitud, (float)longitud, out nSector, out nByte, out lowBits);
            }
            dic.Add("Archivo", fileName);
            dic.Add("Sector", nSector.ToString(CultureInfo.InvariantCulture));
            dic.Add("Byte", nByte + (lowBits ? "L" : "H"));

            return dic;
        }
        #endregion
    }
}
