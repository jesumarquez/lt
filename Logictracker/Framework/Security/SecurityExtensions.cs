#region Usings

using System;

#endregion

namespace Logictracker.Security
{
    public static class SecurityExtensions
    {
        #region DateTime extenders

        /// <summary>
        /// Converts the current DateTime to the user specified TimeZone.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToDisplayDateTime(this DateTime date)
        {
            var user = WebSecurity.AuthenticatedUser;
            return user == null ? date : date.AddHours(user.GmtModifier);
        }

        /// <summary>
        /// Converts the givenn DateTime to an UTC DateTime based on user configuration.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ToDataBaseDateTime(this DateTime date)
        {
            var user = WebSecurity.AuthenticatedUser;
            return user == null ? date : date.AddHours(-user.GmtModifier);
        }

        /// <summary>
        /// Converts the givenn time into the specified time zone.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan ToDisplayTime(this TimeSpan time)
        {
            var user = WebSecurity.AuthenticatedUser;
            return user == null ? time : time.Add(TimeSpan.FromHours(user.GmtModifier));
        }

        /// <summary>
        /// Converts the specified time into a UTC based time.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static TimeSpan ToDataBaseTime (this TimeSpan time)
        {
            var user = WebSecurity.AuthenticatedUser;
            return user == null ? time : time.Add(TimeSpan.FromHours(-user.GmtModifier));
        }

        #endregion
    }
}
