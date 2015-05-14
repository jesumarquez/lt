#region Usings

using System.Text;

#endregion

namespace Logictracker.Web.Helpers.FussionChartHelpers
{
    public class FusionChartsTrendline : FusionChartsDictionary, IFusionChartXML
    {
        #region IFusionChartXML Members

        /// <summary>
        /// Returns a xml line that represents the trendline.
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            var line = new StringBuilder();

            line.Append("<line");

            foreach (var pair in Values) line.Append(string.Format(" {0}='{1}'", pair.Key, pair.Value));

            line.Append("/>");

            return line.ToString();
        }

        #endregion
    }
}
