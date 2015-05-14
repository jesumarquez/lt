#region Usings

using System;
using System.Collections.Generic;
using Logictracker.AVL.Messages;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Unetel.v2.UnexLibs;
using Logictracker.Utils;

#endregion

namespace Logictracker.Unetel.v2
{
	public class Vending
	{
		public static IMessage Parse(String[] partes, INode node)
		{
			var lowCmd = partes[0][1];
			var code = MessageIdentifier.KeepAliveEvent;
			var dt = DateTimeUtils.SafeParseFormat(partes[lowCmd == '1' ? 4 : 3].Replace('*', ','), "yy/MM/dd,HH:mm:ss");
			var extra = "Generic:null";
			switch (lowCmd)
			{
				case '0': return ParseLogin(partes, node);
				case '1':
					{
						var data = VendingData.Get(node.Id, dt);
						data[Convert.ToInt32(partes[3])] = partes[5];
						if (partes[5].EndsWith("$$"))
						{
							extra = "Vending_1_Data:" + String.Join(Environment.NewLine, data);
						}
						else
						{
							VendingData.Set(node.Id, dt, data);
						}
						code = MessageIdentifier.VendingTelemetricData;
						break;
					}
				case '2': code = MessageIdentifier.ReadError; break;
				case '3': code = MessageIdentifier.KeepAliveEvent; break;
				case '4': code = MessageIdentifier.PowerDisconnected; break;
				case '5': code = MessageIdentifier.PowerReconnected; break;
				case '6': code = MessageIdentifier.DoorOpenned; break;
				case '7': code = MessageIdentifier.DoorClosed; break;
			}

			var mid = Convert.ToUInt64(partes[1]);
			var msg = code.FactoryEvent(MessageIdentifier.TelemetricEvent, node.Id, mid, null, dt, null, null);
			msg.SensorsDataString = extra;
			return msg.AddStringToSend(String.Format(@"RV{0};{1:D3};{2:D5};{3:yy/MM/dd,HH:mm:ss}", lowCmd, mid, node.Id, DateTime.UtcNow));
		}

		private static ConfigRequest ParseLogin(String[] partes, INode node)
		{
			var mid = Convert.ToUInt64(partes[1]);
			var crq = new ConfigRequest(node.Id, mid)
			{
				GeoPoint = null,
			};

			crq.AddStringToSend(String.Format(@"RV0;{0:D3};{1:D5};{2:yy/MM/dd,HH:mm:ss}", mid, node.Id, DateTime.UtcNow));

			crq.StringParameters.Add(Indication.indicatedIMEI, partes[4]);
			crq.StringParameters.Add(Indication.indicatedSecret, partes[5]);
			crq.StringParameters.Add(Indication.indicatedFirmwareSignature, partes[6]);
			crq.IntegerParameters.Add(Indication.indicatedConfigurationRevision, SafeConvert.ToInt32(partes[7], -1));
			return crq;
		}

		private static class VendingData
		{
			public static String[] Get(int id, DateTime dt)
			{
				lock (CacheLock)
				{
					if (!Cache.ContainsKey(id))
					{
						Cache.Add(id, new Dictionary<DateTime, String[]>());
					}

					var d1 = Cache[id];

					if (!d1.ContainsKey(dt))
					{
						d1.Add(dt, new String[8]);
					}

					return d1[dt];
				}
			}

			public static void Set(int id, DateTime dt, String[] data)
			{
				lock (CacheLock)
				{
					Cache[id][dt] = data;
				}
			}

			private static readonly Object CacheLock = new Object();
			private static readonly Dictionary<int, Dictionary<DateTime, String[]>> Cache = new Dictionary<int, Dictionary<DateTime, String[]>>();
		}

	}
}