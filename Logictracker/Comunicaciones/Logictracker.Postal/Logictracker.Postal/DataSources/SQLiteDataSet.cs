using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace Urbetrack.Postal.DataSources
{
    class SQLiteDataSet
    {
        public DataSet dataSet;
        private SQLiteDataAdapter _aSQLiteDataAdapter;
        private bool _closeAfter = false;

        public bool readTable(String query)
        {
            if (!SQLiteDB.Connected)
            {
                _closeAfter = true;
                if (!SQLiteDB.connect()) return false;
            }
            _aSQLiteDataAdapter = new SQLiteDataAdapter(String.Concat("SELECT * FROM ",query), SQLiteDB.connection);
            dataSet = new DataSet();
            dataSet.Reset();
            _aSQLiteDataAdapter.Fill(dataSet);
            if (_closeAfter) SQLiteDB.close();
            return true;
        }

        public bool read(String query)
        {
            if (!SQLiteDB.Connected)
            {
                _closeAfter = true;
                if (!SQLiteDB.connect()) return false;
            }
            _aSQLiteDataAdapter = new SQLiteDataAdapter(query, SQLiteDB.connection);
            dataSet = new DataSet();
            dataSet.Reset();
            _aSQLiteDataAdapter.Fill(dataSet);
            if (_closeAfter) SQLiteDB.close();
            return true;
        }

        public bool executeSQL(String sql)
        {
            if (!SQLiteDB.Connected)
            {
                _closeAfter = true;
                if (!SQLiteDB.connect()) return false;
            }
            _aSQLiteDataAdapter = new SQLiteDataAdapter(sql, SQLiteDB.connection);
            dataSet = new DataSet();
            dataSet.Reset();
            _aSQLiteDataAdapter.Fill(dataSet);
            if (_closeAfter) SQLiteDB.close();
            return true;
        }

        public void write()
        {
            SQLiteCommandBuilder cb = new SQLiteCommandBuilder(_aSQLiteDataAdapter);

            cb.QuotePrefix = "[";
            cb.QuoteSuffix = "]";
            _aSQLiteDataAdapter.InsertCommand = cb.GetUpdateCommand();
            _aSQLiteDataAdapter.Update(dataSet);
            
        }


        public void UpdateGPS(List<Ruta> services)
        {
            if (!SQLiteDB.Connected)
            {
                _closeAfter = true;

                SQLiteDB.connect();
            }

            using (var command = SQLiteDB.connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                var ids = string.Join(",", services.Select(s => s.Id.ToString()).ToArray());
                command.CommandText = String.Concat("update rutas set longitud = @longitud, latitud = @latitud where id in (", ids, ")");

                var parameter1 = command.CreateParameter();

                parameter1.ParameterName = "@latitud";
                parameter1.DbType = DbType.Double;
                parameter1.Value = services[0].Latitud;

                command.Parameters.Add(parameter1);

                var parameter2 = command.CreateParameter();

                parameter2.ParameterName = "@longitud";
                parameter2.DbType = DbType.Double;
                parameter2.Value = services[0].Longitud;

                command.Parameters.Add(parameter2);

                command.ExecuteNonQuery();
            }

            if (_closeAfter) SQLiteDB.close();  
        }


        public void UpdateLateralesReferencia(List<Ruta> services)
        {
            if (!SQLiteDB.Connected)
            {
                _closeAfter = true;

                SQLiteDB.connect();
            }

            using (var command = SQLiteDB.connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                var ids = string.Join(",", services.Select(s => s.Id.ToString()).ToArray());
                command.CommandText = String.Concat("update rutas set referencia = @referencia, lateral1 = @lateral1, lateral2 = @lateral2 where id in (", ids, ")");

                var parameter1 = command.CreateParameter();

                parameter1.ParameterName = "@referencia";
                parameter1.DbType = DbType.String;
                parameter1.Value = services[0].Referencia;

                command.Parameters.Add(parameter1);

                var parameter2 = command.CreateParameter();
                parameter2.ParameterName = "@lateral1";
                parameter2.DbType = DbType.String;
                parameter2.Value = services[0].Lateral1;
                command.Parameters.Add(parameter2);

                var parameter3 = command.CreateParameter();
                parameter3.ParameterName = "@lateral2";
                parameter3.DbType = DbType.String;
                parameter3.Value = services[0].Lateral2;
                command.Parameters.Add(parameter3);

                command.ExecuteNonQuery();
            }

            if (_closeAfter) SQLiteDB.close();
        }


        public void UpdateFoto(List<Ruta> services)
        {
            if (!SQLiteDB.Connected)
            {
                _closeAfter = true;

                SQLiteDB.connect();
            }

            using (var command = SQLiteDB.connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                var ids = string.Join(",", services.Select(s => s.Id.ToString()).ToArray());
                command.CommandText = String.Concat("update rutas set foto = @imagen, fecha_foto = current_timestamp where id in (", ids, ")");

                var parameter = command.CreateParameter();

                parameter.ParameterName = "@imagen";
                parameter.DbType = DbType.Binary;
                parameter.Value = services[0].Foto;

                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
            }

            if (_closeAfter) SQLiteDB.close();  
        }
    }
}
