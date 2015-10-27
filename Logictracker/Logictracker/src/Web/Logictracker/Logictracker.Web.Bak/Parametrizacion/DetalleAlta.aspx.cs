using System;
using Logictracker.DAL.NHibernate;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Culture;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class DetalleAlta : SecuredAbmPage<Detalle>
    {
        protected override string RedirectUrl { get { return "DetalleLista.aspx"; } }
        protected override string VariableName { get { return "PAR_DETALLE"; } }
        protected override string GetRefference() { return "PAR_DETALLE"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }
        
        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.TipoEntidad != null && EditObject.TipoEntidad.Empresa != null
                                        ? EditObject.TipoEntidad.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.TipoEntidad != null && EditObject.TipoEntidad.Linea != null
                                        ? EditObject.TipoEntidad.Linea.Id : cbLinea.AllValue);
            
            cbTipoEntidad.SetSelectedValue(EditObject.TipoEntidad != null ? EditObject.TipoEntidad.Id : cbTipoEntidad.NullValue);
            cbDetalle.SetSelectedValue(EditObject.DetallePadre != null ? EditObject.DetallePadre.Id : cbDetalle.NoneValue);
            cbTipoDetalle.SetSelectedValue(EditObject.Tipo);
            cbRepresentacion.SetSelectedValue(EditObject.Representacion);

            txtNombre.Text = EditObject.Nombre;
            txtOrden.Text = EditObject.Orden.ToString();
            txtMascara.Text = EditObject.Mascara;
            txtOpciones.Text = EditObject.Opciones;
            chkFiltro.Checked = EditObject.EsFiltro;
            chkObligatorio.Checked = EditObject.Obligatorio;
        }

        protected override void OnDelete()
        {
            DAOFactory.DetalleDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.TipoEntidad = cbTipoEntidad.Selected > 0 ? DAOFactory.TipoEntidadDAO.FindById(cbTipoEntidad.Selected) : null;
            EditObject.DetallePadre = cbDetalle.Selected > 0 ? DAOFactory.DetalleDAO.FindById(cbDetalle.Selected) : null;
            EditObject.Tipo = cbTipoDetalle.Selected;
            EditObject.Representacion = cbRepresentacion.Selected;
            EditObject.Nombre = txtNombre.Text;
            EditObject.Orden = Convert.ToInt32((string) txtOrden.Text);
            EditObject.Mascara = txtMascara.Text;
            EditObject.Opciones = txtOpciones.Text;
            EditObject.EsFiltro = chkFiltro.Checked;
            EditObject.Obligatorio = chkObligatorio.Checked;
            
            DAOFactory.DetalleDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {   
            var nombre = ValidateEmpty((string) txtNombre.Text, (string) "NAME");
            ValidateInt32(cbTipoDetalle.SelectedValue, "TIPO_DATO");
            ValidateInt32(cbRepresentacion.SelectedValue, "REPRESENTACION");
            var orden = ValidateInt32(txtOrden.Text, "ORDEN");

            if (!DAOFactory.DetalleDAO.IsOrdenUnique(cbEmpresa.Selected, cbLinea.Selected, cbTipoEntidad.Selected, EditObject.Id, orden))
                ThrowDuplicated("ORDEN");

            if (!DAOFactory.DetalleDAO.IsNombreUnique(cbEmpresa.Selected, cbLinea.Selected, cbTipoEntidad.Selected, EditObject.Id, nombre))
                ThrowDuplicated("NAME");

            ValidateEntity(cbTipoEntidad.Selected, "PARENTI76");
            ValidarOpciones();
        }

        private void ValidarOpciones()
        {
            if (txtOpciones.Text.Trim() != "")
            {
                var opciones = txtOpciones.Text.Trim().Split('|');

                foreach (var opcion in opciones)
                {
                    var miembros = opcion.Split('.');

                    if (miembros.Length < 2 || miembros.Length > 3)
                        ThrowInvalidValue("OPCIONES");
                    else
                    {
                        switch (miembros.Length)
                        {
                            case 2:
                                int a;
                                if (cbDetalle.Selected > 0 || !int.TryParse(miembros[0], out a))
                                    ThrowInvalidValue("OPCIONES");
                                break;
                            case 3:
                                int b, c;
                                if (cbDetalle.Selected <= 0 || !int.TryParse(miembros[0], out b) || !int.TryParse(miembros[1], out c))
                                    ThrowInvalidValue("OPCIONES");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        protected void CbRepresentacion_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            cbTipoDetalle.Items.Clear();
            switch (cbRepresentacion.Selected)
            {
                case 1:
                    cbTipoDetalle.AddItem(CultureManager.GetLabel("TEXTO"), 1);
                    cbTipoDetalle.AddItem(CultureManager.GetLabel("NUMERICO"), 2);
                    cbTipoDetalle.AddItem(CultureManager.GetLabel("FECHA"), 3);
                    txtOpciones.Visible = lblOpciones.Visible = lblFormato.Visible = false;
                    break;
                case 2:
                    cbTipoDetalle.AddItem(CultureManager.GetLabel("TEXTO"), 1);
                    cbTipoDetalle.AddItem(CultureManager.GetLabel("NUMERICO"), 2);
                    txtOpciones.Visible = lblOpciones.Visible = lblFormato.Visible = true;
                    break;
                case 3:
                    cbTipoDetalle.AddItem(CultureManager.GetLabel("TEXTO"), 1);
                    txtOpciones.Visible = lblOpciones.Visible = lblFormato.Visible = true;
                    break;
            }
        }
    }
}
