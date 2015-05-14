#region Usings

using System;
using System.Globalization;

#endregion

namespace Logictracker.Web.Monitor.Geometries
{
    /// <summary>
    /// Class the represents a Open Layers geometry point.
    /// </summary>
    public class Point : Geometry
    {
        #region Private Properties

        /// <summary>
        /// The point radius.
        /// </summary>
        private int radius;

        #endregion

        #region Public Properties

        /// <summary>
        /// The point longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// The point latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The point radius. Setting this value greater than 0 makes the point behaive as a geocerca.
        /// </summary>
        public int Radius
        {
            get { return radius; }
            set
            {
                EsGeoCerca = value > 0;
                radius = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Open Layers geometry point constructor.
        /// </summary>
        public Point() {}

        /// <summary>
        /// Open Layers geometry point constructor.
        /// </summary>
        /// <param name="id">The geometry id.</param>
        /// <param name="longitude">The point longitude.</param>
        /// <param name="latitude">The point latitude.</param>
        public Point(string id, Double longitude, Double latitude) : this(id, longitude, latitude, null) {}

        /// <summary>
        /// Open Layers geometry point constructor.
        /// </summary>
        /// <param name="id">The geometry id.</param>
        /// <param name="longitude">The point longitude.</param>
        /// <param name="latitude">The point latitude.</param>
        /// <param name="style">The geometry style.</param>
        public Point(string id, Double longitude, Double latitude, string style)
            : this(id, longitude, latitude, 0 , style) {}

        /// <summary>
        /// Open Layers geometry point constructor.
        /// </summary>
        /// <param name="id">The geometry id.</param>
        /// <param name="longitude">The point longitude.</param>
        /// <param name="latitude">The point latitude.</param>
        /// <param name="radius">The point radius.</param>
        /// <param name="style">The geometry style.</param>
        public Point(string id, Double longitude, Double latitude, int radius, string style)
        {
            Id = id;
            Longitude = longitude;
            Latitude = latitude;
            Radius = radius;
            Style = style;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the Open Layers geometry javascript associated code.
        /// </summary>
        /// <param name="mapJsRef">The map refference.</param>
        /// <returns></returns>
        public override string Code(string mapJsRef)
        {
            return string.IsNullOrEmpty(mapJsRef) ? GetChildCode() : base.Code(mapJsRef);
        }

        /// <summary>
        /// Gets the Open Layers geometry javascript associated code.
        /// </summary>
        /// <returns></returns>
        public string Code() { return Code(string.Empty); }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the specific Open Layers geometry code.
        /// </summary>
        /// <returns></returns>
        protected override string GetChildCode()
        {
            return string.Format("new OL.P({0}, {1})",
                                 Longitude.ToString(CultureInfo.InvariantCulture), Latitude.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the associated geometry style.
        /// </summary>
        /// <param name="mapJsRef">The map refference.</param>
        /// <returns></returns>
        protected override string GetStyle(string mapJsRef)
        {
            return Radius > 0 ? string.Format("{0}.GCS({1}, {2}, {3}, {4})", mapJsRef,
                                              Longitude.ToString(CultureInfo.InvariantCulture), Latitude.ToString(CultureInfo.InvariantCulture),
                                              Radius, Style) : Style;
        }

        #endregion
    }
}