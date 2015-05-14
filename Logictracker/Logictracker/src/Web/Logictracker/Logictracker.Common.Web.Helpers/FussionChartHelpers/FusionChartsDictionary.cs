#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Logictracker.Web.Helpers.FussionChartHelpers
{
    /// <summary>
    /// Represents a properties dictionary for fusion charts.
    /// </summary>
    [Serializable]
    public class FusionChartsDictionary : IDisposable
    {
        #region Private Properties

        /// <summary>
        /// Item properties values.
        /// </summary>
        private readonly Dictionary<string, string> values = new Dictionary<string, string>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the item properties values.
        /// </summary>
        public Dictionary<string, string> Values { get { return values; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose all the allocated resources.
        /// </summary>
        public void Dispose()
        {
            values.Clear();

            GC.Collect();
        }

        /// <summary>
        /// Adds a property and its value to the fusion chart item.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddPropertyValue(string key, string value) { values.Add(key, value); }

        #endregion
    }
}
