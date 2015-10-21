using System;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web.BaseClasses.BasePages;

namespace Logictracker.Parametrizacion
{
    public partial class SubentidadAlta : SecuredAbmPage<SubEntidad>
    {
        protected override string RedirectUrl { get { return "SubentidadLista.aspx"; } }
        protected override string VariableName { get { return "PAR_SUBENTIDAD"; } }
        protected override string GetRefference() { return "PAR_SUBENTIDAD"; }

        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }
        
        private const int ImageWidth = 500;

        private Size ImageSize
        {
            get 
            { 
                var img = Server.MapPath(Config.Directory.AttachDir + EditObject.Entidad.Url);
                using (var bitmap = System.Drawing.Image.FromFile(img))
                {
                    return bitmap.Size;
                }
            }
        }
        public short OffsetX
        {
            get { return (ViewState["OffsetXPlano"] != null ? (short)ViewState["OffsetXPlano"] : (short)0); }
            set { ViewState["OffsetXPlano"] = value; }
        }
        public short OffsetY
        {
            get { return (ViewState["OffsetYPlano"] != null ? (short)ViewState["OffsetYPlano"] : (short)0); }
            set { ViewState["OffsetYPlano"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack && EditObject != null && EditObject.Sensor != null)
            {
                imgAnchor.ImageUrl = Config.Directory.AttachDir + EditObject.Entidad.Url;
                imgAnchor.Width = Unit.Pixel(ImageWidth);
                cbSensor.Items.Insert(0, new ListItem(EditObject.Sensor.Descripcion, EditObject.Sensor.Id.ToString()));
                cbSensor.SetSelectedValue(EditObject.Sensor.Id);
            }
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbTipoEntidad.SetSelectedValue(EditObject.Entidad != null ? EditObject.Entidad.TipoEntidad.Id : cbTipoEntidad.AllValue);
            cbEntidad.SetSelectedValue(EditObject.Entidad != null ? EditObject.Entidad.Id : cbEntidad.NullValue);
            cbTipoMedicion.SetSelectedValue(EditObject.Sensor != null ? EditObject.Sensor.TipoMedicion.Id : cbTipoMedicion.AllValue);
            cbSensor.SetSelectedValue(EditObject.Sensor != null ? EditObject.Sensor.Id : cbSensor.NullValue);

            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;

            var controlaLimites = EditObject.Sensor != null && EditObject.Sensor.TipoMedicion != null && EditObject.Sensor.TipoMedicion.ControlaLimites;

            chkControlaMaximo.Visible = chkControlaMinimo.Visible = lblControlaMaximo.Visible = lblControlaMinimo.Visible = controlaLimites;

            chkControlaMaximo.Checked = EditObject.ControlaMaximo && controlaLimites;
            chkControlaMinimo.Checked = EditObject.ControlaMinimo && controlaLimites;

            lblMaximo.Visible = txtMaximo.Visible = chkControlaMaximo.Checked;
            lblMinimo.Visible = txtMinimo.Visible = chkControlaMinimo.Checked;

            txtMaximo.Text = EditObject.Maximo.ToString(CultureInfo.InvariantCulture);
            txtMinimo.Text = EditObject.Minimo.ToString(CultureInfo.InvariantCulture);

            var ratio = ImageSize.Width * 1.0 / ImageWidth;
            var x = (short)(EditObject.X / ratio);
            var y = (short)(EditObject.Y / ratio);
            MoveAnchor(x, y);
        }

        protected override void OnDelete()
        {
            DAOFactory.SubEntidadDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Entidad = cbEntidad.Selected > 0 ? DAOFactory.EntidadDAO.FindById(cbEntidad.Selected) : null;
            EditObject.Sensor = cbSensor.Selected > 0 ? DAOFactory.SensorDAO.FindById(cbSensor.Selected) : null;
            
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;

            EditObject.ControlaMaximo = chkControlaMaximo.Checked;
            EditObject.ControlaMinimo = chkControlaMinimo.Checked;
            if (EditObject.ControlaMaximo)
                EditObject.Maximo = double.Parse(txtMaximo.Text, CultureInfo.InvariantCulture);
            if (EditObject.ControlaMinimo)
                EditObject.Minimo = double.Parse(txtMinimo.Text, CultureInfo.InvariantCulture);

            var ratio = ImageSize.Width * 1.0 / ImageWidth;
            EditObject.X = OffsetX * ratio;
            EditObject.Y = OffsetY * ratio;
            
            DAOFactory.SubEntidadDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            var code = ValidateEmpty((string) txtCodigo.Text, (string) "CODE");
            ValidateEmpty((string) txtDescripcion.Text, (string) "DESCRIPCION");
            
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbEntidad.Selected, "PARENTI79");

            if (!DAOFactory.SubEntidadDAO.IsCodeUnique(cbEmpresa.Selected, cbEmpresa.Selected, EditObject.Id, code))
                ThrowDuplicated("CODE");

            if (chkControlaMaximo.Checked)
                ValidateDouble(txtMaximo.Text, "MAXIMO");
            if (chkControlaMinimo.Checked)
                ValidateDouble(txtMinimo.Text, "MINIMO");
        }

