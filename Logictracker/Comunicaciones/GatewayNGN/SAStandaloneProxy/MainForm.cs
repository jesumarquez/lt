#region Usings

using System;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;
using Urbetrack.Comm.Core.Fleet;
using Urbetrack.Comm.Core.Transport;
using Urbetrack.Comm.Core.Transport.XBeeRLP;
using Urbetrack.Comm.Tools;
using Urbetrack.Gateway.Joint.MessageQueue;
using Urbetrack.Messaging;
using Urbetrack.Toolkit;
using XbeeCore;

#endregion

namespace Urbetrack.GatewayMovil
{
    public partial class MainForm : Form
    {
        private readonly MessageQueueMonitor posiciones;
        private readonly MessageQueueMonitor combustible;

        public MainForm()
        {
            InitializeComponent();
            posiciones =
                new MessageQueueMonitor(Config.GetConfigurationString("queues", "posiciones", @".\private$\posiciones"));
            combustible =
                new MessageQueueMonitor(Config.GetConfigurationString("queues", "combustible", @".\private$\combustible"));
        }

        private delegate void MainForm_NodeChangeHandler(XBeeAPIPort sender, XBeeNode node);
        public void MainForm_NodeComesUp(XBeeAPIPort sender, XBeeNode node)
        {
            var d = Devices.i().Find(node.IMEI());
            if (d == null) return;
            if (LookupDevice(d.Id) != -1) return;
            AddDevice(d);
        }

        public void MainForm_NodeComesDown(XBeeAPIPort sender, XBeeNode node)
        {
            var d = Devices.i().Find(node.IMEI());
            RemoveDevice(d);
        }

        const float kilo = 1024;
        const float mega = kilo * kilo;
        const float giga = mega * kilo;

        static string HumanReadeable(int val)
        {
            var value = (float)val;
            if (value > giga) 
            {
                return String.Format("{0:F1} GB", value/giga);
            }
            if (value > mega) 
            {
                return String.Format("{0:F1} MB", value/mega);
            }
            return value > kilo ? String.Format("{0:F1} KB", value/kilo) : String.Format("{0:F0} Bytes", value);
        }

        public delegate void MainForm_ReceiveReportHandler(Device d);
        public void MainForm_ReceiveReport(Device d)
        {
            if (d == null)
            {
                T.TRACE("MAINFORM: ReceiveReport Device es NULL");
                return;
            }

            if (LookupDevice(d.Id) == -1)
            {
                T.TRACE("MAINFORM: Add Device");
                AddDevice(d);
            }
            T.TRACE("MAINFORM: UpdateDevice");
            UpdateDevice(d);
        }

        internal static XBeeAPIPort port()
        {   
            var c = Marshall.instance.GetService("Urbetrack") as ServerHandler;
            if (c != null)
            {
                return c.transporteXBEE == null ? null : c.transporteXBEE.GetUart(0);
            }
            return null;
        }
        
        internal static TransporteXBEE transporte()
        {
            var c = Marshall.instance.GetService("Urbetrack") as ServerHandler;
            return c != null ? c.transporteXBEE : null;
        }

