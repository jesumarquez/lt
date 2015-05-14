#region Usings

using System;
using System.Text;

#endregion

namespace Logictracker.Model.Utils
{
    public class Frame : IFrame
    {
        /// <summary>
        /// Tamaño de la carga transportada por el Frame
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Carga transportada por el frame. 
        /// </summary>
        public byte[] Payload { get; private set; }
		public String PayloadAsString { get; set; }
		public String RemoteAddressAsString { get { return ""; } }

	    /// <summary>
	    /// Construye un Frame en base al
	    /// </summary>
	    /// <param name="payload"></param>
	    /// <param name="deviceId"></param>
	    public Frame(byte[] payload, int deviceId)
        {
            Size = payload.GetLength(0);
            Payload = new byte[Size];
            Array.Copy(payload, 0, Payload, 0, Size);
		    DeviceId = deviceId;
        }

        public override string ToString()
        {
            return string.Format("<frame Size={0}/>", Size);
        }

        public IFrame Reuse(string newData)
        {
			if (String.IsNullOrEmpty(newData))
			{
				Payload = null;
				Size = 0;
				return this;
			}
			return Reuse(Encoding.ASCII.GetBytes(newData));
        }

        public IFrame Reuse(byte[] newData)
        {
            return Reuse(newData, newData.GetLength(0));
        }

        public IFrame Reuse(byte[] newData, int newSize)
        {
            Payload = newData;
            Size = newSize;
            return this;
        }

		public byte[] Payload2 { get { return null; } }
		public int Size2 { get { return 0; } }
		public void PostSend(string newData) { }
		public void PostSend(byte[] newData) { }
		public void PostSend(byte[] newData, int newSize) { }
		public bool HasPostSend { get { return false; } }
		public int DeviceId { get; set; }
	}
}