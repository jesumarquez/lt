#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace Logictracker.Web.Helpers.C1Helpers
{
    /// <summary>
    /// C1 data source filter helper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class C1FilterHelper<T>
    {
        #region Public Methods

        /// <summary>
        /// Applyes the givenn filters to the collection.
        /// </summary>
        /// <param name="baseList"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<T> Filter(List<T> baseList, string filter)
        {
            // If a null or empty filter string is passed returns a new list with the original values.
            if (string.IsNullOrEmpty(filter)) return new List<T>(baseList);

            // Parses the givenn filter in order to get all the filters to be applyed.
            var filters = filter.Trim().Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries);

            // Returns a new list with the original values of none filters where found or the list obtained as the result
            // of applying all the givenn filters.
            return filters.Count().Equals(0) ? new List<T>(baseList) : ApplyFilterConditions(baseList, filters);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates conditions from the givenn filters and applyes them to the list.
        /// </summary>
        /// <param name="baseList"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        private static List<T> ApplyFilterConditions(IEnumerable<T> baseList, IEnumerable<string> filters)
        {
            // Generates a new condition collection.
            var conditions = new ConditionCollection(filters.Count());

            // Adds each givenn filter to the condition collection.
            foreach (var condition in filters)
            {
                var auxCondition = condition;

                // Gets the property from the filter.
                var index = auxCondition.IndexOf(' ');

                // If the property spliter value was not found in the filter string throws an exception.
                if (index < 0) throw new Exception("Wrong filter sintax detected when trying to get the property from the filter string.");

                var property = auxCondition.Substring(0, index);
                auxCondition = auxCondition.Substring(index).Trim();

                // Gets the operator and comparisson value from the filter string.
                index = auxCondition.IndexOf(' ');

                // If the operator and comparisson value were spliter value was not found in the filter string throws an exception.
                if (index < 0) throw new Exception("Wrong filter sintax detected when triyng to get the operator and comparisson values from the filter string.");

                var operation = auxCondition.Substring(0, index);
                var value = auxCondition.Substring(index).Trim();

                // Adds a new filter condition to the collection using the parsed property, operator and comparisson values.
                conditions.Add(new Condition{Property = property, Operator = operation, Value = value});
            }

            // Returns a new list that contains all the elements of the original list that responds to the conditions givenn.
            return (from item in baseList where conditions.Evaluate(item) select item).ToList();
        }

        #endregion

        #region Private Classes

        // Represents a filter condition.

        #region Nested type: Condition

        private class Condition
        {
            #region Public Properties

            /// <summary>
            /// The object property to be analyzed.
            /// </summary>
            public string Property { get; set; }

            /// <summary>
            /// The operator to use for the comparisson.
            /// </summary>
            public string Operator { get; set; }

            /// <summary>
            /// The comparisson value for the property.
            /// </summary>
            public string Value { get; set; }

            #endregion

            #region Public Methods

            /// <summary>
            /// Evaluates the item givenn to see if it passes the condition filter.
            /// </summary>
            /// <param name="item"></param>
            /// <returns></returns>
            public bool Evaluate(T item)
            {
                // Gets the property to be compared.
                var property = item.GetType().GetProperty(Property);

                if (property == null)
                    throw new Exception(string.Format("The property {0} was not found in the {1} class when tying to apply a filter condition.",
                                                      Property, typeof (T).FullName));

                // Gets the value of the property.
                var value = property.GetValue(item, null);

                // Gets the refference value for the property.
                var refferenceValue = Convert.ChangeType(Value, property.PropertyType);

                // Returns the result of the comparisson.
                return CompareValues(property, value, refferenceValue);
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Returns the ruslt of the comparisson btween the property value and its refference value.
            /// </summary>
            /// <param name="property"></param>
            /// <param name="value"></param>
            /// <param name="refferenceValue"></param>
            /// <returns></returns>
            private bool CompareValues(PropertyInfo property, object value, object refferenceValue)
            {
                // Depending on the property type performs the associated comparisson.
                switch (property.PropertyType.FullName)
                {
                    case "System.DateTime": return MatchesOperator(((DateTime)value).CompareTo((DateTime)refferenceValue), Operator);
                    case "System.Int32": return MatchesOperator(((int)value).CompareTo((int)refferenceValue), Operator);
                    case "System.String": return MatchesOperator(value.ToString().CompareTo(refferenceValue.ToString()), Operator);
                }

                // Return false as a default scenario.
                return false;
            }

            /// <summary>
            /// Returns the result of the comparisson associated to the specified operator.
            /// </summary>
            /// <param name="compareResult"></param>
            /// <param name="operation"></param>
            /// <returns></returns>
            private static bool MatchesOperator(int compareResult, string operation)
            {
                if (operation.Contains("=") && compareResult.Equals(0)) return true;
                if (operation.Contains(">") && compareResult > 0) return true;
                if (operation.Contains("<") && compareResult < 0) return true;
                return operation == "like" && compareResult.Equals(0);
            }

            #endregion
        }

        #endregion

        #region Nested type: ConditionCollection

        /// <summary>
        /// Represents a list of filter conditions.
        /// </summary>
        private class ConditionCollection : List<Condition>
        {
            #region Constructors

            /// <summary>
            /// Instanciates a condition collection with the specified capacity.
            /// </summary>
            /// <param name="capacity"></param>
            public ConditionCollection(int capacity) : base(capacity) { }

            #endregion

            #region Public Methods

            // Applyies all the filters to the givenn item and returns the result.
            public bool Evaluate(T item)
            {
                foreach (var condition in this) if (!condition.Evaluate(item)) return false;

                return true;
            }

            #endregion
        }

        #endregion

        #endregion
    }
}