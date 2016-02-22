using System;
using System.Linq;
using System.Text;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Interfaces.LoJackApi;
using Logictracker.Layers.MessageQueue;
using Logictracker.Model.Utils;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

namespace Logictracker.Scheduler.Tasks.LoJack
{
    public class Task : BaseTask
    {
        private const string ComponentName = "LoJack API Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var service = new Service();
            var user = GetString("user");
            var password = GetString("password");
            var guid = GetString("guid");
            var empresas = DaoFactory.EmpresaDAO.GetList().Where(e => e.LoJackApiHabilitado);

            STrace.Trace(ComponentName, string.Format("LoJack API Habilitada para {0} empresas", empresas.Count()));

            try
            {
                var resultado = service.GetInfoEntidades(user, password, guid);
                if (resultado.Valido && resultado.Entidades != null)
                {
                    var ultimosSucesos = resultado.Entidades;

                    var i = 1;
                    foreach (var ultimoSuceso in ultimosSucesos)
                    {
                        var fecha = ultimoSuceso.DatosGeograficos.FechaPosicion;
                        var latitud = ultimoSuceso.DatosGeograficos.Latitud;
                        var longitud = ultimoSuceso.DatosGeograficos.Longitud;
                        var velocidad = ultimoSuceso.DatosGeograficos.Velocidad;
                        var curso = ultimoSuceso.DatosGeograficos.Rumbo;
                        var patente = ultimoSuceso.Movil.Dominio;
                        var status = ultimoSuceso.DatosGeograficos.Status;                                

                        var coche = DaoFactory.CocheDAO.FindByPatente(empresas.Select(e => e.Id), patente);
                        if (coche != null)
                        {
                            var dispositivo = coche.Dispositivo;
                            if (dispositivo != null)
                            {
                                var result = new StringBuilder();

                                result.Append(string.Format(">RPI{0}", fecha.ToString("ddMMyyHHmmss")));
                                result.Append(string.Format("{0:00.00000}", latitud).Replace(".", "").Replace(",", ""));
                                result.Append(string.Format("{0:000.00000}", longitud).Replace(".", "").Replace(",", ""));
                                result.Append(string.Format("{0:000}{1:000}", curso, velocidad));
                                result.Append(string.Format("{0:0}{1:00}{2:0000}{3:0}{4:000}{5:0}{6:0}{7:00}{8:0000000000}{9:X2}{10:00}", 1, 0, 0, 1, 10, 1, 1, 0, 0, 0, 0));
                                result.Append(string.Format(";ID={0:0000};#{1:X4}<", dispositivo.Id, i));

                                var cmd = new Frame(Encoding.ASCII.GetBytes(result.ToString()), dispositivo.Id);

                                var parser = new Trax.v1.Parser  {Id = dispositivo.Id};;
                                var msg = parser.Decode(cmd, false);

                                if (msg == null)
                                {
                                    STrace.Error(ComponentName, dispositivo.Id, string.Format("Error Parseando: {0}", cmd.ToString()));
                                    continue;
                                }

                                var queue = GetDispatcherQueue();
                                if (queue == null)
                                {
                                    STrace.Error(ComponentName, "Cola no encontrada: revisar configuracion");
                                    return;
                                }

                                var enviado = queue.Send(msg, string.Empty);
                                        
                                if (enviado)
                                    STrace.Trace(ComponentName, dispositivo.Id, string.Format("Mensaje enviado correctamente a la cola {0}", queue.QueueName));
                                else
                                    STrace.Error(ComponentName, dispositivo.Id, string.Format("No se pudo insertar el mensaje en la cola {0}", queue.QueueName));
                            }
                        }
                        i++;
                    }
                }
                else
                {
                    STrace.Error(ComponentName, string.Format("Error al consultar LoJack API. Código: {0}. Mensaje: {1}", resultado.CodigoError, resultado.MensajeError));
                }
            }
            catch (Exception ex)
            {
                STrace.Exception(ComponentName, ex);
            }
        }

        private IMessageQueue GetDispatcherQueue()
        {
            var queueName = GetString("queuename");

            var umq = new IMessageQueue(queueName);

            return !umq.LoadResources() ? null : umq;
        }
    }
}
