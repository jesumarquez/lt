#region Usings

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Logictracker.Web.Helpers.FussionChartHelpers
{
    /// <summary>
    /// Represents a fusion charts data set.
    /// </summary>
    [Serializable]
    public class FusionChartsDataset : IDisposable, IFusionChartXML
    {
        /// <summary>
        /// Fusion chart configuration options.
        /// </summary>
        private readonly Dictionary<string, string> config = new Dictionary<string, string>();

        /// <summary>
        /// Dataset values
        /// </summary>
        private readonly List<string> values = new List<string>();

        public string Name { get; set; }

        /// <summary>
        /// Getter for the values.
        /// </summary>
        public List<string> Values { get { return values; } }

        #region IDisposable Members

        /// <summary>
        /// Dispose all asigned resources.
        /// </summary>
        public void Dispose()
        {
            values.Clear();

            GC.Collect();
        }

        #endregion

        #region IFusionChartXML Members

        /// <summary>
        /// Represents the xml line for the dataset.
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            var sb = new StringBuilder();

            sb.Append("<dataset");

            foreach (var p in GetProperties()) sb.Append(String.Format(" {0}='{1}'", p.Key, p.Value));

            sb.Append(">");

            foreach (var i in Values) sb.Append(String.Format("<set value='{0}' />", i));

            sb.Append("</dataset>");

            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// Adds a value to the dataset.
        /// </summary>
        /// <param name="value"></param>
        public void addValue(string value) { values.Add(value); }

        /// <summary>
        /// Adds a property for the dataset
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void SetPropertyValue(string property, string value) { config.Add(property, value); }

        /// <summary>
        /// Getter for the properties
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetProperties() { return config; }
    }
}
