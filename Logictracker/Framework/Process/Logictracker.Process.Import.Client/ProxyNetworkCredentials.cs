using System;
using System.Configuration;
using System.Net;

namespace Logictracker.Process.Import.Client
{
    public class ProxyNetworkCredentials : IWebProxy
    {
        public Uri GetProxy(Uri destination)
        {
            var proxy = ConfigurationManager.AppSettings["proxy.address"];
            return new Uri(proxy);
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public ICredentials Credentials
        {
            get
            {
                var username = ConfigurationManager.AppSettings["proxy.username"];
                var password = ConfigurationManager.AppSettings["proxy.password"];
                return new NetworkCredential(username, password);
            }
            set { }
        }
    }
}
