namespace Quartz.Job
{
	/// <summary> 
	/// Interface for objects wishing to receive a 'call-back' from a 
	/// <see cref="FileScanJob" />.
	/// </summary>
	/// <author>James House</author>
	/// <seealso cref="FileScanJob" />
	public interface IFileScanListener
	{
		/// <summary>
		/// Ïnforms that certain file has been updated.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		void FileUpdated(string fileName);
	}
}