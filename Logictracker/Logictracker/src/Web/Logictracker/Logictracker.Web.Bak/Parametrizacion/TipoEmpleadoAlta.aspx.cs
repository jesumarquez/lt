#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_TipoEmpleadoAlta : SecuredAbmPage<TipoEmpleado>
    {
        #region Protected Properties

        /// <summary>
        /// Associated list page.
        /// </summary>
        protected override string RedirectUrl { get { return "TipoEmpleadoLista.aspx"; } }

        /// <summary>
        /// Report Name.
        /// </summary>
        protected override string VariableName { get { return "PAR_TIPO_EMPLEADO"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Binds the current edited object.
        /// </summary>
        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
        }

        /// <summary>
        /// Deletes the current edited object.
        /// </summary>
        protected override void OnDelete()
        {
            DAOFactory.TipoEmpleadoDAO.Delete(EditObject);
        }

        /// <summary>
        /// Saves the current edited object.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.FechaBaja = null;

            EditObject.Linea = ddlPlanta.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
            EditObject.Empresa = ddlLocacion.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlLocacion.Selected) : EditObject.Linea != null ? EditObject.Linea.Empresa : null;

            DAOFactory.TipoEmpleadoDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Validates the current edited object for saving.
        /// </summary>
        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtCodigo.Text)) ThrowMustEnter("CODE");
            if (string.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter("DESCRIPCION");
            if (ddlPlanta.Selected.Equals(0)) ThrowMustEnter("LINEA");
        }

        /// <summary>
        /// Gets the security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "TIPOEMPLEADO"; }

        /// <summary>
        /// Sets the initial location being edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlLocacion_InitialBinding(object sender, EventArgs e)
        {
            if (EditMode) ddlLocacion.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : ddlLocacion.AllValue;
        }

        /// <summary>
        /// Sets the initial company being edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlPlanta_InitialBinding(object sender, EventArgs e)
        {
            if (EditMode) ddlPlanta.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : ddlPlanta.AllValue;
        }


        #endregion
    }
}
