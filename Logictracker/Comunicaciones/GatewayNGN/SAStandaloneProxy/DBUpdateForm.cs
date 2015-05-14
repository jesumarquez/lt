using System;
using System.Windows.Forms;
using Urbetrack.Toolkit;

namespace Urbetrack.GatewayMovil
{
    public partial class DBUpdateForm : Form
    {
        private DBSyncTool dbsync;
        public DBUpdateForm()
        {
            InitializeComponent();
            dbsync = new DBSyncTool(ProgressEvent, FinishEvent);
        }

        private void FinishEvent(Task task, Task.TaskResults result)
        {
            if (InvokeRequired)
            {
                Invoke(new Task.TaskResultHandler(FinishEvent), task, result);
                return;
            }
            DialogResult = result == Task.TaskResults.Success ? DialogResult.OK : DialogResult.Cancel;
            Close();
        }

        private void ProgressEvent(string action, string entity, int total_steps, int done_steps)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new DDL.Toolkit.SqlBatchProgressHandler(ProgressEvent), action, entity, total_steps, done_steps);
                return;
            }
            label2.Text = String.Format("{0} {1}", action, entity);

            if (total_steps == -1)
            {
                if (progressBar1.Maximum <= progressBar1.Step + done_steps) 
                    progressBar1.Value += done_steps;
                return;
            }
            progressBar1.Maximum = total_steps;
            progressBar1.Value = done_steps;
        }

        private void DBUpdateForm_Load(object sender, EventArgs e)
        {
            label2.Text = "Conectando...";
        }
    }
}
