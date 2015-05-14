using System.Collections.Generic;

namespace Logictracker.Web.Monitor.Geometries
{
    /// <summary>
    /// Represents a Open Layers line geometry.
    /// </summary>
    public class Line: Geometry
    {
        #region Private Properties

        /// <summary>
        /// The lines point objects,
        /// </summary>
        private string points = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Open Layers line geometry constructor
        /// </summary>
        public Line() {}

        /// <summary>
        /// Open Layers line geometry constructor
        /// </summary>
        /// <param name="id">The geometry id.</param>
        public Line(string id) : this(id, null) {}

        /// <summary>
        /// Open Layers line geometry constructor
        /// </summary>
        /// <param name="id">The geometry id.</param>
        /// <param name="style">The line style.</param>
        public Line(string id, string style)
        {
            Id = id;
            Style = style;
        }

        #endregion

        #region Public Methods

        public int Count { get; set; }
        /// <summary>
        /// Adds a Open Layers geometry point to the line.
        /// </summary>
        public void AddPoint(Point point)
        {
            if (string.IsNullOrEmpty(points)) points += point.Code();
            else points += string.Concat(",", point.Code());
            Count++;
        }

        public void AddPoints(IEnumerable<Point> pts)
        {
            foreach (var point in pts)
            {
                AddPoint(point);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the specific Open Layers geometry code.
        /// </summary>
        /// <returns></returns>
        protected override string GetChildCode() { return string.Format(@"new OL.L([{0}])", points); }

        #endregion
    }
}