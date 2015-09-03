using System.Collections.Generic;

namespace LogicTracker.App.Web.Api.Models
{
    public class Profile
    {
        public int MobileId { get; set; }
        public int trackingudp { get; set; }
        public int modovendedor { get; set; }
        public int modocamion { get; set; }
        public List<MessageType> Messages { get; set; }
    }
}