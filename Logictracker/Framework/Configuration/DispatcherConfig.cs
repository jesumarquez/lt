#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
    public static partial class Config
    {
        public static class Dispatcher
        {
            /// <summary>
            /// Gets the geocercas refresh interval.
            /// </summary>
            public static Int32 DispatcherGeocercasRefreshRate { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.geocercas.refresh", 15); } }

            /// <summary>
            /// Gets the max hdop value accepted for a position.
            /// </summary>
            public static Int32 DispatcherMaxHdop { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.positions.maxhdop", 10); } }

            /// <summary>
            /// Gets the radius to use for correcting stopped positions.
            /// </summary>
            public static Int32 DispatcherStoppedGeocercaRadius { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.detentions.radius", 50); } }

            /// <summary>
            /// Gets the min time to inform a stopped event.
            /// </summary>
            public static Int32 DispatcherStoppedGeocercaTime { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.detentions.time", 60); } }

            /// <summary>
            /// Gets the max device internal positions cache for the dispatcher.
            /// </summary>
            public static Int32 DispatcherDeviceQueueExpiration { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.positions.device.queue.expiration", 90); } }

            /// <summary>
            /// Gets the max time difference toleration to accept a future position as a valid one.
            /// </summary>
            public static Int32 DispatcherPositionsTimeTolerance { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.positions.timetolerance", 15); } }

            /// <summary>
            /// Gets the minimum duration of a speeding ticket to be considered a valid infraction..
            /// </summary>
            public static Int32 DispatcherMinimumSpeedTicketDuration { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.tickets.minimumduration", 0); } }

            /// <summary>
            /// Gets the delta of speed to consider that this is a valid infraction without taking into account the minimum duration * 2
            /// </summary>
			public static Int32 DispatcherDeltaSpeedTicketSpeed_Overrides_MinimumSpeedTicketDuration { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.tickets.deltaspeedoverridesminimumduration", 0); } }

            /// <summary>
            /// Gets the max time difference toleration to accept a future position as a valid one.
            /// </summary>
            public static Int32 DispatcherDeviceParametersRefresh { get { return ConfigurationBase.GetAsInt32("logictracker.dispatcher.behaivours.device.parameters.refresh", 15); } }

        }
    }
}