        bool events_ready;
        private void updater_Tick(object sender, EventArgs e)
        {
            UpdateStates();

            if (port() == null) return;
            if (!events_ready)
            {
                T.TRACE("Activando Eventos");
                port().NodeComesDown += MainForm_NodeComesDown;
                port().NodeComesUp += MainForm_NodeComesUp;
                transporte().ReceiveReport += MainForm_ReceiveReport;
                events_ready = true;
            }

            lblCTS.BackColor = (port().CTS ? Color.Green : Color.Red);
            lblDSR.BackColor = (port().DSR ? Color.Green : Color.Red);
            lblCD.BackColor = (port().CD ? Color.Green : Color.Red);
            switch (port().Estado)
            {
                case XBeeAPIPort.Estados.MODULO_ENLINEA:
                    lblXbeeStat.Text = String.Format("MODEM {0}", port().MyXbeeAddress);
                    lblXbeeStat.ForeColor = Color.Green;
                    break;
                case XBeeAPIPort.Estados.PUERTO_ABIERTO:
                    lblXbeeStat.Text = "PUERTO OK";
                    lblXbeeStat.ForeColor = Color.Yellow;
                    break;
                case XBeeAPIPort.Estados.PUERTO_CERRADO:
                    lblXbeeStat.Text = "PUERTO ERR";
                    lblXbeeStat.ForeColor = Color.DarkRed;
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitGrid();
            Gateway.Joint.SQLServer.Devices.InitializeProgress += Devices_InitializeProgress;
            Marshall.instance.Start();  
            posiciones.Start();
            combustible.Start();
            UpdateStates();
        }

        private void Devices_InitializeProgress(int total_steps, int done_steps)
        {
            if (InvokeRequired)
            {
                Invoke(new Gateway.Joint.SQLServer.Devices.InitializeProgressHandler(Devices_InitializeProgress),
                       total_steps, done_steps);
            }
            else
            {
                if (total_steps == 0)
                {
                    BackColor = Color.FromArgb(250, 184, 2);
                    Text = "Urbetrack - Gateway Movil";
                    lblXbeeStat.Text = "ESPERA MODEM";
                    return;
                }
                Text = String.Format("Urbetrack - Gateway Movil - Descargando Datos {0}%", (done_steps*100/total_steps));
            }
        }

        bool StartService(ServiceController svc, bool start)
        {
            try
            {
                svc.Refresh();
                if (svc.Status == ServiceControllerStatus.Running)
                {
                    return true;
                }
                if (!start) return false;
                svc.Start();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void UpdateStates()
        {
            coreState.Text = Devices.GetSpineConnectionState() == SpineClientWrap.States.CONNECTED
                                 ? "EN LINEA"
                                 : "SIN CONEXION";
            lblStPump.Text = String.Format("{0} ({1})", combustible.State.Description(), combustible.MessagesInQueue);
            lblStPump.ForeColor = combustible.State.Color();
            lblStPosi.Text = String.Format("{0} ({1})", posiciones.State.Description(), posiciones.MessagesInQueue);
            lblStPosi.ForeColor = posiciones.State.Color();
            //descarga.Enabled = false;
            lblSpineState.Text = Devices.GetSpineConnectionState().Description();
            lblSpineState.ForeColor = Devices.GetSpineConnectionState().Color();
            
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Marshall.instance.Stop();
        }

        private void mnuActiva_Click(object sender, EventArgs e)
        {

        }
               

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.Exit(Process.ExitCodes.Voluntary, "El usuario cerror el formulario.");
        }

        private readonly StackStats dialog = new StackStats();
        private void label1_Click(object sender, EventArgs e)
        {
            dialog.Show(this);
        }

        private void devices_DoubleClick(object sender, EventArgs e)
        {
            mnuActiva_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Exit(Process.ExitCodes.Voluntary,"El usuario presiono el boton 'Terminar'");
        }

        private SourceGrid.Cells.ColumnHeader HeaderVehiculo;
        private SourceGrid.Cells.ColumnHeader HeaderEstado;
        private SourceGrid.Cells.ColumnHeader HeaderDatosPendientes;
        private SourceGrid.Cells.ColumnHeader HeaderProgreso;
        private SourceGrid.Cells.ColumnHeader HeaderDatos;

        private void InitGrid()
        {
            var headerView = new SourceGrid.Cells.Views.ColumnHeader();

            // Agrego la fila con los titulos de las columnas.
            var row = Grid.RowsCount;
            Grid.Rows.Insert(row);

            // la oculto por que esta por las dudas , quedo de mas.
            HeaderVehiculo = new SourceGrid.Cells.ColumnHeader(" Vehiculo ")
            {
                View = headerView,
                AutomaticSortEnabled = false
            };
            Grid[row, 0] = HeaderVehiculo;

            HeaderEstado = new SourceGrid.Cells.ColumnHeader(" Estado ")
            {
                View = headerView,
                AutomaticSortEnabled = false
            };
            Grid[row, 1] = HeaderEstado;

            HeaderDatosPendientes = new SourceGrid.Cells.ColumnHeader(" Datos Pendientes ")
            {                
                View = headerView,
                AutomaticSortEnabled = false
            };
            Grid[row, 2] = HeaderDatosPendientes;

            HeaderProgreso = new SourceGrid.Cells.ColumnHeader(" Progreso ")
            {
                View = headerView,
                AutomaticSortEnabled = false
            };
            Grid[row, 3] = HeaderProgreso;

            HeaderDatos = new SourceGrid.Cells.ColumnHeader(" Datos adicionales ")
            {
                View = headerView,
                AutomaticSortEnabled = false
            };
            Grid[row, 4] = HeaderDatos;            


            Grid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            Grid.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            Grid.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize;
            Grid.Columns[3].MinimalWidth = 100;
            Grid.Columns[3].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            Grid.Columns[4].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            Grid.AutoStretchColumnsToFitWidth = true;
            Grid.AutoSizeCells();
            Grid.Columns.StretchToFit();
        }


        void RemoveDevice(Device d)
        {
            if (Grid.InvokeRequired)
            {
                Grid.Invoke(new AddDeviceHandler(RemoveDevice), d);
                return;
            }

            var pos = LookupDevice(d.Id);
            if (pos == -1) return;

            
        }

        delegate void AddDeviceHandler(Device d);
        void AddDevice(Device d)
        {
            if (Grid.InvokeRequired)
            {
                Grid.Invoke(new AddDeviceHandler(AddDevice), d);
                return;
            }
            
            var pos = Grid.RowsCount;
            Grid.Rows.Insert(pos);

            Grid[pos, HeaderVehiculo.Column.Index] = new SourceGrid.Cells.Cell(d.Vehicle)
            {
                ToolTipText = String.Format("Dispositivo: Id={0} Codigo={1}", d.Id, d.LegacyCode),
                Tag = d.Id
            };

            Grid[pos, HeaderEstado.Column.Index] = new SourceGrid.Cells.Cell(d.XBeeSession.State.Description())
            {
                Tag = d
            };

            Grid[pos, HeaderDatosPendientes.Column.Index] = new SourceGrid.Cells.Cell(HumanReadeable((d.XBeeSession.Report.Tracking) * 20))
            {
                Tag = d.LegacyCode
            };

            var progressBar = new ProgressBar();
            progressBar.Value = 0;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;

            Grid[pos, HeaderProgreso.Column.Index] = new SourceGrid.Cells.Cell("0")
            {
                Tag = progressBar
            };
                        
            Grid.LinkedControls.Add(new SourceGrid.LinkedControlValue(progressBar, new SourceGrid.Position(pos, HeaderProgreso.Column.Index)));

            Grid[pos, HeaderDatos.Column.Index] = new SourceGrid.Cells.Cell(d.XBeeSession.DataReport);

            var menuController = new PopupMenu(this);

            Grid[pos, 0].AddController(menuController);
            Grid[pos, 1].AddController(menuController);
            Grid[pos, 2].AddController(menuController);
            Grid[pos, 3].AddController(menuController);
            Grid[pos, 4].AddController(menuController);

            Grid.AutoSizeCells();
            Grid.Columns.StretchToFit();
        }

        int LookupDevice(int id)
        {
            for (var i = 1; i < Grid.Rows.Count; ++i)
            {
                if (((Device)Grid[i, 1].Tag).Id != id) continue;
                return i;
            }
            return -1;
        }

        delegate void UpdateDeviceHandler(Device d);
        void UpdateDevice(Device d)
        {
            if (Grid.InvokeRequired)
            {
                T.TRACE("InvokeGridRequerido");
                Grid.Invoke(new UpdateDeviceHandler(UpdateDevice), d);
                return;
            }
            T.TRACE("UpdateDevice");
            var pos = LookupDevice(d.Id);
            if (pos == -1) return;

            Grid[pos, 1].Value = d.XBeeSession.StateDescription;
            Grid[pos, 2].Value = HumanReadeable(d.XBeeSession.PendingBytes);
            ((ProgressBar)Grid[pos, 3].Tag).Value = d.XBeeSession.CurrentProgress;
            if (d.XBeeSession.State == XBeeSession.SessionStates.FINISHED)
            {
                Grid[pos, 4].Value = d.XBeeSession.DataReport + " ";
            }
            Grid[pos, 4].Value = d.XBeeSession.DataReport;
            Grid.InvalidateCell(Grid[pos, 1]);
            Grid.InvalidateCell(Grid[pos, 2]);
            Grid.InvalidateCell(Grid[pos, 3]);
            Grid.InvalidateCell(Grid[pos, 4]);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //StartService(svcInterQ, true);
            //StartService(svcOVPN, true);
            Devices.i().LockUpdateDevices();
            var aaa = new DBUpdateForm();
            if (aaa.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show(
                "La actualizacion se completo con exito, la aplicacion se cerrara.",
                "Datos Actualizados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Exit();
                return;
            }
            Devices.i().ReleaseUpdateDevices();
            MessageBox.Show(
                "La actualizacion fallo, asegurese que su conexion a internet funciona correctamente y que la red privada se encuentra operativa. Vuelva a intentarlo en algunos minutos, si el problema persiste, comuniquese con el administrador del sisitema. Gracias.",
                "Imposible actualizar los datos", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            
        }

        private readonly Nomenclaturas nomenclas = new Nomenclaturas();
        
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            nomenclas.Show();
        }

        private void MainForm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            nomenclas.Show();

        }

        private void label5_DoubleClick(object sender, EventArgs e)
        {
            lblSpineState.Visible = true;
            lblInterQ.Visible = true;
            label3.Visible = true;
            label4.Visible = true;

        }

        private void lblStPump_Click(object sender, EventArgs e)
        {

        }
                
    }

    public class PopupMenu : SourceGrid.Cells.Controllers.ControllerBase
    {
        readonly ContextMenu menu = new ContextMenu();
        readonly MainForm Form;

        public PopupMenu(MainForm form)
        {
            Form = form;
            menu.MenuItems.Add("Inicia la descarga de datos de seguimiento.", Menu1_Click);
        }

        int active_row;
        public override void OnMouseUp(SourceGrid.CellContext sender, MouseEventArgs e)
        {
            base.OnMouseUp(sender, e);

            if (e.Button == MouseButtons.Right)
            {
                active_row = sender.Position.Row;
                menu.Show(sender.Grid, new Point(e.X, e.Y));

            }
        }

        private void Menu1_Click(object sender, EventArgs e)
        {
            var d = Form.Grid[active_row, 1].Tag as Device;
            if (d == null)
            {
                MessageBox.Show("Ha ocurrido un error, vuelva a intentarlo.");
                return;
            }

            if (d.XBeeSession.State != XBeeSession.SessionStates.READY_ENQUEUED)
            {
                MessageBox.Show(String.Format("El vehiculo debe figurar \"{0}\" para poder iniciar la descarga", XBeeSession.SessionStates.READY_ENQUEUED.Description()));
                return;
            }

            //var result = MessageBox.Show("ATENCION! Asegurese que el vehiculo esta en contacto, si no es asi, no sera posible obtener los datos solicitados.\nSi el vechiculo esta en marcha, presione OK.", "ATENCION!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            //if (result == DialogResult.OK) 
            
            d.XBeeSession.Activate(0x1);
            d.XBeeSession.ActivateTimestamp = DateTime.Now;
        }

        private DateTime ActivateTimestamp;
    }
}