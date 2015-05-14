#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using C1.C1Report;
using Urbetrack.Common.Utils;

#endregion

namespace Urbetrack.Common.Web.Helpers.C1Helpers
{
    /// <summary>
    /// Collection adapter for C1 Reports.
    /// </summary>
    [Serializable]
    public class C1ReportList<T> : IC1ReportRecordset, IDisposable
    {
        #region Private Properties

        /// <summary>
        /// The original list.
        /// </summary>
        private List<T> baseList;

        /// <summary>
        /// Internal index.
        /// </summary>
        private int i;

        /// <summary>
        /// Working list. This copy of the list can be modified as a result of a filter.
        /// </summary>
        private List<T> list;

        #endregion

        #region Constructors

        /// <summary>
        /// Generates a strongly typed generic list based on the givenn collection.
        /// </summary>
        /// <param name="list">A collection.</param>
        public C1ReportList(IEnumerable list)
        {
            baseList = new List<T>();
            foreach (T item in list) baseList.Add(item);
            this.list = new List<T>(baseList);
        }

        /// <summary>
        /// Generates a strongly typed generic list based on the givenn strongly typed generic collection.
        /// </summary>
        /// <param name="list">A strongly typed collection.</param>
        public C1ReportList(IEnumerable<T> list)
        {
            baseList = new List<T>(list);
            this.list = new List<T>(baseList);
        }

        #endregion

        #region IC1ReportRecordset Members

        /// <summary>
        /// Applies the givenn filter to each element of the list.
        /// </summary>
        /// <param name="filter">A string containing a custom filter to be applied as a filter.</param>
        public void ApplyFilter(string filter)
        {
            list = C1FilterHelper<T>.Filter(baseList, filter);

            i = 0;
        }

        /// <summary>
        /// Sort the list with the givenn sort expression.
        /// </summary>
        /// <param name="sort">A sort expression.</param>
        public void ApplySort(string sort) { list.Sort(new ObjectComparer<T>(sort)); }

        /// <summary>
        /// Determines if the index is set to the first element of the list.
        /// </summary>
        /// <returns>True if the index is set to the first element of the list, otherwise false.</returns>
        public bool BOF() { return i == 0; }

        /// <summary>
        /// Determines if the index is set to the last element of the list.
        /// </summary>
        /// <returns>True if the index is set to the last element of the list, otherwise false.</returns>
        public bool EOF() { return i == list.Count; }

        /// <summary>
        /// Gets the value of the internal index.
        /// </summary>
        /// <returns>The value of the internal index.</returns>
        public int GetBookmark() { return i; }

        /// <summary>
        /// Gets the names of all the properties of the generic type.
        /// </summary>
        /// <returns>A string array of field names.</returns>
        public string[] GetFieldNames()
        {
            var names = new List<string>();

            foreach (var information in typeof(T).GetProperties()) names.Add(information.Name);

            return names.ToArray();
        }

        /// <summary>
        /// Gets the types of all the properties of the generic type.
        /// </summary>
        /// <returns>A type array of fields types.</returns>
        public Type[] GetFieldTypes()
        {
            var types = new List<Type>();

            foreach (var information in typeof(T).GetProperties()) types.Add(information.PropertyType);

            return types.ToArray();
        }

        /// <summary>
        /// Gets the value of the specified field of the current item.
        /// </summary>
        /// <param name="fieldIndex">The field index.</param>
        /// <returns>The field value.</returns>
        public object GetFieldValue(int fieldIndex)
        {
            return (list.Count != 0) ? list[i].GetType().GetProperties()[fieldIndex].GetValue(list[i], null) : null;
        }

        /// <summary>
        /// Moves the internal index to the first element of the list.
        /// </summary>
        public void MoveFirst() { i = 0; }

        /// <summary>
        /// Moves the internal index to the last element of the list.
        /// </summary>
        public void MoveLast() { i = (list.Count - 1); }

        /// <summary>
        /// Moves the internal index to the next element if it is not set to EOF.
        /// </summary>
        public void MoveNext() { if (!EOF()) i++; }

        /// <summary>
        /// Moves the internal index to the previous element if it is not set to BOF.
        /// </summary>
        public void MovePrevious() { if (!BOF()) i--; }

        /// <summary>
        /// Sets the internal index value to the givenn index if it is within the list range.
        /// </summary>
        /// <param name="bkmk">The new index value.</param>
        public void SetBookmark(int bkmk) { if (bkmk >= 0 && bkmk <= (list.Count)) i = bkmk; }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose all asociated rosources.
        /// </summary>
        public void Dispose()
        {
            list.Clear();
            list = null;
            baseList.Clear();
            baseList = null;
            GC.Collect();
        }

        #endregion
    }
}