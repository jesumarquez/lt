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
    /// Mensaje de infraccion de transito para el sistema AVL.
    /// </summary>
    [Serializable]
    public class SpeedingTicket : ITicket, IUserIdentified
    {
        #region Propiedades

        [NonSerialized]
        private readonly Dictionary<string, string> _userSettings;

        public float SpeedReached { get; set; }

        public float SpeedLimit { get; set; } 
        
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

        #region ITicket

        public GPSPoint StartPoint { get; set; }

        public GPSPoint TicketPoint { get; set; }

        public GPSPoint EndPoint { get; set; }
 
        #endregion

        #region IUserIdentified

        public string UserIdentifier { get; set; }

        #endregion

        #region Constructores

        public SpeedingTicket(int deviceId, ulong msgId)
        {
			UniqueIdentifier = ParserUtils.NormalizeMsgId(msgId, null);

            DeviceId = deviceId;

            _userSettings = new Dictionary<string, string>();

            Tiempo = DateTime.UtcNow;
        } 

		public SpeedingTicket(int deviceID, ulong msgId, GPSPoint posIni, GPSPoint posFin, float maxSpeed, float speedLimit, string rfid) : this(deviceID, msgId)
		{
			StartPoint = posIni;
			EndPoint = posFin;
			SpeedLimit = speedLimit;
			SpeedReached = maxSpeed;
			UserIdentifier = rfid;
			Tiempo = (posFin??posIni).GetDate();
		}
        
        #endregion
    }
}