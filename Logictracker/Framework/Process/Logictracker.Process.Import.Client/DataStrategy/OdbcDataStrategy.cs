using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Process.Import.Client.Types;
using System.Data.Odbc;

namespace Logictracker.Process.Import.Client.DataStrategy
{
    public class OdbcDataStrategy:IDataStrategy
    {
        protected Dictionary<string, string> Parameters;
        protected string ConnectionString { get; set; }
        protected string Query { get; set; }

        public OdbcDataStrategy(params IDataSourceParameter[] parameters)
        {
            Parameters = parameters.ToDictionary(k => k.Name.ToLower(), v => v.Value);

            if (!Parameters.ContainsKey("connectionstring")) throw new ApplicationException("No se encontro el parametro connectionstring");
            if (!Parameters.ContainsKey("query")) throw new ApplicationException("No se encontro el parametro query");
            ConnectionString = Parameters["connectionstring"].Trim();
            Query = Parameters["query"].Trim();


            Logger.Info("OdbcDataStrategy iniciado (ConnectionString: " + ConnectionString + " | Query: " + Query);
        }
        #region Implementation of IDataStrategy

        public Table GetNewData()
        {
            ValidateQuery(Query);

            Table table;

            using(var connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = Query;
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    var columns = new List<string>(reader.FieldCount);
                    for (var i = 0; i < reader.FieldCount; i++) columns.Add(reader.GetName(i));

                    table = new Table(true, columns.ToArray());

                    while (reader.Read())
                    {
                        try{
                            var cells = columns.Select((c, i) => reader.GetValue(i))
                                .Select(value => value == DBNull.Value ? null : value)
                                .ToArray();

                            table.Rows.Add(new Record(table, cells));
                        }
                        catch(Exception ex)
                        {
                            Logger.Error("Error parseando registro: " + ex);
                            return null;
                        }
                    }
                }
                else
                {
                    table = new Table();
                }

                reader.Close();
                connection.Close();
            }

            return table;
        }

        public void Revert()
        {
            
        }

        #endregion


        private void ValidateQuery(string commandText)
        {
            if(!commandText.Trim().ToLower().StartsWith("select")) throw new ApplicationException("El valor del parametro query debe ser una instrucción sql select");
            
            var statements = commandText.Split(';');
            if (statements.Select(statement => statement.Trim().ToLower()).Any(st => st.StartsWith("insert") || st.StartsWith("update") || st.StartsWith("delete") || st.StartsWith("truncate") || st.StartsWith("create") || st.StartsWith("alter") || st.StartsWith("drop") || st.StartsWith("exec")))
            {
                throw new ApplicationException("El valor del parametro query debe ser una instrucción sql select");
            }
        }
    }
}
