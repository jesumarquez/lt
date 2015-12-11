using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logictracker.Web.Controllers;

namespace Logictracker.Web.Areas.Ordenes.Controllers
{
    public class ProgramarController : BaseFunctionController, IFunctionController
    {
        // GET: Ordenes/Programar
        public ActionResult Index()
        {
            return View();
        }

        public string VariableName { get { return "PAR_ORDENES"; } }
        public string GetRefference() { return "PAR_ORDENES"; }
    }
}