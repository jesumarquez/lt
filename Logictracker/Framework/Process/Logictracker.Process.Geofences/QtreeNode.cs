using System;
using System.Collections.Generic;
using Logictracker.Types.ValueObject;

namespace Logictracker.Process.Geofences
{
    /// <summary>
    /// Struct for indexing elements with a QuadTree
    /// </summary>
    [Serializable]
    public class QtreeNode
    {
        public static int MaxDepth = 16;

        public DateTime LastUpdate { get; private set; }

        /// <summary>
        /// Parent Node
        /// </summary>
        public QtreeNode Parent { get; private set; }

        /// <summary>
        /// Top Left Quadrant
        /// </summary>
        public QtreeNode Quadrant1 { get; private set; }

        /// <summary>
        /// Top Right Quadrant
        /// </summary>
        public QtreeNode Quadrant2 { get; private set; }

        /// <summary>
        /// Bottom Left Quadrant
        /// </summary>
        public QtreeNode Quadrant3 { get; private set; }

        /// <summary>
        /// Bottom Right Quadrant
        /// </summary>
        public QtreeNode Quadrant4 { get; private set; }

        public double Top { get; private set; }
        public double Bottom { get; private set; }
        public double Left { get; private set; }
        public double Right { get; private set; }

        public int Depth { get; private set; }

        /// <summary>
        /// Offset and Count of elements for this key
        /// </summary>
        public List<Geocerca> Data { get; private set; }

        public QtreeNode()
            :this(null, 90, -90, -180,180)
        {
            LastUpdate = DateTime.UtcNow;
        }
        protected QtreeNode(QtreeNode parent, double top, double bottom, double left, double right)
        {
            Depth = parent == null ? 0 : parent.Depth + 1;
            Parent = parent;
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public QtreeNode GetQuadrant(int i)
        {
            switch (i)
            {
                case 0: return Quadrant1;
                case 1: return Quadrant2;
                case 2: return Quadrant3;
                case 3: return Quadrant4;
            }
            return null;
        }
        protected QtreeNode AddQuadrant(int i)
        {
            double top, bottom, left, right;
            switch (i)
            {
                case 0:
                    top = Top;
                    bottom = (Top + Bottom)/2.0;
                    left = Left;
                    right = (Left + Right)/2.0;
                    Quadrant1 = new QtreeNode(this, top, bottom, left, right);
                    break;
                case 1:
                    top = Top;
                    bottom = (Top + Bottom) / 2.0;
                    left = (Left + Right) / 2.0;
                    right = Right;
                    Quadrant2 = new QtreeNode(this, top, bottom, left, right);
                    break;
                case 2:
                    top = (Top + Bottom) / 2.0;
                    bottom = Bottom;
                    left = Left;
                    right = (Left + Right) / 2.0;
                    Quadrant3 = new QtreeNode(this, top, bottom, left, right);
                    break;
                case 3:
                    top = (Top + Bottom) / 2.0;
                    bottom = Bottom;
                    left = (Left + Right) / 2.0;
                    right = Right;
                    Quadrant4 = new QtreeNode(this, top, bottom, left, right);
                    break;
            }
            return GetQuadrant(i);
        }
        public void AddValue(Geocerca geocerca)
        {
            var node = StoreInNode(geocerca);
            if (node == -1)
            {
                if (Data == null) Data = new List<Geocerca> {geocerca};
                else Data.Add(geocerca);
            }
            else
            {
                var quad = GetQuadrant(node) ?? AddQuadrant(node);
                quad.AddValue(geocerca);
            }
        }

        private int StoreInNode(Geocerca geocerca)
        {
            if(Depth > MaxDepth) return -1;
            var centerY = (Top + Bottom)/2.0;
            var centerX = (Left + Right)/2.0;

            var top = geocerca.MaxY >= centerY;
            var bottom = geocerca.MinY < centerY;
            var right = geocerca.MaxX >= centerX;
            var left = geocerca.MinX < centerX;

            if ((top && bottom) || (right && left)) return -1;

            if (top) return left ? 0 : 1;
            return left ? 2 : 3;
        }

        public List<Geocerca> GetData(double latitud, double longitud)
        {
            var centerY = (Top + Bottom) / 2.0;
            var centerX = (Left + Right) / 2.0;
            var top = latitud >= centerY;
            var left = longitud < centerX;
            var quad = top ? (left ? 0 : 1) : (left ? 2 : 3);
            var node = GetQuadrant(quad);

            List<Geocerca> data = null;
            if (node != null)
            {
                data = node.GetData(latitud, longitud);
                if(Data == null) return data;
            }
            if(data == null) return Data != null ? new List<Geocerca>(Data) : null;
            data.AddRange(Data);
            return data;
        }
    }
}
