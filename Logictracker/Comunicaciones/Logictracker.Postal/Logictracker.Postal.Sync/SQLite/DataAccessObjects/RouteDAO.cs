#region Usings

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Urbetrack.DAL.Factories;
using Urbetrack.Postal.Sync.Enums;
using Urbetrack.Postal.Sync.Helpers;
using Urbetrack.Postal.Sync.SQLite.DataAccessObjects.BaseClasses;
using Urbetrack.Types.BusinessObjects.Postal;
using Urbetrack.Types.ValueObject.Postal;

#endregion

namespace Urbetrack.Postal.Sync.SQLite.DataAccessObjects
{
    /// <summary>
    /// Sqlite route data access class.
    /// </summary>
    public class RouteDAO : BaseDAO<Ruta>
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new sqlite route data access class using the given connection and dao factory.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="daoFactory"></param>
        public RouteDAO(SQLiteConnection connection, DAOFactory daoFactory) : base(connection, daoFactory) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the imported routes in urbetrack.
        /// </summary>
        /// <returns></returns>
        public void UpdateRoutesInUrbetrack()
        {
            var routes = GetRoutes();

            UpdateRoutes(routes);
        }

        /// <summary>
        /// Deletes routes from mobile to avoid updating them again at Urbetrack.
        /// </summary>
        public void DeleteRoutes() { ExecuteNonQuery("delete from rutas;"); }

        /// <summary>
        /// Gets the distributor assigned to the routes sincronized to the currently connected device.
        /// </summary>
        /// <returns></returns>
        public Distribuidor GetPdaDistributor()
        {
            var reader = ExecuteReader("select codigo_distribuidor_ptm from rutas limit 1;");

            var code = reader.Read() ? reader.GetString(0) : String.Empty;

            return code != String.Empty ? DaoFactory.DistribuidorDAO.FindByPtmCode(code) : null;
        }

