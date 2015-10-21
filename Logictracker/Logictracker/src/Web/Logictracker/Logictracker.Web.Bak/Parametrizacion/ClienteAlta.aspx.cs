using System;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class Parametrizacion_ClienteAlta : SecuredAbmPage<Cliente>
    {
        protected override string VariableName { get { return "PAR_CLIENTES"; } }
        protected override string RedirectUrl { get { return "ClienteLista.aspx"; } }
        protected override string GetRefference() { return "RAMAL"; }

        #region Protected Method

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
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);

            txtCodigo.Text = EditObject.Codigo;
            txtDescripcion.Text = EditObject.Descripcion;
            txtDescripcionCorta.Text = EditObject.DescripcionCorta;
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

        protected override void OnSave()
        {
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Empresa = cbEmpresa.Selected > 0
                                     ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected)
                                     : EditObject.Linea != null ? EditObject.Linea.Empresa : null;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.DescripcionCorta = txtDescripcionCorta.Text;
            EditObject.Telefono = txtTelefono.Text;

            EditObject.Comentario1 = txtComentario1.Text;
            EditObject.Comentario2 = txtComentario2.Text;
            EditObject.Comentario3 = txtComentario3.Text;

            if (chkExistente.Checked)
            {
                EditObject.ReferenciaGeografica = SelectGeoRef1.Selected > 0 ? DAOFactory.ReferenciaGeograficaDAO.FindById(SelectGeoRef1.Selected) : null;
            }
            else
            {
                if (!EditMode || !EditObject.Nomenclado)
                {
                    EditObject.ReferenciaGeografica = EditEntityGeoRef1.GetNewGeoRefference();
                }
                EditObject.ReferenciaGeografica.Empresa = EditObject.Empresa;
                EditObject.ReferenciaGeografica.Linea = EditObject.Linea;
                EditObject.ReferenciaGeografica.Descripcion = EditObject.Descripcion;

                var code = EditObject.Codigo;
                var i = 1;


                var byCode = DAOFactory.ReferenciaGeograficaDAO.FindByCodigoStartWith(cbEmpresa.SelectedValues,
                                                                                      cbLinea.SelectedValues,
                                                                                      new[]
                                                                                          {
                                                                                              EditEntityGeoRef1.
                                                                                                  TipoReferenciaGeograficaId
                                                                                          }, code);

                while (byCode.Any(r => r.Codigo == code && EditObject.ReferenciaGeografica.Id != r.Id))
                {
                    code += "(" + i++ + ")";
                }

                EditObject.ReferenciaGeografica.Codigo = code;
                EditObject.ReferenciaGeografica.TipoReferenciaGeografica = DAOFactory.TipoReferenciaGeograficaDAO.FindById(EditEntityGeoRef1.TipoReferenciaGeograficaId);
                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(EditObject.ReferenciaGeografica);
                STrace.Trace("QtreeReset", "ClienteAlta");
            }
            EditObject.Nomenclado = true;

            DAOFactory.ClienteDAO.SaveOrUpdate(EditObject);

            if (EditMode) return;

            var puntoEntrega = new PuntoEntrega
                                   {
                                       Cliente = EditObject,
                                       Baja = false,
                                       Codigo = EditObject.Codigo,
                                       Descripcion = EditObject.Descripcion,
                                       ReferenciaGeografica = EditObject.ReferenciaGeografica,
                                       Telefono = EditObject.Telefono
                                   };

            DAOFactory.PuntoEntregaDAO.SaveOrUpdate(puntoEntrega);
        }

        protected override void OnDelete() { DAOFactory.ClienteDAO.Delete(EditObject); }

        protected override void ValidateSave()
        {
            ValidateEmpty((string) txtCodigo.Text, (string) "CODE");
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");

            var georef = EditEntityGeoRef1.GetNewGeoRefference();
            if (georef == null) ThrowMustEnter("DIRECCION");
        }

        #endregion
    }
}
