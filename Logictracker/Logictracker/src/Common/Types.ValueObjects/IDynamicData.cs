using System.Collections.Generic;

namespace Logictracker.Types.ValueObjects
{
    public interface IDynamicData
    {
        Dictionary<string, object> DynamicData { get; }
    }
}
