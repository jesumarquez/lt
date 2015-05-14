using System;
using System.ServiceModel;
using System.Text;

namespace Logictracker.Process.Import.Client.RemoteServer
{
    public static class Server
    {
        private static ImportSoapClient _import;
        public static ImportSoapClient Import
        {
            get
            {
                return _import ?? (_import = new ImportSoapClient(GetHttpBinding(), new EndpointAddress(ConfigImportClient.ServerUrl)));
            }
        }

        private static BasicHttpBinding GetHttpBinding()
        {
            //var binding = new BasicHttpBinding // PARA http
            //var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport) // PARA https
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
    }
}
