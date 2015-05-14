namespace Logictracker.Cache.Interfaces
{
	/// <summary>
	/// 	Interface for data identified objects.
	/// </summary>
	public interface IDataIdentify
	{
		#region Public Properties

		/// <summary>
		/// 	The id of the current object.
		/// </summary>
		int Id { get; set; }

		#endregion
	}
}