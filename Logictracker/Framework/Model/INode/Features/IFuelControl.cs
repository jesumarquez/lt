namespace Logictracker.Model
{
	/// <summary>
	/// el nodo acepta comandos de control del flujo de combustible
	/// </summary>
	public interface IFuelControl : INode
	{
		///<summary>
		/// deshabilitar el flujo de combustible
		///</summary>
		///<param name="messageId"></param>
		///<param name="immediately">indica si se debe proceder inmediatamente o con velocidad minima</param>
		///<returns></returns>
		bool DisableFuel(ulong messageId, bool immediately);

		///<summary>
		/// habilitar el flujo de combustible
		///</summary>
		///<param name="messageId"></param>
		///<returns></returns>
		bool EnableFuel(ulong messageId);
	}
}
