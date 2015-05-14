#region Usings

using System;

#endregion

namespace Logictracker.Utils
{
    /// <summary>
    /// Generic object comparer based on reflection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StringAsIntComparer<T> : ICustomComparer<T>, IDisposable
    {
        #region Private Properties

        /// <summary>
        /// The sort column.
        /// </summary>
        private readonly string _sortColumn = string.Empty;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private bool _descending;

        #endregion

        #region Constructors

        /// <summary>
        /// Sets the sort expression and direction.
        /// </summary>
        /// <param name="column">The property name.</param>
        /// <param name="descending">The sort direction.</param>
        public StringAsIntComparer(string column, bool descending)
        {
            _sortColumn = column;
            _descending = descending;
        }

        /// <summary>
        /// Obtains the sort field and direction from the givenn sort expression.
        /// </summary>
        /// <param name="sortExpression">A sort expression.</param>
        public StringAsIntComparer(string sortExpression)
        {
            _sortColumn = sortExpression.Split(' ')[0];
            _descending = sortExpression.ToLowerInvariant().EndsWith(" desc");
        }

        #endregion

        #region Public Methods

        #region ICustomComparer<T> Members

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

            int retVal;
            if(xValue == null)
                retVal = -1;
            else if (yValue == null)
                retVal = 1;
            else
            {
                int ix, iy;
                if (int.TryParse(xValue.ToString(), out ix) && int.TryParse(yValue.ToString(), out iy))
                    retVal = ix.CompareTo(iy);
                else
                    retVal = xValue.CompareTo(yValue);
            }
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

        #region ICustomComparer<T> Members

        public bool Descending
        {
            get { return _descending; }
            set { _descending = value; }
        }

        #endregion
    }
}