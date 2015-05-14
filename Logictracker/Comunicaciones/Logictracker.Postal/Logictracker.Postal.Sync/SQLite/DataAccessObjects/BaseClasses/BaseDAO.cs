#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Urbetrack.DAL.Factories;
using Urbetrack.Postal.Sync.SQLite.DataAccessObjects.Interfaces;
using Urbetrack.Types.BusinessObjects.Postal;

#endregion

namespace Urbetrack.Postal.Sync.SQLite.DataAccessObjects.BaseClasses
{
    /// <summary>
    /// Base data access class for sqlite database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseDAO<T> : IGenericSQLiteDAO
    {
        #region Private Properties

        /// <summary>
        /// Current sqlite database connection.
        /// </summary>
        private readonly SQLiteConnection _connection;

        #endregion

        #region Protected Properties

        /// <summary>
        /// Data access factory class.
        /// </summary>
        protected readonly DAOFactory DaoFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new data access class using the givenn sqlite connection and dao factory.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="daoFactory"></param>
        protected BaseDAO(SQLiteConnection connection, DAOFactory daoFactory)
        {
            _connection = connection;

            DaoFactory = daoFactory;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inserts all necesary data associated to the specified distributor into the export sqlite database.
        /// </summary>
        /// <param name="distributor"></param>
        public void InsertForDistributor(GrupoRuta distributor)
        {
            CreateTable();

            var data = GetInsertData(distributor);

            Insert(data);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Executes the givenn non query sql command.
        /// </summary>
        /// <param name="text"></param>
        protected void ExecuteNonQuery(String text)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                using (var command = _connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = text;

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        /// <summary>
        /// Gets the result of executing the givenn sqlite script.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected SQLiteDataReader ExecuteReader(String text)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = text;

                return command.ExecuteReader();
            }
        }

        /// <summary>
        /// Gets the list of available parameters.
        /// </summary>
        /// <returns></returns>
        protected abstract Dictionary<String, DbType> GetParameters();

        /// <summary>
        /// Maps the current object into the pre-defined parameters.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        protected abstract void MapObject(SQLiteCommand command, T data);

        /// <summary>
        /// Gets the table name for the current class.
        /// </summary>
        /// <returns></returns>
        protected abstract String GetTableName();

        /// <summary>
        /// Gets the list of objects to be insertes into the export sqlite database.
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        protected abstract IEnumerable<T> GetInsertData(GrupoRuta distributor);

        /// <summary>
        /// Updates the value of the specified parameter.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        protected void SetParameterValue(SQLiteCommand command, String parameter, Object value) { command.Parameters[parameter].Value = value; }

        /// <summary>
        /// Gets the script for table creation.
        /// </summary>
        /// <returns></returns>
        protected abstract String GetTableCreationScript();

        /// <summary>
        /// Prepares the specified object before insertng it into database.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void PrepareForInsert(T data) {}

        /// <summary>
        /// Validates the specified object before inserting it into the export database.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void Validate(T data) {}

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a list of all the parameters names.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static String GetParametersString(Dictionary<String, DbType> parameters)
        {
            return parameters.Keys.Aggregate(String.Empty, (current, parameter) => String.Concat(current, parameter, ",")).TrimEnd(',');
        }

        /// <summary>
        /// Adds all the specified parameters to the givenn sqlite command.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        private static void AddParameters(SQLiteCommand command, Dictionary<String, DbType> parameters) 
        {
            foreach (var parameter in parameters) command.Parameters.Add(parameter.Key, parameter.Value);
        }

        /// <summary>
        /// Creates the table within the sqlite export database.
        /// </summary>
        private void CreateTable()
        {
            var scipt = GetTableCreationScript();

            ExecuteNonQuery(scipt);
        }

        /// <summary>
        /// Inserts into databse the givenn objects.
        /// </summary>
        /// <param name="objects"></param>
        private void Insert(IEnumerable<T> objects)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                using (var command = _connection.CreateCommand())
                {
                    var tableName = GetTableName();
                    var parameters = GetParameters();
                    var parametersString = GetParametersString(parameters);

                    command.CommandType = CommandType.Text;
                    command.CommandText = String.Format("INSERT INTO {0} VALUES ({1});", tableName, parametersString);

                    AddParameters(command, parameters);

                    foreach (var data in objects)
                    {
                        Validate(data);

                        PrepareForInsert(data);

                        MapObject(command, data);

                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        #endregion
    }
}