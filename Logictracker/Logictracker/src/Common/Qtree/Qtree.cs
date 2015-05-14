using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Urbetrack.QuadTree;
using System.Drawing;
using Urbetrack.Common.Qtree.AutoGen;

namespace Urbetrack.Common.Qtree
{
    public enum QtreeFormat { Torino, Gte }

    public class Qtree: BaseQtree
    {
        protected Repository Repository;
        public AutoGenQt AutoGenerator;
        public override double HorizontalResolution { get { return Repository.HorizontalResolution; } }
        public override double VerticalResolution { get { return Repository.VerticalResolution; } }
        public override double Top { get { return Repository.Headers.GridStructure.Lat_OffSet / 10000000.0; } }
        public override double Left { get { return Repository.Headers.GridStructure.Lon_OffSet / 10000000.0; } }
        public override double Bottom { get { return Top - (Height * VerticalResolution); } }
        public override double Right { get { return Left + (Width * HorizontalResolution); } }
        public virtual ulong Height { get { return Repository.Headers.GridStructure.Lat_GridCount; } }
        public virtual ulong Width { get { return Repository.Headers.GridStructure.Lon_GridCount; } }
        public virtual double FileSectorCount { get { return Repository.Headers.GridStructure.FileSectorCount; } }

        private double HalfHorizontalResolution { get { return Repository.HorizontalResolution/2.0; } }
        private double HalfVerticalResolution { get { return Repository.VerticalResolution/2.0; } }

        

        public Qtree(QtreeFormat format): this(format, false)
        {
        }
        private Qtree(QtreeFormat format, bool alphaChannel)
        {
            Format = format;
            if(!alphaChannel)  AutoGenerator = new AutoGenQt();
        }
        public Qtree(string dir, QtreeFormat format)
            : this(format)
        {
            AlphaChannel = new Qtree(format, true);
            Open(dir);
        }

