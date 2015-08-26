using System;

namespace LogicTracker.App.Web.Api.Models
{
    public class Message
    {
        public long JobId { get; set; }
        public DateTime DateTime { get; set; }
        public MessageType MessageType { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}