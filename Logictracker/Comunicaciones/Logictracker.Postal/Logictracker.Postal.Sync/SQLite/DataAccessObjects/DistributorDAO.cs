#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Urbetrack.DAL.Factories;
using Urbetrack.Postal.Sync.SQLite.DataAccessObjects.BaseClasses;
using Urbetrack.Types.BusinessObjects.Postal;

#endregion

namespace Urbetrack.Postal.Sync.SQLite.DataAccessObjects
{
    /// <summary>
    /// Sqlite distributors data access class.
    /// </summary>
    public class DistributorDAO : BaseDAO<Distribuidor>
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new sqlite distributors data access using the givenn connection and daofactory.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="daoFactory"></param>
        public DistributorDAO(SQLiteConnection connection, DAOFactory daoFactory) : base(connection, daoFactory) { }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Inserts into sql export database the givenn distributor.
        /// </summary>
        /// <param name="distributor"></param>
        protected override IEnumerable<Distribuidor> GetInsertData(GrupoRuta distributor) { return new List<Distribuidor> { distributor.Distribuidor }; }

        /// <summary>
        /// Gets the script for creating the distributors table.
        /// </summary>
        /// <returns></returns>
        protected override string GetTableCreationScript()
        {
            return @"
                CREATE TABLE distribuidores(
	                [id] [int] NOT NULL,
	                [usuario] [varchar] NULL,
	                [clave] [varchar] NULL,
	                [codigo] [varchar] NOT NULL,
	                [nombre] [varchar] NOT NULL,
	                [fecha_modificacion] [datetime] NOT NULL,
	                [fecha_baja] [datetime] NULL
                );";
        }

        /// <summary>
        /// Defines all the aprameters associated to the current object.
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<String, DbType> GetParameters()
        {
            return new Dictionary<String, DbType>
                       {
                           {"@id", DbType.Int32},
                           {"@usuario", DbType.String},
                           {"@clave", DbType.String},
                           {"@codigo", DbType.String},
                           {"@nombre", DbType.String},
                           {"@fecha_modificacion", DbType.DateTime},
                           {"@fecha_baja", DbType.DateTime}
                       };
        }

        /// <summary>
        /// Maps the sql parameters with each object value.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        protected override void MapObject(SQLiteCommand command, Distribuidor data)
        {
            SetParameterValue(command, "@id", data.Id);
            SetParameterValue(command, "@usuario", data.Usuario);
            SetParameterValue(command, "@clave", data.Clave);
            SetParameterValue(command, "@codigo", data.Codigo);
            SetParameterValue(command, "@nombre", data.Nombre);
            SetParameterValue(command, "@fecha_modificacion", data.FechaModificacion);
            SetParameterValue(command, "@fecha_baja", data.FechaBaja);
        }

        /// <summary>
        /// Gets the sqlite export database table name.
        /// </summary>
        /// <returns></returns>
        protected override string GetTableName() { return "distribuidores"; }

        #endregion
    }
}