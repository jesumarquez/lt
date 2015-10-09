using System;
using System.Linq;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using System.Web.UI.WebControls;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class TipoCicloLogisticoAlta : SecuredAbmPage<TipoCicloLogistico>
    {
        protected override String RedirectUrl { get { return "TipoCicloLogisticoLista.aspx"; } }
        protected override String VariableName { get { return "PAR_TIPO_CICLO_LOGISTICO"; } }
        protected override String GetRefference() { return "PAR_TIPO_CICLO_LOGISTICO"; }
        
        private void BindNoAsignados()
        {
            lstNoAsignados.Items.Clear();

            var estados = DAOFactory.EstadoLogisticoDAO.GetByEmpresa(cbEmpresa.Selected);

            if (estados.Count() > 0)
            {
                var items = estados.Select(r => new ListItem(r.Descripcion, r.Id.ToString("#0"))).OrderBy(l => l.Text).ToArray();
                lstNoAsignados.Items.AddRange(items);
            }
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;

            BindNoAsignados();
            lstAsignados.Items.Clear();
            
            if (EditObject.Estados.Count > 0)
            {
                var estados = EditObject.Estados.Cast<EstadoLogistico>().OrderBy(r => r.Descripcion);
                foreach (var estado in estados)
                {
                    var item = new ListItem(estado.Descripcion, estado.Id.ToString("#0"));
                    lstAsignados.Items.Add(item);
                    lstNoAsignados.Items.Remove(item);
                }
            }
        }

        protected void cbEmpresaOnSelectedIndexChanged(object sender, EventArgs e)
        {
            BindNoAsignados();
            lstAsignados.Items.Clear();
        }

        protected override void OnDelete()
        {
            DAOFactory.TipoCicloLogisticoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;

            EditObject.Estados.Clear();
            foreach (ListItem item in lstAsignados.Items)
            {
                var estado = DAOFactory.EstadoLogisticoDAO.FindById(Convert.ToInt32(item.Value));
                EditObject.Estados.Add(estado);
            }
            
            DAOFactory.TipoCicloLogisticoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            var codigo = ValidateEmpty(txtCodigo.Text, "CODE");
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");

            var byCode = DAOFactory.TipoCicloLogisticoDAO.FindByCodigo(cbEmpresa.Selected, codigo);
            ValidateDuplicated(byCode, "CODIGO");
        }

        protected void BtnAgregarOnClick(object sender, EventArgs e)
        {
            var indices = lstNoAsignados.GetSelectedIndices().OrderByDescending(i => i);
            foreach (var indice in indices)
            {
                var item = lstNoAsignados.Items[indice];
                lstAsignados.Items.Add(item);
                lstNoAsignados.Items.RemoveAt(indice);
            }

            lstAsignados.SelectedIndex = -1;
        }

        protected void BtnEliminarOnClick(object sender, EventArgs e)
        {
            var indices = lstAsignados.GetSelectedIndices().OrderByDescending(i => i);
            foreach (var indice in indices)
            {
                var item = lstAsignados.Items[indice];
                lstNoAsignados.Items.Add(item);
                lstAsignados.Items.RemoveAt(indice);
            }

            lstNoAsignados.SelectedIndex = -1;
        }
       
        protected void cbEmpresa_PreBind(Object sender, EventArgs e) { if (EditMode) cbEmpresa.EditValue = EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.NullValue; }
    }
}
