#region Using

using System;

#endregion

namespace Urbetrack.Cache
{
	public static class DevCache
	{
		#region Cache API to store device specific information

		public static T Get<T>(int nodecode, String name) where T : class
		{
			try
			{
				if (nodecode != 0)
				{
					return UrbeCache.Retrieve<T>(typeof(T), TranslateKey(nodecode, name));
				}
			}
			catch {}

			return default(T);
		}

		public static void Set<T>(int nodecode, String name, T setted)
		{
			if (Equals(setted, default(T)))
				UrbeCache.Delete(typeof(T), TranslateKey(nodecode, name));
			else
				UrbeCache.Store(typeof(T), TranslateKey(nodecode, name), setted);
		}

		private static String TranslateKey(int nodecode, String name)
		{
			return String.Format("DevCache:{0}:{1}", nodecode, name);
		}

		#endregion
	}
}
