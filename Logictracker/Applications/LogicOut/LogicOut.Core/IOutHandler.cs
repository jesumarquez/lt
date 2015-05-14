using System.Configuration;
using LogicOut.Core.Export;

namespace LogicOut.Core
{
    public abstract class IOutHandler
    {
        public abstract void Process();
        public string Name { get; private set; }
        
        protected IOutHandler(string name)
        {
            Name = name;
        }
        protected string GetParameterKey(string paramName)
        {
            return string.Format("logicout.{0}.{1}", Name, paramName);
        }
        protected string GetParameterValue(string paramName)
        {
            return ConfigurationManager.AppSettings[GetParameterKey(paramName)];
        }
        public OutData[] GetData(string query, string parameters)
        {
            Logger.Debug("Pidiendo datos al servidor...");

            var outData = Server.ExportData(query, parameters);

            if (!outData.RespuestaOk)
            {
                Logger.Error("No se pudo obtener los datos del server.\n " + outData.Mensaje);
                return null;
            }

            return outData.Resultado;
        }
        public bool MarkAsDone(string query, int id, bool ok)
        {
            try
            {
                Logger.Debug(string.Format("Marcando registro como sincronizado. Server: {0}. Id: {1}", Config.ServerName, id));
                string param = string.Format("id={0};ok={1};server={2};", id, ok ? "true" : "false", Config.ServerName);
                var result = Server.Export.Done(Config.SessionToken, Config.Company, Config.Branch, query, param);
                return result.RespuestaOk && result.Resultado;
            }
            catch
            {
                Logger.Info("No se pudo marcar el registro como sincronizado.");
                return false;
            }
        }
    }
}
