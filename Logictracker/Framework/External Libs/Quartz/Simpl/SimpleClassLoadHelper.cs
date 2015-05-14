#region Usings

using System;
using System.IO;
using Quartz.Spi;

#endregion

namespace Quartz.Simpl
{
	/// <summary> 
	/// A <see cref="ITypeLoadHelper" /> that simply calls <see cref="Type.GetType(string)" />.
	/// </summary>
	/// <seealso cref="ITypeLoadHelper" />
	/// <seealso cref="CascadingClassLoadHelper" />
	/// <author>James House</author>
	public class SimpleClassLoadHelper : ITypeLoadHelper
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

		/// <summary>
		/// Finds a resource with a given name. This method returns null if no
		/// resource with this name is found.
		/// </summary>
		/// <param name="name">name of the desired resource
		/// </param>
		/// <returns> a Uri object</returns>
		public virtual Uri GetResource(string name)
		{
			return null; // TODO ClassLoader.getResource(name);
		}

		/// <summary>
		/// Finds a resource with a given name. This method returns null if no
		/// resource with this name is found.
		/// </summary>
		/// <param name="name">name of the desired resource
		/// </param>
		/// <returns> a Stream object
		/// </returns>
		public virtual Stream GetResourceAsStream(string name)
		{
			return null; // TODO ClassLoader.GetResourceAsStream(name);
		}
	}
}