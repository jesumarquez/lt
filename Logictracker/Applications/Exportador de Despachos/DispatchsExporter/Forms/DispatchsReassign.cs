using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using DispatchsExporter.Properties;
using DispatchsExporter.Exporters;
using DispatchsExporter.Types.ReportObjects;
using DispatchsExporter.Types.SessionObejcts;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace DispatchsExporter.Forms
{
    public partial class DispatchsReassign : Form
    {
        #region Private Properties

        private readonly Usuario user = Session.user;
        private readonly BindingManager.BindingManager bindingManager = Session.bindingManager;
        private readonly DataTransferDAO dao = new DataTransferDAO();
        private int lastselectedIndex;
        private static DispatchsReassign instance;

        #endregion

        #region Initialization and Bindings

        private DispatchsReassign()
        {
            InitializeComponent();
            BindControls();
            RefreshGrid();
        }

        #region Singleton

        /// <summary>
        /// Public Constructor.
        /// </summary>
        public static DispatchsReassign Instance
        {
            get { return instance ?? (instance = new DispatchsReassign()); }
        }

        #endregion

        /// <summary>
        /// Binds all the controls of the page.
        /// </summary>
        private void BindControls()
        {
            BindCenters();
            BindVehicles();
        }

        /// <summary>
        /// Binds the Vehicles ListBox
        /// </summary>
        private void BindVehicles() { bindingManager.BindVehicles(lbVehicles,(int)ddlCentroCarga.SelectedValue); }

        /// <summary>
        /// Binds the center DropDown List
        /// </summary>
        private void BindCenters() { bindingManager.BindCenters(ddlCentroCarga); }

        /// <summary>
        /// Binds The Grid Data.
        /// </summary>
        private void RefreshGrid()
        {
            if (ddlCentroCarga.SelectedItem == null) return;

            bindingManager.BindReassignmentGrid(grdDespachos,(int) ddlCentroCarga.SelectedValue);

            var list = (List<GridDespacho>)grdDespachos.DataSource;

            foreach (var d in list)
            {
                d.Icon = AllowsReassign(d) ? Resources.wrong : Resources.OK;
                grdDespachos.Refresh();
            }
        }

        #endregion

        #region Events Handling

        private void btnCerrar_Click(object sender, EventArgs e) { Application.Exit(); }
        private void DispatchsView_FormClosed(object sender, FormClosedEventArgs e) { Application.Exit(); }

        /// <summary>
        /// Reloads the grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { lblError.Text = String.Empty; RefreshGrid(); }

        /// <summary>
        /// Generates the JDEdwards File sttopping when it finds a supervisor-assigned dispatch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!ReadyToExport())
            {
                if (user.Lineas.Count > 0) return;
                lblError.ForeColor = Color.Red;
                lblError.Text = "No posee permisos para reasignar despachos. Contacte con su supervisor.";
                lblError.Refresh();
                return;
            }
            var exporter = new ExporterHelper();
            try
            {
                progressBar.Visible = true;
                progressBar.Step = 25;
                lblError.ForeColor = Color.Green;
                lblError.Text = String.Empty;
                lblError.Refresh();
                progressBar.Refresh();

                exporter.ExportDispatchsToLogictracker((int)ddlCentroCarga.SelectedValue);
                lblError.Text = "Despachos exportados correctamente";
                progressBar.PerformStep();
                progressBar.Refresh();
                lblError.Refresh();
                    
                exporter.ExportToJDEdwards((int)ddlCentroCarga.SelectedValue);
                lblError.Text = "Archivos JDEdwards generados correctamente";
                progressBar.PerformStep();
                progressBar.Refresh();
                lblError.Refresh();

                exporter.ExportNivelesToLogictracker((int)ddlCentroCarga.SelectedValue);
                lblError.Text = "Niveles del Tanque exportados correctamente";
                progressBar.PerformStep();
                progressBar.Refresh();
                lblError.Refresh();

                exporter.ExportRemitosToLogictracker((int)ddlCentroCarga.SelectedValue);
                lblError.Text = "Remitos y Ajustes exportados correctamente";
                progressBar.PerformStep();
                progressBar.Refresh();
                lblError.Refresh();

                
                lblError.Text = "Exportación Realizada con éxito";
                lblError.Refresh();
                grdDespachos.DataSource = null;
                grdDespachos.Refresh();
            }
            catch(Exception ex)
            {
                progressBar.Visible = false;
                progressBar.Value = 0;
                lblError.ForeColor = Color.Red;
                lblError.Text = ex.Message;
            }
        }

        /// <summary>
        /// Cheks if the grid is ready to export.
        /// </summary>
        /// <returns></returns>
        private bool ReadyToExport()
        {
            var list = (List<GridDespacho>) grdDespachos.DataSource;
            foreach(var d in list)
            {
                if (AllowsReassign(d))
                {
                    d.Icon = Resources.wrong;
                    grdDespachos.Refresh();
                    return false;
                }
                d.Icon = Resources.OK;
                grdDespachos.Refresh();
            }
            return true;
        }

        /// <summary>
        /// When a dispatch is clicked, if its possible, allows reassignment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdDespachos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //var datasource = (List<GridDespacho>)grdDespachos.DataSource;
            //if (e.RowIndex < 0 || user.Lineas.Count > 0 || (e.RowIndex >= 0 && !AllowsReassign(datasource[e.RowIndex]))) return;
            if (e.RowIndex < 0 || user.Lineas.Count > 0) return;
            lastselectedIndex = e.RowIndex;
            SetAssignmentVisibility(true);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var f = ImportMenu.Instance;
            f.Show();
            f.Activate();
            Hide();
        }

        #endregion

        #region Reassignment Methods

        /// <summary>
        /// Checks if the dispatch needs to be reassigned.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static bool AllowsReassign(GridDespacho d)
        {
            var str = ConfigurationManager.AppSettings["ReAssignmentIDS"];
            var reassignments = str.Split(';');

            foreach (var s in reassignments)
            {
                if (d.Patente.Contains(s) || d.Interno.Contains(s)) return true;
            }
            return false;
        }

        /// <summary>
        /// Reassings the dispatch to the selected mobile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAceptarVehiculo_Click(object sender, EventArgs e)
        {
            var list = (List<GridDespacho>)grdDespachos.DataSource;
            var dispatch = list[lastselectedIndex];
            var mobile = (Coche)lbVehicles.SelectedItem;
            dispatch.Interno = mobile.Interno;
            dispatch.Patente = mobile.Patente;
            dispatch.MobileID = mobile.Id;

            if(mobile.CentroDeCostos != null) dispatch.DescriCentroDeCostos = mobile.CentroDeCostos.Descripcion;

            grdDespachos.DataSource = list;

            dao.UpdateDispatch(dispatch.ID, dispatch.MobileID);          

            SetAssignmentVisibility(false);
            dispatch.Icon = Resources.OK;
            grdDespachos.Refresh();
        }

        /// <summary>
        /// Cancels the assignation of a mobile to a dispatch.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelarVehiculo_Click(object sender, EventArgs e) { SetAssignmentVisibility(false); }


        /// <summary>
        /// Sets the visibility of the dispatch reasignment controls.
        /// </summary>
        /// <param name="status"></param>
        private void SetAssignmentVisibility(bool status)
        {
            reassignamentGrouping.Visible = status;
            grdDespachos.Enabled = !status;
            ddlCentroCarga.Enabled = !status;
        }

        #endregion
    }
}