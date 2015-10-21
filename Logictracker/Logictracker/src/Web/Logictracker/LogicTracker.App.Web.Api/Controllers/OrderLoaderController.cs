using System.Web.Http;
using Logictracker.Tracker.Services;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class OrderLoaderController : ApiController
    {
        IRoutingService RoutingService { get; set; }


    }
}
