namespace Logictracker.Model
{
	/// <summary>
	/// El INote tiene imagenes
	/// </summary>
	public interface IPicture : INode
	{
		/// <summary>
		/// Obtener las imagenes de este periodo de tiempo
		/// </summary>
		/// <param name="messageId"></param>
		/// <param name="from">indica el inicio del periodo de tiempo de donde se quieren obtener las imagenes, formato ddMMyyHHmmss utc</param>
		/// <param name="to">indica el fin del periodo de tiempo de donde se quieren obtener las imagenes, formato ddMMyyHHmmss utc</param>
		bool RetrievePictures(ulong messageId, string from, string to);
	}
}