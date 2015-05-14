using Geocoder.Data.SessionManagement;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Data;
using System.IO;
namespace Geocoder.Data.Generation
{
	public class SchemaGenerator
	{
		private readonly string _sessionFactoryConfigPath;
		private ISession NHibernateSession
		{
			get
			{
				return NHibernateSessionManager.Instance.GetSessionFrom(this._sessionFactoryConfigPath);
			}
		}
		public SchemaGenerator(string sessionFactoryConfigPath)
		{
			if (sessionFactoryConfigPath == null)
			{
				throw new ArgumentNullException("sessionFactoryConfigPath");
			}
			this._sessionFactoryConfigPath = sessionFactoryConfigPath;
		}
		public string ExportSchema()
		{
			Configuration configuration = new Configuration();
			configuration.Configure(this._sessionFactoryConfigPath);
			SchemaExport schemaExport = new SchemaExport(configuration);
			schemaExport.SetDelimiter(";");
			StringWriter stringWriter = new StringWriter();
			schemaExport.Execute(true, true, false, this.NHibernateSession.Connection, stringWriter);
			string result = stringWriter.ToString();
			stringWriter.Close();
			return result;
		}
		public void Execute(string query)
		{
			IDbConnection connection = this.NHibernateSession.Connection;
			IDbCommand dbCommand = connection.CreateCommand();
			dbCommand.CommandText = query;
			dbCommand.CommandType = CommandType.Text;
			dbCommand.ExecuteNonQuery();
		}
	}
}
