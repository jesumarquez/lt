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
	public class UserMessage : IGeoMultiPoint
    {
        #region IMessage

        public ulong UniqueIdentifier { get; private set; }

		public Dictionary<String, String> UserSettings { get; private set; }

        public DateTime Tiempo { get; set; }

		[NonSerialized]
		private byte[] _response;
		public byte[] Response
		{
			get { return _response; }
			set { _response = value; }
		}
		
		public int DeviceId { get; private set; } 
        
        #endregion

		#region IGeoPoint

		public List<GPSPoint> GeoPoints { get; set; }

		#endregion

        #region Constructores

        public UserMessage(int deviceId, ulong msgId)
        {
			UniqueIdentifier = ParserUtils.NormalizeMsgId(msgId, null);
            DeviceId = deviceId;
            UserSettings = new Dictionary<string, string>();
			GeoPoints = new List<GPSPoint>();

            Tiempo = DateTime.UtcNow;
        }

        private UserMessage() : this(0, 0) { }

        #endregion
	}
}