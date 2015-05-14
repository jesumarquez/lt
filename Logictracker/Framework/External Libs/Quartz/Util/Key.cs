namespace Quartz.Util
{
	/// <summary>
	/// object representing a job or trigger key.
	/// </summary>
	/// <author>  <a href="mailto:jeff@binaryfeed.org">Jeffrey Wescott</a>
	/// </author>
	public class Key : Pair
	{
		/// <summary>
		/// Get the name portion of the key.
		/// </summary>
		/// <returns> the name
		/// </returns>
		public virtual string Name
		{
			get { return (string) First; }
		}

		/// <summary> <p>
		/// Get the group portion of the key.
		/// </p>
		/// 
		/// </summary>
		/// <returns> the group
		/// </returns>
		public virtual string Group
		{
			get { return (string) Second; }
		}

		/// <summary> Construct a new key with the given name and group.
		/// 
		/// </summary>
		/// <param name="name">
		/// the name
		/// </param>
		/// <param name="group">
		/// the group
		/// </param>
		public Key(string name, string group)
		{
			base.First = name;
			base.Second = group;
		}

		/// <summary> <p>
		/// Return the string representation of the key. The format will be:
		/// &lt;group&gt;.&lt;name&gt;.
		/// </p>
		/// 
		/// </summary>
		/// <returns> the string representation of the key
		/// </returns>
		public override string ToString()
		{
			return Group + '.' + Name;
		}
	}
}