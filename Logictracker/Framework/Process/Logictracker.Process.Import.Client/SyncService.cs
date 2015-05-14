using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Timers;
using Logictracker.Process.Import.Client.Mapping;
using Logictracker.Process.Import.Client.Parsers;
using Logictracker.Process.Import.Client.RemoteServer;
using Logictracker.Process.Import.Client.Transform;
using System.Xml.Serialization;

namespace Logictracker.Process.Import.Client
{
    public class SyncService
    {
        private Timer Timer { get; set; }
        public Configuration Configuration { get; set; }
        public string SessionToken { get; set; }
        private bool _stillProcessing;

        public SyncService()
        {
            Configurate();
        }

        public void Start()
        {
            Logger.Info("Servicio Iniciado");
            Timer.Start();
            Timer_Elapsed(Timer, null);
        }

        public void Stop()
        {
            Timer.Stop();
            Logger.Info("Servicio Detenido");
        }

        protected void Configurate()
        {
            try
            {
                Logger.Info("Configurando servicio");
                Timer = new Timer(ConfigImportClient.Interval * 60 * 1000);
                Timer.Elapsed += Timer_Elapsed;

                var mappingFile = ConfigImportClient.MappingFile;

                LoadMapping(mappingFile);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                throw;
            }
        }

        protected void LoadMapping(string mappingFile)
        {
            if(!Path.IsPathRooted(mappingFile.Trim()))
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                mappingFile = Path.Combine(path, mappingFile);
            }
            var fs = new FileStream(mappingFile, FileMode.Open);
            var xml = new XmlSerializer(typeof(Configuration));
            Configuration = (Configuration)xml.Deserialize(fs);
            fs.Close();
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_stillProcessing) return;

            _stillProcessing = true;

            try
            {
                Process();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                _stillProcessing = false;
            }
        }
        public void Process()
        {
            if (!Login()) return;

            var transform = DataTransformFactory.GetDataTransform(-1);
            if (transform == null) throw new DataTransformException();

            var connectionError = false;

            foreach (var import in Configuration.Import)
            {
                if (connectionError) break;
                var imported = 0;

                var strategy = import.DataSource.DataStrategy;
                var newData = strategy.GetNewData();
                if (newData == null || newData.Rows.Count == 0) continue;

                var parsers = import.Entity.Select(ent => ParserFactory.GetEntityParser(ent)).Where(parser => parser != null);

                foreach (var row in newData.Rows)
                {
                    if (connectionError) break;
                    try
                    {
                        foreach (var parser in parsers)
                        {
                            var data = parser.Parse(row);

                            if (data.Operation == -1) continue;

                            var encodedData = transform.Encode(data);

                            Logger.Info("Importing data with session: " + SessionToken);
                            var result = Server.Import.ImportData(SessionToken, ConfigImportClient.Company, ConfigImportClient.Branch, encodedData, data.Version);
                            
                            if (!result.RespuestaOk && !IsSessionActive())
                            {
                                Logger.Info("Inactive session: " + SessionToken);
                                Login();
                                Logger.Info("Importing data with session: " + SessionToken);
                                result = Server.Import.ImportData(SessionToken, ConfigImportClient.Company, ConfigImportClient.Branch, encodedData, data.Version);
                            }

                            if (!result.RespuestaOk) throw new ApplicationException(result.Mensaje);
                            imported++;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(TimeoutException))
                        {
                            Logger.Error("Error de conexión. No se puede acceder al server.");
                            connectionError = true;
                        }
                        else Logger.Error(ex);

                        // TODO: Guardar Registro como mal importado en archivo de errores para reprocesar (csv)
                    }
                }

                if (imported == 0 || connectionError) import.DataSource.DataStrategy.Revert();
            }
        }
        protected bool Login()
        {
            if (IsSessionActive()) return true;

            var result = Server.Import.Login(ConfigImportClient.UserName, ConfigImportClient.Password);
            if (!result.RespuestaOk) throw new ApplicationException(result.Mensaje);

            SessionToken = result.Resultado;
            Logger.Info("New session initialized: " + SessionToken);

            return true;
        }

        protected bool IsSessionActive()
        {
            if (string.IsNullOrEmpty(SessionToken)) return false;
            var result = Server.Import.IsActive(SessionToken);
            if (!result.RespuestaOk)
            {
                Logger.Info(result.Mensaje);
            }
            return result.Resultado;
        }
    }
}
