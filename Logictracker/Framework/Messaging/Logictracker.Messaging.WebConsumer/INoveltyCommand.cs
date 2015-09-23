using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logictracker.Messaging.WebConsumer
{
    public interface INoveltyCommand
    {
        string RouteCode { get; set; }
        int DeliveryStatus { get; set; }
        bool IsSource { get; set; }
        DateTime InitialDate { get; set; }
    }
}
