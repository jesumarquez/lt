using System;
using Logictracker.DatabaseTracer.Types;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class LogContextVo
    {
        public const int IndexKey = 0;
        public const int IndexValue = 1;

        [GridMapping(Index = IndexKey, ResourceName = "Labels", VariableName = "KEY")]
        public string Key { get; set; }

        [GridMapping(Index = IndexValue, ResourceName = "Labels", VariableName = "VALUE")]
        public string Value { get; set; }

        public LogContextVo(LogContext logContext)
        {
            Key = logContext.Key;
            Value = logContext.Value;
        }
    }
}
