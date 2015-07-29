using Logictracker.Tracker.Parser.Spi;

namespace Logictracker.Tracker.Parser.Caesat.Comm
{
    class CaesatParserClient : BaseParserClient
    {
        protected const int DefaultPort = 5006;
        //        protected const string DefaultIpIpAddress = "190.220.14.98";

        public CaesatParserClient()
        {
            Port = DefaultPort;
            //       IpAddress = DefaultIpIpAddress;
        }

        public override string Name
        {
            get { return "CaesatParser.Client"; }
        }

        public override IParser Parser { get; set; }
        public override ITranslator Translator { get; set; }
    }
}
