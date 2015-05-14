using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Layers.MessageQueue;
using Logictracker.Messaging;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

namespace Logictracker.Scheduler.Tasks.ControlAcceso
{
    public class Task : BaseTask
    {
        private const String ComponentName = "Control Acceso Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");
            var queue = GetDispatcherQueue();
            if (queue == null)
            {
                STrace.Error(ComponentName, "No se encontró la cola del Dispatcher");
                return;
            }
            
            var puertas = DaoFactory.PuertaAccesoDAO.GetList(new[] {-1}, new[] {-1});

            foreach (var puertaAcceso in puertas)
            {
                if (puertaAcceso.Vehiculo != null && puertaAcceso.Vehiculo.Dispositivo != null && DaoFactory.DetalleDispositivoDAO.GetAccesoHabilitadoValue(puertaAcceso.Vehiculo.Dispositivo.Id))
                {
                    STrace.Trace(ComponentName, String.Format("Acceso habilitado: {0}", puertaAcceso.Descripcion));
                    var ip = DaoFactory.DetalleDispositivoDAO.GetAccesoIpValue(puertaAcceso.Vehiculo.Dispositivo.Id);
                    var user = DaoFactory.DetalleDispositivoDAO.GetAccesoUsuarioValue(puertaAcceso.Vehiculo.Dispositivo.Id);
                    var pass = DaoFactory.DetalleDispositivoDAO.GetAccesoPasswordValue(puertaAcceso.Vehiculo.Dispositivo.Id);
                    var inicio = DaoFactory.DetalleDispositivoDAO.GetAccesoInicioValue(puertaAcceso.Vehiculo.Dispositivo.Id);
                    STrace.Trace(ComponentName, String.Format("IP: {0}, User: {1}, Pass: {2}, Inicio: {3}", ip, user, pass, inicio.ToString("dd/MM/yyyy")));

                    var ultimoAcceso = DaoFactory.EventoAccesoDAO.FindLastEventForCompanyAndBase(puertaAcceso.Empresa.Id,
                                                                                                 puertaAcceso.Linea != null
                                                                                                    ? puertaAcceso.Linea.Id
                                                                                                    : -1,
                                                                                                 DateTime.UtcNow);
                    var lastLog = ultimoAcceso != null ? ultimoAcceso.Fecha.AddHours(-3) : inicio;

                    STrace.Trace(ComponentName, String.Format("LastLog: {0}", lastLog.ToString("dd/MM/yyyy HH:mm:ss")));

					var pct100Dumper = new Pct100Dumper(new Uri("http://" + ip), user, pass, lastLog, DateTime.UtcNow);
                    var reader = new StreamReader(pct100Dumper.Execute());
                    var archivo = reader.ReadToEnd();
                    var accesos = archivo.Replace("\r\n", "|").Split('|');
                    var logueos = new List<Logueo>();

                    for (var i = 1; i < accesos.Length - 1; i++)
                    {
                        try
                        {
                            var campos = accesos[i].Split(',');
                            if(campos.Length != 8)
                            {
                                STrace.Trace(ComponentName, String.Format("No se puede parsear: {0}", accesos[i]));
                                continue;
                            }
                            var nroTarjeta = campos[3];
                            var date = campos[4].Split('/');
                            var mes = Convert.ToInt32(date[0]);
                            var dia = Convert.ToInt32(date[1]);
                            var anio = Convert.ToInt32(date[2]);
                            var time = campos[5].Split(':');
                            var hora = Convert.ToInt32(time[0]);
                            var min = Convert.ToInt32(time[1]);
                            var seg = Convert.ToInt32(time[2]);
                            var evento = campos[6];

                            var log = new Logueo
                            {
                                NroTarjeta = nroTarjeta,
                                Fecha = new DateTime(anio, mes, dia, hora, min, seg, DateTimeKind.Utc),
                                Evento = evento == "Entrada" ? 3 : 4
                            };

                            STrace.Trace(ComponentName, String.Format("LogFecha: {0}, LastLog: {1}", log.Fecha, lastLog));

                            if (log.Fecha > lastLog)
                            {
                                STrace.Trace(ComponentName, String.Format("Acceso: {0}", accesos[i]));
                                logueos.Add(log);
                            }
                        }
                        catch (Exception ex)
                        {
                            STrace.Exception(ComponentName, ex);
                        }
                    }

                    foreach (var logueo in logueos.OrderBy(l => l.Fecha))
                    {
                        STrace.Debug(ComponentName, String.Format("Envio: DateTime={0} Rfid={1} Evento={2}", logueo.Fecha.AddHours(3), logueo.NroTarjeta, logueo.Evento));
                        var msg = MessageIdentifierX.FactoryRfid(puertaAcceso.Vehiculo.Dispositivo.Id, 0, null, logueo.Fecha.AddHours(3), logueo.NroTarjeta, logueo.Evento);
                        queue.Send(msg);
                    }
                }
                else
                {
                    STrace.Trace(ComponentName, String.Format("Acceso no habilitado: {0}", puertaAcceso.Descripcion));
                }
            }
        }
        
        private IMessageQueue GetDispatcherQueue()
        {
            var queueName = GetString("queuename");
            var queueType = GetString("queuetype");
            if (String.IsNullOrEmpty(queueName)) return null;

            var umq = new IMessageQueue(queueName);
            if (queueType.ToLower() == "xml") umq.Formatter = "XmlMessageFormatter";

            return !umq.LoadResources() ? null : umq;
        }

        public class Logueo
        {
            public string NroTarjeta { get; set; }
            public DateTime Fecha { get; set; }
            public int Evento { get; set; }
        }
    }
}