using System;
using Logictracker.Cache.Interfaces;

namespace Logictracker.Model
{
    public enum NodeTypes
    {
        Trax, Skypatrol, Castel, Coyote, Cusat, Digitax, Fulmar, Gstraq, Maxtrack, Minimt, Mobileapps, Orbcomm, Redsos, Simcom, Simcomlite, Unetelv1, Unetelv2, Melano, Virloc, Novatel, Absolut, Siac, Vigilia, ControlSat, Hawk
    }
    
    /// <summary>
    /// Define las caracteristicas comunes de todos los dispositivos que
    /// reportan al sistema. Lo que se llamo "Las cosas que reportan".
	/// </summary>
	public interface INode : IDataIdentify
    {
        NodeTypes NodeType { get; }

        /// <summary>
        /// Identificador unico del dispositivo a nivel de red LogicTracker.
        ///  </summary>
        /// <remarks>
        /// <para>
		/// El Id, es el equivalente a la direccion IP o a la MAC Addres
        /// de una placa de red. Ningun componente de la red aceptara que un
		/// mismo Id este asignado a dispositivos diferentes, dado que se utiliza
		/// como clave primara de la tabla de dispositivos. 
        /// </para>
        /// <para>
		/// La coexistencia de 2 dispositivos con mismo Id dentro de la red de dispositivos,
        /// generara comportamientos impredecibles, y en caso de ser detectada, el
		/// administrador deshabilitara el dispositivo inmediatamente.
        /// </para> 
        /// </remarks>
        new int Id { get; set; }

        /// <summary>
		/// Identificador unico del dispositivo a nivel de operador (International Mobile Equipment Identity).
        /// </summary>
		String Imei { get; set; }

        /// <summary>
        /// Identificador unico del dispositivo a nivel de operador (International Mobile Equipment Identity).
        /// </summary>
        int? IdNum { get; set; }

        /// <summary>
        /// Fecha/Hora de la ultimo paquete recivido.
        /// </summary>
        DateTime LastPacketReceivedAt { get; set; }

		/// <summary>
		/// Obtiene un nuevo id para un paquete a enviar al dispositivo generado desde la plataforma
		/// </summary>
        UInt32 NextSequence { get; }

        UInt32? Sequence { get; set; }

		IDataProvider DataProvider { get; set; }

		int Port { get; set; }

	    INode Factory(IFrame frame, int formerId);

		IMessage Decode(IFrame frame);

		INode FactoryShallowCopy(int newId, int newIdNum, String newImei);

	    INode Get(int id);

	    INode FindByIMEI(String imei);
        
        INode FindByIdNum(int idNum);

	    bool IsPacketCompleted(byte[] payload, int start, int count, out int detectedCount, out bool ignoreNoise);

		bool ChecksCorrectIdFlag { get; }

	    bool ExecuteOnGuard(Action execute, String callerName, String detailForOnFail);
    }

	public static class INodeX
	{
		public static int GetDeviceId(this INode node)
		{
			return node == null ? 0 : node.Id;
		}
	}
}
