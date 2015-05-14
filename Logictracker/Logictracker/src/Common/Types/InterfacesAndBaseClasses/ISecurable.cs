using System;

namespace Logictracker.Types.InterfacesAndBaseClasses
{
    public interface ISecurable : IHasEmpresa, IHasLinea
    {
    }

    public static class ISecurableX
    {
        public static DateTime ToLocal(this ISecurable securable, DateTime date)
        {
            TimeZoneInfo tzi;
            return ToLocal(securable, date, out tzi);
        }
        private static DateTime ToLocal(this ISecurable securable, DateTime date, out TimeZoneInfo timeZoneInfo)
        {
            timeZoneInfo = null;

            var timeZoneId =  securable == null ? null 
                            : securable.Linea != null ? securable.Linea.TimeZoneId 
                            : securable.Empresa != null ? securable.Empresa.TimeZoneId : null;

            if (timeZoneId == null) return date;

            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                return date.AddHours(timeZoneInfo.BaseUtcOffset.TotalHours);
            }
            catch
            {
                return date;
            }
        }
        public static string ToLocalString(this ISecurable securable, DateTime date, bool includeGmt)
        {
            TimeZoneInfo timeZoneInfo;
            var local = ToLocal(securable, date, out timeZoneInfo);

            var fecha = string.Format("{0} {1}", local.ToShortDateString(), local.ToShortTimeString());

            return includeGmt && timeZoneInfo != null ? string.Concat(fecha, string.Format(" ({0})", timeZoneInfo.DisplayName)) : fecha;
        }
    }
}
