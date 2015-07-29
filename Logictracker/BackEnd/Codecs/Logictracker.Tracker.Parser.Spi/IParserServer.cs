using System;
using Logictracker.Model;

namespace Logictracker.Tracker.Parser.Spi
{
    public interface IParserServer
    {
        string Name { get; }
        Action<IMessage> Callback { get; set; }
        IParser Parser { get; }
        ITranslator Translator { get; set; }
        void Start();
        void Stop();
    }
}
