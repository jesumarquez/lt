#region Usings

using System;
using System.Linq;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTurnosAlta : SecuredAbmPage<Shift>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_TURNOS"; } }

        /// <summary>
        /// Associated list page.
        /// </summary>
        protected override string RedirectUrl { get { return "TurnosLista.aspx"; } }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Data binding for editing the current object.
        /// </summary>
        protected override void Bind()
        {
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;

            tpDesde.SelectedTime = TimeSpan.FromHours(EditObject.Inicio);
            tpHasta.SelectedTime = TimeSpan.FromHours(EditObject.Fin);

            chkLunes.Checked = EditObject.Lunes;
            chkMartes.Checked = EditObject.Martes;
            chkMiercoles.Checked = EditObject.Miercoles;
            chkJueves.Checked = EditObject.Jueves;
            chkViernes.Checked = EditObject.Viernes;
            chkSabado.Checked = EditObject.Sabado;
            chkDomingo.Checked = EditObject.Domingo;

            chkAplicaFeriados.Checked = EditObject.AplicaFeriados;

            var vehiculos = EditObject.Asignaciones.OfType<Coche>().Select(p => p.Id).ToList();
            ddlVehiculo.SelectedValues = vehiculos;
        }

        /// <summary>
        /// Company intial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DdlDistritoInitialBinding(object sender, EventArgs e)
        {
            if (EditMode) ddlDistrito.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : EditObject.Linea != null ? EditObject.Linea.Empresa.Id : ddlDistrito.AllValue;
        }

        /// <summary>
        /// Location initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DdlBaseInitialBinding(object sender, EventArgs e) { if (EditMode) ddlBase.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : ddlBase.AllValue; }

        /// <summary>
        /// Deletes the specified shift.
        /// </summary>
        protected override void OnDelete() { DAOFactory.ShiftDAO.Delete(EditObject); }

        /// <summary>
        /// Validate suser data before saving.
        /// </summary>
        protected override void ValidateSave()
        {
            if (tpDesde.SelectedTime >= tpHasta.SelectedTime) throw new Exception(CultureManager.GetError("DESDE_MAYOR_HASTA"));

            if (!(chkDomingo.Checked || chkSabado.Checked || chkViernes.Checked || chkJueves.Checked || chkMiercoles.Checked
                || chkMartes.Checked || chkLunes.Checked)) throw new Exception(CultureManager.GetError("MUST_SELECT_SHIFT_DAYS"));

            var codigo = ValidateEmpty(txtCodigo.Text, "CODE");

            var byCodigo = DAOFactory.ShiftDAO.FindByCode(ddlDistrito.SelectedValues, ddlBase.SelectedValues, codigo);
            ValidateDuplicated(byCodigo, "CODE");
        }

        /// <summary>
        /// Saves the user modified data.
        /// </summary>
        protected override void OnSave()
        {
            EditObject.Linea = ddlBase.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBase.Selected) : null;
            EditObject.Empresa = EditObject.Linea != null ? EditObject.Linea.Empresa : 
                     ddlDistrito.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistrito.Selected) : null;
            
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;

            EditObject.Inicio = tpDesde.SelectedTime.TotalHours;
            EditObject.Fin = tpHasta.SelectedTime.TotalHours;

            EditObject.Lunes = chkLunes.Checked;
            EditObject.Martes = chkMartes.Checked;
            EditObject.Miercoles = chkMiercoles.Checked;
            EditObject.Jueves = chkJueves.Checked;
            EditObject.Viernes = chkViernes.Checked;
            EditObject.Sabado = chkSabado.Checked;
            EditObject.Domingo = chkDomingo.Checked;

            EditObject.AplicaFeriados = chkAplicaFeriados.Checked;

            if (!ddlVehiculo.SelectedValues.Contains(0))
                SaveAssignments();

            DAOFactory.ShiftDAO.SaveOrUpdate(EditObject);
        }

        /// <summary>
        /// Gets security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "PAR_TURNOS"; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves all the Shift Assignments.
        /// </summary>
        private void SaveAssignments()
        {
            EditObject.Asignaciones.Clear();
            foreach (var vehiculo in ddlVehiculo.SelectedValues)
            {
                EditObject.Asignaciones.Add(DAOFactory.CocheDAO.FindById(vehiculo));
            }
        }

        protected void btSemana_Click(object sender, EventArgs e)
        {
            var allChecked = chkViernes.Checked && chkJueves.Checked && chkMiercoles.Checked && chkMartes.Checked && chkLunes.Checked;
            chkViernes.Checked = chkJueves.Checked = chkMiercoles.Checked = chkMartes.Checked = chkLunes.Checked = !allChecked;
        }

        protected void btFinDeSemana_Click(object sender, EventArgs e)
        {
            var allChecked = chkSabado.Checked && chkDomingo.Checked;
            chkSabado.Checked = chkDomingo.Checked = !allChecked;
        }

        #endregion
    }
}
