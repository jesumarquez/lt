#region Usings

using System.Data;
using Common.Logging;

#endregion

namespace Quartz.Impl.AdoJobStore
{
	/// <summary>
	/// This is a driver delegate for the PostgreSQL ADO.NET driver.
	/// </summary>
	/// <author>Marko Lahma</author>
	public class PostgreSQLDelegate : StdAdoDelegate
	{

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSQLDelegate"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="tablePrefix">The table prefix.</param>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="dbProvider">The db provider.</param>
        public PostgreSQLDelegate(ILog log, string tablePrefix, string instanceId, IDbProvider dbProvider)
            : base(log, tablePrefix, instanceId, dbProvider)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSQLDelegate"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="tablePrefix">The table prefix.</param>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="dbProvider">The db provider.</param>
        /// <param name="useProperties">if set to <c>true</c> [use properties].</param>
        public PostgreSQLDelegate(ILog log, string tablePrefix, string instanceId, IDbProvider dbProvider, bool useProperties)
            : base(log, tablePrefix, instanceId, dbProvider, useProperties)
		{
		}

		//---------------------------------------------------------------------------
		// protected methods that can be overridden by subclasses
		//---------------------------------------------------------------------------
        protected override byte[] ReadBytesFromBlob(IDataReader dr, int colIndex)
        {
            if (dr.IsDBNull(colIndex))
            {
                return null;
            }

            // PostgreSQL reads all data at once

            var dataLength = dr.GetBytes(colIndex, 0, null, 0, 0);
            var data = new byte[dataLength];
            dr.GetBytes(colIndex, 0, data, 0, 0);
            return data;
        }

	}

	// EOF
}