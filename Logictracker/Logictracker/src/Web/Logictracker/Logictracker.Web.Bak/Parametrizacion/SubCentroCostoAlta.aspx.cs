using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class SubCentroCostoAlta : SecuredAbmPage<SubCentroDeCostos>
    {
        protected override string VariableName { get { return "SUBCENTROS_COSTOS"; } }
        protected override string RedirectUrl { get { return "SubCentroCostoLista.aspx"; } }
        protected override string GetRefference() { return "SUBCENTRO_COSTOS"; }

        protected override void OnDelete() { DAOFactory.SubCentroDeCostosDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = ddlEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlEmpresa.Selected) : EditObject.Linea != null ? EditObject.Linea.Empresa : null;
            EditObject.CentroDeCostos = cbCentroDeCostos.Selected > 0 ? DAOFactory.CentroDeCostosDAO.FindById(cbCentroDeCostos.Selected) : null;

            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Codigo = txtCodigo.Text.Trim();
            EditObject.Objetivo = Convert.ToInt32((string) txtObjetivo.Text);

            DAOFactory.SubCentroDeCostosDAO.SaveOrUpdate(EditObject);
        }
        
        protected override void Bind()
        {
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
            txtObjetivo.Text = EditObject.Objetivo.ToString("#0");
        }

        protected void DdlEmpresaPreBind(object sender, EventArgs e) { if (EditMode) ddlEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : ddlEmpresa.AllValue; }

        protected void CbLineaPreBind(object sender, EventArgs e) { if (EditMode) cbLinea.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue; }

        protected void CbCentroDeCostosPreBind(object sender, EventArgs e) { if (EditMode) cbCentroDeCostos.EditValue = EditObject.CentroDeCostos != null ? EditObject.CentroDeCostos.Id : 0; }

        protected override void ValidateSave()
        {
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            ValidateEmpty((string) txtCodigo.Text, (string) "CODE");
            ValidateInt32(txtObjetivo.Text, "OBJETIVO");

            if (!DAOFactory.SubCentroDeCostosDAO.IsCodeUnique(txtCodigo.Text.Trim(), ddlEmpresa.Selected, cbLinea.Selected, cbCentroDeCostos.Selected, EditObject.Id))
                ThrowDuplicated("CODE");
        }
    }
}
