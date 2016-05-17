using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Sync;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Web.Parametrizacion
{
    public partial class ParametrizacionTarjetaAlta : SecuredAbmPage<Tarjeta>
    {
        protected override string RedirectUrl { get { return "TarjetaLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TARJETAS"; } }
        protected override string GetRefference() { return "TARJETA"; }

        protected void cbEmpresa_PreBind(object sender, EventArgs e) { if (EditMode) cbEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : EditObject.Linea != null ? EditObject.Linea.Empresa.Id : cbEmpresa.AllValue; }

        protected void cbLinea_PreBind(object sender, EventArgs e) { if (EditMode) cbLinea.EditValue = EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue; }

        protected override void OnSave()
        {
            var syncEmpleado = EditMode && EditObject.Pin != txtPIN.Text.Trim();
            EditObject.Numero = txtNumero.Text.Trim();
            EditObject.Pin = txtPIN.Text.Trim();
            EditObject.PinHexa = txtPinHexa.Text.Trim();
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = cbEmpresa.Selected > 0 
                                    ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) 
                                    : cbLinea.Selected > 0
                                        ? DAOFactory.LineaDAO.FindById(cbLinea.Selected).Empresa 
                                        : null;
            var codigo = GetCodigoAcceso(txtCodAcceso.Text.Trim());
            EditObject.CodigoAcceso = codigo > 0 ? codigo : 0;
            DAOFactory.TarjetaDAO.SaveOrUpdate(EditObject);

            if (syncEmpleado)
            {
                var empleado = DAOFactory.EmpleadoDAO.FindByRfid(cbEmpresa.Selected, EditObject.Pin);
                if(empleado != null)
                {
                    DAOFactory.EmpleadoDAO.EnqueueSync(empleado, OutQueue.Queries.Molinete, OutQueue.Operations.Empleado);
                }
            }
        }

        protected override void OnDelete()
        {
            if (DAOFactory.EmpleadoDAO.FindByTarjeta(EditObject.Id) != null) throw new Exception(CultureManager.GetError("CARD_USED"));

            DAOFactory.TarjetaDAO.Delete(EditObject);
        }

        protected override void Bind()
        {
            txtNumero.Text = EditObject.Numero;
            txtPIN.Text = EditObject.Pin;
            txtPinHexa.Text = EditObject.PinHexa;
            txtCodAcceso.Text = EditObject.CodigoAcceso == 0 ? string.Empty : EditObject.CodigoAcceso.ToString();
        }

        protected override void ValidateSave()
        {
            var numero = ValidateEmpty(txtNumero.Text, "NUMERO");
            var pin = ValidateEmpty(txtPIN.Text, "PIN");
            var codigo = GetCodigoAcceso(ValidateEmpty(txtCodAcceso.Text, "ACCESS_CODE"));

            if (codigo == -2) ThrowError("NO_NUMERIC_CODE_ACCESS");

            var tarjeta = DAOFactory.TarjetaDAO.FindByNumero(cbEmpresa.SelectedValues, numero);
            ValidateDuplicated(tarjeta, "NUMERO");

            tarjeta = DAOFactory.TarjetaDAO.FindByPin(cbEmpresa.SelectedValues, pin);
            ValidateDuplicated(tarjeta, "Pin");

            if (codigo > 0)
            {
                tarjeta = DAOFactory.TarjetaDAO.FindByCodigoAcceso(cbEmpresa.SelectedValues, codigo);
                ValidateDuplicated(tarjeta, "ACCESS_CODE");
            }
        }

        private static int GetCodigoAcceso(string codigoAcceso)
        {
            if (string.IsNullOrEmpty(codigoAcceso)) return -1;
            int i;
            return int.TryParse(codigoAcceso, out i) ? i : -2;
        }        
    }
}
