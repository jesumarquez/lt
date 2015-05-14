#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

#endregion

namespace Logictracker.AVL.Messages
{
    /// <summary>
    /// Mensaje de solicitud de configuracion, usado por los 
    /// dispositivos cuando se reconectan al sistema.
    /// </summary>
    [Serializable]
    public class ConfigRequest : IGeoPoint
    {
        #region Miembros de IMessage
  
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

        #region Miembros de IGeoPoint
        /// <summary>
        /// Posicion desde donde se solicito la configuracion
        /// </summary>
        public GPSPoint GeoPoint { get; set; }
        #endregion

        #region Miembros de ConfigRequest
        /// <summary>
        /// Mapa para almacenar los parametros de tipo entero.
        /// </summary>
        public Dictionary<string, int> IntegerParameters { get; private set; }

        /// <summary>
        /// Mapa para almacenar los parametros de tipo string.
        /// </summary>
        public Dictionary<string, string> StringParameters { get; private set; }

        #endregion

        #region Constructores

        public ConfigRequest(int deviceId, ulong msgID)
        {
			UniqueIdentifier = ParserUtils.NormalizeMsgId(msgID, null);
            DeviceId = deviceId;
			UserSettings = new Dictionary<String, String>();
			IntegerParameters = new Dictionary<String, int>();
			StringParameters = new Dictionary<String, String>();

			Tiempo = DateTime.UtcNow;
        }

        #endregion
    }
}