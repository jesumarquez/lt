#region Usings

using System;
using Logictracker.AVL.Messages;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Messaging;
using Logictracker.Model;
using Logictracker.Unetel.v2.UnexLibs;
using Logictracker.Utils;

#endregion

namespace Logictracker.Unetel.v2
{
	public class EventoBotonera
	{
		public static IMessage Parse(String[] partes, INode node, String buff)
		{
			//An;msgid;idmovil;aa/dd/mm,hh:mm:ss; número de botonera @checksum 

			var lowCmd = partes[0][1];
			MessageIdentifier code;
			switch (lowCmd)
			{
				case '0': //login
					return ParseALogin(partes, node);
				case '1': //boton 1 - alarma 911
					code = MessageIdentifier.KeyboardButton1;
					break;
				case '2': //boton 2 - emergencia medica
					code = MessageIdentifier.KeyboardButton2;
					break;
				case '3': //boton 3 - bomberos
					code = MessageIdentifier.KeyboardButton3;
					break;
				case '4': //corte de alimentacion
					code = MessageIdentifier.SensorPowerDisconected;
					break;
				case '5': //ganancia de alimentacion
					code = MessageIdentifier.SensorPowerReconected;
					break;
				case '6': //keepalive
					code = MessageIdentifier.KeyboardKeepAlive;
					break;
				default: throw new NotImplementedException("Evento de botonera no reconocido");
			}
			if (Convert.ToInt32(partes[2]) != node.Id) return null;

			var mid = Convert.ToUInt64(partes[1]);
			var dt = DateTimeUtils.SafeParseFormat(partes[3].Replace('*', ','), "yy/MM/dd,HH:mm:ss");
			var msg = code.FactoryEvent(code == MessageIdentifier.KeyboardKeepAlive ? MessageIdentifier.TelemetricData : MessageIdentifier.TelemetricEvent, node.Id, mid, null, dt, null, null);

		    msg.SensorsDataString = "Generic:" + (code == MessageIdentifier.KeyboardKeepAlive ? "Online" : String.Empty);
			STrace.Debug(typeof(EventoBotonera).FullName, node.Id, String.Format("no se agrego SensorsDataString, datos: {0}", buff));
			
			return msg.AddStringToSend(String.Format(@"RA{0};{1:D3};{2:D5};{3:yy/MM/dd,HH:mm:ss}", lowCmd, mid, node.Id, DateTime.UtcNow));
		}

		private static ConfigRequest ParseALogin(String[] partes, INode node)
		{
			var mid = Convert.ToUInt64(partes[1]);
			var crq = new ConfigRequest(node.Id, mid)
			{
				GeoPoint = null,
			};
			//A0;Msgid;Serie;IMEI;Pass;Version ;Versión de Parámetros ;Tiempo entre tomas ;IP primaria ;IP secundaria@checksum
			//A0;000;00000;0000000000;0355826019078657;E57290F2;7.01;00000;30;190.2.37.141:4040;unex.hopto.org:3030@3D

			crq.AddStringToSend(String.Format(@"RA0;{0:D3};{1:D5};{2:yy/MM/dd,HH:mm:ss}", mid, node.Id, DateTime.UtcNow));

			crq.StringParameters.Add(Indication.indicatedIMEI, partes[3]);
			crq.StringParameters.Add(Indication.indicatedSecret, partes[4]);
			crq.StringParameters.Add(Indication.indicatedFirmwareSignature, partes[5]);
			crq.IntegerParameters.Add(Indication.indicatedConfigurationRevision, SafeConvert.ToInt32(partes[6], -1));
			return crq;
		}
	}
}