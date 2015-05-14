#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class AssistCargo
        {
            /// <summary>
            /// Gets the AssistCargo web service user
            /// </summary>
            public static String AssistCargoWebServiceUser { get { return ConfigurationBase.GetAsString("logictracker.webservice.assistcargo.user", "telef"); } }

            /// <summary>
            /// Gets the AssistCargo web service password
            /// </summary>
            public static String AssistCargoWebServicePassword { get { return ConfigurationBase.GetAsString("logictracker.webservice.assistcargo.pass", "Kurtz"); } }

            /// <summary>
            /// Gets the AssistCargo web service password
            /// </summary>
            public static String AssistCargoEventQueue { get { return ConfigurationBase.GetAsString("logictracker.queues.assistcargo.events", ".\\private$\\eventos_assistcargo"); } }

            public static String AssistCargoDisableFuelCode { get { return ConfigurationBase.GetAsString("logictracker.interfaces.assistcargo.eventcodes.disablefuel", "CC"); } }
            public static String AssistCargoDisableFuelInmediatelyCode { get { return ConfigurationBase.GetAsString("logictracker.interfaces.assistcargo.eventcodes.disablefuelinmediately", "CI"); } }
            public static String AssistCargoEnableFuelCode { get { return ConfigurationBase.GetAsString("logictracker.interfaces.assistcargo.eventcodes.enablefuel", "RC"); } }

        }
    }
}
