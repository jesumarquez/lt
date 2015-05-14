#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Logictracker.Web.Helpers.FussionChartHelpers
{
    /// <summary>
    /// Fusion chart xml helper.
    /// </summary>
    public class FusionChartsHelper : IDisposable
    {
        #region Private Properties

        /// <summary>
        /// Fusion chart configuration options.
        /// </summary>
        private readonly Dictionary<string, string> config = new Dictionary<string, string>();

        /// <summary>
        /// A list of items to be represented at the chart.
        /// </summary>
        private readonly List<FusionChartsItem> items = new List<FusionChartsItem>();

        /// <summary>
        /// A list of trendlines to be displayed at the graph.
        /// </summary>
        private readonly List<FusionChartsTrendline> trendlines = new List<FusionChartsTrendline>();

        /// <summary>
        /// The xml file.
        /// </summary>
        private string xmlFile;

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
        /// Adds all items for the current graph.
        /// </summary>
        private void AddItems() { foreach (var item in items) xmlFile = string.Concat(xmlFile, item.ToXML()); }

        /// <summary>
        /// Adds all trendlines for the current graph.
        /// </summary>
        private void AddTrendLines()
        {
            if (trendlines.Count.Equals(0)) return;

            xmlFile = string.Concat(xmlFile, "<trendlines>");

            foreach (var trendline in trendlines) xmlFile = string.Concat(xmlFile, trendline.ToXML());

            xmlFile = string.Concat(xmlFile, "</trendlines>");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Disposes all the allocated resources.
        /// </summary>
        public void Dispose()
        {
            config.Clear();

            items.Clear();

            GC.Collect();
        }

        /// <summary>
        /// Adds a configuration entry to the fusion chart configuration values.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddConfigEntry(string key, string value) { config.Add(key, value); }

        /// <summary>
        /// Adds a fusion chart item to be represented at the chart.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(FusionChartsItem item) { items.Add(item); }

        /// <summary>
        /// Adds a trendline to be represented at the graph.
        /// </summary>
        /// <param name="trendline"></param>
        public void AddTrendLine(FusionChartsTrendline trendline) { trendlines.Add(trendline); }

        /// <summary>
        /// Builds the final xml file based on the givenn configuration and items.
        /// </summary>
        public string BuildXml()
        {
            xmlFile = GetConfigLine();

            AddItems();

            AddTrendLines();

            return string.Concat(xmlFile, "</graph>");
        }

        /// <summary>
        /// Returns all the items inserted.
        /// </summary>
        /// <returns></returns>
        public List<FusionChartsItem> GetCurrentItems() { return items; }

        #endregion
    }
}