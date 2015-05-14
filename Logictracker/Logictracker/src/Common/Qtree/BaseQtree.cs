#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logictracker.Qtree.AutoGen;
using Logictracker.QuadTree.Data;

#endregion

namespace Logictracker.Qtree
{
    public abstract class BaseQtree: IDisposable
    {
        public virtual double HorizontalResolution { get { return 1; } }
        public virtual double VerticalResolution { get { return 1; } }
        private double HalfHorizontalResolution { get { return HorizontalResolution / 2.0; } }
        private double HalfVerticalResolution { get { return VerticalResolution / 2.0; } }

        public virtual double Top { get { return 90; } }
        public virtual double Left { get { return -180; } }
        public virtual double Bottom { get { return -90; } }
        public virtual double Right { get { return 180; } }

        public BaseQtree AlphaChannel;
        public abstract QtreeFormats Format { get; }

        protected Repository Repository;
        private AutoGenerator _autoGen;
        public AutoGenerator AutoGenerator
        {
            get { return _autoGen ?? (_autoGen = new AutoGenerator()); }
        }

        public virtual bool IsInsideQtree(double latitud, double longitud)
        {
            return !(latitud < Bottom || latitud > Top || longitud < Left || longitud > Right);
        }
        public virtual bool IsInsideQtree(double top, double bottom, double left, double right)
        {
            return !(top < Bottom || bottom > Top || left > Right || right < Left);
        }

        public static Color GetColorForLevel(int level)
        {
            switch (level)
            {
                case 0: return Color.Gray;
                case 1: return Color.Orange;
                case 2: return Color.Cyan;
                case 3: return Color.Violet;
                case 4: return Color.Green;
                case 5: return Color.Red;
                case 6: return Color.Blue;
                case 7: return Color.Yellow;
                case 8: return Color.LightSkyBlue;
                case 9: return Color.Lavender;
                case 10: return Color.Pink;
                case 11: return Color.LightSeaGreen;
                case 12: return Color.DarkKhaki;
                case 13: return Color.DarkMagenta;
                case 14: return Color.Chocolate;
                case 15: return Color.Brown;
                default: return Color.White;
            }
        }

        public virtual void Commit()
        {
            Repository.IndexFile.CommitLevel1Cache();
        }

        public virtual QLeaf GetQLeaf(double latitud, double longitud)
        {
            var inside = IsInsideQtree(latitud, longitud);
            var value = inside ? Repository.GetPositionClass((float)latitud, (float)longitud) : 0;
            var locked = inside && GetLock(latitud, longitud);
            var index = GetIndex(latitud, longitud);
            var pos = GetLatLon(index);
            return new QLeaf { Index = index, Posicion = pos, Valor = value, Locked = locked };
        }
        public virtual bool GetLock(double latitud, double longitud)
        {
            if (AlphaChannel == null) return false;
            return AlphaChannel.GetQLeaf(latitud, longitud).Valor > 0;
        }
        public virtual QLeaf SetLock(double latitud, double longitud, bool valor)
        {
            if (AlphaChannel == null) return null;
            AlphaChannel.SetValue(latitud, longitud, valor ? 1 : 0);
            return GetQLeaf(latitud, longitud);
        }
        public virtual QLeaf SetValue(double latitud, double longitud, int valor)
        {
            return SetValue(latitud, longitud, valor, false);
        }
        public virtual QLeaf SetValue(double latitud, double longitud, int valor, bool lockLeaf)
        {
            var inside = IsInsideQtree(latitud, longitud);
            if (inside) Repository.SetPositionClass((float)latitud, (float)longitud, valor);
            var index = GetIndex(latitud, longitud);
            var pos = GetLatLon(index);
            if (lockLeaf && AlphaChannel != null) AlphaChannel.SetValue(latitud, longitud, 1);
            return new QLeaf { Index = index, Posicion = pos, Valor = inside ? valor : 0 };
        }

