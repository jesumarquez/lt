#region Usings

using System;
using System.IO;
using System.Web;

#endregion

namespace Logictracker.Metrics.Enums
{
    #region Public Enums

    /// <summary>
    /// Enum that defines the currently available metrics modules.
    /// </summary>
    public enum MetricModule
    {
        LogictrackerWeb,
        LogictrackerDispatcher,
        LogictrackerScheduler,
        LogictrackerDal,
        LogictrackerGatewaySampeV1,
        LogictrackerGatewaySampeV2,
        LogictrackerGatewayTrax,
        LogictrackerGatewayTorino,
        LogictrackerGatewayEnfora,
        LogictrackerMailSender,
        LogictrackerMessageSaver,
        LogictrackerFileManager,
        LogictrackerCache,
        LogictrackerNotifier,
        LogictrackerPositionsGenerator
    }

    #endregion

    #region Public Classes

    /// <summary>
    /// Class that extends the metrics module enum behaivour.
    /// </summary>
    public static class MetricModuleExtensions
    {
        /// <summary>
        /// Gets the description associated to the givenn metric module.
        /// </summary>
        /// <param name="metricModule"></param>
        /// <returns></returns>
        public static String GetDescription(this MetricModule metricModule)
        {
            switch (metricModule)
            {
                case MetricModule.LogictrackerWeb: return "Logictracker.Web";
                case MetricModule.LogictrackerDispatcher: return "Logictracker.Dispatcher";
                case MetricModule.LogictrackerScheduler: return "Logictracker.Scheduler";
                case MetricModule.LogictrackerDal: return "Logictracker.DAL";
                case MetricModule.LogictrackerGatewaySampeV1: return "Logictracker.Gateway.SampeV1";
                case MetricModule.LogictrackerGatewaySampeV2: return "Logictracker.Gateway.SampeV2";
                case MetricModule.LogictrackerGatewayTrax: return "Logictracker.Gateway.Trax";
                case MetricModule.LogictrackerGatewayTorino: return "Logictracker.Gateway.Torino";
                case MetricModule.LogictrackerGatewayEnfora: return "Logictracker.Gateway.Enfora";
                case MetricModule.LogictrackerMailSender: return "Logictracker.Mailing";
                case MetricModule.LogictrackerMessageSaver: return "Logictracker.MessageSaver";
                case MetricModule.LogictrackerFileManager: return "Logictracker.FileManager";
                case MetricModule.LogictrackerCache: return "Logictracker.Cache";
                case MetricModule.LogictrackerNotifier: return "Logictracker.Notifier";
                case MetricModule.LogictrackerPositionsGenerator: return "Logictracker.PositionsGenerator";
                default: return String.Empty;
            }
        }

        /// <summary>
        /// Gets the performance counter category for the specified module.
        /// </summary>
        /// <param name="metricModule"></param>
        /// <returns></returns>
        public static String GetCategoryName(this MetricModule metricModule)
        {
            if (HttpContext.Current != null) return "Logictracker.Web";

            var args = Environment.GetCommandLineArgs();

            if (args == null || args.Length.Equals(0)) return "Logictracker";

            var application = args[1];

            return Path.GetFileNameWithoutExtension(application);
        }
    }

    #endregion
}