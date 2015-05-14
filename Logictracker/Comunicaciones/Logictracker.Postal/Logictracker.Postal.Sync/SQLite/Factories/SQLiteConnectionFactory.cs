#region Usings

using System;
using System.Data.SQLite;
using System.IO;

#endregion

namespace Urbetrack.Postal.Sync.SQLite.Factories
{
    /// <summary>
    /// Helper class for managing sqlite database.
    /// </summary>
    public static class SQLiteConnectionFactory
    {
        #region Public Methods

        /// <summary>
        /// Opens a connection to the local sqllite export database.
        /// </summary>
        /// <param name="databasePath">The path to allocate the database file.</param>
        /// <param name="databaseFileName">The database file name.</param>
        /// <param name="cretaeFile">Determines wither to use the existent database file or to create it.</param>
        /// <returns></returns>
        public static SQLiteConnection OpenSqliteConnection(String databasePath, String databaseFileName, Boolean cretaeFile)
        {
            var databaseFile = Path.Combine(databasePath, databaseFileName);

            if (cretaeFile) CreateDatabaseFile(databasePath, databaseFile);

            var connectionString = String.Format("Data Source={0};Version=3;", databaseFile);

            var connection = new SQLiteConnection(connectionString);

            connection.Open();

            return connection;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Deletes previous existent files and creates a new database file.
        /// </summary>
        /// <param name="databasePath"></param>
        /// <param name="databaseFile"></param>
        private static void CreateDatabaseFile(String databasePath, String databaseFile)
        {
            if (!Directory.Exists(databasePath)) Directory.CreateDirectory(databasePath);
            else if (File.Exists(databaseFile)) File.Delete(databaseFile);

            SQLiteConnection.CreateFile(databaseFile);
        }

        #endregion
    }
}