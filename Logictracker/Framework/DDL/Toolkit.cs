using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using Logictracker.DatabaseTracer.Core;

namespace Logictracker.DDL
{
    public static class Toolkit
    {
        #region Delegates

        public delegate void SqlBatchProgressHandler(string action, string entity, int totalSteps, int doneSteps);

        #endregion

        public static SqlDataReader RetrieveTable(SqlConnection connection, String table, SqlTransaction transaction)
        {
            try
            {
				STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/RETRIEVE_TABLE: {0}, catalogo={1}, tabla={2}, transaccional={3}", connection.DataSource, connection.Database, table, transaction != null ? "SI" : "NO"));
                table = "SELECT * FROM " + table;
                var command = new SqlCommand(table, connection, transaction);
                return command.ExecuteReader();
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(Toolkit).FullName, e);
                throw new Exception("SQLTOOLKIT/RETRIEVE_TABLE: imposible eliminar la tabla.", e);
            }
        }

        public static int TruncateTable(SqlConnection connection, String tableName, SqlTransaction transaction)
        {
            try
            {
                STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/TRUNCATE_TABLE: {0}, catalogo={1}, tabla={2}, transaccional={3}",
                        connection.DataSource, connection.Database, tableName, transaction != null ? "SI" : "NO"));
                var query = "DELETE FROM " + tableName;
                var command = new SqlCommand(query, connection, transaction);
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Toolkit).FullName,e);
                throw new Exception("SQLTOOLKIT/TRUNCATE_TABLE: imposible truncar la tabla.", e);
            }
        }

        public static int CountTableRows(SqlConnection connection, String tableName, SqlTransaction transaction)
        {
            try
            {
                var commandRowCount = new SqlCommand("SELECT COUNT(*) FROM " + tableName, connection, transaction);
                return Convert.ToInt32(commandRowCount.ExecuteScalar());
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Toolkit).FullName,e);
                throw new Exception("SQLTOOLKIT/COUNT_TABLE: error contabilizando tabla.", e);
            }
        }

        public static int SelectInto(SqlConnection connection, String sourceTableName, String destTableName, String Where, bool dropfirst, SqlTransaction transaction)
        {
            try
            {
                var whereExpression = String.IsNullOrEmpty(Where) ? "" : "WHERE " + Where;
                try
                {
                    if (dropfirst) DropTable(connection, destTableName, transaction);
                }
                catch (Exception)
                {
                    STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/SELECT_INTO: {0}, catalogo={1}, destination_table={2}, no existe.", connection.DataSource, connection.Database, destTableName));
                }
                STrace.Debug(typeof(Toolkit).FullName, String.Format(
                        "SQLTOOLKIT/SELECT_INTO: {0}, catalogo={1}, source_query='{2}', destination_table={3}, transaccional={4}",
                        connection.DataSource, connection.Database, sourceTableName, destTableName,
                        transaction != null ? "SI" : "NO"));
                var commandRowCount =
                    new SqlCommand(
                        "SELECT * INTO " + destTableName + " FROM " + sourceTableName + " " + whereExpression,
                        connection, transaction);
                return commandRowCount.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Toolkit).FullName,e);
                throw new Exception("SQLTOOLKIT/SELECT_INTO: imposible eliminar la tabla.", e);
            }
        }

        public static int DropTable(SqlConnection connection, String tableName, SqlTransaction transaction)
        {
            try
            {
                STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/DROP_TABLE: {0}, catalogo={1}, tabla={2}, transaccional={3}",
                        connection.DataSource, connection.Database, tableName, transaction != null ? "SI" : "NO"));
                var commandRowCount = new SqlCommand("DROP TABLE " + tableName, connection, transaction);
                return commandRowCount.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Toolkit).FullName, e);
                throw new Exception("SQLTOOLKIT/DROP_TABLE: imposible eliminar la tabla.", e);
            }
        }

        public static int DropDatabase(string instance, string catalog)
        {
            var connStr = MakeConnectionString(instance, "master");
            STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/DROP_DATABASE: {0}, catalogo={1}.", instance, catalog));
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var command = new SqlCommand("DROP DATABASE " + catalog, conn);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Toolkit).FullName,e);
                throw new Exception(
                    "SQLTOOLKIT/DROP_DATABASE: imposible eliminar la base de datos " + catalog + " de la instancia " +
                    instance, e);
            }
        }

        public static int CreateDatabase(string instance, string catalog)
        {
            var connStr = MakeConnectionString(instance, "master");
            try
            {
                STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/CREATE_DATABASE: {0}, catalogo={1}.", instance, catalog));

                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var command = new SqlCommand("CREATE DATABASE " + catalog
                                                 + " COLLATE SQL_Latin1_General_CP1_CI_AS", conn);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Toolkit).FullName,e);
                throw new Exception(
                    "SQLTOOLKIT/CREATE_DATABASE: imposible crear la base de datos " + catalog + " en la instancia " +
                    instance, e);
            }
        }

        public static int RenameTable(SqlConnection connection, String tableName, String newTableName, SqlTransaction transaction)
        {
            try
            {
                STrace.Debug(typeof(Toolkit).FullName, String.Format(
                    "SQLTOOLKIT/RENAME_TABLE: {0}, catalogo={1}, src_table={2}, dst_table={3}, transaccional={4}",
                     connection.DataSource, connection.Database, tableName, newTableName, transaction != null ? "SI" : "NO"));

                var commandRowCount = new SqlCommand("sp_rename '" + tableName + "', '" + newTableName + "'", connection,
                                                     transaction);
                return commandRowCount.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                STrace.Exception(typeof(Toolkit).FullName,e);
                throw new Exception("SQLTOOLKIT/RENAME_TABLE: imposible renombrar la tabla.", e);
            }
        }

        private static SqlBatchProgressHandler _copyTableSqlBatchProgress;
        private static String _copyTableTable;
        private static int _copyTableRecords;

        public enum BulkCopyStates
        {
            [Description("No Iniciado")] WfBegin,
            [Description("Seleccionando")] SNAPSHOTING,
            [Description("Cargando")] LOADING,
            [Description("Preparando")] APPROACH,
            [Description("Copiando...")] BULKCOPY,
            [Description("Copiando")] COPIED,
            [Description("Verificando")] VERIFING,
            [Description("Terminando")] COMPLETE,
        }

        public static int CopyBuggyTable(SqlConnection sourceConnection, SqlConnection destinationConnection, SqlTable table,
                                    SqlBatchProgressHandler sqlBatchProgress, SqlTransaction transaction)
        {
            _copyTableSqlBatchProgress = sqlBatchProgress;
            var state = BulkCopyStates.WfBegin;
            try
            {
                STrace.Debug(typeof(Toolkit).FullName, String.Format(
                    "SQLTOOLKIT/COPY_TABLE: {0}, catalogo={1}, table={2}, transaccional={3}",
                     sourceConnection.DataSource, sourceConnection.Database, table, transaction != null ? "SI" : "NO"));
                //WORKAROUND: debido a un defecto en SqlBulkCopy
                // se hace este rodeo de usar una tabla intermedia,
                // la cosa es que si la tabla incluye un . en su nombre
                // el metodo falla. Todas nuestras tablas incluyen
                // . en el nombre.
                // http://support.microsoft.com/kb/944389/en-us/

                var tempTable = "srctmp_" + table.Synonym;
                var safeTable = "dsttmp_" + table.Synonym;

				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.SNAPSHOTING).ToString("G"), table.ToString(), -1, 1);
                SelectInto(sourceConnection, table.Synonym, tempTable, table.SourceWhere, true, null);
                _copyTableRecords = CountTableRows(sourceConnection, tempTable, null);
                var timeout = Math.Max(6000, _copyTableRecords);
                int fileSteps;
                int batchSize;
                if (_copyTableRecords < 10)
                {
                    fileSteps = 1;
                    batchSize = 1;
                }
                else if (_copyTableRecords < 100)
                {
                    fileSteps = 10;
                    batchSize = 10;
                }
                else 
                {
                    fileSteps = Math.Min(500,Math.Max(100, _copyTableRecords / 100));
                    batchSize = Math.Min(50,Math.Max(10, _copyTableRecords / 100));
                }
				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.LOADING).ToString("G"), table.ToString(), -1, 1);
                var source = RetrieveTable(sourceConnection, tempTable, null);
				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.APPROACH).ToString("G"), table.ToString(), -1, 1);
                // creo una tabla temporal igual a la destino, y la vacio.
                RenameTable(destinationConnection, table.QualifiedName, safeTable, transaction);
				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.BULKCOPY).ToString("G"), table.ToString(), -1, 1);
                using (var destination = new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.KeepIdentity,
                                                         transaction))
                {
                    _copyTableTable = table.ToString();
                    destination.BatchSize = batchSize;
                    destination.SqlRowsCopied += DestinationSqlRowsCopied;
                    destination.NotifyAfter = fileSteps;
                    destination.BulkCopyTimeout = timeout;
                    destination.DestinationTableName = safeTable;
                    destination.WriteToServer(source);
                }
                source.Close();
				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.VERIFING).ToString("G"), table.ToString(), -1, 1);
                var records = CountTableRows(destinationConnection, safeTable, transaction);
				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.COMPLETE).ToString("G"), table.ToString(), -1, 1);
                RenameTable(destinationConnection, safeTable, table.FullName, transaction);
                return records;
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(Toolkit).FullName, e, String.Format("Imposible copiar la tabla {0} estado {1}", table.Synonym, state));
                throw new Exception("SQLTOOLKIT/COPY_TABLE: imposible copiar la tabla.", e);
            }
        }

        public static int CopyTable(SqlConnection sourceConnection, SqlConnection destinationConnection, SqlTable table,
                            SqlBatchProgressHandler sqlBatchProgress, SqlTransaction transaction)
        {
            _copyTableSqlBatchProgress = sqlBatchProgress;
            var state = BulkCopyStates.WfBegin;
            try
            {
                STrace.Debug(typeof(Toolkit).FullName, String.Format(
                    "SQLTOOLKIT/COPY_TABLE: {0}, catalogo={1}, table={2}, transaccional={3}",
                     sourceConnection.DataSource, sourceConnection.Database, table, transaction != null ? "SI" : "NO"));

				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.SNAPSHOTING).ToString("G"), table.ToString(), -1, 1);
                _copyTableRecords = CountTableRows(sourceConnection, table.Synonym, null);
                var timeout = Math.Max(6000, _copyTableRecords);
                int fileSteps;
                int batchSize;
                if (_copyTableRecords < 10)
                {
                    fileSteps = 1;
                    batchSize = 1;
                }
                else if (_copyTableRecords < 100)
                {
                    fileSteps = 10;
                    batchSize = 10;
                }
                else
                {
                    fileSteps = Math.Min(500, Math.Max(100, _copyTableRecords / 100));
                    batchSize = Math.Min(50, Math.Max(10, _copyTableRecords / 100));
                }
				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.LOADING).ToString("G"), table.ToString(), -1, 1);
                var source = RetrieveTable(sourceConnection, table.Synonym, null);
				LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.BULKCOPY).ToString("G"), table.ToString(), -1, 1);
                using (var destination = new SqlBulkCopy(destinationConnection, SqlBulkCopyOptions.KeepIdentity,
                                                         transaction))
                {
                    _copyTableTable = table.ToString();
                    destination.BatchSize = batchSize;
                    destination.SqlRowsCopied += DestinationSqlRowsCopied;
                    destination.NotifyAfter = fileSteps;
                    destination.BulkCopyTimeout = timeout;
                    destination.DestinationTableName = table.Synonym;
                    destination.WriteToServer(source);
                }
                source.Close();
                LaunchSqlBatchProgressHandler(sqlBatchProgress, (state = BulkCopyStates.VERIFING).ToString("G"), table.ToString(), -1, 1);
                return CountTableRows(destinationConnection, table.Synonym, transaction);
            }
            catch (Exception e)
            {
				STrace.Exception(typeof(Toolkit).FullName, e, String.Format("Imposible copiar la tabla {0} estado {1}", table.Synonym, state));
                throw new Exception("SQLTOOLKIT/COPY_TABLE: imposible copiar la tabla.", e);
            }
        }


        static void DestinationSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            LaunchSqlBatchProgressHandler(_copyTableSqlBatchProgress, String.Format("Copiando... registros {0}/{1} transferidos a", e.RowsCopied, _copyTableRecords), _copyTableTable, -1, 1);
        }

        public static void LaunchSqlBatchProgressHandler(SqlBatchProgressHandler handler, string action, string entity,
                                                         int totalSteps, int doneSteps)
        {
            if (handler == null) return;
            handler(action, entity, totalSteps, doneSteps);
        }

        public static int ComputeSteps(int tables)
        {
            // son 2 pasadas por tablas.
            // la primera solo trunca las tablas en destino.
            // la segunda las Descarga.
            // la tercera las Copia.
            return tables*4;
        }

        public static bool Syncronize(IEnumerable<SqlTable> tables,
                                      string sourceConnectionString,
                                      string destinationConnectionString,
                                      SqlBatchProgressHandler sqlBatchProgress)
        {
            using (var dc = new SqlConnection(destinationConnectionString))
            {
                var counter = new List<SqlTable>(tables);
                dc.Open();
                var transaction = dc.BeginTransaction();
                var transactionSuccess = false;
                var steps = 0;
                foreach (var table in tables)
                {
                    LaunchSqlBatchProgressHandler(sqlBatchProgress, "Truncando tabla ", table.ToString(), ComputeSteps(counter.Count),
                                                  steps++);
                    var c = TruncateTable(dc, table.Synonym, transaction);
                    STrace.Debug(typeof(Toolkit).FullName, String.Format("La Tabla {0} fue truncada, tenia {1} registros.", table, c));
                }
                try
                {
                    using (var sc = new SqlConnection(sourceConnectionString))
                    {
                        sc.Open();
                        foreach (var table in tables)
                        {
                            LaunchSqlBatchProgressHandler(sqlBatchProgress, "Copiando tabla ", table.ToString(), ComputeSteps(counter.Count),
                                                          steps++);
                            var c = CopyTable(sc, dc, table, sqlBatchProgress, transaction);
                            
                            STrace.Debug(typeof(Toolkit).FullName, String.Format("Tabla [{0}] copiados {1} registros.", table, c));
                        }
                    }
                    transactionSuccess = true;
                    return true;
                }
                catch (Exception e)
                {
                    STrace.Exception(typeof(Toolkit).FullName,e);
                    transactionSuccess = false;
                    return false;
                }
                finally
                {
                    if (transactionSuccess)
                    {
                        STrace.Debug(typeof(Toolkit).FullName,"Database Syncronization Success.");
                        transaction.Commit();
                    }
                    else
                    {
                        STrace.Debug(typeof(Toolkit).FullName,"Database Syncronization Failure.");
                        transaction.Rollback();
                    }
                }
            }
        }

        private static string _defaultInstance;

        public static string GetDefaultInstance()
        {
            if (!string.IsNullOrEmpty(_defaultInstance)) return _defaultInstance;
            var instances = GetLocalInstances();
            if (instances.Count == 0)
            {
                _defaultInstance = Environment.MachineName;
                try 
				{
					using (var tmp = new SqlConnection(MakeConnectionString(_defaultInstance,"master")))
					{
						tmp.Open();
						return _defaultInstance;
					}
                }
				catch (Exception e)
                {
                    _defaultInstance = "";
                    STrace.Exception(typeof(Toolkit).FullName,e);
                    throw new Exception("SQLTOOLKIT/GET_DEFAULT_INSTANCE: imposible obtener la instancia por defecto de " + Environment.MachineName, e);
                }
            }
            _defaultInstance = instances.Count == 1 ? instances[0] : instances.Find(MatchDefaultInstance);
            if (!MatchDefaultInstance(_defaultInstance))
            {
                var tmpDefaultInstance = Environment.MachineName;
                try
                {
                    using (var tmp = new SqlConnection(MakeConnectionString(tmpDefaultInstance, "master")))
                    {
                        tmp.Open();
                        _defaultInstance = tmpDefaultInstance;
                        return _defaultInstance;
                    }
                }
                catch (Exception e)
                {
					STrace.Exception(typeof(Toolkit).FullName, e);
                }
            }
            return _defaultInstance;
        }

        private static bool MatchDefaultInstance(string name)
        {
            return !name.Contains(@"\");
        }

        /// <summary>
        /// Determina si existe una instalacion local de SQL Server.
        /// </summary>
        /// <returns></returns>
        public static bool FindLocalInstalation()
        {
            var services = ServiceController.GetServices();
            foreach (var service in services.Where(service => service != null))
            {
            	STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/GET_INSTANCES: Servicio {0} ({1})", service.ServiceName, service.DisplayName));
            	if (service.ServiceName == "MSSQLSERVER") return true;
            	if (service.ServiceName.Contains("MSSQL$")) return true;
            }
            return false;    
        }

        public static List<string> GetLocalInstances()
        {
            var results = new List<string>();
            var services = ServiceController.GetServices();
            foreach (var service in services.Where(service => service != null))
            {
            	STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/GET_INSTANCES: Servicio {0} ({1})", service.ServiceName, service.DisplayName));
            	if (service.ServiceName == "MSSQLSERVER")
            	{
            		// la instancia local, la ponemos siempre al principio.
            		results.Insert(0,"(local)");
            		continue;
            	}
            	if (!service.ServiceName.Contains("MSSQL$")) continue;
            	var instance = service.ServiceName.Split("$".ToCharArray())[1];
            	STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/GET_INSTANCES: Instancia '(local)\\{0}' agregada.", instance));
            	results.Add(@"(local)\" + instance);
            }
            return results;
        }

        /// <summary>
        /// Retorna las instancias de SQL de la red local.
        /// </summary>
        /// <remarks>
        /// Lista las instancias detectadas por el Explorador de SQL (SQL Browser).
        /// 
        /// NOTA: debido a un error aun sin solucion en el framework .NET, la instacia 
        /// local por defecto no la detecta.
        /// </remarks>
        /// <returns>Una lista de strings que contienen el camino a cada instancia conocida.</returns>
        public static List<string> GetInstances()
        {
            try
            {
                var dataTable = SqlDataSourceEnumerator.Instance.GetDataSources();
                var results = new List<string>();
                foreach (DataRow r in dataTable.Rows)
                {
                    var server = r["ServerName"].ToString();
                    var strver = r["Version"].ToString();
                    var instance = r["InstanceName"].ToString();
                    var path = string.Format("{0}{1}", server, string.IsNullOrEmpty(instance) ? "" : @"\" + instance);
                    if (string.IsNullOrEmpty(strver))
                    {
                        STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/GET_INSTANCES: Instancia {0} descartada, imposible obtener la version.", path));
                        continue;
                    }
                    var version = Convert.ToInt32(strver.Split(".".ToCharArray())[0]);
                    if (version < 9)
                    {
                        STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/GET_INSTANCES: Instancia {0} descartada, solo SQL 2005 o superior.", path));
                        continue;
                    }
                    STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/GET_INSTANCES: Instancia {0} agregada, version {1}.", path, strver));
                    results.Add(path);
                }
                return results;
            }
            catch
            {
                return null;
            }
        }

        public static string MakeConnectionString(string instance, string catalog)
        {
            return String.Format("packet size=4096;integrated security=SSPI;" +
                                 "data source=\"{0}\";persist security info=False;" +
                                 "initial catalog={1}", instance, catalog);
        }

        
        public static string GetDefaultConnectionString()
        {
            return String.Format("packet size=4096;integrated security=SSPI;" +
                                 "data source=\"{0}\";persist security info=False;" +
                                 "initial catalog=logictracker", GetDefaultInstance());
        }

        public static List<string> GetDatabases(string instance)
        {
            try
            {
                var connStr = MakeConnectionString(instance, "master");
                using (var conn = new SqlConnection(connStr))
                {
                	conn.Open();
                	var command = new SqlCommand("EXEC sp_databases", conn);
                	var reader = command.ExecuteReader();
                	var results = new List<string>();
                	var sysDbs = new List<string> {"master", "model", "msdb", "tempdb"};
                	while (reader.Read())
                	{
                		var name = reader.GetString(reader.GetOrdinal("DATABASE_NAME"));
                		if (sysDbs.Contains(name)) continue;
                		results.Add(name);
                	}
                	return results;
                }
            }
            catch
            {
                return null;
            }
        }

        public static string GetEmbeddedScript(string name)
        {
            var asm = Assembly.GetExecutingAssembly();
            var str = asm.GetManifestResourceStream(asm.GetName().Name + "." + name);
            if (str != null)
            {
                var reader = new StreamReader(str);
                return reader.ReadToEnd();
            }
            throw new Exception("Imposible obtener el recurso embebido " + name);
        }

        public static void InstallDatabase(string catalogo, string embeddedScript)
        {
            var instance = GetDefaultInstance();
            if (string.IsNullOrEmpty(instance))
            {
                throw new Exception("No se pudo obtener la instacia por defecto de SQL 2005.");
            }
            var bases = GetDatabases(instance);
            if (bases.Contains(catalogo))
            {
                throw new Exception("El catalogo " + catalogo + " ya existe.");
            }
            //var r = CreateDatabase(instance, catalogo);
            var connStr = MakeConnectionString(instance, catalogo);
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                var script = GetEmbeddedScript(embeddedScript);
                var lines = regex.Split(script);

                foreach (var line in lines.Where(line => line.Length > 0))
                {
                	using (var command = new SqlCommand(line, conn))
                	{
                		STrace.Debug(typeof(Toolkit).FullName, String.Format("SQLTOOLKIT/INSTALL_SCRIPT: [{0}]", line.Replace("\r\n","")));
                		command.ExecuteNonQuery();
                	}
                }
            }
        }

        #region Nested type: SqlTable

        public class SqlTable
        {
            public string FullName;
            public string QualifiedName;
            public string Synonym;
            public string SourceWhere = "";

            public override String ToString()
            {
                return FullName;
            }
        }

        #endregion
    }
}