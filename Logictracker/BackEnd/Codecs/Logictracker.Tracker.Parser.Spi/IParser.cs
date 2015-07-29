using System.Collections.Generic;

namespace Logictracker.Tracker.Parser.Spi
{
    public interface IParser
    {
        IList<IFrame> Parse(string receivedData);
    }
}
