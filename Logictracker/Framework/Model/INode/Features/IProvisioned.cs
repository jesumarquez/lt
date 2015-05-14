using System;
using System.Linq;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Model
{
    /// <summary>
    /// Agrega la caracteristica de soportar configuracion remota.
    /// </summary>
	public interface IProvisioned : INode
    {
    	/// <summary>
    	/// Agrega o Actualiza el parametro dado a la tabla de parametros 
    	/// del INode.
    	/// </summary>
    	/// <param name="messageId"></param>
    	/// <param name="parameter">codigo del parametro.</param>
    	/// <param name="value">valor del parametro.</param>
    	/// <param name="revision">numero de revision del parametro</param>
		/// <param name="hashdetalles">hash de las revisiones de los detalles de este dispositivo</param>
    	bool SetParameter(ulong messageId, String parameter, String value, int revision, int hashdetalles);
    }

	public static class IProvisionedX
	{
		public static int GetHashFromRevisions(this IProvisioned dev, out DetalleDispositivo[] detalles)
		{
			detalles = dev.DataProvider.GetDetallesDispositivo(dev.Id)
				.Where(param => (param.TipoParametro.Consumidor == "SetupParametersHook" || param.TipoParametro.Consumidor == "D"))// && param.Revision > config)
				.ToArray();

			return (int)(((uint)detalles.Aggregate("", (aa, bb) => aa + ";" + bb).GetHashCode()) % 65535);
		}
	}
}