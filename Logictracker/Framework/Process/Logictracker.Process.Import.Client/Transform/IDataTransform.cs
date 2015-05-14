using Logictracker.Process.Import.Client.Types;

namespace Logictracker.Process.Import.Client.Transform
{
    public interface IDataTransform
    {
        string Encode(IData data);
        IData Decode(string encoded);
    }
}
