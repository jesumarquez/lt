#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Messages;

#endregion

namespace Logictracker.Model
{
	public interface IDataProvider
	{
        List<Mensaje> GetResponsesMessagesTable(int deviceId, int revision);

		List<Mensaje> GetCannedMessagesTable(int deviceId, int revision);

        Dispositivo GetDispositivo(int deviceId);

		DetalleDispositivo GetDetalleDispositivo(int deviceId, String name);

		void SetDetalleDispositivo(int deviceId, String name, String value, String type);

		List<DetalleDispositivo> GetDetallesDispositivo(int deviceId);

		String GetConfiguration(int deviceId);

		INode Get(int deviceId, INode parser);

		INode FindByIMEI(String imei, INode parser);

        INode FindByIdNum(int idNum, INode parser);

		byte[] GetFirmware(int deviceId);
	}
}