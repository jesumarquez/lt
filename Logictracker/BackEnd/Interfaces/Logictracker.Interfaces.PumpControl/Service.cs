using System;
using System.ServiceModel;
using System.Text;
using Logictracker.Interfaces.PumpControl.PumpControlService;

namespace Logictracker.Interfaces.PumpControl
{
    public class Service
    {
		private const String Url = "http://usloft2490.serverloft.com/s/wservices/GpsFleetService";

        private GpsFleetServiceClient _client;
        private GpsFleetServiceClient Client
        {
            get { return _client ?? (_client = new GpsFleetServiceClient(GetHttpBinding(), new EndpointAddress(Url))); }
        }

        public GetFirstPendingTransactionResponse GetFirstPendingTransaction(string user, string pass, string company)
        {
		    return GetFirstPendingTransaction(user, pass, company, string.Empty);
        }

        public GetFirstPendingTransactionResponse GetFirstPendingTransaction(string user, string pass, string company, string lastId)
        {
            return GetFirstPendingTransaction(user, pass, company, lastId, string.Empty, null);
        }

        public GetFirstPendingTransactionResponse GetFirstPendingTransaction(string user, string pass, string company, string lastId, string patente, DateTime? desde)
        {
            var obj = new GetFirstPendingTransaction
                          {
                              User = user,
                              Password = pass,
                              Company = company,
                              LastId = lastId,
                              CarPlate = patente,
                              DateFrom = (desde.HasValue ? desde.Value : new DateTime()).ToString("dd-MM-yyyy")
                          };

            return Client.getFirstPendingTransaction(obj);
        }

        public SetLastInformedTransactionResponse SetLastInformedTransaction(string user, string pass, string company, string lastId)
        {
            var obj = new SetLastInformedTransaction
                          {
                              User = user,
                              Password = pass,
                              Company = company,
                              TrxId = lastId
                          };
            
            return Client.setLastInformedTransaction(obj);
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
