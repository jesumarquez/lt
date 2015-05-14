using System;

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class LogicOut
        {
            public static class Fichada
            {
                public static String QueueName
                {
                    get { return ConfigurationBase.GetAsString("logicout.fichada.queuename", ""); }
                }

                public static String QueueType
                {
                    get { return ConfigurationBase.GetAsString("logicout.fichada.queuetype", ""); }
                }
            }
        }
    }
}
