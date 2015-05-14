using System;
using System.Windows.Forms;
using DispatchsExporter.Exporters;
using DispatchsExporter.Types.SessionObejcts;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;

namespace DispatchsExporter.Forms
{
    public partial class ImportMenu : Form
    {
        #region Private Properties

        private readonly Usuario user = Session.user;
        private readonly DataTransferDAO dao = new DataTransferDAO();
        private static ImportMenu instance;

        #endregion

        #region Singleton

        private ImportMenu()
        {
            InitializeComponent();
            btnExportarDespachos.Select();
        }

        /// <summary>
        /// Public Constructor.
        /// </summary>
        public static ImportMenu Instance { get { return instance ?? (instance = new ImportMenu()); } }

        #endregion

        #region Private Methods

        private void btnSalir_Click(object sender, EventArgs e) { Application.Exit(); }

        private void Import_FormClosed(object sender, FormClosedEventArgs e) { Application.Exit(); }

        /// <summary>
        /// Disables the buttons and shows the progress bar.
        /// </summary>
        private void RefreshControlsBeforeInitiatingImport()
        {
            btnVerExportados.Enabled = false;
            btnExportarDespachos.Enabled = false;
            progressBar.Visible = true;
            lblMessage.Visible = true;
            lblMessage.Text = "Por favor, aguarde mientras se importan los datos.";
            lblMessage.Refresh();
        }

        /// <summary>
        /// Imports the Dispatchs and Shows the Reassign Form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshControlsBeforeInitiatingImport();
                ImportDispatchsFromCenters();
                RestoreDefaultControlValues();
                var f = DispatchsReassign.Instance;
                Hide();
                f.Show();
            }
            catch(Exception ex)
            {   
                STrace.Exception(GetType().FullName,ex);
                lblMessage.Text = String.Concat("Error al realizar la importacion de los despachos.\r\n",ex.Message,ex.InnerException);
                btnSalir.Visible = true;
                btnSalir.Enabled = true;
                lblMessage.Visible = true;
                progressBar.Visible = false;
            }
        }

        private void RestoreDefaultControlValues()
        {
            btnExportarDespachos.Enabled = true;
            btnVerExportados.Enabled = true;
            progressBar.Visible = false;
            lblMessage.Visible = false;

        }

        /// <summary>
        /// Imports The Vehicles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportarVehiculos_Click(object sender, EventArgs e)
        {
            var f = DispatchsView.Instance;
            Hide();
            f.Show();
        }

        /// <summary>
        /// Imports all the Centers Data.
        /// </summary>
        private void ImportDispatchsFromCenters()
        {
            progressBar.Value = 0;
            progressBar.Step = 25;
            progressBar.PerformStep();
            dao.ImportDespachosFromAllCentersByUsuario(user);
            progressBar.Step = 50;
            progressBar.PerformStep();
            progressBar.Step = 25;
            progressBar.PerformStep();
        }

        #endregion

    }
}