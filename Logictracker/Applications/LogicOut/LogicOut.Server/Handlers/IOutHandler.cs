namespace LogicOut.Server.Handlers
{
    public interface IOutHandler
    {
        OutData[] Process(int empresa, int linea, QueryParams parameters);
    }
}
