#region Usings

using System;
using System.Net;
using Logictracker.Model.EnumTypes;

#endregion

namespace Logictracker.Model
{
    /// <summary>
	/// Estado de una session.
	/// Provee la funcionalidad de vincular una conexion de red, a un INode.
	/// Link mantiene toda la informacion transiente necesaria por los 3 
	/// niveles de Red, Link y Transporte durante una conexion a otro punto 
	/// de la red.
	/// </summary>
    public interface ILink 
    {
        /// <summary>
        /// Variable de Estado del Link.
        /// </summary>
        LinkStates State { get; set; }

		/// <summary>
		/// Dispositivo al que apunta el link.
		/// </summary>
		INode Device { get; set; }

		/// <summary>
        /// Direccion de red subyacente para encaminar el punto.
        /// </summary>
        EndPoint EndPoint { get; set; }

        /// <summary>
        /// Edad de la ultima actualizacion del Link
        /// </summary>
        DateTime Age { get; set; }

        /// <summary>
        /// Objeto de estado de Usuario.
        /// </summary>
        object UserState { get; set; }

		/// <summary>
		/// Enlace al UnderlayingNetworkLayer con comunicacion activa con el Device
		/// </summary>
		IUnderlayingNetworkLayer UnderlayingNetworkLayer { get; set; }
    }

	public static class ILinkX
	{
		public static int GetDeviceId(this ILink me)
		{
			return me == null ? 0 : me.Device.GetDeviceId();
		}
	}

}