        public QLeaf[] Paint(double latitud, double longitud, int pincel, int valor)
        {
            return Paint(latitud, longitud, pincel, valor, false);
        }
        public QLeaf[] Paint(double latitud, double longitud, int pincel, int valor, bool lockLeaf)
        {
            var leafs = new List<QLeaf>(pincel * pincel);
            var idx = GetIndex(latitud - ((pincel - 1) * HalfVerticalResolution), longitud - ((pincel - 1) * HalfHorizontalResolution));

            for (var i = idx.Y; i < idx.Y + pincel; i++)
                for (var j = idx.X; j < idx.X + pincel; j++)
                {
                    var pos = GetCenterLatLon(new QIndex { X = j, Y = i });
                    var leaf = SetValue(pos.Latitud, pos.Longitud, valor);
                    //var leaf = GetQLeaf(pos.Latitud, pos.Longitud);
                    leaf.Locked |= lockLeaf;
                    leafs.Add(leaf);
                }
            if (lockLeaf && AlphaChannel != null) AlphaChannel.Paint(latitud, longitud, pincel, 1);

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
                var pos = GetCenterLatLon(leaf.Posicion);
                SetValue(pos.Latitud, pos.Longitud, color);
                leaf.Valor = color;
                leaf.Locked |= lockLeaf;
                Commit();
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
                /*double tmpS;
                if (dx > 0 && s - (3 * GridSize) > 0)
                {
                    tmpS = (s * dx * Math.Sqrt(GridSize + dy * dy / (dx * dx)) - dx - (s / 2.0) * dy) / dx;
                    tmpS = tmpS + GridSize;
                }
                else*/
                var tmpS = sV;
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
                /*double tmpS;
                if (s - (3 * GridSize) > 0)
                {
                    tmpS = (s * dy * Math.Sqrt(GridSize + dx * dx / (dy * dy)) - (s * 2) * dx - dy) / dy;
                    tmpS = tmpS + GridSize;
                }
                else */
                var tmpS = sH;
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

            var miny = points.Min(p => p.Latitud);
            var maxy = points.Max(p => p.Latitud);

            for (y = miny; y <= maxy; y += VerticalResolution)
            {
                var polyInts = new List<double>();

                for (var i = 0; i < n; i++)
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
                for (var i = 0; i < polyInts.Count; i += 2)
                    q.AddRange(GetBoxLeaves(polyInts[i], y, polyInts[i + 1] - polyInts[i] + HorizontalResolution, VerticalResolution));
            }
            return q;
        }

        public virtual QLatLon GetLatLon(QIndex index)
        {
            return new QLatLon { Latitud = VerticalResolution * index.Y + (Top), Longitud = HorizontalResolution * index.X + Left };
        }
        public virtual QLatLon GetCenterLatLon(QLatLon latlon)
        {
            return new QLatLon { Latitud = latlon.Latitud - HalfVerticalResolution, Longitud = latlon.Longitud + HalfHorizontalResolution };
        }
        public virtual QLatLon GetCenterLatLon(QIndex index)
        {
            return GetCenterLatLon(GetLatLon(index));
        }
        public virtual QIndex GetIndex(double latitud, double longitud)
        {
            return new QIndex
            {
                X = Convert.ToInt64(Math.Floor((longitud - Left) / HorizontalResolution)),
                Y = Convert.ToInt64(Math.Ceiling((latitud - Top) / VerticalResolution))
            };
        }

        public virtual Box GetBoxFromBounds(double top, double bottom, double left, double right)
        {
            return new Box(top, bottom, left, right, this);
        }

        public abstract void Open(string directory);
        public static BaseQtree Open(string directory, QtreeFormats format)
        {
            BaseQtree q;
            switch(format)
            {
                case QtreeFormats.Gte: q = new GteQtree(); break;
                case QtreeFormats.Torino: q = new TorinoQtree(); break;
                default: return null;
            }
            q.Open(directory);
            return q;
        }
        public abstract void Create(string directory, GridStructure structure);
        public abstract Dictionary<string, string> GetFileInfo();
        public abstract Dictionary<string, string> GetCellInfo(double latitud, double longitud);

        public virtual void Generate()
        {
            AutoGenerator.Generate(this);
        }
        public virtual void Close()
        {
            if (Repository != null) Repository.Close();
            Repository = null;
            if (AlphaChannel != null) AlphaChannel.Close();
            AlphaChannel = null;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
