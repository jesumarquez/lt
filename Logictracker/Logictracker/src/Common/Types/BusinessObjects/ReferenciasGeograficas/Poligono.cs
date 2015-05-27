#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Iesi.Collections;
using Logictracker.Types.Algoritmos;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.InterfacesAndBaseClasses;
using Logictracker.Utils;

#endregion

namespace Logictracker.Types.BusinessObjects.ReferenciasGeograficas
{
    [Serializable]
    public class Poligono : IAuditable
    {
        private PointF _center;

        private double _innermaxx;
        private double _innermaxy;
        private double _innerminx;
        private double _innerminy;

        private ISet<Punto> _puntos;

        private bool _centerValid;
        private bool _validBounds;

        #region IAuditable

        public virtual int Id { get; set; }
        public virtual Type TypeOf() { return GetType(); }

        #endregion

        public virtual int Radio { get; set; }
        public virtual Vigencia Vigencia { get; set; }

        public virtual double MaxX { get; set; }
        public virtual double MaxY { get; set; }
        public virtual double MinX { get; set; }
        public virtual double MinY { get; set; }

        public virtual ISet<Punto> Puntos { get { return _puntos ?? (_puntos = new HashSet<Punto>()); } }

        public virtual PointF FirstPoint
        {
            get
            {
                var points = ToPointFList();

                return points.Count > 0 ? points[0] : new PointF();
            }
        }

        /// <summary>
        /// Returns a Point representing the midpoint between all the extents.
        /// This is not the same as the centroid, which is wieghted by the
        /// enclosed area.  The center will automatically correct itself if points are added.
        /// </summary>
        public virtual PointF Centro
        {
            get
            {
                if (Radio > 0) return FirstPoint;

                if (!_centerValid)
                {
                    CalculateBounds();

                    _center = new PointF((float)(MinX + MaxX) / 2, (float)(MinY + MaxY) / 2);

                    _centerValid = true;
                }

                return _center;
            }
        }

        public virtual void AddPoints(IEnumerable<PointF> points)
        {
            _validBounds = false;
            _centerValid = false;

            var index = 0;

            foreach (var point in points)
            {
                Puntos.Add(new Punto {Latitud = point.Y, Longitud = point.X, Orden = index, Poligono = this });

                index++;
            }

            if (index > 1) Radio = 0;
        }

        protected virtual void CalculateBounds()
        {
            if (_validBounds) return;

            if (Radio > 0)
            {
                var box = Radio/((111200*1.414235)/110000.0);

                _innerminx = FirstPoint.X - box;
                _innermaxx = FirstPoint.X + box;
                _innerminy = FirstPoint.Y - box;
                _innermaxy = FirstPoint.Y + box;
            }

            _validBounds = true;
        }

        public virtual void GenerateBounds()
        {
            if (Radio > 0)
            {
                var box = Radio/110000.0;

                MinY = FirstPoint.Y - box;
                MaxY = FirstPoint.Y + box;
                MinX = FirstPoint.X - box;
                MaxX = FirstPoint.X + box;
            }
            else
            {
                var firtPoint = Puntos.Cast<Punto>().First();

                MinY = firtPoint.Latitud;
                MaxY = firtPoint.Latitud;
                MinX = firtPoint.Longitud;
                MaxX = firtPoint.Longitud;

                foreach (var punto in Puntos.Cast<Punto>())
                {
                    if (punto.Latitud < MinY) MinY = punto.Latitud;
                    if (punto.Latitud > MaxY) MaxY = punto.Latitud;
                    if (punto.Longitud < MinX) MinX = punto.Longitud;
                    if (punto.Longitud > MaxX) MaxX = punto.Longitud;
                }
            }
        }

