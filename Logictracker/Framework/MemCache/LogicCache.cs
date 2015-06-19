using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Cache.Interfaces;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Memcached.ClientLibrary;

namespace Logictracker.Cache
{
	/// <summary>
    /// 	Class for managing Logictracker cached data.
	/// </summary>
	public static class LogicCache
	{
		#region Private Properties

		/// <summary>
		/// 	Memcache connections pool.
		/// </summary>
		private static readonly SockIOPool Pool;

		/// <summary>
        /// 	Logictracker default cache instance.
		/// </summary>
		private static readonly MemcachedClient DefaultCache;

		#endregion

		#region Constructors

		/// <summary>
        /// 	Instanciates Logictracker cache enviroment.
		/// </summary>
		static LogicCache()
		{
			try
			{
                Pool = SockIOPool.GetInstance(Config.Cache.LogictrackerCachePoolName);

                Pool.SetServers(Config.Cache.LogictrackerCacheServers);

				Pool.Initialize();

                DefaultCache = new MemcachedClient { PoolName = Config.Cache.LogictrackerCachePoolName, EnableCompression = false };
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// 	Gets the translated key for the specified object.
		/// </summary>
		/// <param name = "data"></param>
		/// <param name = "key"></param>
		/// <returns></returns>
		private static String TranslateKey(this IDataIdentify data, String key)
		{
			return String.Format("{0}[{1}][{2}]", data.GetType().Name.Replace("Proxy", String.Empty), data.Id, key);
		}

		/// <summary>
		/// 	Gets the translated key for the specified type.
		/// </summary>
		/// <param name = "type"></param>
		/// <param name = "key"></param>
		/// <returns></returns>
		private static String TranslateKey(Type type, String key)
		{
			return String.Format("{0}[{1}]", type.FullName, key);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// 	Determines if the givenn key is present in cache for the current object.
		/// </summary>
		/// <param name = "data"></param>
		/// <param name = "key"></param>
		/// <returns></returns>
		public static Boolean KeyExists(this IDataIdentify data, String key)
		{
			try
			{
				var translateKey = data.TranslateKey(key);

				return DefaultCache.KeyExists(translateKey);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);

				return false;
			}
		}

		/// <summary>
		/// 	Determines if the givenn key is present in cache for the current type.
		/// </summary>
		/// <param name = "type"></param>
		/// <param name = "key"></param>
		/// <returns></returns>
		public static Boolean KeyExists(Type type, String key)
		{
			try
			{
				var translateKey = TranslateKey(type, key);

				return DefaultCache.KeyExists(translateKey);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);

				return false;
			}
		}

		/// <summary>
		/// 	Stores the specified value associated to the identifier object.
		/// </summary>
		/// <param name = "data"></param>
		/// <param name = "key"></param>
		/// <param name = "value"></param>
		public static void Store(this IDataIdentify data, String key, Object value)
		{
			try
			{
				var translatedKey = data.TranslateKey(key);

				DefaultCache.Set(translatedKey, value);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);
			}
		}
        public static void Store(string key, Object value)
        {
            try
            {
                DefaultCache.Set(key, value);
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(LogicCache).FullName, e);
            }
        }

		/// <summary>
		/// 	Stores the specified value associated to the identifier object.
		/// </summary>
		/// <param name = "data"></param>
		/// <param name = "key"></param>
		/// <param name = "value"></param>
		/// <param name = "expiry"></param>
		public static void Store(this IDataIdentify data, String key, Object value, DateTime expiry)
		{
			try
			{
				var translatedKey = data.TranslateKey(key);

				DefaultCache.Set(translatedKey, value, expiry);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);
			}
		}

		/// <summary>
		/// 	Stores the specified value associated to the identifier type.
		/// </summary>
		/// <param name = "type"></param>
		/// <param name = "key"></param>
		/// <param name = "value"></param>
		public static void Store(Type type, String key, Object value)
		{
			try
			{
				var translatedKey = TranslateKey(type, key);

				DefaultCache.Set(translatedKey, value);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);
			}
		}

		/// <summary>
		/// 	Stores the specified value associated to the identifier type.
		/// </summary>
		/// <param name = "type"></param>
		/// <param name = "key"></param>
		/// <param name = "value"></param>
		/// <param name = "expiry"></param>
		public static void Store(Type type, String key, Object value, DateTime expiry)
		{
			try
			{
				var translatedKey = TranslateKey(type, key);

				DefaultCache.Set(translatedKey, value, expiry);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);
			}
		}

		/// <summary>
		/// 	Retrieves the value associated to the givenn key and identifier object.
		/// </summary>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "data"></param>
		/// <param name = "key"></param>
		/// <returns></returns>
		public static T Retrieve<T>(this IDataIdentify data, String key) where T : class
		{
			try
			{
				var translateKey = data.TranslateKey(key);

				var value = DefaultCache.Get(translateKey);

				return value as T;
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);

				return default(T);
			}
		}
        public static T Retrieve<T>(string key) where T : class
        {
            try
            {
                var value = DefaultCache.Get(key);

                return value as T;
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(LogicCache).FullName, e);

                return default(T);
            }
        }
		/// <summary>
		/// 	Deletes from cache the item associated to the specified key.
		/// </summary>
		/// <param name = "data"></param>
		/// <param name = "key"></param>
		/// <returns></returns>
		public static Boolean Delete(this IDataIdentify data, String key)
		{
			try
			{
				var translateKey = TranslateKey(data, key);

				return DefaultCache.Delete(translateKey);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);

				return false;
			}
		}
        public static Boolean Delete(string key)
        {
            try
            {
                return DefaultCache.Delete(key);
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(LogicCache).FullName, e);

                return false;
            }
        }
		/// <summary>
		/// 	Retrieves the value associated to the given key and identifier type.
		/// </summary>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "type"></param>
		/// <param name = "key"></param>
		/// <returns></returns>
		public static T Retrieve<T>(Type type, String key) where T : class
		{
			try
			{
				var translateKey = TranslateKey(type, key);

				var value = DefaultCache.Get(translateKey);

				return value as T;
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);

				return default(T);
			}
		}
        public static T RetrieveMultiple<T>(Type type, IEnumerable<string> keys) where T : class
        {
            try
            {
                var translateKeys = keys.Select(key => TranslateKey(type, key));

                var value = DefaultCache.GetMultiple(translateKeys.ToArray());

                return value as T;
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(LogicCache).FullName, e);

                return default(T);
            }
        }

		/// <summary>
		/// 	Deletes from cache the item associated to the specified key.
		/// </summary>
		/// <param name = "type"></param>
		/// <param name = "key"></param>
		/// <returns></returns>
		public static Boolean Delete(Type type, String key)
		{
			try
			{
				var translateKey = TranslateKey(type, key);

				return DefaultCache.Delete(translateKey);
			}
			catch (Exception e)
			{
				STrace.Exception(typeof(LogicCache).FullName, e);

				return false;
			}
		}

		#endregion
	}
}