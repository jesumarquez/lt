using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logictracker.Types.ValueObject.Positions;

namespace HandlerTest.Classes
{
    public interface IPosition
    {
        event EventHandler MapInitialized;
        LogPosicionVo LastPosition { get; }
    }
}
