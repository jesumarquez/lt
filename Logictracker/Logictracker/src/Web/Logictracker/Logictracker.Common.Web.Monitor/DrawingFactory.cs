#region Usings

using System.Globalization;

#endregion

namespace Logictracker.Web.Monitor
{
    /// <summary>
    /// Open Layers drawing factory.
    /// </summary>
    public static class DrawingFactory
    {
        #region Public Methods

        /// <summary>
        /// Gets a new Open Layers size object with the givenn values.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static string GetSize(double width, double height)
        {
            return string.Format("new OL.S({0},{1})", width.ToString(CultureInfo.InvariantCulture),
                                 height.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets a new Open Layers offset object.
        /// </summary>
        /// <param name="x">The x axis offset.</param>
        /// <param name="y">The y axis offset.</param>
        /// <returns></returns>
        public static string GetOffset(double x, double y)
        {
            return string.Format("new OL.PX({0},{1})", x.ToString(CultureInfo.InvariantCulture),
                                 y.ToString(CultureInfo.InvariantCulture));
        }

        #endregion
    }
}