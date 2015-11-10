using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logictracker.Web.Controllers
{
    public class OrdersController : BaseFunctionController, IFunctionController
    {
        // GET: Orders
        public ActionResult Index()
        {
            return View("OrdersList");
        }

        public string ReferenceName
        {
            get
            {
                return "Orders";
            }
        }
    }
}