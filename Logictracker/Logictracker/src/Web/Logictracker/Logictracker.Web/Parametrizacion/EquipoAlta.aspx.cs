#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

#endregion

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_EquipoAlta : SecuredAbmPage<Equipo>
    {
        #region Private Constants

        private const string CLIENT = "CLIENT";
        private const string CODE = "CODE";
        private const string DESCRIPCION = "DESCRIPCION";
        private const string DIRECCION = "DIRECCION";

        #endregion

        #region Protected Properties

        protected override string VariableName { get { return "PAR_EQUIPOS"; } }
        protected override string RedirectUrl { get { return "EquipoLista.aspx"; } }
        protected override string GetRefference() { return "EQUIPO"; }

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            chkExistente.Attributes.Add("onclick",
                                        string.Format(@"
                var panSel = $get('{0}'); var panNew = $get('{1}');
                if(panSel.style.display == 'none') {{ panSel.style.display = '';panNew.style.display = 'none';}}
                else {{ panNew.style.display = '';panSel.style.display = 'none';}}", panSelectGeoRef.ClientID, panNewGeoRef.ClientID));
        }
        protected override void Bind()
        {
            cbCliente.EditValue = EditObject.Cliente.Id;
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            cbEmpleado.EditValue = EditObject.Empleado != null ? EditObject.Empleado.Id:-2;
            cbTarjeta.EditValue = EditObject.Tarjeta != null ? EditObject.Tarjeta.Id : -1;

            SelectGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
            EditEntityGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);

            DocumentList1.LoadDocumentos(-1, -1, -1, -1, EditObject.Id);
        }

        protected override void OnDelete() { DAOFactory.EquipoDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Cliente = DAOFactory.ClienteDAO.FindById(cbCliente.Selected);
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Empleado = cbEmpleado.Selected == -2 
                                      ? null
                                      : DAOFactory.EmpleadoDAO.FindById(cbEmpleado.Selected);
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : EditObject.Linea != null ? EditObject.Linea.Empresa : null;
            EditObject.Tarjeta = cbTarjeta.Selected == -1
                                     ? null
                                     : DAOFactory.TarjetaDAO.FindById(cbTarjeta.Selected);

            if (chkExistente.Checked)
            {
                EditObject.ReferenciaGeografica = SelectGeoRef1.Selected > 0 ? DAOFactory.ReferenciaGeograficaDAO.FindById(SelectGeoRef1.Selected) : null;
            }
            else
            {
                EditObject.ReferenciaGeografica = EditEntityGeoRef1.GetNewGeoRefference();
                EditObject.ReferenciaGeografica.Descripcion = EditObject.Descripcion;
                EditObject.ReferenciaGeografica.Codigo = EditObject.Codigo;
                EditObject.ReferenciaGeografica.Empresa = EditObject.Empresa;
                EditObject.ReferenciaGeografica.Linea = EditObject.Linea;
                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(EditObject.ReferenciaGeografica);
                STrace.Trace("QtreeReset", "EquipoAlta");
            }

            DAOFactory.EquipoDAO.SaveOrUpdate(EditObject);
        }

        protected override void OnDuplicate()
        {
            base.OnDuplicate();
            DocumentList1.ClearDocumentos();
        }

        protected override void ValidateSave()
        {
            base.ValidateSave();

            if (cbCliente.SelectedIndex < 0) ThrowMustEnter(CLIENT);

            if (string.IsNullOrEmpty(txtCodigo.Text.Trim())) ThrowMustEnter(CODE);

            if (string.IsNullOrEmpty(txtDescripcion.Text.Trim())) ThrowMustEnter(DESCRIPCION);

            var georef = EditEntityGeoRef1.GetNewGeoRefference();

            if (georef == null) ThrowMustEnter(DIRECCION);
        }

        /// <summary>
        /// Empleado T initial binding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbEmpleado_PreBind(object sender, EventArgs e)
        {
            if (EditMode) cbEmpleado.EditValue = EditObject.Empleado != null ? EditObject.Empleado.Id : cbEmpleado.NullValue;
        }

        protected void cbEmpresa_InitialBinding(object sender, EventArgs e)
        {
            if (EditMode) cbEmpresa.SelectedValue = EditObject.Empresa != null ? EditObject.Empresa.Id.ToString() : cbEmpresa.NullValue.ToString();
        }

        protected void cbLinea_InitialBinding(object sender, EventArgs e)
        {
            if (EditMode) cbLinea.SelectedValue = EditObject.Linea != null ? EditObject.Linea.Id.ToString() : cbLinea.NullValue.ToString();
        }

        #endregion
    }
}
