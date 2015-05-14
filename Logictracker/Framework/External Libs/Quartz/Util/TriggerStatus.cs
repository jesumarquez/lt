#region Usings

#if NET_20
using System.Globalization;
using NullableDateTime = System.Nullable<System.DateTime>;

#else
using Nullables;
#endif

#endregion

namespace Quartz.Util
{
    /// <summary> 
    /// Object representing a job or trigger key.
    /// </summary>
    /// <author>James House</author>
    public class TriggerStatus : Pair
    {
        /// <summary> 
        /// Construct a new TriggerStatus with the status name and nextFireTime.
        /// </summary>
        /// <param name="status">The trigger's status</param>
        /// <param name="nextFireTimeUtc">The next UTC time the trigger will fire</param>
        public TriggerStatus(string status, NullableDateTime nextFireTimeUtc)
        {
            base.First = status;
            base.Second = nextFireTimeUtc;
        }

        /// <summary>
        /// Gets or sets the job key.
        /// </summary>
        /// <value>The job key.</value>
        public virtual Key JobKey { get; set; }


        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public virtual Key Key { get; set; }

        /// <summary>
        /// Get the name portion of the key.
        /// </summary>
        /// <returns> the name </returns>
        public virtual string Status
        {
            get { return (string) First; }
        }

        /// <summary>
        /// Get the group portion of the key.
        /// </summary>
        /// <returns> the group </returns>
        public virtual NullableDateTime NextFireTimeUtc
        {
            get { return (NullableDateTime) Second; }
        }

        // TODO: Repackage under spi or root pkg ?, put status constants here.

        /// <summary>
        /// Return the string representation of the TriggerStatus.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "status: {0}, next fire = {1}", Status, NextFireTimeUtc.Value.ToString("r", CultureInfo.InvariantCulture));
        }
    }
}