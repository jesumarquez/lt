#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Model;
using Logictracker.Model.Utils;
using Logictracker.Utils;

#endregion

namespace Logictracker.AVL.Messages
{
    [Serializable]
    public class Event : IGeoPoint, IUserIdentified, IExtraData
    {
        #region Propiedades
        
        [NonSerialized]
        private readonly Dictionary<String, String> _userSettings;

        public short Code { get; private set; }

        public byte[] Payload { get; set; }

        public int PayloadSize { get; set; }
 
		public const short GenericMessage = 32767; //0x7FFF,

		public String SensorsDataString { get; set; }
		[NonSerialized] 
		private Dictionary<String, String> _sensorsDataDict;
	    public Dictionary<String, String> SensorsDataDict
	    {
		    get
		    {
			    if (_sensorsDataDict == null)
			    {
				    _sensorsDataDict = new Dictionary<String, String>();
				    var nvps = SensorsDataString.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				    foreach (var nvpa in nvps.Select(nvp => nvp.Split(':')))
					    _sensorsDataDict.Add(nvpa[0], nvpa[1]);
			    }

			    return _sensorsDataDict;
		    }
	    }

		public int TiempoEnMarcha { get; set; }

        #endregion

        #region IMessage

		public Dictionary<String, String> UserSettings { get { return _userSettings; } }

        public DateTime Tiempo { get; set; }

        public DateTime TiempoAlta { get; private set; }

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

        #region IExtraData

        public List<Int64> Data { get; private set; }

        #endregion

        #region IGeoPoint

        public GPSPoint GeoPoint { get; set; }  

        #endregion

        #region IUserIdentified

        public string UserIdentifier { get; set; } 

        #endregion

        #region Constructores

		public Event(short code, short subcode, int DeviceId, ulong msgId, GPSPoint pos, DateTime dt, String rfid, IEnumerable<Int64> ed, bool genericFlag)
		{
			UniqueIdentifier = ParserUtils.NormalizeMsgId(msgId, null);

			this.DeviceId = DeviceId;

			Tiempo = dt;
			TiempoAlta = DateTime.UtcNow;
			GeoPoint = pos;
            Code = subcode;
			UserIdentifier = rfid;
			_userSettings = new Dictionary<String, String>();
			Data = new List<Int64>();

			if (genericFlag)
			{
				Code = code;
				Data.Add(subcode);
			}

			if (ed != null) Data.AddRange(ed);
		}

        #endregion
    }
}