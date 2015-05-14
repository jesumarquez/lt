namespace Logictracker.Model
{
    ///<summary>
    /// 
    ///</summary>
	public interface IUserIdentified : IMessage
    {
        ///<summary>
        /// Obtiene o establece la identificacion electronica del
        /// chofer del vehiculo.
        ///</summary>
        string UserIdentifier { get; set; }

    }

	///<summary>
	///</summary>
	public static class IUserIdentifiedExtensionMethods
	{
		/// <summary>
		/// Obtiene el Identificador del Chofer del vehiculo.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static string GetRiderId(this IMessage message)
		{
			try
			{
				if (message is IUserIdentified) return (message as IUserIdentified).UserIdentifier;
			}
			catch {}
			return "";
		}
	}
}