#region Usings

using System;
using Logictracker.AVL.Messages;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Unetel.v2.UnexLibs;
using Logictracker.Utils;

#endregion

namespace Logictracker.Unetel.v2
{
	public class EventoTemperatura
	{
		public static IMessage Parse(String[] partes, INode node)
		{
			//Tn;msgid;idmovil;Temperatura;aa/dd/mm,hh:mm:ss; número de sensor @checksum 

			var lowCmd = partes[0][1];
			var subcode = MessageIdentifier.TemperatureInfo;
			switch (lowCmd)
			{
				case '0': //login
					return ParseTLogin(partes, node);
				case '1': //Evento de medición.
					subcode = MessageIdentifier.TemperatureInfo;
					break;
				case '2': //Evento de desconexión del sensor.
					subcode = MessageIdentifier.TemperatureDisconected;
					break;
				case '3': //Evento de desconexión de la alimentación principal del sensor
					subcode = MessageIdentifier.TemperaturePowerDisconected;
					break;
				case '4': //Evento de reconexión de la alimentación principal del sensor
					subcode = MessageIdentifier.TemperaturePowerReconected;
					break;
				case '5': //Evento descongelamiento de heladera (Botón oprimido)
					subcode = MessageIdentifier.TemperatureThawingButtonPressed;
					break;
				case '6': //Evento de fin de descongelamiento de heladera (Botón liberado)
					subcode = MessageIdentifier.TemperatureThawingButtonUnpressed;
					break;
				case '7': //Evento de puerta abierta
					subcode = MessageIdentifier.DoorOpenned;
					break;
				case '8': //Evento de puerta cerrada
					subcode = MessageIdentifier.DoorClosed;
					break;
			}
			//if (Convert.ToInt32(partes[2]) != node.Id) return null;

			IMessage msg;
			var mid = Convert.ToUInt64(partes[1]);
			if (partes.Length < 6)
			{
				msg = new UserMessage(node.Id, mid);
			}
			else
			{
				var dt = DateTimeUtils.SafeParseFormat(partes[4].Replace('*', ','), "yy/MM/dd,HH:mm:ss");
				MessageIdentifier code;
				switch (subcode)
				{
					case MessageIdentifier.TemperatureInfo:
						code = MessageIdentifier.TelemetricData;
						break;
					//debe enviarlos como M2M sino se descartan por dispositivo no asignado!
					//case MessageIdentifier.DoorClosed:
					//case MessageIdentifier.DoorOpenned:
						//code = MessageIdentifier.GenericMessage;
						//break;
					default:
						code = MessageIdentifier.TelemetricEvent;
						break;
				}
				var msg_ = subcode.FactoryEvent(code, node.Id, mid, null, dt, null, null);
				msg_.SensorsDataString = partes[5].TrimStart('0') + ":" + partes[3];
				msg = msg_;
			}

			return msg.AddStringToSend(String.Format(@"RT{0};{1:D3};{2:D5};{3:yy/MM/dd,HH:mm:ss}", lowCmd, mid, node.Id, DateTime.UtcNow));
		}

		private static ConfigRequest ParseTLogin(String[] partes, INode node)
		{
			var mid = Convert.ToUInt64(partes[1]);
			var crq = new ConfigRequest(node.Id, mid)
			{
				GeoPoint = null,
			};
			//T0;Msgid;IdDispositivo;Serie;IMEI;Pass;Version ;Versión de Parámetros ;Tiempo entre tomas ;IP primaria ;IP secundaria@checksum
			//T0;000;00000;0000000000;0355826019078657;E57290F2;7.01;00000;30;190.2.37.141:4040;unex.hopto.org:3030@3D

			crq.AddStringToSend(String.Format(@"RT0;{0:D3};{1:D5};{2:yy/MM/dd,HH:mm:ss}", mid, node.Id, DateTime.UtcNow));

			crq.StringParameters.Add(Indication.indicatedIMEI, partes[4]);
			crq.StringParameters.Add(Indication.indicatedSecret, partes[5]);
			crq.StringParameters.Add(Indication.indicatedFirmwareSignature, partes[6]);
			crq.IntegerParameters.Add(Indication.indicatedConfigurationRevision, SafeConvert.ToInt32(partes[7], -1));
			return crq;
		}
	}
}