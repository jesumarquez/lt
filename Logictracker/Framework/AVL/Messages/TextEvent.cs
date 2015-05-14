#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

#endregion

namespace Logictracker.AVL.Messages
{
    [Serializable]
    public class TextEvent : IGeoPoint, IUserIdentified
    {
        #region Propiedades

        /// <summary>
        /// Mensaje del usuario
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Parametros de usuario.
        /// </summary>
        [NonSerialized]
        private readonly Dictionary<string, string> _userSettings;
        
        #endregion

        #region IMessage

		public Dictionary<String, String> UserSettings { get { return _userSettings; } }

        public DateTime Tiempo { get; set; }

		[NonSerialized]
		private byte[] _response;
		public byte[] Response
		{
			get { return _response; }
			set { _response = value; }
		}

    	public ulong UniqueIdentifier { get; private set; }

        public int DeviceId { get; private set; }
        
        #endregion

        #region IGeoPoint

        public GPSPoint GeoPoint { get; set; }
        
        #endregion

        #region IUserIdentified

        public string UserIdentifier { get; set; } 
        
        #endregion

        #region Constructores

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="msgId"></param>
        /// <param name="dt"></param>
        public TextEvent(int deviceId, ulong msgId, DateTime dt)
        {
			UniqueIdentifier = ParserUtils.NormalizeMsgId(msgId, null);

            DeviceId = deviceId;

            _userSettings = new Dictionary<string, string>();

            Tiempo = dt;
        }

        /// <summary>
        /// constructor para xmlformater
        /// </summary>
        private TextEvent() : this(0, 0, DateTime.UtcNow) { } 
        
        #endregion
    }
}