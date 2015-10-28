using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logictracker.Types.SecurityObjects;

namespace Logictracker.Web.Controllers
{
    public class BaseFunctionController : Controller
    {
        public Module Module { get; set; }
    }
}