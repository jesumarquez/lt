using System;

namespace Logictracker.Messaging.WebConsumer
{
    [Serializable]
    public class NoveltyCommand : INoveltyCommand
    {
        public string RouteCode { get; set; }
        public int DeliveryStatus { get; set; }
        public bool IsSource { get; set; }
        public DateTime InitialDate { get; set; }
    }
}
