using Logictracker.Process.Import.Client.Types;

namespace Logictracker.Process.Import.Client.DataStrategy
{
    public interface IDataStrategy
    {
        Table GetNewData();
        void Revert();
    }
}
