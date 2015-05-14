#region Usings

using System;
using System.Text;

#endregion

namespace Logictracker.Web.Helpers.FussionChartHelpers
{
    /// <summary>
    /// Represents a fusion chart item.
    /// </summary>
    [Serializable]
    public class FusionChartsItem : FusionChartsDictionary, IFusionChartXML
    {
        #region IFusionChartXML Members

        /// <summary>
        /// Returns a xml line that represents the item.
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            var line = new StringBuilder();

            line.Append("<set");

            foreach (var pair in Values) line.Append(string.Format(" {0}='{1}'", pair.Key, pair.Value));

            line.Append("/>");

            return line.ToString();
        }

        #endregion
    }
}