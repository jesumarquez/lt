using System;

namespace Logictracker.Model
{
    public interface IFrame
    {
        /// <summary>
        /// Tamaño de la carga transportada por el Frame
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Carga transportada por el frame. 
        /// </summary>
        byte[] Payload { get; }

		String PayloadAsString { get; set; }
		String RemoteAddressAsString { get; }

        IFrame Reuse(string newData);
        IFrame Reuse(byte[] newData);
        IFrame Reuse(byte[] newData, int newSize);

		int Size2 { get; }
		byte[] Payload2 { get; }
		void PostSend(String newData);
		void PostSend(byte[] newData);
		void PostSend(byte[] newData, int newSize);
    	bool HasPostSend { get; }
		int DeviceId { get; set; }
    }
}