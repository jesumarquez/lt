#region Usings

using System;

#endregion

namespace Logictracker.DatabaseTracer.Enums
{
    #region Public Enums

    /// <summary>
    /// Log modules enumeration.
    /// </summary>
    public enum LogModules
    {
        LogictrackerWeb,
        LogictrackerGateway,
        Scheduler
    };

    #endregion

    #region Public Classes

    /// <summary>
    /// Extension methods for log modules enumeration.
    /// </summary>
    public static class LogModulesExtensions
    {
        #region Public Methods

        /// <summary>
        /// Gets the description associated to the specified log module.
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static String GetDescription(this LogModules module)
        {
            switch (module)
            {
                case LogModules.LogictrackerWeb: return "Logictracker.Web";
                case LogModules.LogictrackerGateway: return "Logictracker.BackEnd";
                case LogModules.Scheduler: return "Scheduler";
                default: return String.Empty;
            }
        }

        #endregion
    }

    #endregion
}