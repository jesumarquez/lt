using Logictracker.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logictracker.Web.Areas.Rechazos.Controllers
{
    public class RechazoController : BaseFunctionController, IFunctionController
    {
        // GET: Rechazos/Rechazo
        public ActionResult Index()
        {
            return View();
        }

        public string ReferenceName
        {
            get { return "PAR_RECHAZO"; }
        }
    }
}