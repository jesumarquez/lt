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
	public class Position : IGeoMultiPoint
	{
		#region IMessage

		public Dictionary<String, String> UserSettings { get { return _userSettings; } }

		[NonSerialized]
		private readonly Dictionary<String, String> _userSettings;

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

        #region IGeoMultiPoint

        public List<GPSPoint> GeoPoints { get; private set; }
        
        #endregion

        #region Constructors

        public Position(int deviceId, ulong msgId, DateTime dt)
        {
			UniqueIdentifier = ParserUtils.NormalizeMsgId(msgId, null);

			DeviceId = deviceId;

			_userSettings = new Dictionary<String, String>();

            GeoPoints = new List<GPSPoint>();

            Tiempo = dt;
        }

        private Position() : this(0, 0, DateTime.UtcNow) { }

        #endregion
    }

	#region Extensions

	public static class PositionX
	{
		public static IMessage ToPosition(this GPSPoint p, int deviceId, ulong msgId)
		{
			if (p == null) return new UserMessage(deviceId, msgId);

			var pos = new Position(deviceId, msgId, p.Date);
		    p.DeviceId = deviceId;
			pos.GeoPoints.Add(p);
			return pos;
		}

		public static IMessage ToPosition(this IEnumerable<GPSPoint> pl, int deviceId, ulong msgId)
		{
			if (pl == null) return new UserMessage(deviceId, msgId);

			var pos = new Position(deviceId, msgId, pl.FirstOrDefault().GetDate());
			pos.GeoPoints.AddRange(pl);
			return pos;
		}
	}
 
	#endregion
}