using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LogicTracker.App.Web.Api.Controllers
{
    public class BaseController : ApiController
    {
        public static String GetDeviceId(HttpRequestMessage request)
        {
            String DeviceId = null;
            IEnumerable<string> keys = null;

            if (request.Headers.TryGetValues("DeviceId", out keys))
            {
                DeviceId = keys.First();
            }

            return DeviceId;
        }
    }
}
