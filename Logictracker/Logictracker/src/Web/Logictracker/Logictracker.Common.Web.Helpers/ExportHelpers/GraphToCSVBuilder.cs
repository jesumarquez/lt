#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Types.ValueObjects;
using Logictracker.Web.Helpers.FussionChartHelpers;

#endregion

namespace Logictracker.Web.Helpers.ExportHelpers
{
    /// <summary>
    /// Csv builder helper for exporting graphs.
    /// </summary>
	public class GraphToCsvBuilder : BaseCsvBuilder
    {
        public GraphToCsvBuilder()
        {
        }

        public GraphToCsvBuilder(char separator): base(separator)
        {

        }
        #region Public Methods

        /// <summary>
        /// Export the graph associated to the specified data into a csv file.
        /// </summary>
        /// <param name="xLabel"></param>
        /// <param name="yLabel"></param>
        /// <param name="categories"></param>
        /// <param name="graphList"></param>
        public void ExportGraph(String xLabel, String yLabel, List<String> categories, IEnumerable<FusionChartsDataset> graphList)
        {
            var str = String.Concat(xLabel, Separator, yLabel);

            Sb.AppendLine(Format(str));
            
            for (var i = 0; i < categories.Count; i++ )
            {
                var str2 = graphList.Aggregate(String.Empty, (current, d) => String.Concat(current, d.Values[i], Separator));

                str = String.Concat(categories[i], Separator, str2.TrimEnd(Separator));

                Sb.AppendLine(Format(str.Replace('.',',')));
            }
        }

        #endregion
    }
}
