using System.Net.Sockets;

namespace Logictracker.Tracker.Parser.Spi
{
    public class ClientData
    {
        public BaseParserServer ParserServer { get; set; }
        public Socket Client { get; set; }
        public byte[] Data { get; set; }
    }
}
