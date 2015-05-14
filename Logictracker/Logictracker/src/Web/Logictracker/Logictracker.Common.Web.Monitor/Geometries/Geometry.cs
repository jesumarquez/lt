namespace Logictracker.Web.Monitor.Geometries
{
    /// <summary>
    /// Class that represents an Open Layers geometry.
    /// </summary>
    public abstract class Geometry
    {
        #region Private Properties

        /// <summary>
        /// The geometry associated id.
        /// </summary>
        private string id = string.Empty;

        /// <summary>
        /// The line style.
        /// </summary>
        private string style = string.Empty;

        #endregion

        #region Protected Properties

        /// <summary>
        /// The line style.
        /// </summary>
        protected string Style
        {
            get { return style; }
            set { style = value; }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The geometry associated id.
        /// </summary>
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Determines if the givenn geometry behaives as a geocerca.
        /// </summary>
        public bool EsGeoCerca { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the Open Layers geometry javascript associated code.
        /// </summary>
        /// <param name="mapJsRef">The map refference.</param>
        /// <returns></returns>
        public virtual string Code(string mapJsRef)
        {
            return string.IsNullOrEmpty(Style)
                       ? string.Format("new OL.V({0})", GetChildCode())
                       : string.Format("new OL.V({0}, null, {1})", GetChildCode(), GetStyle(mapJsRef));
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the specific Open Layers geometry code.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetChildCode();

        /// <summary>
        /// Gets the associated geometry style.
        /// </summary>
        /// <param name="mapJsRef">The map refference.</param>
        /// <returns></returns>
        protected virtual string GetStyle(string mapJsRef) { return Style; }

        #endregion
    }
}