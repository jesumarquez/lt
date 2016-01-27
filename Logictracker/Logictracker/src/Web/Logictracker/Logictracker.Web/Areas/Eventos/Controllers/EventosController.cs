using Logictracker.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logictracker.Web.Areas.Eventos.Controllers
{
    public class EventosController : BaseFunctionController, IFunctionController
    {
        public string VariableName
        {
            get { return "PAR_EVENTOS"; }
        }

        public string GetRefference()
        {
            return "PAR_EVENTOS";
        }

        // GET: Eventos/Eventos
        public ActionResult Index()
        {
            return View();
        }
    }
}