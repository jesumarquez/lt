#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Model;
using Logictracker.Model.Utils;

#endregion

namespace Logictracker.AVL.Messages
{
    /// <summary>
    /// Mensaje de solicitud de configuracion, usado por los 
    /// dispositivos cuando se reconectan al sistema.
    /// </summary>
    [Serializable]
    public class HardwareStatus : IMessage
    {
		#region Propiedades

		public String Datos { get; set; } 

		#endregion

        #region IMessage
  
        /// <summary>
        /// Identificador univoco del mensaje.
        /// </summary>
        public ulong UniqueIdentifier { get; private set; }

        /// <summary>
        /// Identificador Transaccional del Mensaje.
        /// </summary>
		public Dictionary<String, String> UserSettings { get; private set; }

    	public DateTime Tiempo { get; set; }

		[NonSerialized]
		private byte[] _response;
		public byte[] Response
		{
			get { return _response; }
			set { _response = value; }
		}

    	/// <summary>
        /// Identificador del INode que origino el mensaje.
        /// </summary>
        public int DeviceId { get; private set; }

        #endregion

        #region Constructores

        public HardwareStatus(int deviceId, ulong msgId)
        {
			UniqueIdentifier = ParserUtils.NormalizeMsgId(msgId, null);
            DeviceId = deviceId;
            UserSettings = new Dictionary<String, String>();

			Tiempo = DateTime.UtcNow;
        }

		#endregion
        
    }
}