using System.Data;
using System.Data.SqlClient;

namespace Logictracker.DAL.NHibernate
{
    class DriverConnectionProvider : global::NHibernate.Connection.DriverConnectionProvider
    {
        public override IDbConnection GetConnection()
        {
            var connection = base.GetConnection();
            const string cmdStr = @"SET QUOTED_IDENTIFIER ON; SET ARITHABORT ON; SET NUMERIC_ROUNDABORT OFF; SET CONCAT_NULL_YIELDS_NULL ON; SET ANSI_NULLS ON; SET ANSI_PADDING ON; SET ANSI_WARNINGS ON;";

            using (var comm = new SqlCommand(cmdStr, (SqlConnection) connection))
            {
                comm.ExecuteNonQuery();
            }
            return connection;
        }
    }
}
