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
    /// Reasons sqlite data access class.
    /// </summary>
    public class ReasonDAO : BaseDAO<Motivo>
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new reasons data aceess class using the givenn connection and dao factory.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="daoFactory"></param>
        public ReasonDAO(SQLiteConnection connection, DAOFactory daoFactory) : base(connection, daoFactory) { }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Inserts the reason codes specified for the current distributor route.
        /// </summary>
        /// <param name="distributor"></param>
        protected override IEnumerable<Motivo> GetInsertData(GrupoRuta distributor) { return GetReasons(distributor.Distribuidor); }

        /// <summary>
        /// Defines all the parameters associated to the current table.
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<String, DbType> GetParameters()
        {
            return new Dictionary<String, DbType>
                       {
                           {"@id", DbType.Int32},
                           {"@cliente", DbType.Int32},
                           {"@descripcion", DbType.String},
                           {"@es_entrega", DbType.Boolean},
                           {"@orden", DbType.Int32},
                           {"@es_devolucion", DbType.Boolean},
                           {"@es_gestion", DbType.Boolean},
                           {"@fecha_modificacion", DbType.DateTime},
                           {"@fecha_baja", DbType.DateTime},
                           {"@codigo", DbType.String}
                       };
        }

        /// <summary>
        /// Maps the current reason into the pre-defined parameters.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        protected override void MapObject(SQLiteCommand command, Motivo data)
        {
            SetParameterValue(command, "@id", data.Id);
            SetParameterValue(command, "@descripcion", data.Descripcion);
            SetParameterValue(command, "@es_entrega", data.EsEntrega);
            SetParameterValue(command, "@orden", data.Orden);
            SetParameterValue(command, "@es_devolucion", data.EsDevolucion);
            SetParameterValue(command, "@es_gestion", data.EsGestion);
            SetParameterValue(command, "@fecha_modificacion", data.FechaModificacion);
            SetParameterValue(command, "@fecha_baja", data.FechaBaja);
            SetParameterValue(command, "@codigo", data.Codigo);
        }

        /// <summary>
        /// Gets the reons table name.
        /// </summary>
        /// <returns></returns>
        protected override String GetTableName() { return "motivos"; }

        /// <summary>
        /// Gets the script for creating the reasons table name.
        /// </summary>
        /// <returns></returns>
        protected override String GetTableCreationScript()
        {
            return @"
                CREATE TABLE motivos(
	                [id] [int] NOT NULL,
	                [cliente] [int] NULL,
	                [descripcion] [varchar] NOT NULL,
	                [es_entrega] [bit] NULL,
	                [orden] [int] NOT NULL,
	                [es_devolucion] [bit] NULL,
	                [es_gestion] [bit] NULL,
	                [fecha_modificacion] [datetime] NOT NULL,
	                [fecha_baja] [datetime] NULL,
	                [codigo] [varchar] NOT NULL
                );";
        }

        /// <summary>
        /// Validates the givenn reason before inserting it into database.
        /// </summary>
        /// <param name="data"></param>
        protected override void Validate(Motivo data)
        {
            if (!data.EsDevolucion.HasValue || !data.EsEntrega.HasValue || !data.EsGestion.HasValue) 
                throw new Exception("Existen motivos mal configurados en Urbetrack. Valide que todos los motivos tengan definido si es una entrega, devolución o gestión.");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the reasons associated to the 
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        private IEnumerable<Motivo> GetReasons(Distribuidor distributor) { return DaoFactory.MotivoDAO.GetReasons(distributor); }

        #endregion
    }
}