        /// <summary>
        /// Compares the supplied point and determines whether or not it is inside the Rectangular Bounds
        /// of the Polygon.
        /// </summary>
        /// <param name="pt">The PointF to compare.</param>
        /// <returns>True if the PointF is within the Rectangular Bounds, False if it is not.</returns>
        public virtual bool IsInBounds(PointF pt)
        {
            CalculateBounds();

            if (pt.X < MinX || pt.X > MaxX) return false;

            return pt.Y >= MinY && pt.Y <= MaxY;
        }

        public virtual bool IsInInnerBounds(PointF pt)
        {
            if (Radio <= 0) return false;

            return pt.X > _innerminx && pt.X < _innermaxx && pt.Y > _innerminy && pt.Y < _innermaxy;
        }

        public virtual bool Contains(double latitud, double longitud) { return Contains(new PointF((float) longitud, (float) latitud)); }

        /// <summary>
        /// Compares the supplied point and determines whether or not it is inside the Actual Bounds
        /// of the Polygon.
        /// </summary>
        /// <remarks>The calculation formula was converted from the C version available at
        /// http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        /// </remarks>
        /// <param name="pt">The PointF to compare.</param>
        /// <returns>True if the PointF is within the Actual Bounds, False if it is not.</returns>
        public virtual bool Contains(PointF pt)
        {
            var isIn = false;

            if (IsInBounds(pt))
            {
                if (Radio > 0)
                {
                    if (IsInInnerBounds(pt)) return true;

                    var distancia = Distancias.Loxodromica(FirstPoint.Y, FirstPoint.X, pt.Y, pt.X);

                    isIn = distancia <= Radio;
                }
                else
                {

                    var pts = ToPointFList();

                    int i, j;

                    // The following code is converted from a C version found at 
                    // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
                    for (i = 0, j = pts.Count - 1; i < pts.Count; j = i++)
                    {
                        if ((((pts[i].Y <= pt.Y) && (pt.Y < pts[j].Y)) || ((pts[j].Y <= pt.Y) && (pt.Y < pts[i].Y)) ) &&
                            (pt.X < (pts[j].X - pts[i].X)*(pt.Y - pts[i].Y)/(pts[j].Y - pts[i].Y) + pts[i].X))
                        {
                            isIn = !isIn;
                        }
                    }
                }
            }

            return isIn;
        }

        public virtual List<PointF> ToPointFList() { return (from Punto p in Puntos select p.ToPointF()).ToList(); }

        public virtual void Clear()
        {
            _validBounds = false;
            _centerValid = false;
            Puntos.Clear();
            Radio = 0;
        }

        private static IEnumerable<PointF> ComputeOctPts(IList<PointF> inputPts)
        {
            var pts = new List<PointF>(8);

            for (var j = 0; j < 8; j++) pts.Add(inputPts[0]);

            for (var i = 1; i < inputPts.Count; i++)
            {
                if (inputPts[i].X < pts[0].X) pts[0] = inputPts[i];
                if (inputPts[i].X - inputPts[i].Y < pts[1].X - pts[1].Y) pts[1] = inputPts[i];
                if (inputPts[i].Y > pts[2].Y) pts[2] = inputPts[i];
                if (inputPts[i].X + inputPts[i].Y > pts[3].X + pts[3].Y) pts[3] = inputPts[i];
                if (inputPts[i].X > pts[4].X) pts[4] = inputPts[i];
                if (inputPts[i].X - inputPts[i].Y > pts[5].X - pts[5].Y) pts[5] = inputPts[i];
                if (inputPts[i].Y < pts[6].Y) pts[6] = inputPts[i];
                if (inputPts[i].X + inputPts[i].Y < pts[7].X + pts[7].Y) pts[7] = inputPts[i];
            }

            return pts;
        }

        private static List<PointF> ComputeOctRing(IList<PointF> inputPts)
        {
            var octPts = ComputeOctPts(inputPts);
            var coordList = new List<PointF>(octPts);

            // points must all lie in a line
            return coordList.Count < 3 ? null : coordList;
        }

