using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Logictracker.Web.Controllers.api
{
    public class RechazoController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "id1", "id2" };
        }
    }
}
