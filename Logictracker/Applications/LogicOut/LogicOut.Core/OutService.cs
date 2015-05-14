using System;
using System.Linq;
using System.Timers;

namespace LogicOut.Core
{
    public class OutService
    {
        private Timer Timer { get; set;}
        public void OnStart(string[] args)
        {
            try
            {
                Logger.Info(string.Format("Servicio iniciado.\r\nHandlers:\r\n{0}\r\nIntervalo:{1}\r\n",
                                          Config.Handlers.Aggregate("",
                                                                    (h, n) =>
                                                                    string.Concat("\t", n.Key, " = ", n.Value, "\r\n")),
                                          Config.Interval));
                Timer = new Timer(Config.Interval*60*1000);
                Timer.Elapsed += Timer_Elapsed;
                Timer.Enabled = true;
                Timer.Start();
                Process();
            }
            catch(Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }


        public void OnStop()
        {
            try
            {
                Timer.Stop();
                Timer.Dispose();
                Timer = null;
                Logger.Info("Servicio detenido.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Process();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public void Process()
        {
            Server.Login();

            foreach (var handlerClass in Config.Handlers)
            {
                try
                {
                    var handler = GetHandler(handlerClass.Key, handlerClass.Value);
                    if (handler == null) continue;

                    Logger.Debug("Ejecutando handler " + handlerClass.Key);
                    handler.Process();

                    Logger.Debug("Finalizada handler " + handlerClass.Key);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
        }

        private static IOutHandler GetHandler(string name, string clazz)
        {
            var type = Type.GetType(clazz);
            return Activator.CreateInstance(type, name) as IOutHandler;
        }
    }
}
