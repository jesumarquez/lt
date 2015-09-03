using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogicTracker.App.Web.Api.Models;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class MessageList
    {
        public CustomMessage[] MessageItems { get; set; }
    }
}
