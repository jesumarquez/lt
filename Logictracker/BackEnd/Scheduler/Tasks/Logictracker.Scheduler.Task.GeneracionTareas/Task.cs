using System;
using System.Collections.Generic;
using System.Text;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

namespace Logictracker.Scheduler.Tasks.GeneracionTareas
{
    public class Task : BaseTask
    {
        private const String ComponentName = "Generación de Tareas Scheduler Task";

        protected override void OnExecute(Timer timer)
        {
            STrace.Trace(ComponentName, "Inicio de la tarea");

            var desde = DateTime.UtcNow.AddHours(-3).Date.AddDays(1).AddHours(3);

            switch (DateTime.UtcNow.AddHours(-3).DayOfWeek)
            {
                case DayOfWeek.Friday: desde = desde.AddDays(2); break;
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    STrace.Trace(ComponentName, "Fin de la tarea");
                    return;
            }
            var hasta = desde.AddDays(1);

            var empresas = DaoFactory.EmpresaDAO.GetList();

            foreach (var empresa in empresas)
            {
                var centrosDeCosto = DaoFactory.CentroDeCostosDAO.GetList(new[] {empresa.Id}, new[] {-1}, new[] {-1});

                foreach (var centroDeCosto in centrosDeCosto)
                {
                    if (centroDeCosto.GeneraDespachos)
                    {
                        STrace.Trace(ComponentName, String.Format("Chequeo habilitado para: {0} - ({1})", centroDeCosto.Descripcion, empresa.RazonSocial));

                        var tareas = DaoFactory.ViajeDistribucionDAO.GetList(new[] {empresa.Id},
                                                                             new[] {-1}, // LINEAS
                                                                             new[] {-1}, // TRANSPORTISTAS
                                                                             new[] {-1}, // DEPARTAMENTOS
                                                                             new[] {centroDeCosto.Id},
                                                                             new[] {-1}, // SUB CENTROS DE COSTO
                                                                             new[] {-1}, // VEHICULOS
                                                                             desde,
                                                                             hasta);

                        STrace.Trace(ComponentName, String.Format("Cantidad de tareas cargadas: {0}", tareas.Count));

                        if (tareas.Count == 0)
                        {
                            var responsables = new StringBuilder();
                            if (centroDeCosto.Empleado != null && centroDeCosto.Empleado.Mail.Trim() != string.Empty)
                                responsables = responsables.Append(centroDeCosto.Empleado.Mail.Trim());

                            if (centroDeCosto.Departamento != null && centroDeCosto.Departamento.Empleado != null && centroDeCosto.Departamento.Empleado.Mail.Trim() != string.Empty)
                                responsables = responsables.Append(";" + centroDeCosto.Departamento.Empleado.Mail.Trim());

                            var parameters = new List<string> {centroDeCosto.Descripcion, desde.ToString("yyyy/MM/dd")};

                            SendMail(responsables.ToString(), parameters);
                        }
                    }
                }
            }

            STrace.Trace(ComponentName, "Fin de la tarea");
        }

        private void SendMail(string responsables, List<string> parameters)
        {   
            var configFile = Config.Mailing.GeneracionTareasMailingConfiguration;
            
            if (string.IsNullOrEmpty(configFile)) throw new Exception("No pudo cargarse configuracion de mailing.");

            var sender = new MailSender(configFile);

            SendMailToAllDestinations(sender, parameters, responsables);
        }
    }
}