        public void Open(string dir)
        {
            var repo = new Repository();
            var so = new GridStructure();

            switch (Format)
            {
                case QtreeFormat.Gte: repo.Open<GeoGrillas>(dir, ref so); break;
                case QtreeFormat.Torino: repo.Open<GR2>(dir, ref so); break;
            }
            Repository = repo;

            if(AlphaChannel == null) return;
            var lockDir = Path.Combine(dir, "locks");
            if(!Directory.Exists(lockDir))
            {
                Directory.CreateDirectory(lockDir);
                switch (Format)
                {
                    case QtreeFormat.Gte: CreateGte(lockDir); break;
                    case QtreeFormat.Torino: CreateTorino(lockDir); break;
                }
            }
            AlphaChannel.Open(lockDir);
        }
        public static Qtree CreateTorino(string dir)
        {
            var q = new Qtree(QtreeFormat.Gte);
            var repo = new Repository();
            var so = new GridStructure();
            repo.Init<GR2>(dir, so);
            q.Repository = repo;
            q.Close();
            return q;
        }
        public static Qtree CreateGte(string dir)
        {
            return CreateGte(dir, new GridStructure
                                   {
                                       Signature = "GeoGrillas",
                                       Lat_Grid = 5600,
                                       Lon_Grid = 5600,
                                       Lat_GridCount = 3600,
                                       Lon_GridCount = 3600,
                                       Lat_OffSet = -345300000,
                                       Lon_OffSet = -590000000,
                                       FileSectorCount = 113,
                                   });
        }
        public static Qtree CreateGte (string dir, double top, double left, double bottom, double right, ulong cellWidth, ulong cellHeight)
        {
            var latGrid = cellWidth;
            var lonGrid = cellHeight;
            var latOffset = Convert.ToInt64(top*10000000);
            var lonOffset = Convert.ToInt64(left*10000000);
            var latGridCount = Convert.ToUInt64(Math.Ceiling((latOffset - (bottom * 10000000)) / latGrid));
            var lonGridCount = Convert.ToUInt64(Math.Ceiling(((right*10000000) - lonOffset)/lonGrid));
            var sectorCount = Convert.ToUInt64(Math.Ceiling(lonGridCount/32.0));
            return CreateGte(dir, new GridStructure
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
        public static Qtree CreateGte(string dir, GridStructure so)
        {
            var q = new Qtree(QtreeFormat.Gte);
            var repo = new Repository();
            repo.Init<GeoGrillas>(dir, so);
            //repo.Open<GR2>(dir, ref so);
            q.Repository = repo;
            q.Close();
            return q;
        }
        
        public void Close()
        {
            if (Repository != null) Repository.Close();
            Repository = null;
            if(AlphaChannel != null) AlphaChannel.Close();
        }

        public void Generate()
        {
            AutoGenerator.Generate(this);
        }

        public string GetFileNameAndIndexes(float latF, float lonF, out long numSector, out long numByte, out bool low_bits)
        {
            if (Format == QtreeFormat.Torino || !IsInsideQtree(latF, lonF))
            {
                numSector = 0;
                numByte = 0;
                low_bits = true;
                return string.Empty;
            }
            return (Repository.IndexFile as GeoGrillas).GetFileNameAndIndexes(latF, lonF, out numSector, out numByte, out low_bits);
        }
        public QLeaf GetQLeaf(double lat, double lon)
        {
            var inside = IsInsideQtree(lat, lon);
            var value = inside ? Repository.GetPositionClass((float)lat, (float)lon) : 0;
            var locked = inside ? GetLock(lat, lon) : false;
            var index = GetIndex(lat, lon);
            var pos = GetLatLon(index);
            return new QLeaf { Index = index, Posicion = pos, Valor = value, Locked = locked };
        }
        public bool GetLock(double lat, double lon)
        {
            if(AlphaChannel == null) return false;
            return AlphaChannel.GetQLeaf(lat, lon).Valor > 0;
        }
        public QLeaf SetLock(double latitud, double longitud, bool valor)
        {
            if (AlphaChannel == null) return null;
            AlphaChannel.SetValue(latitud, longitud, valor ? 1 : 0);
            return GetQLeaf(latitud, longitud);
        }
        public QLeaf SetValue(double lat, double lon, int valor)
        {
            return SetValue(lat, lon, valor, false);
        }
        public QLeaf SetValue(double lat, double lon, int valor, bool lockLeaf)
        {
            var inside = IsInsideQtree(lat, lon);
            if(inside) Repository.SetPositionClass((float)lat, (float)lon, valor);
            var index = GetIndex(lat, lon);
            var pos = GetLatLon(index);
            if (lockLeaf && AlphaChannel != null) AlphaChannel.SetValue(lat, lon, 1);
            return new QLeaf {Index = index, Posicion = pos, Valor = inside ? valor : 0 };
        }
        public QLeaf[] Paint(double latitud, double longitud, int pincel, int valor)
        {
            return Paint(latitud, longitud, pincel, valor, false);
        }
        public QLeaf[] Paint(double latitud, double longitud, int pincel, int valor, bool lockLeaf)
        {
            var leafs = new List<QLeaf>(pincel*pincel);
            var idx = GetIndex(latitud - ((pincel - 1) * HalfVerticalResolution), longitud - ((pincel - 1) * HalfHorizontalResolution));
            
            for (var i = idx.Y; i < idx.Y + pincel; i++)
                for (var j = idx.X; j < idx.X + pincel; j++)
                {
                    var pos = GetCenterLatLon(new QIndex { X = j, Y = i });
                    SetValue(pos.Latitud, pos.Longitud, valor);
                    var leaf = GetQLeaf(pos.Latitud, pos.Longitud);
                    leaf.Locked |= lockLeaf;
                    leafs.Add(leaf);
                }
            if(lockLeaf && AlphaChannel != null) AlphaChannel.Paint(latitud, longitud, pincel, 1);

            return leafs.ToArray();
        }
 
        public QLeaf[] PaintPolygon(List<PointF> points, int color)
        {
            return PaintPolygon(points, color, false);
        }
        public QLeaf[] PaintPolygon(List<PointF> points, int color, bool lockLeaf)
        {
            var leafs = FillPolygon(points.Select(p => new QLatLon { Latitud = p.Y, Longitud = p.X }).ToList());
            foreach (var leaf in leafs)
            {
                var pos = leaf.Posicion;//GetCenterLatLon(leaf.Posicion);
                SetValue(pos.Latitud, pos.Longitud, color);
                leaf.Valor = color;
                leaf.Locked |= lockLeaf;
            }

            if (lockLeaf && AlphaChannel != null) AlphaChannel.PaintPolygon(points, 1);

            return leafs.ToArray();
        }
 
        public List<QLeaf> GetCenteredBoxLeaves(double lon, double lat, double width, double height)
        {
            var y = lat + ((height - VerticalResolution) / 2.0);
            var x = lon - ((width - HorizontalResolution) / 2.0);
            return GetBoxLeaves(x, y, width, height);
        }
        public List<QLeaf> GetBoxLeaves(double lon, double lat, double width, double height)
        {
            var q = new List<QLeaf>();
            for (double w = 0; w < width; w += HorizontalResolution)
                for (double h = 0; h < height; h += VerticalResolution)
                    q.Add(GetQLeaf(lat - h, lon + w));

            return q;
        }
        public List<QLeaf> MakeLeafLine(double x1, double y1, double x2, double y2, int brushSize)
        {
            var q = new List<QLeaf>();
            if (x1 > x2)
            {
                var tmpX2 = x2;
                var tmpY2 = y2;
                x2 = x1;
                y2 = y1;
                x1 = tmpX2;
                y1 = tmpY2;
            }
            var dx = x2 - x1;
            var dy = Math.Abs(y2 - y1);
            var x = x1;
            var y = y1;
            var yIncr = (y1 > y2) ? -VerticalResolution : VerticalResolution;

            var sH = brushSize * HorizontalResolution;
            var sV = brushSize * VerticalResolution;
            if (dx >= dy)
            {
                double tmpS;
                /*if (dx > 0 && s - (3 * GridSize) > 0)
                {
                    tmpS = (s * dx * Math.Sqrt(GridSize + dy * dy / (dx * dx)) - dx - (s / 2.0) * dy) / dx;
                    tmpS = tmpS + GridSize;
                }
                else*/
                tmpS = sV;
                var adH = sH / 2.0;

                var pr = dy * 2.0;
                var pru = pr - (dx * 2.0);
                var p = pr - dx;
                var ox = x;
                while ((dx -= HorizontalResolution) > 0)
                {
                    x += HorizontalResolution;
                    if (p > 0)
                    {
                        q.AddRange(GetCenteredBoxLeaves(ox, y, x - ox + adH, tmpS));
                        y += yIncr;
                        p += pru;
                        ox = x;
                    }
                    else p += pr;
                }
                q.AddRange(GetCenteredBoxLeaves(ox, y, x2 - ox + adH + HorizontalResolution, tmpS));
            }

            else
            {
                double tmpS;
                /*if (s - (3 * GridSize) > 0)
                {
                    tmpS = (s * dy * Math.Sqrt(GridSize + dx * dx / (dy * dy)) - (s * 2) * dx - dy) / dy;
                    tmpS = tmpS + GridSize;
                }
                else */
                tmpS = sH;
                var ad = sV / 2.0;

                var pr = dx * 2.0;
                var pru = pr - (dy * 2.0);
                var p = pr - dy;
                var oy = y;
                if (y2 <= y1)
                {
                    ad += VerticalResolution;
                    while ((dy -= VerticalResolution) > 0)
                    {
                        if (p > 0)
                        {
                            q.AddRange(GetCenteredBoxLeaves(x += HorizontalResolution, y, tmpS, oy - y + ad));
                            y += yIncr;
                            p += pru;
                            oy = y;
                        }
                        else
                        {
                            y += yIncr;
                            p += pr;
                        }
                    }
                    q.AddRange(GetCenteredBoxLeaves(x2, y2, tmpS, oy - y2 + ad));
                }
                else
                {
                    while ((dy -= VerticalResolution) > 0)
                    {
                        y += yIncr;
                        if (p > 0)
                        {
                            q.AddRange(GetCenteredBoxLeaves(x += HorizontalResolution, oy, tmpS, y - oy + ad));
                            p += pru;
                            oy = y;
                        }
                        else p += pr;
                    }
                    q.AddRange(GetCenteredBoxLeaves(x2, oy, tmpS, y2 - oy + ad + VerticalResolution));
                }
            }

            return q;
        }


        public List<QLeaf> FillPolygon(List<QLatLon> points)
        {
            var q = new List<QLeaf>();
            double y;

            var n = points.Count;
            if (n == 0) return q;

            double miny = points.Min(p => p.Latitud);
            double maxy = points.Max(p => p.Latitud);

            for (y = miny; y <= maxy; y += VerticalResolution)
            {
                var polyInts = new List<double>();

                for (int i = 0; i < n; i++)
                {
                    int ind1;
                    int ind2;
                    if (i == 0)
                    {
                        ind1 = n - 1;
                        ind2 = 0;
                    }
                    else
                    {
                        ind1 = i - 1;
                        ind2 = i;
                    }
                    var p1 = points[ind1];
                    var p2 = points[ind2];
                    if (p1.Latitud == p2.Latitud) continue;
                    if (p1.Latitud > p2.Latitud)
                    {
                        p1 = points[ind2];
                        p2 = points[ind1];
                    }

                    if ((y >= p1.Latitud) && (y < p2.Latitud))
                        polyInts.Add(((y - p1.Latitud) * (p2.Longitud - p1.Longitud) / (p2.Latitud - p1.Latitud) + p1.Longitud));

                    else if ((y == maxy) && (y > p1.Latitud) && (y <= p2.Latitud))
                        polyInts.Add(((y - p1.Latitud) * (p2.Longitud - p1.Longitud) / (p2.Latitud - p1.Latitud) + p1.Longitud));

                }
                polyInts = polyInts.OrderBy(p => p).ToList();
                for (int i = 0; i < polyInts.Count; i += 2)
                    q.AddRange(GetBoxLeaves(polyInts[i], y, polyInts[i + 1] - polyInts[i] + HorizontalResolution, VerticalResolution));
            }
            return q;
        }

        public QLatLon GetLatLon(QIndex index)
        {
            return new QLatLon { Latitud = VerticalResolution * index.Y + (Top), Longitud = HorizontalResolution * index.X + Left };
        }
    
        public QIndex GetIndex(double latitud, double longitud)
        {
            return new QIndex
                       {
                           X = Convert.ToInt32(Math.Floor((longitud - Left)/HorizontalResolution)),
                           Y = Convert.ToInt32(Math.Ceiling((latitud - Top)/VerticalResolution))
                       };
        }

        public QLatLon GetCenterLatLon(QLatLon latlon)
        {
            return new QLatLon{Latitud = latlon.Latitud - HalfVerticalResolution, Longitud = latlon.Longitud + HalfHorizontalResolution};
        }
        public QLatLon GetCenterLatLon(QIndex index)
        {
            return GetCenterLatLon(GetLatLon(index));
        }

        

        public override bool IsInsideQtree(double latitud, double longitud)
        {
            return !(latitud < Bottom || latitud > Top || longitud < Left || longitud > Right);
        }
    }
}
