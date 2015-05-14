#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Logictracker.Utils
{
    /// <summary>
    /// Multi criteria object comparer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiCriteriaObjectComparer<T> : IComparer<T>, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initialices the sort expression collection.
        /// </summary>
        public MultiCriteriaObjectComparer() { _sortExpressions = new List<KeyValuePair<string, bool>>(); }

        /// <summary>
        /// Recieves a collection of sortexpressions expressed as keyvaluepairs.
        /// </summary>
        /// <param name="sortExpressions"></param>
        public MultiCriteriaObjectComparer(IList<KeyValuePair<string, bool>> sortExpressions) { _sortExpressions = sortExpressions; }

        /// <summary>
        /// Recieves a colection of sort expression expressed as "column sortdirection".
        /// </summary>
        /// <param name="sortExpressions"></param>
        public MultiCriteriaObjectComparer(IEnumerable<string> sortExpressions)
        {
            var sort = new List<KeyValuePair<string, bool>>();

            foreach (var sortExpression in sortExpressions)
            {
                var sortColumn = sortExpression.Split(' ')[0];
                var descending = sortExpression.ToLowerInvariant().EndsWith(" desc");

                sort.Add(new KeyValuePair<string, bool>(sortColumn, descending));
            }

            _sortExpressions = sort;
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// List of sort expressions.
        /// </summary>
        private IList<KeyValuePair<string, bool>> _sortExpressions;

        #endregion

        #region Public Methods

        #region IComparer<T> Members

        /// <summary>
        /// Compares the two givenn objects using multiple crtieria.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T x, T y)
        {
            var retValue = 0;

            var comparers = new Dictionary<string, ObjectComparer<T>>();

            foreach (var sortExpression in _sortExpressions)
            {
                if (!comparers.ContainsKey(sortExpression.Key)) comparers.Add(sortExpression.Key, new ObjectComparer<T>(sortExpression.Key, sortExpression.Value));
                
                var comparer = comparers[sortExpression.Key];

                retValue = comparer.Compare(x, y);

                if (retValue.Equals(0)) continue;

                return retValue;
            }

            return retValue;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose all asigned resources.
        /// </summary>
        public void Dispose()
        {
            _sortExpressions = new List<KeyValuePair<string, bool>>();

            GC.Collect();
        }

        #endregion

        /// <summary>
        /// Adds a new sort expression.
        /// </summary>
        /// <param name="column">The sort column.</param>
        /// <param name="descending">The sort direction.</param>
        public void AddSortExpression(string column, bool descending) { _sortExpressions.Add(new KeyValuePair<string, bool>(column, descending)); }

        #endregion
    }
}