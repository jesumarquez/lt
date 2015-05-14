using System;

namespace Logictracker.Process.Import.Client.Parsers
{
    public class ParserException:Exception
    {
        private readonly string _message;

        public override string Message
        { 
            get
            {
                return _message;
            } 
        }
        public ParserException()
        {
        }
        public ParserException(string message)
        {
            _message = message;
        }
    }
}
