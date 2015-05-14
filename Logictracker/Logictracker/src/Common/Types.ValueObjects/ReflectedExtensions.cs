using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Logictracker.Types.ValueObjects
{
	public static class TypesReflectedExtensions
    {
        public static bool HasProperty(this object o, string propertyName)
        {
            return o.GetType().GetProperty(propertyName) != null;
        }

        public static object GetReflectedValue(this object o, string propertyName)
        {
            if (o.HasProperty(propertyName))
                return o.GetType().GetProperty(propertyName).GetValue(o, null);

            var dynamicObject = o as IDynamicData;
            if (dynamicObject == null || !dynamicObject.DynamicData.ContainsKey(propertyName)) return null;

            return dynamicObject.DynamicData[propertyName];
        }

        public static IEnumerable<GridMappingAttribute> GetGridMappingAttributes(this PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(GridMappingAttribute), false).Cast<GridMappingAttribute>();
        }

        public static bool IsNumeric(this Type t)
        {
            var numericTypes = new[]
                                      {
                                          typeof (byte), typeof (short), typeof (int), typeof (long), typeof (decimal),
                                          typeof (float), typeof (Single), typeof (double), typeof (uint), typeof (ushort),
                                          typeof (ulong)
                                      };
            try
            {
                return numericTypes.Any(nt => nt == t);
            }
            catch { return false; }
        }

        public static bool IsNumeric(this object o)
        {
            try
            {
                Convert.ToDouble(o);
                return true;
            }
            catch { return false; }
        }
    }
}
