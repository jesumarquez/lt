using System;
using System.Linq;
using Logictracker.Model;

namespace Logictracker.Layers.DeviceCommandCodecs
{
    public abstract class TrimbleDeviceCommand : BaseDeviceCommand
    {
        protected const string commandPattern = @"\>(?<command>[^;]*)[^\<]*\<";
        // tested with TRAX AND VIRLOC
        protected const string commandAttributesPattern = @"(?:(?<=[;])(?:(?<param>[^;\<#]*?)[:|=](?<value>[^;\<]*?)|(?<param>#(?:LOG|IP\d|SM\d|SDM)?)(?:[:])?(?<value>[0-9A-F]{4}))(?=;|\<|$))";
        protected const string iDNumPattern = @"(?:(?<=[;])(?:(?:ID)[=](?<value>[^;\<]*?))(?=;|\<|$))";
        
        public class Attributes : BaseDeviceCommand.Attributes
        {            
            public const string MessageId = "#";
        }        

        protected TrimbleDeviceCommand(string command) : this(command, null, null) { }

        protected TrimbleDeviceCommand(string command, INode node, DateTime? ExpiresOn) : base(command, node, ExpiresOn)
        {
            if (MessageId == null && _node != null)
                MessageId = _node.NextSequence;
        }        

        protected override string getCommandAttributesPattern()
        {
            return commandAttributesPattern;
        }

        public override DeviceCommandResponseStatus isExpectedResponse(BaseDeviceCommand response)
        {
            var rcmd = response.getCommand();

            var rcmd_fistLetter = rcmd.Substring(0, 1);
            var cmd_firstLetter = _command.Substring(0, 1);
            
            return (new[] {"E", "R"}.Any(l => l == rcmd_fistLetter)) &&
                (new[] {"Q", "S"}.Any(l => l == cmd_firstLetter)) ? DeviceCommandResponseStatus.Valid : DeviceCommandResponseStatus.Invalid;
        }

        public bool isValid()
        {
            return ((hasIdNum() && isIdNumValid()) || _command != null);
        }

        public bool isIdNumValid()
        {
            return (_node != null && _node.IdNum == IdNum);
        }

        public bool hasIdNum()
        {
            return IdNum != null && IdNum.Value != 0;
        }

        public bool hasMessasgeId()
        {
            return MessageId != null && MessageId.Value != 0;
        }
    }
}
