using System;
using System.Linq;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Parametrizacion
{
    public partial class PlantaAlta : SecuredAbmPage<Linea>
    {
        #region Protected Properties

        protected override string VariableName { get { return "PAR_BASE"; } }
        protected override string RedirectUrl { get { return "PlantaLista.aspx"; } }
        protected override string GetRefference() { return "PLANTA"; }

        #endregion

        #region Private Properties

        private string TimeZoneId
        {
            get { return ddlTimeZone.SelectedValue; }
            set
            {
                var li = ddlTimeZone.Items.FindByValue(value);
                if (li != null) li.Selected = true;
            }
        }

        #endregion

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            chkExistente.Attributes.Add("onclick", string.Format(@"var panSel = $get('{0}'); var panNew = $get('{1}');
            if(panSel.style.display == 'none') {{ panSel.style.display = '';panNew.style.display = 'none';}}
            else {{ panNew.style.display = '';panSel.style.display = 'none';}}", panSelectGeoRef.ClientID, panNewGeoRef.ClientID));
        }

        protected override void OnSave()
        {
            var identificaChoferesChanged = EditObject.IdentificaChoferes != chkIdentificaChoferes.Checked;

            EditObject.DescripcionCorta = txtCodigo.Text.Trim();
            EditObject.Descripcion = txtDescripcion.Text.Trim();
            EditObject.Telefono = txtTelefono.Text;
            EditObject.Mail = txtMail.Text;
            EditObject.TimeZoneId = TimeZoneId;
            EditObject.Interfaceable = chkInterface.Checked;
            EditObject.IdentificaChoferes = chkIdentificaChoferes.Checked;
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbLocacion.Selected);

            var user = DAOFactory.UsuarioDAO.FindById(Usuario.Id);

            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
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
                        EditObject.ReferenciaGeografica.Empresa = EditObject.Empresa;
                        EditObject.ReferenciaGeografica.Linea = EditObject;

                        var code = EditObject.DescripcionCorta;
                        var i = 1;

                        var byCode = DAOFactory.ReferenciaGeograficaDAO.FindByCodigoStartWith(cbLocacion.SelectedValues, new[] {EditObject.Id},
                            new[] {EditEntityGeoRef1.TipoReferenciaGeograficaId}, code);

                        while (byCode.Any(r => r.Codigo == code && EditObject.ReferenciaGeografica.Id != r.Id))
                        {
                            code += "(" + i++ + ")";
                        }

                        EditObject.ReferenciaGeografica.Codigo = code;

                        DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(EditObject.ReferenciaGeografica);
                    }

                    if (!EditMode &&
                        user.PorLinea)
                    {
                        user.AddLinea(DAOFactory.LineaDAO.FindById(EditObject.Id));

                        DAOFactory.UsuarioDAO.SaveOrUpdate(user);
                    }

                    DAOFactory.LineaDAO.SaveOrUpdate(EditObject);

                    transaction.Commit();

                    if (EditMode && identificaChoferesChanged) UpdateCochesIdentificaChofer(EditObject.IdentificaChoferes);
                }
                catch (Exception ex)
                {
                    STrace.Exception(GetType().FullName, ex, "OnSave();");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        STrace.Exception(GetType().FullName, ex2, "OnSave(); doing Rollback()");
                    }
                    throw ex;
                }
            }
        }

        private void UpdateCochesIdentificaChofer(bool identificaChoferes)
        {
            var coches = DAOFactory.CocheDAO.GetList(new[] { -1 }, new[] { EditObject.Id }, new[] { -1 }, new[] { -1 }, new[] { -1 })
              .Where(c => c.Transportista == null && c.Linea != null && c.IdentificaChoferes != identificaChoferes);

            foreach (var c in coches)
            {
                c.IdentificaChoferes = identificaChoferes;
                DAOFactory.CocheDAO.SaveOrUpdate(c);
            }
        }

        protected override void OnDelete() { DAOFactory.LineaDAO.Delete(EditObject); }

        protected override void Bind()
        {
            cbLocacion.SetSelectedValue(EditObject.Empresa.Id);
            txtCodigo.Text = EditObject.DescripcionCorta;
            txtDescripcion.Text = EditObject.Descripcion;
            txtTelefono.Text = EditObject.Telefono;
            txtMail.Text = EditObject.Mail;
            TimeZoneId = EditObject.TimeZoneId;
            chkInterface.Checked = EditObject.Interfaceable;
            SelectGeoRef1.IdLinea = EditObject.Id;
            EditEntityGeoRef1.SetLinea(EditObject.Id);
            SelectGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
            EditEntityGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
            chkIdentificaChoferes.Checked = EditObject.IdentificaChoferes;
        }

        protected override void ValidateSave()
        {
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            if (code.Length > 8) ThrowError("CODE_LONG");

            var name = ValidateEmpty((string) txtCodigo.Text, (string) "NAME");

            ValidateEntity(cbLocacion.Selected, "PARENTI01");

            var linea = DAOFactory.LineaDAO.FindByCodigo(cbLocacion.Selected, code);
            ValidateDuplicated(linea, "CODE");

            linea = DAOFactory.LineaDAO.FindByNombre(cbLocacion.Selected, name);
            ValidateDuplicated(linea, "NAME");

            ValidateGeoRefference();
        }

        protected void cbLocacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(EditMode) return;
            var empresa = cbLocacion.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbLocacion.Selected) : null;
            chkIdentificaChoferes.Checked = empresa != null ? empresa.IdentificaChoferes : false;
        }

        #endregion

        #region Private Methods

        private void ValidateGeoRefference()
        {
            ReferenciaGeografica referenciaGeografica;

            if (chkExistente.Checked) referenciaGeografica = SelectGeoRef1.Selected > 0 ? DAOFactory.ReferenciaGeograficaDAO.FindById(SelectGeoRef1.Selected) : null;
            else referenciaGeografica = EditEntityGeoRef1.GetNewGeoRefference();

            if (referenciaGeografica == null) throw new Exception(CultureManager.GetError("MUST_ENTER_GEO_REF"));
            if (referenciaGeografica.Direccion == null && referenciaGeografica.Poligono == null) throw new Exception(CultureManager.GetError("MUST_ENTER_DIR_POL"));
        }

        #endregion
    }
}