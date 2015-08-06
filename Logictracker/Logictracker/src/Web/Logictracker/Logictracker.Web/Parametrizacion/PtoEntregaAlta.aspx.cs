using System;
using System.Linq;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class PtoEntregaAlta : SecuredAbmPage<PuntoEntrega>
    {
        protected override string RedirectUrl { get { return "PtoEntregaLista.aspx"; } }
        protected override string VariableName { get { return "PTOS_ENTREGA"; } }
        protected override string GetRefference() { return "PTO_ENTREGA"; }

        #region Protected Method

        protected void Page_Load(object sender, EventArgs e)
        {
            chkExistente.Attributes.Add("onclick", string.Format(@"var panSel = $get('{0}'); var panNew = $get('{1}');
                if(panSel.style.display == 'none') {{ panSel.style.display = '';panNew.style.display = 'none';}}
                else {{ panNew.style.display = '';panSel.style.display = 'none';}}", panSelectGeoRef.ClientID, panNewGeoRef.ClientID));
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Cliente.Empresa != null ? EditObject.Cliente.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Cliente.Linea != null ? EditObject.Cliente.Linea.Id : cbLinea.AllValue);
            cbResponsable.SetSelectedValue(EditObject.Responsable != null ? EditObject.Responsable.Id : cbResponsable.NoneValue);
            ddlCliente.SetSelectedValue(EditObject.Cliente.Id);

            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            txtTelefono.Text = EditObject.Telefono;

            txtComentario1.Text = EditObject.Comentario1;
            txtComentario2.Text = EditObject.Comentario2;
            txtComentario3.Text = EditObject.Comentario3;

            if (EditMode && !EditObject.Nomenclado)
            {
                lblDireccion.Text = EditObject.DireccionNomenclada;
            }
            else
            {
                SelectGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
                EditEntityGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
            }
        }

        protected override void ValidateSave()
        {
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            var cliente = ValidateEntity(ddlCliente.Selected, "CLIENT");

            var byCode = DAOFactory.PuntoEntregaDAO.FindByCode(new[] {cbEmpresa.Selected}, new[] {cbLinea.Selected}, new[] {cliente}, code);
            ValidateDuplicated(byCode, "CODE");

            var georef = EditEntityGeoRef1.GetNewGeoRefference();
            if(georef == null) ThrowMustEnter("DIRECCION");
        }

        protected override void OnSave()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {
                try
                {
                    EditObject.Cliente = DAOFactory.ClienteDAO.FindById(ddlCliente.Selected);
                    EditObject.Responsable = cbResponsable.Selected > 0 ? DAOFactory.EmpleadoDAO.FindById(cbResponsable.Selected) : null;
                    EditObject.Codigo = txtCodigo.Text;
                    EditObject.Descripcion = txtDescripcion.Text;
                    EditObject.Telefono = txtTelefono.Text;
                    EditObject.Nomenclado = true;

                    EditObject.Comentario1 = txtComentario1.Text;
                    EditObject.Comentario2 = txtComentario2.Text;
                    EditObject.Comentario3 = txtComentario3.Text;

                    if (chkExistente.Checked)
                    {
                        EditObject.ReferenciaGeografica = SelectGeoRef1.Selected > 0
                            ? DAOFactory.ReferenciaGeograficaDAO.FindById(SelectGeoRef1.Selected)
                            : null;
                    }
                    else
                    {
                        EditObject.ReferenciaGeografica = EditEntityGeoRef1.GetNewGeoRefference();
                        EditObject.ReferenciaGeografica.Descripcion = EditObject.Descripcion;
                        EditObject.ReferenciaGeografica.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                        EditObject.ReferenciaGeografica.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;

                        var code = EditObject.Codigo;
                        EditObject.ReferenciaGeografica.Codigo = code;

                        var i = 1;

                        var byCode = DAOFactory.ReferenciaGeograficaDAO.FindByCodigoStartWith(cbEmpresa.SelectedValues, cbLinea.SelectedValues,
                            new[] {EditEntityGeoRef1.TipoReferenciaGeograficaId}, code);

                        while (byCode.Any(r => r.Codigo == code && EditObject.ReferenciaGeografica.Id != r.Id))
                        {
                            code += "(" + i++ + ")";
                        }

                        EditObject.ReferenciaGeografica.Codigo = code;

                        DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(EditObject.ReferenciaGeografica);
                        STrace.Trace("QtreeReset", "PtoEntregaAlta");
                    }

                    DAOFactory.PuntoEntregaDAO.SaveOrUpdate(EditObject);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Deletes the current Point of delivery being edited.
        /// </summary>
        protected override void OnDelete() { DAOFactory.PuntoEntregaDAO.Delete(EditObject); }

        #endregion
    }
}
