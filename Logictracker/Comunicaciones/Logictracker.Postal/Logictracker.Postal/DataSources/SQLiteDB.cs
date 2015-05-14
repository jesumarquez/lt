using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Data;

namespace Urbetrack.Postal.DataSources
{
    class SQLiteDB
    {
        public static System.Data.SQLite.SQLiteConnection connection;
        static string dbFileName = "urbetrack_postal.sqlite";
        public static bool Connected = false;


        public static bool connect()
        {
            if (!File.Exists(dbFileName)) {
                Connected = false;
                return false;
            } else  {
                String s = String.Concat("Data Source=", dbFileName,";Version=3;New=False;Compress=True;");

                connection = new System.Data.SQLite.SQLiteConnection(s);
                connection.Open();
                Connected = true;
                return true;

            }
        }

        public static void close()
        {
            Connected = false;
            connection.Close();
        }


      }
}
    