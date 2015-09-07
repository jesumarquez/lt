using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class PlanAlta : SecuredAbmPage<Plan>
    {
        protected override string VariableName { get { return "PAR_PLANES"; } }

        protected override string RedirectUrl { get { return "PlanLista.aspx"; } }

        protected override string GetRefference() { return "PAR_PLANES"; }

        protected override bool AddButton { get { return false; } }

        protected override bool DuplicateButton { get { return false; } }

        protected override void Bind()
        {
            cbEmpresa.SelectedValue = EditObject.Empresa.ToString();
            cbTipoLinea.SelectedValue = EditObject.TipoLinea.ToString();
            txtCodigoAbono.Text = EditObject.CodigoAbono;
            txtCosto.Text = EditObject.CostoMensual.ToString("#0.00");
            txtAbonoDatos.Text = EditObject.AbonoDatos.ToString("#0");
            cbUnidadMedida.SelectedValue = EditObject.UnidadMedida.ToString();
        }

        protected override void OnDelete()
        {
            DAOFactory.PlanDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected;
            EditObject.TipoLinea = cbTipoLinea.Selected;
            EditObject.CodigoAbono = txtCodigoAbono.Text.Trim();
            EditObject.CostoMensual = Convert.ToDouble((string) txtCosto.Text.Replace(".",","));
            EditObject.AbonoDatos = Convert.ToInt32((string) txtAbonoDatos.Text);
            EditObject.UnidadMedida = cbUnidadMedida.Selected;
            
            DAOFactory.PlanDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEmpty((string) txtCodigoAbono.Text, (string) "CODIGO_ABONO");
            ValidateDouble(txtCosto.Text, "COSTO_MENSUAL");
            ValidateInt32(txtAbonoDatos.Text, "ABONO_DATOS");
        }
    }
}
