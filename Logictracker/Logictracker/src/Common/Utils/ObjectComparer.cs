#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Logictracker.Utils
{
    /// <summary>
    /// Generic object comparer based on reflection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectComparer<T> : IComparer<T>, IDisposable
    {
        #region Private Properties

        /// <summary>
        /// The sort direction.
        /// </summary>
        private readonly bool _descending;

        /// <summary>
        /// The sort column.
        /// </summary>
        private readonly string _sortColumn = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the sort expression and direction.
        /// </summary>
        /// <param name="column">The property name.</param>
        /// <param name="descending">The sort direction.</param>
        public ObjectComparer(string column, bool descending)
        {
            _sortColumn = column;
            _descending = descending;
        }

        /// <summary>
        /// Obtains the sort field and direction from the givenn sort expression.
        /// </summary>
        /// <param name="sortExpression">A sort expression.</param>
        public ObjectComparer(string sortExpression)
        {
            _sortColumn = sortExpression.Split(' ')[0];
            _descending = sortExpression.ToLowerInvariant().EndsWith(" desc");
        }

        #endregion

        #region Public Methods

        #region IComparer<T> Members

        /// <summary>
        /// Compares two objects using the givenn sort expression and direction.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(T x, T y)
        {
            var xValue = x.GetType().GetProperty(_sortColumn).GetValue(x, null) as IComparable;
            var yValue = y.GetType().GetProperty(_sortColumn).GetValue(y, null) as IComparable;

            var retVal = xValue == null && yValue == null ? 0 : xValue == null ? -1 : yValue == null ? 1 : xValue.CompareTo(yValue);

            return _descending ? -retVal : retVal;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes all assigned resources.
        /// </summary>
        public void Dispose() { GC.Collect(); }

        #endregion

        #endregion
    }
}