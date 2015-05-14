namespace Logictracker.Model
{
	/// <summary>
	/// Reporta temperatura
	/// </summary>
	public interface ITemperature : INode
	{
		/// <summary>
		/// </summary>
		/// <param name="messageId"></param>
		/// <param name="timerInterval"></param>
		bool ReportTemperature(ulong messageId, int timerInterval);

		/// <summary>
		/// </summary>
		/// <param name="messageId"></param>
		bool ReportTemperatureStop(ulong messageId);
	}
}