#region Usings

using System;

#endregion

namespace Logictracker.ExpressionEvaluator
{
    public static class ExpressionContext
    {
        public static string CreateExpression(string expression, Object context)
        {
            foreach (var property in context.GetType().GetProperties())
            {
                var propertyName = String.Format("[{0}]", property.Name);
                var propertyValue = property.GetValue(context, null);

                //if (propertyValue != null)
                //{
                var value = GetStringValue(propertyValue, property.PropertyType);
                expression = expression.Replace(propertyName, value);
                //}
                
            }

            return expression;
        }

        public static T EvaluateExpression<T>(string expression, Object context)
        {
            var exp = CreateExpression(expression, context);
            return ExpressionEvaluator.Evaluate<T>(exp);
        }
        private static string GetStringValue(object propertyValue, Type propertyType)
        {

            if (propertyType == typeof(string))
            {
                return propertyValue == null ? string.Empty : string.Concat("\"", propertyValue, "\"");
            }
            if (propertyType == typeof(DateTime?))
            {
                var dt = propertyValue as DateTime?;
                return dt.HasValue ? String.Format("(new DateTime?({0}))", GetStringValue(dt.Value, typeof(DateTime))) : "new DateTime?()";
            }
            if (propertyType == typeof(DateTime))
            {
                if (propertyValue == null) return "DateTime.MinValue";
                var dt = (DateTime)propertyValue;
                return String.Format("(new DateTime({0},{1},{2},{3},{4},{5},{6}))",
                   dt.Year,
                   dt.Month,
                   dt.Day,
                   dt.Hour,
                   dt.Minute,
                   dt.Second,
                   dt.Millisecond);
            }

            return propertyValue == null ? "0" : propertyValue.ToString();
        }
    }
}
