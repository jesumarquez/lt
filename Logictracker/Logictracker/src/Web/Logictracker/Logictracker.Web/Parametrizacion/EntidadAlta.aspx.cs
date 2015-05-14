using System;
using System.IO;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.DAL.NHibernate;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class EntidadAlta : SecuredAbmPage<EntidadPadre>
    {
        protected override string RedirectUrl { get { return "EntidadLista.aspx"; } }
        protected override string VariableName { get { return "PAR_ENTIDAD"; } }
        protected override string GetRefference() { return "PAR_ENTIDAD"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbDispositivo.Enabled = WebSecurity.IsSecuredAllowed(Securables.AssingDevices);
        } 

        protected void Page_Load(object sender, EventArgs e)
        {
            ctrlDetalles.IdTipoEntidad = cbTipoEntidad.Selected;
            ctrlDetalles.LoadDetalles(cbTipoEntidad.Selected);

            ToolBar.AsyncPostBacks = false;

            chkExistente.Attributes.Add("onclick",
                                        string.Format(@"
                                        var panSel = $get('{0}'); 
                                        var panNew = $get('{1}');
                                        if(panSel.style.display == 'none') 
                                        {{ 
                                            panSel.style.display = '';
                                            panNew.style.display = 'none';
                                        }}
                                        else 
                                        {{ 
                                            panNew.style.display = '';
                                            panSel.style.display = 'none';
                                        }}", 
                                        panSelectGeoRef.ClientID, 
                                        panNewGeoRef.ClientID));
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            
            if (EditObject.Dispositivo != null)
                cbDispositivo.Items.Insert(0, new ListItem(EditObject.Dispositivo.Codigo, EditObject.Dispositivo.Id.ToString()));
            
            cbDispositivo.SetSelectedValue(EditObject.Dispositivo != null ? EditObject.Dispositivo.Id : cbDispositivo.NullValue);
            cbTipoEntidad.SetSelectedValue(EditObject.TipoEntidad != null ? EditObject.TipoEntidad.Id : cbTipoEntidad.NullValue);
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;

            SelectGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
            EditEntityGeoRef1.SetReferencia(EditObject.ReferenciaGeografica);
            
            if (EditObject.Url != null && EditObject.Url.Trim() != "")
            {
                btnImagen.Enabled = true;
                var split = EditObject.Url.Split('\\');
                var filename = split[split.Length - 1];
                
                btnImagen.OnClientClick = "javascript:window.open('Attach/" + filename + "','imagen');";
            }

            ctrlDetalles.BindDetalles(cbTipoEntidad.Selected, EditObject);
        }

        protected override void OnDelete()
        {
            DAOFactory.EntidadDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            using (var transaction = SmartTransaction.BeginTransaction())
            {

                try
                {
                    var actualizarSub = EditObject.Empresa != null && EditObject.Empresa.Id != cbEmpresa.Selected;

                    EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
                    EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
                    EditObject.Dispositivo = cbDispositivo.Selected > 0 ? DAOFactory.DispositivoDAO.FindById(cbDispositivo.Selected) : null;
                    EditObject.TipoEntidad = cbTipoEntidad.Selected > 0 ? DAOFactory.TipoEntidadDAO.FindById(cbTipoEntidad.Selected) : null;
                    EditObject.Descripcion = txtDescripcion.Text;
                    EditObject.Codigo = txtCodigo.Text;
                    if (chkExistente.Checked)
                    {
                        EditObject.ReferenciaGeografica = SelectGeoRef1.Selected > 0
                            ? DAOFactory.ReferenciaGeograficaDAO.FindById(SelectGeoRef1.Selected)
                            : null;
                    }
                    else
                    {
                        var geoRef = EditObject.ReferenciaGeografica ?? EditEntityGeoRef1.GetNewGeoRefference();

                        geoRef.Empresa = EditObject.Empresa;
                        geoRef.Linea = EditObject.Linea;
                        geoRef.TipoReferenciaGeografica = DAOFactory.TipoReferenciaGeograficaDAO.FindById(EditEntityGeoRef1.TipoReferenciaGeograficaId);
                        geoRef.Icono = DAOFactory.IconoDAO.FindById(EditEntityGeoRef1.IconId);
                        geoRef.Descripcion = EditObject.Descripcion;
                        geoRef.Codigo = EditObject.Codigo;
                        DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(geoRef);
                        EditObject.ReferenciaGeografica = geoRef;
                    }

                    if (flImagen.HasFile)
                    {
                        if (!Directory.Exists(Config.Directory.AttachDir))
                            Directory.CreateDirectory(Config.Directory.AttachDir);

                        var filename = GetFileName(flImagen.FileName);
                        var path = Server.MapPath(Config.Directory.AttachDir) + filename;

                        flImagen.SaveAs(path);
                        EditObject.Url = filename;
                    }

                    foreach (var valor in ctrlDetalles.GetValores())
                    {
                        var detalle = DAOFactory.DetalleDAO.FindByIdAndTipoEntidad(valor.Key, cbTipoEntidad.Selected);
                        if (detalle != null) // EXISTE ESTE DETALLE PARA ESTE TIPO DE ENTIDAD ?
                        {
                            var detalleValor = EditObject.GetDetalle(detalle.Id);

                            if (detalleValor != null) // EXISTE UN DETALLE VALOR PARA LA ENTIDAD ?
                            {
                                detalleValor.ValorDt = valor.Value.ValorDt;
                                detalleValor.ValorNum = valor.Value.ValorNum;
                                detalleValor.ValorStr = valor.Value.ValorStr;
                            }
                            else
                            {
                                detalleValor = valor.Value;
                                detalleValor.Entidad = EditObject;

                                EditObject.Detalles.Add(detalleValor);
                            }

                            if (detalle.Obligatorio)
                            {
                                switch (detalle.Tipo)
                                {
                                    case 1:
                                        if (detalle.Representacion == 2)
                                        {
                                            if (detalleValor.ValorNum == 0)
                                                ThrowError("MUST_ENTER_VALUE", detalle.Nombre);
                                        }
                                        else if (detalleValor.ValorStr.Trim().Equals(string.Empty))
                                            ThrowError("MUST_ENTER_VALUE", detalle.Nombre);
                                        break;
                                    case 2:
                                        if (detalleValor.ValorNum == 0)
                                            ThrowError("MUST_ENTER_VALUE", detalle.Nombre);
                                        break;
                                    case 3:
                                        if (detalleValor.ValorDt == null ||
                                            detalleValor.ValorDt == DateTime.MinValue)
                                            ThrowError("MUST_ENTER_VALUE", detalle.Nombre);
                                        break;
                                }
                            }
                        }
                    }

                    if (actualizarSub) DAOFactory.EntidadDAO.SaveAndUpdateSubEntidades(EditObject);
                    else DAOFactory.EntidadDAO.SaveOrUpdate(EditObject);

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
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");

            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbTipoEntidad.Selected, "PARENTI76");
            ValidateEntity(cbDispositivo.Selected, "PARENTI08");

            if (!DAOFactory.EntidadDAO.IsCodeUnique(cbEmpresa.Selected, cbLinea.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");

            if (chkExistente.Checked)
                ValidateEntity(SelectGeoRef1.Selected, "PARENTI05");
            else
            {
                var georef = EditEntityGeoRef1.GetNewGeoRefference();
                if (georef == null)
                    ThrowMustEnter("DIRECCION");
            }
        }

        private static string GetFileName(string filename)
        {
            var name = Path.GetFileNameWithoutExtension(filename);
            var ext = Path.GetExtension(filename);
            var newFilename = filename;
            var count = 2;

            while (File.Exists(Path.Combine(Config.Directory.AttachDir, newFilename)))
            {
                newFilename = string.Concat(name, "(", count, ")", ext);

                count++;
            }

            return newFilename;
        }
    }
}
