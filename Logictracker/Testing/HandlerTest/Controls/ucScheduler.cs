using System;
using System.Collections.Generic;
using System.Windows.Forms;
using HandlerTest.Classes;
using Logictracker.Scheduler.Core.Tasks;
using Timer = Logictracker.Scheduler.Timer;

namespace HandlerTest.Controls
{
    public partial class ucScheduler : UserControl, IScheduler
    {
        public ITestApp TestApp { get { return ParentForm as ITestApp; } }

        private List<SchedulerTask> Tareas = new List<SchedulerTask>
                {
                    new SchedulerTask("Datamart", "Logictracker.Scheduler.Tasks.Mantenimiento.DatamartGeneration,Logictracker.Scheduler.Tasks.Mantenimiento"),
                    new SchedulerTask("Datos Operativos de Vehiculos", "Logictracker.Scheduler.Tasks.Mantenimiento.VehicleData,Logictracker.Scheduler.Tasks.Mantenimiento"),
                    new SchedulerTask("Vencimiento de Documentación", "Logictracker.Scheduler.Tasks.VencimientoDocumentos.Task,Logictracker.Scheduler.Tasks.VencimientoDocumentos")
                    
                };

        public ucScheduler()
        {
            InitializeComponent();
            
        }
        private void UcSchedulerLoad(object sender, EventArgs e)
        {
            if (TestApp != null) BindTareas();
        }
        private void BindTareas()
        {
            cbTask.DataSource = Tareas;
        }

        private void BtProcesarClick(object sender, EventArgs e)
        {
            var clase = ((SchedulerTask)cbTask.SelectedItem).ClassName;
            var param = txtParameters.Text.Trim();
            var task = GetTask(clase, param);
            if (task == null)
            {
                MessageBox.Show("No se puede construir la el objeto " + clase);
                return;
            }

            task.Execute(new Timer());

        }
        private static ITask GetTask(String className, String parameters)
        {
            try
            {
                var t = Type.GetType(className, true);
                if (t == null) { return null; }

                var constInfo = t.GetConstructor(new Type[0]);
                if (constInfo == null) { return null; }

                var task = (ITask)constInfo.Invoke(null);
                task.SetParameters(parameters);

                return task;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
