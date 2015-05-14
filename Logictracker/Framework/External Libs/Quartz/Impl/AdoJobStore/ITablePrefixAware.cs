namespace Quartz.Impl.AdoJobStore
{
    /// <summary>
    /// Interface for Quartz objects that need to know what the table prefix of
    /// the tables used by a ADO.NET JobStore is.
    /// </summary>
    public interface ITablePrefixAware
    {
        string TablePrefix { get; }
    }
}