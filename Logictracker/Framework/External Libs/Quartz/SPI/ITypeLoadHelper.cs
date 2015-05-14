#region Usings

using System;
using System.IO;

#endregion

namespace Quartz.Spi
{
	/// <summary> 
	/// An interface for classes wishing to provide the service of loading classes
	/// and resources within the scheduler...
	/// </summary>
	/// <author>James House</author>
	public interface ITypeLoadHelper
	{
		/// <summary> 
		/// Called to give the ClassLoadHelper a chance to Initialize itself,
		/// including the oportunity to "steal" the class loader off of the calling
		/// thread, which is the thread that is initializing Quartz.
		/// </summary>
		void Initialize();

		/// <summary> 
		/// Return the class with the given name.
		/// </summary>
		Type LoadType(string name);

		/// <summary> 
		/// Finds a resource with a given name. This method returns null if no
		/// resource with this name is found.
		/// </summary>
		/// <param name="name">name of the desired resource
		/// </param>
		/// <returns> a java.net.URL object
		/// </returns>
		Uri GetResource(string name);

		/// <summary> 
		/// Finds a resource with a given name. This method returns null if no
		/// resource with this name is found.
		/// </summary>
		/// <param name="name">name of the desired resource
		/// </param>
		/// <returns> a java.io.InputStream object
		/// </returns>
		Stream GetResourceAsStream(string name);
	}
}