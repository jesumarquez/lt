#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Model;
using Logictracker.Model.Utils;

#endregion

namespace Logictracker.AVL.Messages
{
    [Serializable]
    public class SetDetail : IMessage
    {
        public static class Fields
        {
            public const string Name = "Name";
            public const string Value = "Value";
            public const string TipoDato = "TipoDato";
            public const string Consumidor = "Consumidor";
            public const string Editable = "Editable";
            public const string Metadata = "Metadata";
            public const string ValorInicial = "ValorInicial";
            public const string RequiereReset = "RequiereReset";
        }

        #region IMessage

        public ulong UniqueIdentifier { get; private set; }

		public Dictionary<String, String> UserSettings { get; private set; }

        public DateTime Tiempo { get; set; }

    	public byte[] Response { get; set; }

		public int DeviceId { get; private set; } 
        
        #endregion

        #region Constructores

        public SetDetail(int deviceId, ulong msgId)
        {
			UniqueIdentifier = ParserUtils.NormalizeMsgId(msgId, null);
            DeviceId = deviceId;
            UserSettings = new Dictionary<string, string>();

            Tiempo = DateTime.UtcNow;
        }

		private SetDetail() : this(0, 0) { }

        #endregion
	}
}