#region Usings

using System.Runtime.Remoting.Messaging;
using System.Web;

#endregion

namespace Quartz.Util
{
	/// <summary>
	/// Wrapper class to access thread local data.
	/// Data is either accessed from thread or HTTP Context's 
	/// data if HTTP Context is avaiable.
	/// </summary>
	/// <author>Marko Lahma (.NET)</author>
	public sealed class LogicalThreadContext
	{
		private LogicalThreadContext()
		{
		}
		
		/// <summary>
		/// Retrieves an object with the specified name.
		/// </summary>
		/// <param name="name">The name of the item.</param>
		/// <returns>The object in the call context associated with the specified name or null if no object has been stored previously</returns>
		public static object GetData(string name)
		{
			var ctx = HttpContext.Current;
			if (ctx == null)
			{
				return CallContext.GetData(name);
			}
			else
			{
				return ctx.Items[name];
			}
		}

		/// <summary>
		/// Stores a given object and associates it with the specified name.
		/// </summary>
		/// <param name="name">The name with which to associate the new item.</param>
		/// <param name="value">The object to store in the call context.</param>
		public static void SetData(string name, object value)
		{
			var ctx = HttpContext.Current;
			if (ctx == null)
			{
				CallContext.SetData(name, value);
			}
			else
			{
				ctx.Items[name] = value;
			}
		}

		/// <summary>
		/// Empties a data slot with the specified name.
		/// </summary>
		/// <param name="name">The name of the data slot to empty.</param>
		public static void FreeNamedDataSlot(string name)
		{
			var ctx = HttpContext.Current;
			if (ctx == null)
			{
				CallContext.FreeNamedDataSlot(name);
			}
			else
			{
				ctx.Items.Remove(name);
			}
		}
	}
}