        protected void ImgAnchor_Click(object sender, ImageClickEventArgs e)
        {
            MoveAnchor((short)e.X, (short)e.Y);
        }

        protected void CbEntidadOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var entidad = DAOFactory.EntidadDAO.FindById(cbEntidad.Selected);
            if (entidad != null && entidad.Id != 0)
            {
                imgAnchor.ImageUrl = Config.Directory.AttachDir + entidad.Url;
                imgAnchor.Width = Unit.Pixel(ImageWidth);
            }
        }

        private void MoveAnchor(short x, short y)
        {
            OffsetX =  x;
            OffsetY =  y;
            imgAnchorPointer.Visible = true;
            imgAnchorPointer.Style.Add(HtmlTextWriterStyle.Left, x + "px");
            imgAnchorPointer.Style.Add(HtmlTextWriterStyle.Top, y + "px");
            imgAnchorPointer.Style.Add("position", "absolute");
        }

        protected void CbSensor_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var sensor = cbSensor.Selected > 0 ? DAOFactory.SensorDAO.FindById(cbSensor.Selected) : null;

            if (sensor != null && sensor.TipoMedicion != null)
            {
                if (sensor.TipoMedicion.ControlaLimites)
                {
                    chkControlaMaximo.Visible = chkControlaMinimo.Visible = lblControlaMaximo.Visible = lblControlaMinimo.Visible = true;
                    txtMaximo.Visible = lblMaximo.Visible = chkControlaMaximo.Checked;
                    txtMinimo.Visible = lblMinimo.Visible = chkControlaMinimo.Checked;
                }
                else
                {
                    chkControlaMaximo.Visible = chkControlaMinimo.Visible = txtMaximo.Visible = txtMinimo.Visible = false;
                    lblControlaMaximo.Visible = lblControlaMinimo.Visible = lblMaximo.Visible = lblMinimo.Visible = false;
                    chkControlaMaximo.Checked = chkControlaMinimo.Checked = false;
                }
            }
            else
            {
                chkControlaMaximo.Visible = chkControlaMinimo.Visible = txtMaximo.Visible = txtMinimo.Visible = false;
                lblControlaMaximo.Visible = lblControlaMinimo.Visible = lblMaximo.Visible = lblMinimo.Visible = false;
                chkControlaMaximo.Checked = chkControlaMinimo.Checked = false;
            }
        }

        protected void ChkControlaMaximo_OnCheckedChanged(object sender, EventArgs e)
        {
            txtMaximo.Visible = lblMaximo.Visible = chkControlaMaximo.Checked;
        }

        protected void ChkControlaMinimo_OnCheckedChanged(object sender, EventArgs e)
        {
            txtMinimo.Visible = lblMinimo.Visible = chkControlaMinimo.Checked;
        }
    }
}
