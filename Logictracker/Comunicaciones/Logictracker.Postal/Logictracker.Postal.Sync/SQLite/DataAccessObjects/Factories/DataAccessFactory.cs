#region Usings

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Urbetrack.DAL.Factories;
using Urbetrack.Postal.Sync.SQLite.DataAccessObjects.Interfaces;
using Urbetrack.Postal.Sync.SQLite.Factories;

#endregion

namespace Urbetrack.Postal.Sync.SQLite.DataAccessObjects.Factories
{
    /// <summary>
    /// Sqlite data access factory class.
    /// </summary>
    public class DataAccessFactory : IDisposable
    {
        #region Private Properties

        /// <summary>
        /// Dictionary for holding the currently instanciated daos.
        /// </summary>
        private Dictionary<Type, IGenericSQLiteDAO> _daos = new Dictionary<Type, IGenericSQLiteDAO>();

        /// <summary>
        /// Current sqlite connection.
        /// </summary>
        private readonly SQLiteConnection _connection;

        /// <summary>
        /// Data access class.
        /// </summary>
        private DAOFactory _daoFactory;
        private DAOFactory DaoFactory { get { return _daoFactory ?? (_daoFactory = new DAOFactory()); } }

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new sqlite data access factory class opening a connection with the specified values.
        /// </summary>
        /// <param name="databasePath"></param>
        /// <param name="databaseFileName"></param>
        /// <param name="createDatabase"></param>
        public DataAccessFactory(String databasePath, String databaseFileName, Boolean createDatabase)
        {
            _connection = SQLiteConnectionFactory.OpenSqliteConnection(databasePath, databaseFileName, createDatabase);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Sqlite distributors data access class.
        /// </summary>
        public DistributorDAO DistribuidorDAO { get { return GetDao<DistributorDAO>(); } }

        /// <summary>
        /// Sqlite reasons data access class.
        /// </summary>
        public ReasonDAO ReasonDAO { get { return GetDao<ReasonDAO>(); } }

        /// <summary>
        /// Sqlite service type data access class.
        /// </summary>
        public ServiceTypeDAO ServiceTypeDAO { get { return GetDao<ServiceTypeDAO>(); } }

        /// <summary>
        /// Sqlite route data access class.
        /// </summary>
        public RouteDAO RouteDAO { get { return GetDao<RouteDAO>(); } }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets an instance of the dao associated to the givenn type.
        /// </summary>
        /// <returns></returns>
        private T GetDao<T>() where T : IGenericSQLiteDAO
        {
            var type = typeof(T);

            if (!_daos.ContainsKey(type))
            {
                var constructor = type.GetConstructor(new [] { typeof(SQLiteConnection), typeof(DAOFactory) });

                var dao = constructor.Invoke(new Object[] { _connection, DaoFactory }) as IGenericSQLiteDAO;

                _daos.Add(type, dao);
            }

            return (T)_daos[type];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose all assigned resources.
        /// </summary>
        public void Dispose()
        {
            _daos.Clear();

            _daos = null;

            _connection.Close();

            _connection.Dispose();

            if (_daoFactory != null) _daoFactory.Dispose();
        }

        #endregion
    }
}