#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Logictracker.Web.Helpers.FussionChartHelpers
{
    /// <summary>
    /// Fusion Charts Multi-Series Helper.
    /// </summary>
    public class FusionChartsMultiSeriesHelper : IDisposable
    {
        #region Private Properties

        /// <summary>
        /// Fusion chart categories.
        /// </summary>
        private readonly List<string> categories = new List<string>();

        /// <summary>
        /// Fusion chart configuration options.
        /// </summary>
        private readonly Dictionary<string, string> config = new Dictionary<string, string>();

        /// <summary>
        /// A list of datasets to be represented at the chart.
        /// </summary>
        private readonly List<FusionChartsDataset> datasets = new List<FusionChartsDataset>();

        /// <summary>
        /// A list of trendlines to be displayed at the graph.
        /// </summary>
        private readonly List<FusionChartsTrendline> trendlines = new List<FusionChartsTrendline>();

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the configuration header for the file.
        /// </summary>
        /// <returns></returns>
        private string GetConfigLine()
        {
            var header = new StringBuilder();

            header.Append("<graph");

            foreach (var pair in config) header.Append(string.Format(" {0}='{1}'", pair.Key, pair.Value));

            header.Append(">");

            return header.ToString();
        }

        /// <summary>
        /// Generates the Categories XML
        /// </summary>
        /// <returns></returns>
        private string GetCategories()
        {
            var sb = new StringBuilder();

            sb.Append("<categories>");

            foreach (var c in categories) sb.Append(String.Format("<category name='{0}' hoverText='{1}' showName='1' />", c,c));

            sb.Append("</categories>");

            return sb.ToString();
        }

        /// <summary>
        /// Generates the Datasets XML
        /// </summary>
        /// <returns></returns>
        private string GetDatasets()
        {
            var sb = new StringBuilder();

            foreach (var d in datasets) sb.Append(d.ToXML());

            return sb.ToString();
        }

        /// <summary>
        /// Adds all trendlines for the current graph.
        /// </summary>
        private string GetTrendLines()
        {
            if (trendlines.Count.Equals(0)) return string.Empty;

            var lines = new StringBuilder();

            lines.Append("<trendlines>");

            foreach (var trendline in trendlines) lines.Append(trendline.ToXML());

            lines.Append("</trendlines>");

            return lines.ToString();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Disposes all the allocated resources.
        /// </summary>
        public void Dispose()
        {
            config.Clear();

            categories.Clear();

            datasets.Clear();

            GC.Collect();
        }

        /// <summary>
        /// Builds the final xml file based on the givenn configuration,categories,datasets and items
        /// </summary>
        public string BuildXml() { return string.Concat(GetConfigLine(), GetCategories(), GetDatasets(), GetTrendLines(), "</graph>"); }

        /// <summary>
        /// Adds a configuration entry to the fusion chart configuration values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddConfigEntry(string key, string value) { config.Add(key, value); }

        /// <summary>
        /// Adds a Category to the Multi Series graph
        /// </summary>
        /// <param name="name"></param>
        public void AddCategory(string name) { categories.Add(name); }

        /// <summary>
        /// Adds a DataSet to the Multi Series graph.
        /// </summary>
        /// <param name="dataset"></param>
        public void AddDataSet(FusionChartsDataset dataset) { datasets.Add(dataset); }

        /// <summary>
        /// Adds a trendline to be represented at the graph.
        /// </summary>
        /// <param name="trendline"></param>
        public void AddTrendLine(FusionChartsTrendline trendline) { trendlines.Add(trendline); }

        /// <summary>
        /// Returns the list of categories.
        /// </summary>
        /// <returns></returns>
        public List<string> GetCategoriesList() { return categories; }

        #endregion
    }
}
