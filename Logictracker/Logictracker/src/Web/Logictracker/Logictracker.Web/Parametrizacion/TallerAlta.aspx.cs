using System;
using System.Linq;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class ParametrizacionTallerAlta : SecuredAbmPage<Taller>
    {
        protected override string RedirectUrl { get { return "TallerLista.aspx"; } }
        protected override string VariableName { get { return "PAR_TALLERES"; } }
        protected override string GetRefference() { return "TALLER"; }

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
            txtTelefono.Text = EditObject.Telefono;
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;

            SelectGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
            EditEntityGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
        }

        protected override void OnDelete() { DAOFactory.TallerDAO.Delete(EditObject); }

        protected override void OnSave()
        {
            EditObject.Telefono = txtTelefono.Text.Trim();
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Codigo = txtCodigo.Text.Trim();
            
            if (chkExistente.Checked)
            {
                EditObject.ReferenciaGeografica = SelectGeoRef1.Selected > 0 ? DAOFactory.ReferenciaGeograficaDAO.FindById(SelectGeoRef1.Selected) : null;
            }
            else
            {
                EditObject.ReferenciaGeografica = EditEntityGeoRef1.GetNewGeoRefference();
                EditObject.ReferenciaGeografica.Empresa = EditObject.Empresa;
                EditObject.ReferenciaGeografica.Linea = EditObject.Linea;
                EditObject.ReferenciaGeografica.Descripcion = EditObject.Descripcion;

                var code = txtCodigo.Text.Trim();
                var i = 1;

                var byCode = DAOFactory.ReferenciaGeograficaDAO.FindByCodigoStartWith(new[] {-1},
                                                                                      new[] {-1},
                                                                                      new[] {EditEntityGeoRef1.TipoReferenciaGeograficaId},
                                                                                      code);

                while (byCode.Any(r => r.Codigo == code && EditObject.ReferenciaGeografica.Id != r.Id))
                {
                    code += "(" + i++ + ")";
                }
                
                EditObject.ReferenciaGeografica.Codigo = code;

                DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(EditObject.ReferenciaGeografica);
            }

            DAOFactory.TallerDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            if (string.IsNullOrEmpty(txtDescripcion.Text.Trim())) ThrowMustEnter("NAME");
            if (string.IsNullOrEmpty(txtCodigo.Text.Trim())) ThrowMustEnter("CODE");
           
            var georef = EditEntityGeoRef1.GetNewGeoRefference();

            if (georef == null) ThrowMustEnter("DIRECCION");
        }
    }
}
