using System;
using System.Windows.Forms;
using DispatchsExporter.Types.SessionObejcts;

namespace DispatchsExporter.Forms
{
    public partial class DispatchsView : Form
    {
        #region Private Properties

        private readonly BindingManager.BindingManager bindingManager = Session.bindingManager;
        private static DispatchsView instance;

        #endregion

        #region Singleton

        /// <summary>
        /// Public Constructor.
        /// </summary>
        public static DispatchsView Instance
        {
            get
            {
                if (instance == null) instance = new DispatchsView();
                return instance;
            }
        }

        #endregion

        #region Public Methods

        private DispatchsView()
        {
            InitializeComponent();
            BindCenters();
        }

        /// <summary>
        /// Binds the center DropDown List
        /// </summary>
        private void BindCenters() { bindingManager.BindCenters(ddlCentroDeCarga); }

        #endregion

        #region Private Methods

        private void DispatchsView_FormClosed(object sender, FormClosedEventArgs e) { Application.Exit(); }

        /// <summary>
        /// Searchs the Exported Dispatchs between selected dates.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            grdDespachos.Visible = true;
            bindingManager.BindDespachosGrid(grdDespachos, Convert.ToInt32(ddlCentroDeCarga.SelectedValue), dpDesde.Value, dpHasta.Value);
        }

        #endregion

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var f = ImportMenu.Instance;
            f.Show();
            f.Activate();
            Hide();
        }
    }
}
