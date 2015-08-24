using System;
using System.ServiceModel;
using System.Text;

namespace Logictracker.Interfaces.SqlToWebServices
{
    public static class ServiceConfig
    {
        public static BasicHttpBinding GetHttpBinding()
        {
            var binding = new BasicHttpBinding
            {
                SendTimeout = TimeSpan.FromMinutes(1),
                OpenTimeout = TimeSpan.FromMinutes(1),
                CloseTimeout = TimeSpan.FromMinutes(1),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                AllowCookies = false,
                BypassProxyOnLocal = false,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MessageEncoding = WSMessageEncoding.Text,
                TextEncoding = Encoding.UTF8,
                TransferMode = TransferMode.Buffered,
                UseDefaultWebProxy = true
            };
            // I think most (or all) of these are defaults--I just copied them from app.config:
            binding.TransferMode = TransferMode.Buffered;
            binding.MaxBufferPoolSize = 524288;
            binding.MaxBufferSize = 65536;
            binding.MaxReceivedMessageSize = 65536;
            return binding;
        }
		public static EndpointAddress GetEndpointAddress(String asmxFile)
        {
            //return new EndpointAddress("http://localhost/Logictracker/App_Services/" + asmxFile);
            return new EndpointAddress("http://web.logictracker.com/App_Services/" + asmxFile);
        }
    }
}
