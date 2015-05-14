using System;
using System.ServiceModel;
using System.Text;
using Logictracker.Interfaces.LoJackApi.LoJackApi;

namespace Logictracker.Interfaces.LoJackApi
{
    public class Service
    {
        private const String Url = "http://www.lojackgis.com.ar/ApiLogisticaService/Lojack_ApiGenericaWebService.asmx";

        private Service1SoapClient _client;
        private Service1SoapClient Client
        {
            get { return _client ?? (_client = new Service1SoapClient(GetHttpBinding(), new EndpointAddress(Url))); }
        }

        public ResultadoUltimosSucesos GetUltimosSucesos(string userLogin, string userPassword, string guid, int idSucesoRegistro)
        {
            return Client.GetUltimosSucesos(userLogin, userPassword, guid, idSucesoRegistro);
        }

        public ResultadoInfoEntidades GetInfoEntidades(string userLogin, string userPassword, string guid)
        {
            return Client.GetInfoEntidades(userLogin, userPassword, guid);
        }

        private static BasicHttpBinding GetHttpBinding()
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
    }
}
