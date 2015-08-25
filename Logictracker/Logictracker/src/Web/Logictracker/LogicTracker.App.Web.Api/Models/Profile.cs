using System.Collections.Generic;

namespace LogicTracker.App.Web.Api.Models
{
    public class Profile
    {
        public int MobileId { get; set; }
        public List<MessageType> Messages { get; set; }
    }
}