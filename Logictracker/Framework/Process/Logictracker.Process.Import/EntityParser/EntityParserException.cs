using System;

namespace Logictracker.Process.Import.EntityParser
{
    public class EntityParserException:Exception
    {
        private readonly string _message;

        public override string Message
        { 
            get
            {
                return _message;
            } 
        }
        public EntityParserException()
        {
        }
        public EntityParserException(string message)
        {
            _message = message;
        }
    }
}
