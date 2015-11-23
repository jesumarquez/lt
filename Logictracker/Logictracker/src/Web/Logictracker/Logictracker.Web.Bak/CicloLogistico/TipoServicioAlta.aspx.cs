using System;
using System.Globalization;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.CicloLogistico
{
    public partial class TipoServicioAlta : SecuredAbmPage<TipoServicioCiclo>
    {
        protected override string VariableName
        {
            get { return "CLOG_TIPOSERVICIO"; }
        }

        protected override string RedirectUrl
        {
            get { return "TipoServicioLista.aspx"; }
        }

        protected override string GetRefference()
        {
            return "CLOG_TIPOSERVICIO";
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            txtDemora.Text = EditObject.Demora.ToString(CultureInfo.InvariantCulture);
            chkDefault.Checked = EditObject.Default;
        }

        protected override void OnDelete()
        {
            DAOFactory.TipoServicioCicloDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                    EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                    EditObject.Codigo = txtCodigo.Text.Trim();
                    EditObject.Descripcion = txtDescripcion.Text.Trim();
                    EditObject.Demora = Convert.ToInt32(txtDemora.Text.Trim());
                    EditObject.Default = chkDefault.Checked;

                    if (EditObject.Default)
                    {
                        var list = DAOFactory.TipoServicioCicloDAO.FindList(cbEmpresa.SelectedValues, cbLinea.SelectedValues);
                        foreach (var tipoServicio in list)
                        {
                            if (tipoServicio.Id == EditObject.Id) continue;
                            tipoServicio.Default = false;
                            DAOFactory.TipoServicioCicloDAO.SaveOrUpdate(tipoServicio);
                        }
                    }

                    DAOFactory.TipoServicioCicloDAO.SaveOrUpdate(EditObject);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            var code = ValidateEmpty(txtCodigo.Text, "CODE");
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            var demora = ValidateEmpty(txtDemora.Text, "DESVIO");
            ValidateInt32(demora, "DEMORA");

            var byCode = DAOFactory.TipoServicioCicloDAO.FindByCode(cbEmpresa.SelectedValues, cbLinea.SelectedValues, code);
            ValidateDuplicated(byCode, "CODE");
        }

    }
}