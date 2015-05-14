#region Usings

using System.Collections;

#endregion

namespace Quartz.Collection
{
	/// <summary>
	/// Collection manipulation related utility methods.
	/// </summary>
	public sealed class CollectionUtil
	{
        private CollectionUtil()
        {
            
        }

		/// <summary>
		/// Removes the specified item from list of items and returns 
		/// whether removal was success.
		/// </summary>
		/// <param name="items">The items to remove from.</param>
		/// <param name="item">The item to remove.</param>
		/// <returns></returns>
		public static bool Remove(IList items, object item)
		{
			for (var i = 0; i < items.Count; ++i)
			{
				if (items[i] == item)
				{
					items.RemoveAt(i);
					return true;
				}
			}
			return false;
		}
	}
}