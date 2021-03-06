#region Usings

using System;
using System.Collections;
using System.Data;
using System.Globalization;
using Common.Logging;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;

#endregion

namespace Quartz.Util
{
	/// <summary>
	/// Manages a collection of IDbProviders, and provides transparent access
	/// to their database.
	/// </summary>
	/// <seealso cref="IDbProvider" />
	/// <author>James House</author>
	/// <author>Sharada Jambula</author>
	/// <author>Mohammad Rezaei</author>
	public class DBConnectionManager
	{
        public const string PropertyDbPrefix = "quartz.db.";
        
        private static readonly DBConnectionManager instance = new DBConnectionManager();
	    private static readonly ILog log = LogManager.GetLogger(typeof (DBConnectionManager));

        private readonly IDictionary providers = new Hashtable();

		/// <summary> 
		/// Get the class instance.
		/// </summary>
		/// <returns> an instance of this class
		/// </returns>
		public static DBConnectionManager Instance
		{
			get
			{
				// since the instance variable is initialized at class loading time,
				// it's not necessary to synchronize this method */
				return instance;
			}
		}


		/// <summary> 
		/// Private constructor
		/// </summary>
		private DBConnectionManager()
		{
		}

        /// <summary>
        /// Adds the connection provider.
        /// </summary>
        /// <param name="dataSourceName">Name of the data source.</param>
        /// <param name="provider">The provider.</param>
        public virtual void AddConnectionProvider(string dataSourceName, IDbProvider provider)
		{
            log.Info(string.Format("Registering datasource '{0}' with db provider: '{1}'", dataSourceName, provider));
			providers[dataSourceName] = provider;
		}

		/// <summary>
		/// Get a database connection from the DataSource with the given name.
		/// </summary>
		/// <returns> a database connection </returns>
		public virtual IDbConnection GetConnection(string dsName)
		{
		    var provider = GetDbProvider(dsName);

			return provider.CreateConnection();
		}

		/// <summary> 
		/// Shuts down database connections from the DataSource with the given name,
		/// if applicable for the underlying provider.
		/// </summary>
		/// <returns> a database connection </returns>
		public virtual void Shutdown(string dsName)
		{
		    var provider = GetDbProvider(dsName);
			provider.Shutdown();
		}

	    public DbMetadata GetDbMetadata(string dsName)
	    {
            return GetDbProvider(dsName).Metadata;
        }

        /// <summary>
        /// Gets the db provider.
        /// </summary>
        /// <param name="dsName">Name of the ds.</param>
        /// <returns></returns>
	    public IDbProvider GetDbProvider(string dsName)
	    {
            if (dsName == null || dsName.Length == 0)
            {
                throw new ArgumentException("DataSource name cannot be null or empty", "dsName");
            }

            if (!providers.Contains(dsName) || providers[dsName] == null)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "There is no DataSource named '{0}'", dsName));
            }
            return (IDbProvider) providers[dsName];
        }
	}
}
