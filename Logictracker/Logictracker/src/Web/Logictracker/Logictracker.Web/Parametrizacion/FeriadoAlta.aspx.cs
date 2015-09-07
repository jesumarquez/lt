#region Usings

using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_FeriadoAlta : SecuredAbmPage<Feriado>
    {
        #region Private Properties

        /// <summary>
        /// Description variable name.
        /// </summary>
        private const string Descripcion = "DESCRIPCION";

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "PAR_FERIADOS"; } }
        protected override string RedirectUrl { get { return "FeriadoLista.aspx"; } }
        protected override string GetRefference() { return "FERIADO"; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Deletes the current edited object.
        /// </summary>
        protected override void OnDelete() { DAOFactory.FeriadoDAO.Delete(EditObject); }

        /// <summary>
        /// Saves the current edited object.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = ddlEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlEmpresa.Selected) : EditObject.Linea != null ? EditObject.Linea.Empresa : null;
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Fecha = dpFecha.SelectedDate;

            DAOFactory.FeriadoDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Company initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbLinea_PreBind(object sender, EventArgs e) { if (EditMode) cbLinea.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue; }

        /// <summary>
        /// Binds the current edited object.
        /// </summary>
        protected override void Bind()
        {
            txtDescripcion.Text = EditObject.Descripcion;
            dpFecha.SelectedDate = EditObject.Fecha;
        }

        /// <summary>
        /// Empresa mobile Binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlEmpresa_PreBind(object sender, EventArgs e) { if (EditMode) ddlEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : ddlEmpresa.AllValue; }
    
        /// <summary>
        /// Validates the current edited object.
        /// </summary>
        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text)) ThrowMustEnter(Descripcion);
        }

        #endregion
    }
}
