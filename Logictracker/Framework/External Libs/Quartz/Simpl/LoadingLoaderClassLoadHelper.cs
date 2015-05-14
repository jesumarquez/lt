#region Usings

using System;
using System.IO;
using Quartz.Spi;

#endregion

namespace Quartz.Simpl
{
	/// <summary>
	/// A <see cref="ITypeLoadHelper" /> that uses either the loader of it's own
	/// class.
	/// </summary>
	/// <seealso cref="ITypeLoadHelper" />
	/// <seealso cref="SimpleClassLoadHelper" />
	/// <seealso cref="CascadingClassLoadHelper" />
	/// <author>James House</author>
	public class LoadingLoaderClassLoadHelper : ITypeLoadHelper
	{
		/// <summary> 
		/// Called to give the ClassLoadHelper a chance to Initialize itself,
		/// including the oportunity to "steal" the class loader off of the calling
		/// thread, which is the thread that is initializing Quartz.
		/// </summary>
		public virtual void Initialize()
		{
		}

		/// <summary> Return the class with the given name.</summary>
		public virtual Type LoadType(string name)
		{
			return Type.GetType(name);
		}

		/// <summary> Finds a resource with a given name. This method returns null if no
		/// resource with this name is found.
		/// </summary>
		/// <param name="name">name of the desired resource
		/// </param>
		/// <returns> a java.net.URL object
		/// </returns>
		public virtual Uri GetResource(string name)
		{
			//UPGRADE_ISSUE: Method 'java.lang.ClassLoader.getResource' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1000_javalangClassLoader_3"'
			return null; //ClassLoader.getResource(name);
		}

		/// <summary> Finds a resource with a given name. This method returns null if no
		/// resource with this name is found.
		/// </summary>
		/// <param name="name">name of the desired resource
		/// </param>
		/// <returns> a java.io.InputStream object
		/// </returns>
		public virtual Stream GetResourceAsStream(string name)
		{
			//UPGRADE_ISSUE: Method 'java.lang.ClassLoader.getResourceAsStream' was not converted. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1000_javalangClassLoader_3"'
			return null; // ClassLoader.getResourceAsStream(name);
		}
	}
}