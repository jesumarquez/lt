using System.Web.Mvc;
using Logictracker.Web.Controllers;

namespace Logictracker.Web.Areas.Rechazos.Controllers
{
    public class RechazoController : BaseFunctionController, IFunctionController
    {
        // GET: Rechazos/Rechazo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Item(string operation,int? id)
        {
            
            return View("Item");
        }
        public ActionResult EditItem()
        {
            return View("EditItem");
        }

        // GET: Rechazos/Rechazo/Estadisticas
        public ActionResult Estadisticas()
        {
            return View();
        }

        public string VariableName { get { return "PAR_RECHAZO"; } }
        public string GetRefference() { return "PAR_RECHAZO"; }
    }
}