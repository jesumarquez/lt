#region Usings

using System;

#endregion

namespace Urbetrack.Postal.Sync.Enums
{
    #region Public Enums

    /// <summary>
    /// Defines the main route states for handling associated logic.
    /// </summary>
    public enum RouteState { Pending, Transfered, Modified, Closed }

    #endregion

    #region Public Classes

    /// <summary>
    /// Extension methods for the route state enumeration.
    /// </summary>
    public static class RouteStateExtenssions
    {
        /// <summary>
        /// Gets the numeric value associated to the specified route state.
        /// </summary>
        /// <param name="routeState"></param>
        /// <returns></returns>
        public static Int32 GetValue(this RouteState routeState)
        {
            switch (routeState)
            {
                case RouteState.Pending: return 0;
                case RouteState.Transfered: return 1;
                case RouteState.Modified: return 2;
                case RouteState.Closed: return 3;
                default: return -1;
            }
        }
    }

    #endregion
}