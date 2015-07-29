using Logictracker.Tracker.Parser.Spi;

namespace Logictracker.Tracker.Parser.Caesat.Comm
{
    internal class CaesatParserServer : BaseParserServer
    {
        protected const int DefaultPort = 8510;

        public CaesatParserServer()
        {
            Port = DefaultPort;
        }

        public override string Name
        {
            get { return "CaesatParser.Server"; }
        }

        public override IParser Parser { get; set; }
        public override ITranslator Translator { get; set; }
    }
}
