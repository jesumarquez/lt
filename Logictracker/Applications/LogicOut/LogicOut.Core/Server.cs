using System;
using System.ServiceModel;
using System.Text;
using LogicOut.Core.Export;

namespace LogicOut.Core
{
    public static class Server
    {
        private static ExportSoapClient _export;
        public static ExportSoapClient Export
        {
            get
            {
                return _export ?? (_export = new ExportSoapClient(GetHttpBinding(), new EndpointAddress(Config.ServiceUrl)));
            }
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
            binding.MaxBufferPoolSize = 2147483647;
            binding.MaxBufferSize = 2147483647;
            binding.MaxReceivedMessageSize = 2147483647;
            return binding;
        }

        public static bool Login()
        {
            if (IsSessionActive())
            {
                Logger.Debug("La sesión con el server ya está activa.");
                return true;
            }

            Logger.Debug("Autenticando...");
            var result = Export.Login(Config.UserName, Config.Password);
            if (!result.RespuestaOk) throw new ApplicationException(result.Mensaje);

            Config.SessionToken = result.Resultado;
            Logger.Info("Autenticación correcta.");

            return true;
        }

        public static bool IsSessionActive()
        {
            if (string.IsNullOrEmpty(Config.SessionToken)) return false;
            var result = Export.IsActive(Config.SessionToken);
            if (!result.RespuestaOk) throw new ApplicationException(result.Mensaje);
            return result.Resultado;
        }

        public static RespuestaOfArrayOfOutData ExportData(string query, string parameters)
        {
            Login();
            return Export.ExportData(Config.SessionToken, Config.Company, Config.Branch, query, parameters);
        }
    }
}
