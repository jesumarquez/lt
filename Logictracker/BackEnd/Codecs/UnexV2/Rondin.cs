#region Usings

using System;
using System.Globalization;
using Logictracker.AVL.Messages;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Unetel.v2.UnexLibs;
using Logictracker.Utils;

#endregion

namespace Logictracker.Unetel.v2
{
	public class Rondin
	{
		public static IMessage Parse(String[] partes, INode node)
		{
			var mid = Convert.ToUInt32(partes[1]);
			var dt = DateTimeUtils.SafeParseFormat(partes[3].Trim(), "yyMMddHHmmss");
			var lowCmd = partes[0][1];
			var code = MessageIdentifier.KeepAliveEvent;
			var extra = "Generic:null";
			switch (lowCmd)
			{
				case '1':
					{
						extra = String.Format(CultureInfo.InvariantCulture, @"Rondin_1_CheckPoint:{0}", partes[4].TrimStart('0').Trim());
						code = MessageIdentifier.CheckpointReached;
						break;
					}
				case '2':
					{
						code = MessageIdentifier.DeviceOpenned;
						break;
					}
				case '3': return ParseRLogin(partes, node, mid, dt);
				case '4':
                case '⌂':
					{
						code = MessageIdentifier.KeepAliveEvent;
						lowCmd = '4';
						break;
					}
				case '5': code = MessageIdentifier.BateryLow; break;
				case '6': code = MessageIdentifier.BateryReConected; break;
				case '7': code = MessageIdentifier.BateryDisconected; break;
                
			}

			var msg = code.FactoryEvent(MessageIdentifier.TelemetricEvent, node.Id, mid, null, dt, null, null);
			msg.SensorsDataString = extra;
			return msg.AddStringToSend(String.Format(@"RE{0};{1:D3}", lowCmd, mid));
		}

		private static ConfigRequest ParseRLogin(String[] partes, INode node, ulong mid, DateTime dt)
		{
			var crq = new ConfigRequest(node.Id, mid)
			{
				GeoPoint = null,
				Tiempo = dt,
			};

			crq.AddStringToSend(String.Format(@"RE3;{0:D3}CH;{1:D3};{2};{3:yy/MM/dd,HH:mm:ss}", mid, node.NextSequence, node.Imei, DateTime.UtcNow));
			crq.StringParameters.Add(Indication.indicatedIMEI, partes[2]);
			return crq;
		}
	}
}