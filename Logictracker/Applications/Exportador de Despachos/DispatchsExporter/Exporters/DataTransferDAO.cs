using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects;
using NHibernate;

namespace DispatchsExporter.Exporters
{
    public class DataTransferDAO
    {
        #region Protected Properties

        protected readonly DAOFactory daof = new DAOFactory();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates and adds a new parameter with the givenn values in the command.
        /// </summary>
        /// <param name="command">The command to add the parameter to.</param>
        /// <param name="type">The parameter db type.</param>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        protected static void CreateParameter(IDbCommand command, DbType type, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.DbType = type;
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generates the JDEdwards file for an specific center.
        /// </summary>
        /// <param name="center"></param>
        public void GenerateJDEdwardsFileForCenter(string center)
        {
            using (var command = daof.Session.Connection.CreateCommand())
            {
                command.CommandText = "sp_makeDespachosTextFileForJD_Edwards";
                command.CommandType = CommandType.StoredProcedure;
                CreateParameter(command, DbType.String, "@center", center);
                CreateParameter(command, DbType.String, "@FilePath", ConfigurationManager.AppSettings["JDEdwardsDirectory"]);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Reassigns the dispatchs to the given mobile.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mobileID"></param>
        public void UpdateDispatch(int id, int mobileID)
        {
            using (var command = daof.Session.Connection.CreateCommand())
            {
                var text = String.Format("update opecomb02 set rela_parenti03 = {0} where id_opecomb02 = {1}",
                                         mobileID,id);
                command.CommandType = CommandType.Text;
                command.CommandText = text;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Imports the FAFNIR levels for the selected linea.
        /// </summary>
        /// <param name="linea"></param>
        public void ImportNivelesFromCenter(int linea)
        {
            string connection = GetConnectionsStringByLinea(linea);

            var conn = connection.Split('.');

            var idLinea = Convert.ToInt32(conn[0]);
            var connStr = conn[1];
            var Tanque = daof.TanqueDAO.FindMainTankByLinea(idLinea);

            if (Tanque == null) throw new Exception("Tanque no encontrado para la Linea seleccionada");

            try{
                using (var command = daof.Session.Connection.CreateCommand())
                {
                    command.CommandText = "extract_SensorData";
                    command.CommandType = CommandType.StoredProcedure;
                    CreateParameter(command, DbType.Int32, "@idTanque", Tanque.Id);
                    CreateParameter(command, DbType.String, "@LinkedServer", connStr);
                    CreateParameter(command, DbType.String, "@msgQueue", ConfigurationManager.AppSettings["queue"]);
                    command.ExecuteNonQuery();
                }
            }catch(Exception )
            {
                //T.WARNING("Error al exportar los remitos. Tanque:{0};LinkedServer:{1};Queue:{2}\r\n.", Tanque.Id, connStr, ConfigurationManager.AppSettings["queue"]);
                //T.EXCEPTION(e);
                var line = daof.LineaDAO.FindById(linea);
                throw new Exception(String.Format("Exportador de Niveles: Error al intentar conectarse a {0}. \r\nIntente en unos momentos. ", line.Descripcion));
            }
        }


        /// <summary>
        /// Imports the SIRAC Remitos for the selected Linea.
        /// </summary>
        /// <param name="linea"></param>
        public void ImportRemitosFromCenter(int linea)
        {

            string connection = GetConnectionsStringByLinea(linea);

            var idLinea = Convert.ToInt32(connection.Split('.')[0]);
            var connStr = connection.Split('.')[1];
            var Tanque = daof.TanqueDAO.FindMainTankByLinea(idLinea);

            if (Tanque == null) throw new Exception("Tanque no encontrado para la Linea seleccionada");

            try{
                using (var command = daof.Session.Connection.CreateCommand())
                {
                    command.CommandText = "extract_RemitosData";
                    command.CommandType = CommandType.StoredProcedure;
                    CreateParameter(command, DbType.Int32, "@idTanque", Tanque.Id);
                    CreateParameter(command, DbType.String, "@LinkedServer", connStr);
                    CreateParameter(command, DbType.String, "@msgQueue", ConfigurationManager.AppSettings["queue"]);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception )
            {
                //T.WARNING("Error al exportar los remitos. Tanque:{0};LinkedServer:{1};Queue:{2}\r\n.", Tanque.Id, connStr, ConfigurationManager.AppSettings["queue"]);
                //T.EXCEPTION(e);
                var line = daof.LineaDAO.FindById(linea);
                throw new Exception(String.Format("Exportador de Remitos: Error al intentar conectarse a {0}. \r\nIntente en unos momentos. ", line.Descripcion));
            }
        }

        /// <summary>
        /// Consolidates the Vehicles around all the bases.
        /// </summary>
        public void ConsolidateVehicles()
        {
            using(var command = daof.Session.Connection.CreateCommand())
            {
                command.CommandText = "consolidate_Vehicles";
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Imports to database all SIRAC dispatchs from all the bases allowed for the selected Perfil.
        /// </summary>
        /// <param name="user"></param>
        public void ImportDespachosFromAllCentersByUsuario(Usuario user)
        {
            var connections = GetConnectionsStringByPefil(user);

            foreach (var c in connections)
            {
                var idLinea = Convert.ToInt32(c.Split('.')[0]);
                var connStr = c.Split('.')[1];
                var Tanque = daof.TanqueDAO.FindMainTankByLinea(idLinea);

                if (Tanque == null) throw new Exception("Tanque no encontrado para la Linea seleccionada");

                try
                {
                    using (var command = daof.Session.Connection.CreateCommand())
                    {
                        command.CommandText = "import_DespachosData";
                        CreateParameter(command, DbType.Int32, "@idTanque", Tanque.Id);
                        CreateParameter(command, DbType.String, "@LinkedServer", connStr);
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                    }
                }catch(Exception )
                {
                    //T.WARNING("Importador de Despachos. Tanque:{0};Server:{1}\r\n",Tanque.Id,connStr);
                    //T.EXCEPTION(e);
                    var linea = daof.LineaDAO.FindById(idLinea);
                    throw new Exception(String.Format("Importador de Despachos: Error al intentar conectarse a {0}. \r\nIntente en unos momentos. ",linea.Descripcion));
                }
            }
        }

        /// <summary>
        /// Exported all dispatchs to Logictracker.
        /// </summary>
        /// <param name="linea"></param>
        public void ExportDespachosToLogictracker(int linea)
        {
            var Tanque = daof.TanqueDAO.FindMainTankByLinea(linea);

            if (Tanque == null) throw new Exception("Tanque no encontrado para la Linea seleccionada");

            try
            {
                using (var command = daof.Session.Connection.CreateCommand())
                {
                    command.CommandText = "export_DespachosData";
                    CreateParameter(command, DbType.Int32, "@idTanque", Tanque.Id);
                    CreateParameter(command, DbType.Int32, "@idLinea", linea);
                    CreateParameter(command, DbType.String, "@msgQueue", ConfigurationManager.AppSettings["queue"]);
                    command.CommandType = CommandType.StoredProcedure;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw new Exception("Error al exportar los despachos hacia Logictracker. \r\nIntente en unos momentos. ");
            }
        }

        #endregion

        #region Private Methods

        private static IEnumerable<string> GetConnectionsStringByPefil(Usuario user)
        {
            var connStr =
                ConfigurationManager.AppSettings[String.Concat("SQL.",user.Lineas.IsEmpty ? "all" : (from Linea l in user.Lineas select l).First().Id.ToString())];

            if( connStr == null || connStr.Equals(String.Empty)) throw new Exception("Error al obtener la ConnectionString para el perfil asociado");

            return connStr.Split(';');
        }

        private static string GetConnectionsStringByLinea(int linea)
        {
            var connStr = ConfigurationManager.AppSettings[String.Concat("SQL.",linea.ToString())];

            if (connStr == null || connStr.Equals(String.Empty)) throw new Exception("Error al obtener la ConnectionString para la Linea seleccionada");

            return connStr;
        }

        #endregion
    }
}