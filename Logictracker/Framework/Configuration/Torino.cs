#region Usings

using System;

#endregion

namespace Logictracker.Configuration
{
	public static partial class Config
	{
		public static class Torino
		{
			public static Boolean XbeeTransportEnabled { get { return ConfigurationBase.GetAsBoolean("torino.xbee_transport_enabled", false); } }
			public static Boolean EmbeddedInterqEnabled { get { return ConfigurationBase.GetAsBoolean("torino.embeddedinterq_enabled", true); } }
			public static Boolean DisableParameters { get { return ConfigurationBase.GetAsBoolean("global.disable_parameters", false); } }
			public static Boolean FixMissmatchConfiguration { get { return ConfigurationBase.GetAsBoolean("devices.fix_missmatch_configuration", false); } }
			public static Boolean DecorateFixWithZone { get { return ConfigurationBase.GetAsBoolean("devices.decorate_fix_with_zone", false); } }
			public static Boolean TcpFotaServerEnabled { get { return ConfigurationBase.GetAsBoolean("torino.tcpfotaserver_enabled", true); } }
			public static Boolean DisableQtree { get { return ConfigurationBase.GetAsBoolean("global.disable_qtree", false); } }
			public static String StorageDatabasePath { get { return ConfigurationBase.GetAsString("storage.database_path", "storage"); } }
			public static String CommanderMQ { get { return ConfigurationBase.GetAsString("devices.commander_mq", "commander_2"); } }
			public static String PrivateQueuePrefix { get { return ConfigurationBase.GetAsString("devices.private_queue_prefix", "ZZDEV_"); } }
			public static String SpineUrl { get { return ConfigurationBase.GetAsString("spine.url", String.Empty); } }
			public static String EmbeddedInterqIncommingQueue { get { return ConfigurationBase.GetAsString("torino.embeddedinterq_incomming_queue", "agc_gtw2disp"); } }
			public static String QtreeRepository { get { return ConfigurationBase.GetAsString("devices.qtree_repository", null); } }
			public static String ConnectionString { get { return ConfigurationBase.GetAsString("global.connection_string", "error"); } }
			public static String XbeeTransportUart { get { return ConfigurationBase.GetAsString("torino.xbee_transport_uart", "COM1"); } }
            public static String XbeeTransportCodec { get { return ConfigurationBase.GetAsString("torino.xbee_transport_codec", "Logictracker.Comm.Core.Codecs.LogictrackerCodec,Logictracker.Comm.Core"); } }
			public static Int32 DBRefreshPeriod { get { return ConfigurationBase.GetAsInt32("devices.db_refresh_period", 60 * 30); } }
			public static Int32 FotaserverListenPort { get { return ConfigurationBase.GetAsInt32("torino.fotaserver_listen_port", 2358); } }
			public static Int32 EmbeddedInterqListenPort { get { return ConfigurationBase.GetAsInt32("torino.embeddedinterq_listen_port", 2357); } }
			public static Int32 QtreeMaxPageOperations { get { return ConfigurationBase.GetAsInt32("devices.qtree_max_page_operations", 12); } }
			public static Int32 KeepaliveInterval { get { return ConfigurationBase.GetAsInt32("devices.keepalive_interval", 60000); } }
			public static Int32 Torino10WindowsSize { get { return ConfigurationBase.GetAsInt32("devices.torino10_windows_size", 3); } }
			public static Int32 Torino05WindowsSize { get { return ConfigurationBase.GetAsInt32("devices.torino05_windows_size", 2); } }
			public static Int32 BlockDataTransferDeviceserverPort { get { return ConfigurationBase.GetAsInt32("torino.blockdatatransfer_deviceserver_port", 2358); } }
		}
	}
}
