using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Logictracker.Tracker.Tests.Webservice
{
    /// <summary>
    /// Summary description for calculator
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/",
    Description = "A Simple Web Calculator Service",
    Name = "CalculatorWebService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Calculator : System.Web.Services.WebService
    {

        [WebMethod]
        public int Add(int x, int y)
        {
            return x + y;
        }

        [WebMethod]
        public int Subtract(int x, int y)
        {
            return x - y;
        }

        [WebMethod]
        public int Multiply(int x, int y)
        {
            return x*y;
        }

        [WebMethod]
        public int Division(int x, int y)
        {
            return x/y;
        }
    }
}
