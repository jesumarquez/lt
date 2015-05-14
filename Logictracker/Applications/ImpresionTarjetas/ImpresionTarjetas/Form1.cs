using System;
using System.Linq;
using System.Windows.Forms;
using Tarjetas;

namespace TarjetasSAI
{
    public partial class Form1 : Form
    {
        protected DataAccess data;

        public Form1()
        {
            InitializeComponent();
#if DLS
            cbEmpresa.Enabled = cbCambiarEmpresa.Enabled = btCambiarEmpresa.Enabled= false;
#endif
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            data = new DataAccess();
            LoadEmpresas();
            LoadEmpleados();
            LoadFondo();
        }
        private void cbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmpleados();
        }
        private void LoadEmpleados()
        {
            var empresa = cbEmpresa.SelectedItem as empresa;
            dataGridView1.DataSource = data.GetAllEmpleados(empresa);
            RefreshStatus();
        }
        private void LoadEmpresas()
        {
            var empresas = data.GetAllEmpresas();
            cbEmpresa.DataSource = empresas.ToList();
            cbCambiarEmpresa.ComboBox.DisplayMember = "nombre";
            cbCambiarEmpresa.ComboBox.DataSource = empresas.ToList();
        }

        public void LoadFondo()
        {
            var active = data.GetActiveBack();
            statusEstilo.Text = active == null ? "(ninguno)" : active.nombre;
        }
        
        private void btNewEmp_Click(object sender, EventArgs e)
        {
            var empform = new NewEmpleado();
            empform.Closed += empform_Closed;
            empform.ShowDialog();
        }

        void empform_Closed(object sender, EventArgs e)
        {
            LoadEmpleados();
        }

        private PrintView printview;
        private void btPrint_Click(object sender, EventArgs e)
        {
            if(data.GetActiveBack() == null)
            {
                MessageBox.Show("No hay ningun estilo de tarjeta activo");
                return;
            }

            UpdateChoferes();

            if (printview == null) printview = new PrintView(data);
            printview.Show(this);
            printview.LoadData();
        }
        

        private void UpdateChoferes()
        {
            data.ClearImprimir();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.Selected) continue;
                var emp = row.DataBoundItem as empleado;

                data.AddImprimir(new imprimir
                                     {
                                         legajo = emp.legajo,
                                         apellido = emp.apellido,
                                         nombre = emp.nombre,
                                         documento = emp.documento,
                                         upcode = emp.upcode,
                                         code = emp.code,
                                         foto = emp.foto
                                     });
            }
            data.SubmitChanges();
        }
        
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            dataGridView1.SelectAll();
            RefreshStatus();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Selected = false;
            }
            RefreshStatus();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Selected = !row.Selected;
            }
            RefreshStatus();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var emp = row.DataBoundItem as empleado;
                if (emp == null) continue;
                if (emp.legajo == txtSelLegajo.Text.Trim())
                {
                    row.Selected = true;
                    txtSelLegajo.Text = string.Empty;
                    return;
                }
            }
            
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            lblItemSel.Text = dataGridView1.SelectedRows.Count + " seleccionados.";
            lblTotal.Text = dataGridView1.Rows.Count + " elementos. ";
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            RefreshStatus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Importar Datos
            var impform = new Importador();
            impform.Closed += empform_Closed;
            impform.ShowDialog();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if(!row.Selected) continue;
                var emp = row.DataBoundItem as empleado;
                if (emp == null) continue;
                emp.impreso = !emp.impreso;
                data.SubmitChanges();
            }
        }

        private void btBuscar_Click(object sender, EventArgs e)
        {
            
            var txt = txtBuscar.Text.TrimEnd().ToLower();
            if (string.IsNullOrEmpty(txt))
            {
                btLimpiar_Click(sender, e);
            }
            else
            {
                var currencyManager1 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
                currencyManager1.SuspendBinding();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    var value = new[]
                                    {
                                        row.Cells[1].Value.ToString().ToLower(),
                                        row.Cells[2].Value.ToString().ToLower(),
                                        row.Cells[3].Value.ToString().ToLower(),
                                        row.Cells[4].Value.ToString().ToLower()
                                    };
                    row.Visible = value.Any(x=>x.Contains(txt));
                }
                currencyManager1.ResumeBinding();
            }
        }

        private void btLimpiar_Click(object sender, EventArgs e)
        {
            var currencyManager1 = (CurrencyManager)BindingContext[dataGridView1.DataSource];
            currencyManager1.SuspendBinding();
            txtBuscar.Text = string.Empty;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Visible = true;
            }
            currencyManager1.ResumeBinding();
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var empform = new NewEmpleado();
            empform.Closed += empform_Closed;
            var emp = dataGridView1.Rows[e.RowIndex].DataBoundItem as empleado;
            empform.ShowEmpleado(emp);
            empform.ShowDialog();
        }

        private void btConfig_Click(object sender, EventArgs e)
        {
            var configform = new FrmConfig(data);
            configform.Closed += configform_Closed;
            configform.ShowDialog();
        }

        void configform_Closed(object sender, EventArgs e)
        {
            LoadEmpresas();
            LoadFondo();
        }

        private void btCambiarEmpresa_Click(object sender, EventArgs e)
        {
            var empresa = cbCambiarEmpresa.SelectedItem as empresa;
            var pregunta = string.Format("¿Está seguro que quiere asignar la empresa {0} a {1} elementos?",
                                         empresa.nombre,
                                         dataGridView1.SelectedRows.Count);
            if (MessageBox.Show(pregunta, "Cambiar empresa", MessageBoxButtons.YesNo) == DialogResult.No) return;
            foreach(DataGridViewRow row in dataGridView1.SelectedRows)
            {
                var empleado = row.DataBoundItem as empleado;
                empleado.empresa = empresa.id;
            }
            data.SubmitChanges();
            LoadEmpleados();
        }
    }
}
