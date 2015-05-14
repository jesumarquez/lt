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
    /// Sqlite service type data acccess class.
    /// </summary>
    public class ServiceTypeDAO : BaseDAO<TipoServicio>
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new sqlite service type sdata access class using the givenn connection and dao factory.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="daoFactory"></param>
        public ServiceTypeDAO(SQLiteConnection connection, DAOFactory daoFactory) : base(connection, daoFactory) { }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the service type associated to the specified distributor that must be inserted into the sqlite export database.
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        protected override IEnumerable<TipoServicio> GetInsertData(GrupoRuta distributor) { return DaoFactory.TipoServicioDAO.GetServiceTypes(distributor.Distribuidor); }

        /// <summary>
        /// Defines the parameters associated to a service type.
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<String, DbType> GetParameters()
        {
            return new Dictionary<String, DbType>
                       {
                           {"@codigo", DbType.String},
                           {"@descripcion", DbType.String},
                           {"@icono", DbType.Int32},
                           {"@descripcion_corta", DbType.String},
                           {"@confirma", DbType.Int16},
                           {"@con_foto", DbType.Int16},
                           {"@con_laterales", DbType.Int16},
                           {"@con_referencia", DbType.Int16},
                           {"@con_gps", DbType.Int16},
                           {"@fecha_modificacion", DbType.DateTime},
                           {"@id", DbType.Int32},
                           {"@fecha_baja", DbType.DateTime}
                       };
        }

        /// <summary>
        /// Maps the current object into the pre-defined parameters.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        protected override void MapObject(SQLiteCommand command, TipoServicio data)
        {
            SetParameterValue(command, "@codigo", data.Codigo);
            SetParameterValue(command, "@descripcion", data.Descripcion);
            SetParameterValue(command, "@icono", null);
            SetParameterValue(command, "@descripcion_corta", data.DescripcionCorta);
            SetParameterValue(command, "@confirma", data.ConAcuse);
            SetParameterValue(command, "@con_foto", data.ConFoto);
            SetParameterValue(command, "@con_laterales", data.ConLaterales);
            SetParameterValue(command, "@con_referencia", data.ConReferencia);
            SetParameterValue(command, "@con_gps", data.ConGPS);
            SetParameterValue(command, "@fecha_modificacion", data.FechaModificacion);
            SetParameterValue(command, "@id", data.Id);
            SetParameterValue(command, "@fecha_baja", data.FechaBaja);
        }

        /// <summary>
        /// Gets the sqlite table name associated to the current type.
        /// </summary>
        /// <returns></returns>
        protected override String GetTableName() { return "tipos_de_servicio"; }

        /// <summary>
        /// Gets the service type sqlite table creation script.
        /// </summary>
        /// <returns></returns>
        protected override String GetTableCreationScript()
        {
            return @"
                CREATE TABLE tipos_de_servicio(
	                [codigo] [varchar] NOT NULL,
	                [descripcion] [varchar] NOT NULL,
	                [icono] [int] NULL,
	                [descripcion_corta] [varchar] NULL,
	                [confirma] [smallint] NULL,
	                [con_foto] [smallint] NULL,
	                [con_laterales] [smallint] NULL,
	                [con_referencia] [smallint] NULL,
	                [con_gps] [smallint] NULL,
	                [fecha_modificacion] [datetime] NOT NULL,
	                [id] [int] NOT NULL,
	                [fecha_baja] [datetime] NULL
                );";
        }

        /// <summary>
        /// Validates the specified service type before inserting it into database.
        /// </summary>
        /// <param name="data"></param>
        protected override void Validate(TipoServicio data)
        {
            if (!data.ConAcuse.HasValue || !data.ConFoto.HasValue || !data.ConGPS.HasValue || !data.ConLaterales.HasValue ||!data.ConReferencia.HasValue || String.IsNullOrEmpty(data.DescripcionCorta))
                throw new Exception("Existen tipos de servicio invalidos. Verifique que los mismos tengan correctamente cargada su descripción corta y comportamiento");
        }

        #endregion
    }
}
