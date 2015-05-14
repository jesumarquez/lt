namespace Logictracker.Web.Monitor.Geometries
{
    public class Polygon: Geometry
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
        public Polygon() {}

        /// <summary>
        /// Open Layers line geometry constructor
        /// </summary>
        /// <param name="id">The geometry id.</param>
        public Polygon(string id) : this(id, null) {}

        /// <summary>
        /// Open Layers line geometry constructor
        /// </summary>
        /// <param name="id">The geometry id.</param>
        /// <param name="style">The line style.</param>
        public Polygon(string id, string style)
        {
            Id = id;
            Style = style;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a Open Layers geometry point to the line.
        /// </summary>
        public void AddPoint(Point point)
        {
            if (string.IsNullOrEmpty(points)) points += point.Code();
            else points += string.Concat(",", point.Code());
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the specific Open Layers geometry code.
        /// </summary>
        /// <returns></returns>
        protected override string GetChildCode() { return string.Format(@"new OL.PY(new OL.LR([{0}]))", points); }

        #endregion
    }
}