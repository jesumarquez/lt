using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Logictracker.Web.Controllers.api
{
    public class DummyController : ApiController
    {
        // GET: api/Dummy
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Dummy/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Dummy
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Dummy/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Dummy/5
        public void Delete(int id)
        {
        }
    }
}