        public static List<PointF> Reduce(List<PointF> pts)
        {
            var polyPts = ComputeOctRing(pts);

            // unable to compute interior polygon for some reason
            if (polyPts == null) return pts;

            // add points defining polygon
            var reducedSet = polyPts.ToList();

            /*
             * Add all unique points not in the interior poly.
             * CGAlgorithms.IsPointInRing is not defined for points actually on the ring,
             * but this doesn't matter since the points of the interior polygon
             * are forced to be in the reduced set.
             */
            reducedSet.AddRange(pts.Where(t => !CGAlgorithms.IsPointInRing(t, polyPts.ToArray())));

            return GrahamScan(PreSort(reducedSet));
        }

        private static List<PointF> PreSort(List<PointF> pts)
        {
            // find the lowest point in the set. If two or more points have
            // the same minimum y coordinate choose the one with the minimu x.
            // This focal point is put in array location pts[0].
            for (var i = 1; i < pts.Count; i++)
            {
                if ((pts[i].Y >= pts[0].Y) && ((pts[i].Y != pts[0].Y) || (pts[i].X >= pts[0].X))) continue;

                var t = pts[0];

                pts[0] = pts[i];
                pts[i] = t;
            }

            // sort the points radially around the focal point.
            pts.Sort(new RadialComparator(pts[0]));

            return pts;
        }

        public static List<PointF> GrahamScan(IList<PointF> c)
        {
            PointF p;

            var ps = new Stack<PointF>(c.Count);

            ps.Push(c[0]);
            ps.Push(c[1]);
            ps.Push(c[2]);

            for (var i = 3; i < c.Count; i++)
            {
                p = ps.Pop();

                while (CGAlgorithms.ComputeOrientation(ps.Peek(), p, c[i]) > 0) p = ps.Pop();

                ps.Push(p);
                ps.Push(c[i]);
            }

            ps.Push(c[0]);

            return new List<PointF>(ps);
        }

        #region Nested type: RadialComparator

        /// <summary>
        /// Compares <see cref="PointF" />s for their angle and distance
        /// relative to an origin.
        /// </summary>
        private class RadialComparator : IComparer<PointF>
        {
            private readonly PointF _origin;

            /// <summary>
            /// Initializes a new instance of the <see cref="RadialComparator"/> class.
            /// </summary>
            /// <param name="origin"></param>
            public RadialComparator(PointF origin) { _origin = origin; }

            #region IComparer<PointF> Members

            /// <summary>
            /// 
            /// </summary>
            /// <param name="p1"></param>
            /// <param name="p2"></param>
            /// <returns></returns>
            public int Compare(PointF p1, PointF p2) { return PolarCompare(_origin, p1, p2); }

            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="o"></param>
            /// <param name="p"></param>
            /// <param name="q"></param>
            /// <returns></returns>
            private static int PolarCompare(PointF o, PointF p, PointF q)
            {
                double dxp = p.X - o.X;
                double dyp = p.Y - o.Y;
                double dxq = q.X - o.X;
                double dyq = q.Y - o.Y;

                var orient = CGAlgorithms.ComputeOrientation(o, p, q);

                if (orient == CGAlgorithms.CounterClockwise) return 1;
                if (orient == CGAlgorithms.Clockwise) return -1;

                // points are collinear - check distance
                var op = dxp * dxp + dyp * dyp;
                var oq = dxq * dxq + dyq * dyq;

                if (op < oq) return -1;

                return op > oq ? 1 : 0;
            }

        }

        #endregion

        public virtual bool Equals(Poligono obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.Radio != Radio) return false;
            if (obj.Puntos.Count != Puntos.Count) return false;

            var thisPoints = ToPointFList();
            var objPoints = obj.ToPointFList();

            for (var i = 0; i < thisPoints.Count; i++) if (!thisPoints[i].Equals(objPoints[i])) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == typeof (Poligono) && Equals((Poligono) obj);
        }

        public override int GetHashCode() { return Id * Radio * Puntos.Count; }
    }
}