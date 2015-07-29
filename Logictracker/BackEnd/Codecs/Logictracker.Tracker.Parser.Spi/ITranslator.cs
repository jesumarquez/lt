using System.Collections.Generic;
using IMessage = Logictracker.Model.IMessage;

namespace Logictracker.Tracker.Parser.Spi
{
    public interface ITranslator
    {
        IList<IMessage> Translate(IList<IFrame> frames);
    }
}
