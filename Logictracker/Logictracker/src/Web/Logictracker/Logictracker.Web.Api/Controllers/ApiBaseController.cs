using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LogicTracker.App.IServices;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class ApiBaseController : ApiController
    {
        public IRouteService RouteService { get; set; }

        public String GetDeviceId(HttpRequestMessage request)
        {
            String DeviceId = null;
            IEnumerable<string> keys = null;

            if (request.Headers.TryGetValues("DeviceId", out keys))
            {
                DeviceId = keys.First();
            }

            return DeviceId;
        }

        public void ValidateRequest(HttpRequestMessage request)
        {
            if (GetDeviceId(request) == null)
            {
                ThrowAnauthorized();
            }
            else
            {
                var device = RouteService.GetDeviceByImei(GetDeviceId(request));

                if (device == null)
                    ThrowAnauthorized();
            }
        }

        public static void ThrowAnauthorized()
        {

            //var errorResponse = Request.CreateErrorResponse(statusCode, message);
            //throw new HttpResponseException(Reques);

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        }
    }
}
