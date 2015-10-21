using System.Web.Mvc;
using Logictracker.Tracker.Services;
using Logictracker.Tracker.Application.Services;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class RoutingController : Controller
    {
        IRoutingService RoutingService { get; set; }

        public RoutingController()
        {
            RoutingService = new RoutingService();
        }

        // GET: Routing
        public ActionResult Index()
        {
            ViewBag.Title = "Routing Page";

            //RoutingService.BuildProblem();
            RoutingService.FillTable();
            
            return View();
        }
    }
}