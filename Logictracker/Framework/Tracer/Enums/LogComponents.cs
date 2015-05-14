using System;

namespace Logictracker.DatabaseTracer.Enums
{
    public enum LogComponents
    {
        DatamartPrincipal,
        DatamartDistribucion,
        DatamartViajes
    };

    public static class LogComponentsExtensions
    {
        public static String GetDescription(this LogComponents component)
        {
            switch (component)
            {
                case LogComponents.DatamartPrincipal: return "Logictracker.Scheduler.Tasks.Mantenimiento.DatamartGeneration";
                case LogComponents.DatamartDistribucion: return "Logictracker.Scheduler.Tasks.Mantenimiento.DatamartDistribucionTask";
                case LogComponents.DatamartViajes: return "Logictracker.Scheduler.Tasks.Mantenimiento.DatamartViajeTask";
                default: return String.Empty;
            }
        }
    }
}