#region Usings

using System;
using System.Globalization;
using System.Threading;

#endregion

namespace Quartz.Impl.AdoJobStore
{
    /// <summary> 
    /// Internal database based lock handler for providing thread/resource locking 
    /// in order to protect resources from being altered by multiple threads at the 
    /// same time.
    /// </summary>
    /// <author>James House</author>
    public class StdRowLockSemaphore : DBSemaphore
    {
        /*
		* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		* 
		* Constants.
		* 
		* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		*/

        public static readonly string SelectForLock =
            string.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0}{1} WHERE {2} = @lockName FOR UPDATE", TablePrefixSubst, TableLocks,
                          ColumnLockName);

        /*
		* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		* 
		* Constructors.
		* 
		* ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		*/

        /// <summary>
        /// Initializes a new instance of the <see cref="StdRowLockSemaphore"/> class.
        /// </summary>
        /// <param name="tablePrefix">The table prefix.</param>
        /// <param name="selectWithLockSQL">The select with lock SQL.</param>
        /// <param name="dbProvider"></param>
        public StdRowLockSemaphore(string tablePrefix, string selectWithLockSQL, IDbProvider dbProvider) : base(tablePrefix, selectWithLockSQL, SelectForLock, dbProvider)
        {
        }
    /**
     * Execute the SQL select for update that will lock the proper database row.
     */
    protected override void ExecuteSQL(ConnectionAndTransactionHolder conn, string lockName, string expandedSQL)
    {
        try {
            using (var cmd = AdoUtil.PrepareCommand(conn, expandedSQL))
            {
                AdoUtil.AddCommandParameter(cmd, 1, "lockName", lockName);

                using (var rs = cmd.ExecuteReader())
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug("Lock '" + lockName + "' is being obtained: " + Thread.CurrentThread.Name);
                    }
                    
                    if (!rs.Read())
                    {
                        throw new Exception(AdoJobStoreUtil.ReplaceTablePrefix("No row exists in table " + TablePrefixSubst + TableLocks + " for lock named: " + lockName, TablePrefix));
                    }
                }
            }
        } catch (Exception sqle) {

            if (Log.IsDebugEnabled) {
                Log.Debug(
                    "Lock '" + lockName + "' was not obtained by: " + 
                    Thread.CurrentThread.Name);
            }
            
            throw new LockException("Failure obtaining db row lock: "
                    + sqle.Message, sqle);
        } 
    
    }
    }
}