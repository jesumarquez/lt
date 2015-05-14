#region Usings

using System.Drawing;
using Logictracker.Utils;
using System.Globalization;

#endregion

namespace Logictracker.Web.Monitor
{
    /// <summary>
    /// Open Layers styles factory.
    /// </summary>
    public static class StyleFactory
    {
        #region Public Methods

        /// <summary>
        /// Gets a line style for the givenn color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string GetLineFromColor(Color color)
        {
            return string.Concat("{strokeColor: '", HexColorUtil.ColorToHex(color), "', strokeWidth: 4}");
        }
        public static string GetDashedLineFromColor(Color color)
        {
            return string.Concat("{strokeColor: '", HexColorUtil.ColorToHex(color), "', strokeWidth: 4, strokeDashstyle: 'dash'}");
        }

        public static string GetDottedLineFromColor(Color color)
        {
            return string.Concat("{strokeColor: '", HexColorUtil.ColorToHex(color), "', strokeWidth: 4, strokeDashstyle: 'dot'}");
        }
        
        public static string GetLineFromColor(Color color, int size, double opacity)
        {
            return string.Concat("{strokeColor: '", HexColorUtil.ColorToHex(color), "', strokeWidth:" + size + ", strokeOpacity: " + opacity.ToString("0.0", CultureInfo.InvariantCulture) + "}");
        }
        public static string GetDashedLineFromColor(Color color, int size, double opacity)
        {
            return string.Concat("{strokeColor: '", HexColorUtil.ColorToHex(color), "', strokeWidth: " + size + ", strokeDashstyle: 'dash', strokeOpacity: " + opacity.ToString("0.0", CultureInfo.InvariantCulture) + "}");
        }

        public static string GetDottedLineFromColor(Color color, int size, double opacity)
        {
            return string.Concat("{strokeColor: '", HexColorUtil.ColorToHex(color), "', strokeWidth: " + size + ", strokeDashstyle: 'dot', strokeOpacity: " + opacity.ToString("0.0", CultureInfo.InvariantCulture) + "}");
        }
        /// <summary>
        /// Gets a yellow line style.
        /// </summary>
        /// <returns></returns>
        public static string GetYellowLine() { return "{strokeColor: '#00FF00', strokeWidth: 4}"; }

        /// <summary>
        /// Gets a orange line style.
        /// </summary>
        /// <returns></returns>
        public static string GetOrangeLine() { return "{strokeColor: '#FAB802', strokeWidth: 4}"; }

        /// <summary>
        /// Gets a red line style.
        /// </summary>
        /// <returns></returns>
        public static string GetRedLine() { return "{strokeColor: 'red', strokeWidth: 7}"; }

        /// <summary>
        /// Gets a blue point style.
        /// </summary>
        /// <returns></returns>
        public static string GetBluePoint() { return "{strokeColor: 'blue', fillColor: 'blue', fillOpacity: 0.5}"; }


        public static string GetHandlePoint() { return "{strokeColor: '#666666', fillColor: '#ffffff', pointRadius: 4, strokeWidth: 2}"; }

        /// <summary>
        /// Gets a point style for the givenn color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string GetPointFromColor(Color color)
        {
            return string.Format("{{strokeColor: '{0}', fillColor: '{0}', fillOpacity: 0.5}}", HexColorUtil.ColorToHex(color));
        }

        public static string GetPolygonFromColor(Color backColor, Color borderColor)
        {

            return string.Format("{{strokeColor: '{0}', fillColor: '{0}', fillOpacity: 0.5, strokeColor: '{1}', strokeWidth: 1}}", HexColorUtil.ColorToHex(backColor), HexColorUtil.ColorToHex(borderColor));
        }

        #endregion
    }
}