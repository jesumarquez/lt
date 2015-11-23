#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Organizacion
{
    public partial class AltaSistema : SecuredAbmPage<Sistema>
    {
        #region Protected Properties

        /// <summary>
        /// The list associated page for the abm.
        /// </summary>
        protected override string RedirectUrl { get { return "SubSistemaLista.aspx"; } }

        #endregion

        #region Protected Methods

        protected override string VariableName { get { return "SOC_SISTEMAS"; } }

        /// <summary>
        /// Binds the system being edited.
        /// </summary>
        protected override void Bind()
        {
            ddlMenuResource.SelectedValue = EditObject.Descripcion;
            txtURL.Text = EditObject.Url;
            if (EditObject.Orden > -1) npOrden.Value = EditObject.Orden;
            chkEnabled.Checked = EditObject.Enabled == 1;
        }

        /// <summary>
        /// Deletes the current object being edited.
        /// </summary>
        protected override void OnDelete() { DAOFactory.SistemaDAO.Delete(EditObject); }

        /// <summary>
        /// Saves changes made by the user.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Descripcion = ddlMenuResource.SelectedValue;
            EditObject.Url = txtURL.Text;
            EditObject.Orden = npOrden.Value > -1 ? Convert.ToSByte(npOrden.Value) : (sbyte)-1;
            EditObject.Enabled = (short)(chkEnabled.Checked ? 1 : 0);

            DAOFactory.SistemaDAO.SaveOrUpdate(EditObject);

            ReloadUserFunctions();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "SUBSISTEMA"; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reloads user funtions.
        /// </summary>
        private void ReloadUserFunctions()
        {
            Usuario.SetModules(DAOFactory.PerfilDAO.FindMovMenuBySistema(Usuario.IdPerfiles));
            Usuario.SetSecurables(DAOFactory.PerfilDAO.GetAsegurables(Usuario.IdPerfiles));
        }

        #endregion

    }
}
