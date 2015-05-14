#region Usings

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Logictracker.Model;

#endregion

namespace Logictracker.InterQueue.OpaqueMessage
{
    [Serializable]
    public partial class OpaqueMessage : IMessage, ISerializable
    {
        /// <summary>
        /// Identificador univoco del mensaje.
        /// </summary>
        public ulong UniqueIdentifier { get; private set; }

        /// <summary>
        /// Identificador del INode que origino el mensaje.
        /// </summary>
		public int DeviceId { get; private set; }

        /// <summary>
        /// Parametros de usuario
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
        /// Cola donde se origino el mensaje
        /// </summary>
        public String SourceQueueName { get; set; }

        /// <summary>
        /// Identificador del INode destinatario el mensaje.
        /// </summary>
        public int DestinationNodeCode { get; set; }

        /// <summary>
        /// Cola destino en el INode destinatario el mensaje.
        /// </summary>
        public String DestinationQueueName { get; set; }

        /// <summary>
        /// Etiqueta de Message de MSMQ
        /// </summary>
        public String Label { get; set; }

        /// <summary>
        /// Tamaño del Cuerpo Opaque
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Cuerpo Opaco
        /// </summary>
        public byte[] OpaqueBody { get; set; }

        /// <summary>
        /// Tipo de dato en cuerpo opaco
        /// </summary>
        public int OpaqueBodyType { get; set; }


        private static ulong _nextUid;
        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public OpaqueMessage()
        {
            if (_nextUid == 0) _nextUid = (ulong)DateTime.Now.Ticks;
            UniqueIdentifier = _nextUid++;
            UserSettings = new Dictionary<string, string>();
        }
    }
}