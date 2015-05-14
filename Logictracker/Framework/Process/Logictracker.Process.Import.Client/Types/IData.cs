using System.Collections.Generic;

namespace Logictracker.Process.Import.Client.Types
{
    public interface IData
    {
        int Version { get; }
        int Entity { get; }
        int Operation { get; set; }
        int[] Properties { get; }
        string[] Values { get; }
        IData Pack();
        string this[int property] { get; }
        Dictionary<string, string> GetCustomFields();
    }
}
