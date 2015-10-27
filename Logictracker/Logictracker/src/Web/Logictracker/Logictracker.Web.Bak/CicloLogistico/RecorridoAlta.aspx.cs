using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.HtmlControls;
using Logictracker.Types.BusinessObjects.CicloLogistico;
using Logictracker.Utils;
using Logictracker.Web.BaseClasses.BasePages;
using System.Drawing;

namespace Logictracker.CicloLogistico
{
    public partial class CicloLogistico_RecorridoAlta : SecuredAbmPage<Recorrido>
    {
        protected override string VariableName { get { return "CLOG_RECORRIDO"; } }
        protected override string RedirectUrl { get { return "RecorridoLista.aspx"; } }
        protected override string GetRefference() { return "CLOG_RECORRIDO"; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if(!IsPostBack) if (!SetCenterLinea()) EditLine1.SetCenter(-34.6134981326759, -58.4255323559046);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Map Style
                var link = new HtmlGenericControl("link");
                link.Attributes.Add("rel", "stylesheet");
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("href", ResolveUrl("~/App_Styles/openlayers.css"));
                Page.Header.Controls.AddAt(0, link);
            }
        }
    
        protected bool SetCenterLinea()
        {
            if (cbLinea.Selected <= 0) return false;
            var l = DAOFactory.LineaDAO.FindById(cbLinea.Selected);
            if (l.ReferenciaGeografica == null || (l.ReferenciaGeografica.Direccion == null && l.ReferenciaGeografica.Poligono == null)) return false;

            var lat = l.ReferenciaGeografica.Direccion != null ? l.ReferenciaGeografica.Direccion.Latitud : l.ReferenciaGeografica.Poligono.Centro.Y;
            var lon = l.ReferenciaGeografica.Direccion != null ? l.ReferenciaGeografica.Direccion.Longitud : l.ReferenciaGeografica.Poligono.Centro.X;

            EditLine1.SetCenter(lat, lon);
            return true;
        }

        protected void cbLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            var points = EditLine1.Points.Get();
            if(points == null || points.Count == 0) SetCenterLinea();
        }
        protected void btInvertir_Click(object sender, EventArgs e)
        {
            EditLine1.Invertir();
        }
        protected void btLimpiar_Click(object sender, EventArgs e)
        {
            EditLine1.Clear();
        }
 
        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa!= null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            txtNombre.Text = EditObject.Nombre;
            txtDesvio.Text = EditObject.Desvio.ToString(CultureInfo.InvariantCulture);

            var points = EditObject.Detalles.Select(detalle => new PointF((float) detalle.Longitud, (float) detalle.Latitud)).ToList();
            EditLine1.SetLine(points);

            if (points.Count > 0) EditLine1.SetCenter(points[0].Y, points[0].X);
        }

        protected override void OnDelete()
        {
            DAOFactory.RecorridoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = cbEmpresa.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected) : null;
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.Codigo = txtCodigo.Text.Trim();
            EditObject.Nombre = txtNombre.Text.Trim();
            EditObject.Desvio = Convert.ToInt32(txtDesvio.Text.Trim());

            var points = EditLine1.Points.Get();
            EditObject.Detalles.Clear();
            DetalleRecorrido last = null;
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var det = new DetalleRecorrido {Latitud = point.Y, Longitud = point.X, Recorrido = EditObject, Orden = i};
                det.Distancia = last == null
                                    ? 0
                                    : Distancias.Loxodromica(last.Latitud, last.Longitud, det.Latitud, det.Longitud)/1000.0;
                EditObject.Detalles.Add(det);
                last = det;
            }


            DAOFactory.RecorridoDAO.SaveOrUpdate(EditObject);
        }
        protected override void ValidateSave()
        {
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            var code = ValidateEmpty(txtCodigo.Text, "CODE");
            ValidateEmpty(txtNombre.Text, "NAME");

            ValidateEmpty(txtDesvio.Text, "DESVIO");
            ValidateInt32(txtDesvio.Text, "DESVIO");

            var byCode = DAOFactory.RecorridoDAO.FindByCode(cbEmpresa.SelectedValues, cbLinea.SelectedValues, code);
            ValidateDuplicated(byCode, "CODE");


            var points = EditLine1.Points.Get();
            if(points ==  null || points.Count == 0)
            {
                ThrowMustEnter("RECORRIDO");
            }
        }


    
    }
}
