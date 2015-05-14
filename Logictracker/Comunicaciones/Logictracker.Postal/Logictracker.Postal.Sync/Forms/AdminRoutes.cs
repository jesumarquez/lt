#region Usings

using System;
using System.ComponentModel;
using System.Linq;
using Urbetrack.Postal.Sync.BaseClasses;
using Urbetrack.Types.BusinessObjects.Postal;

#endregion

namespace Urbetrack.Postal.Sync.Forms
{
    /// <summary>
    /// Form for unassining routes.
    /// </summary>
    public partial class AdminRoutes : BaseForm
    {
        #region Private Properties

        /// <summary>
        /// Main form.
        /// </summary>
        private readonly Main _parent;

        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates a new route administration form desabling the calling parent.
        /// </summary>
        /// <param name="parent"></param>
        public AdminRoutes(Main parent)
        {
            InitializeComponent();

            _parent = parent;

            _parent.Enabled = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Unisigns routes, refresh the parent form and closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUnasingRoutesClick(Object sender, EventArgs e)
        {
            try
            {
                if (radAsignadas.Checked)
                {
                    UnassignRoutes();
                }
                else if (radActivas.Checked)
                {
                    DeleteRoutes();
                }
                else if (radEliminadas.Checked)
                {
                    UndeleteRoutes();
                }

                RefreshParent();

                BindRoutes();
                //Close();
            }
            catch(Exception ex) { InformsException(ex, "Ocurrió un error realizando la operación seleccionada."); }
        }

        /// <summary>
        /// Refreshs the list of routes that need to be sincronized within the parent form.
        /// </summary>
        private void RefreshParent() { _parent.RefreshRoutes(); }

        /// <summary>
        /// Unassigns the currently selected routes.
        /// </summary>
        private void UnassignRoutes() { foreach (var distributor in lbRoutes.SelectedItems.OfType<GrupoRuta>()) DaoFactory.RutaDAO.UnassignRoutes(distributor); }

        private void DeleteRoutes() { foreach (var distributor in lbRoutes.SelectedItems.OfType<GrupoRuta>()) DaoFactory.RutaDAO.DeleteRoutes(distributor); }

        private void UndeleteRoutes() { foreach (var distributor in lbRoutes.SelectedItems.OfType<GrupoRuta>()) DaoFactory.RutaDAO.UndeleteRoutes(distributor); }

        /// <summary>
        /// Binds currently assigned routes.
        /// </summary>
        private void BindRoutes()
        {
            try 
            {
                var days = Convert.ToInt32(txtDays.Value);
                if (radAsignadas.Checked)
                {
                    btnUnasignRoutes.Text = "Desasignar";
                    lbRoutes.DataSource = DaoFactory.RutaDAO.GetAssignedDistributors(days);
                }
                else if (radActivas.Checked)
                {
                    btnUnasignRoutes.Text = "Eliminar";
                    lbRoutes.DataSource = DaoFactory.RutaDAO.GetAvailableDistributors(days);
                }
                else if (radEliminadas.Checked)
                {
                    btnUnasignRoutes.Text = "Reactivar";
                    lbRoutes.DataSource = DaoFactory.RutaDAO.GetDeletedDistributors(days);
                }
            }
            catch (Exception ex) { InformsException(ex, "Ocurrio un error intentando obtener las rutas a administrar."); }
        }

        /// <summary>
        /// Enables or disables the unasign route button according to the number of selected routes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbRoutesSelectedIndexChanged(Object sender, EventArgs e) { btnUnasignRoutes.Enabled = lbRoutes.SelectedItems.Count > 0; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Binds currently assigned routes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e) { BindRoutes(); }

        /// <summary>
        /// Dispose the data access factory and re-enables the calling parent form.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            DisposeResources();

            _parent.Enabled = true;
        }

        #endregion

        private void RadioChackedChanged(object sender, EventArgs e)
        {
            BindRoutes();
        }

        private void txtDays_ValueChanged(object sender, EventArgs e)
        {
            BindRoutes();
        }

        private void AdminRoutes_Shown(object sender, EventArgs e)
        {
            CenterToParent();
        }
    }
}