        public string GetPdaRoute()
        {
            var reader = ExecuteReader("select numero_de_ruta from rutas limit 1;");

            var code = reader.Read() ? reader.GetString(0) : String.Empty;
            reader.Close();
            return code;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Defines the current
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<String, DbType> GetParameters()
        {
            return new Dictionary<String, DbType>
                       {
                           {"@id", DbType.Int32},
                           {"@numero_de_ruta", DbType.String},
                           {"@numero_de_item", DbType.Int16},
                           {"@codigo_distribuidor_ptm", DbType.String},
                           {"@distribuidor", DbType.Int32},
                           {"@codigo_tipo_de_servicio_ptm", DbType.String},
                           {"@tipo_de_servicio", DbType.Int32},
                           {"@cliente", DbType.Int32},
                           {"@destinatario", DbType.String},
                           {"@direccion", DbType.String},
                           {"@id_pieza", DbType.Int32},
                           {"@pieza", DbType.String},
                           {"@estado", DbType.Int32},
                           {"@motivo", DbType.Int32},
                           {"@latitud", DbType.Double},
                           {"@longitud", DbType.Double},
                           {"@foto", DbType.Binary},
                           {"@fecha_foto", DbType.DateTime},
                           {"@fecha_motivo", DbType.DateTime},
                           {"@lateral1", DbType.String},
                           {"@lateral2", DbType.String},
                           {"@referencia", DbType.String},
                           {"@dispositivo", DbType.Int32},
                           {"@fecha_baja", DbType.DateTime},
                           {"@fecha_modificacion", DbType.DateTime}
                       };
        }

        /// <summary>
        /// Maps the specified route into the pre-defined parameters.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        protected override void MapObject(SQLiteCommand command, Ruta data)
        {
            SetParameterValue(command, "@id", data.Id);
            SetParameterValue(command, "@numero_de_ruta", data.NumeroRuta);
            SetParameterValue(command, "@numero_de_item", data.Item);
            SetParameterValue(command, "@codigo_distribuidor_ptm", data.CodigoDistribuidor);
            SetParameterValue(command, "@distribuidor", data.Distribuidor.Id);
            SetParameterValue(command, "@codigo_tipo_de_servicio_ptm", data.CodigoTipoServicio);
            SetParameterValue(command, "@tipo_de_servicio", data.TipoServicio.Id);
            SetParameterValue(command, "@cliente", data.Cliente);
            SetParameterValue(command, "@destinatario", data.Destinatario);
            SetParameterValue(command, "@direccion", data.Direccion);
            SetParameterValue(command, "@id_pieza", data.IdPieza);
            SetParameterValue(command, "@pieza", data.Pieza);
            SetParameterValue(command, "@estado", data.Estado);
            SetParameterValue(command, "@motivo", null);
            SetParameterValue(command, "@latitud", null);
            SetParameterValue(command, "@longitud", null);
            SetParameterValue(command, "@foto", null);
            SetParameterValue(command, "@fecha_foto", null);
            SetParameterValue(command, "@fecha_motivo", null);
            SetParameterValue(command, "@lateral1", null);
            SetParameterValue(command, "@lateral2", null);
            SetParameterValue(command, "@referencia", null);
            SetParameterValue(command, "@dispositivo", data.Dispositivo.Id);
            SetParameterValue(command, "@fecha_baja", null);
            SetParameterValue(command, "@fecha_modificacion", data.FechaModificacion);
        }

        /// <summary>
        /// Gets the associated sqlite table name.
        /// </summary>
        /// <returns></returns>
        protected override String GetTableName() { return "rutas"; }

        /// <summary>
        /// Gets the routes that should be inserted for the specified distributor.
        /// </summary>
        /// <param name="distributor"></param>
        /// <returns></returns>
        protected override IEnumerable<Ruta> GetInsertData(GrupoRuta distributor) { return DaoFactory.RutaDAO.GetRoutes(distributor); }

        /// <summary>
        /// Gets the sqlite route table creation script.
        /// </summary>
        /// <returns></returns>
        protected override String GetTableCreationScript()
        {
            return @"
                CREATE TABLE rutas(
	                [id] [int] NOT NULL,
	                [numero_de_ruta] [varchar] NOT NULL,
	                [numero_de_item] [smallint] NOT NULL,
	                [codigo_distribuidor_ptm] [varchar] NOT NULL,
	                [distribuidor] [int] NULL,
	                [codigo_tipo_de_servicio_ptm] [varchar] NOT NULL,
	                [tipo_de_servicio] [int] NULL,
	                [cliente] [int] NOT NULL,
	                [destinatario] [varchar] NOT NULL,
	                [direccion] [varchar] NOT NULL,
	                [id_pieza] [int] NOT NULL,
	                [pieza] [varchar] NOT NULL,
	                [estado] [int] NOT NULL,
	                [motivo] [int] NULL,
	                [latitud] [float] NULL,
	                [longitud] [float] NULL,
	                [foto] [blob] NULL,
	                [fecha_foto] [datetime] NULL,
	                [fecha_motivo] [datetime] NULL,
	                [lateral1] [varchar] NULL,
	                [lateral2] [varchar] NULL,
	                [referencia] [varchar] NULL,
	                [dispositivo] [int] NULL,
	                [fecha_baja] [datetime] NULL,
	                [fecha_modificacion] [datetime] NOT NULL
                );";
        }

        /// <summary>
        /// Prepares the specified route before inserting it into the sqlite export database.
        /// </summary>
        /// <param name="data"></param>
        protected override void PrepareForInsert(Ruta data)
        {
            var deviceCode = ActiveSyncHelper.GetDeviceCode();
            var serviceType = DaoFactory.TipoServicioDAO.FindByCode(data.CodigoTipoServicio);
            var distributor = DaoFactory.DistribuidorDAO.FindByPtmCode(data.CodigoDistribuidor);

            data.Dispositivo = DaoFactory.DispositivoDAO.GetByCode(deviceCode);
            data.TipoServicio = serviceType;
            data.Distribuidor = distributor;
            data.FechaModificacion = DateTime.Now;
            data.Estado = RouteState.Transfered.GetValue();

            DaoFactory.RutaDAO.Update(data);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sincronize the modified routes into Urbetrack.
        /// </summary>
        /// <param name="routes"></param>
        private void UpdateRoutes(IEnumerable<RutaVO> routes)
        {
            foreach (var route in routes)
            {
                var ruta = DaoFactory.RutaDAO.FindById(route.Id);

                var reason = route.Motivo.HasValue ? DaoFactory.MotivoDAO.FindById(route.Motivo.Value) : null;

                ruta.Estado = RouteState.Closed.GetValue();
                ruta.Motivo = reason;
                ruta.Latitud = route.Latitud;
                ruta.Longitud = route.Longitud;
                ruta.Foto = route.Foto;
                ruta.FechaFoto = route.FechaFoto;
                ruta.FechaMotivo = route.FechaMotivo;
                ruta.Lateral1 = route.Lateral1;
                ruta.Lateral2 = route.Lateral2;
                ruta.Referencia = route.Referencia;

                DaoFactory.RutaDAO.Update(ruta);
            }
        }

        /// <summary>
        /// Maps the retrieved routes into value objects routes.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static IEnumerable<RutaVO> MapRoutes(IDataReader reader)
        {
            var routes = new List<RutaVO>();

            while (reader.Read())
            {
                var route = new RutaVO
                                {
                                    Id = reader.GetInt32(0),
                                    NumeroDeRuta = reader.GetString(1),
                                    NumeroDeItem = reader.GetInt16(2),
                                    CodigoDistribuidor = reader.GetString(3),
                                    Distribuidor = reader.GetInt32(4),
                                    CodigoTipoServicio = reader.GetString(5),
                                    TipoServicio = reader.GetInt32(6),
                                    Cliente = reader.GetInt32(7),
                                    Destinatario = reader.GetString(8),
                                    Direccion = reader.GetString(9),
                                    IdPieza = reader.GetInt32(10),
                                    Pieza = reader.GetString(11),
                                    Estado = reader.GetInt32(12),
                                    Motivo = reader.IsDBNull(13) ? null : (Int32?) reader.GetInt32(13),
                                    Latitud = reader.IsDBNull(14) ? null : (Double?) reader.GetDouble(14),
                                    Longitud = reader.IsDBNull(15) ? null : (Double?) reader.GetDouble(15),
                                    Foto = reader.IsDBNull(16) ? null : (Byte[])reader.GetValue(16),
                                    FechaFoto = reader.IsDBNull(17) ? null : (DateTime?) reader.GetDateTime(17),
                                    FechaMotivo = reader.IsDBNull(18) ? null : (DateTime?) reader.GetDateTime(18),
                                    Lateral1 = reader.IsDBNull(19) ? null : reader.GetString(19),
                                    Lateral2 = reader.IsDBNull(20) ? null : reader.GetString(20),
                                    Referencia = reader.IsDBNull(21) ? null : reader.GetString(21),
                                    Dispositivo = reader.IsDBNull(22) ? null : (Int32?) reader.GetInt32(22),
                                    FechaBaja = reader.IsDBNull(23) ? null : (DateTime?) reader.GetDateTime(23),
                                    FechaModificacion = reader.GetDateTime(24)
                                };

                routes.Add(route);
            }

            return routes;
        }

        /// <summary>
        /// Retrieves modified routes from the connected device.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RutaVO> GetRoutes()
        {
            var reader = ExecuteReader("select * from rutas where estado > 1;");

            return MapRoutes(reader);
        }

        #endregion
    }
}