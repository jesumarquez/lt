using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Geometries;
using Logictracker.Web.BaseClasses.BasePages;
using NHibernate.Util;
using Point = Logictracker.Web.Monitor.Geometries.Point;

namespace Logictracker.Parametrizacion
{
    public partial class ZonaAlta : SecuredAbmPage<Zona>
    {
        protected override string RedirectUrl { get { return "ZonaLista.aspx"; } }
        protected override string VariableName { get { return "PAR_ZONAS"; } }
        protected override string GetRefference() { return "PAR_ZONAS"; }
        protected const int MinZoomLevel = 7;
        protected const string LayerAsignados = "Asignados";
        protected const string LayerNoAsignados = "No Asignados";
        
        protected override bool AddButton { get { return true; } }
        protected override bool DuplicateButton { get { return true; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (IsPostBack) return;

            var googleMapsEnabled = true;
            var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
            if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
            {
                var empresa = usuario.Empresas.First() as Empresa;
                if (empresa != null)
                    googleMapsEnabled = empresa.GoogleMapsEnabled;
            }

            Monitor.Initialize(googleMapsEnabled);
            Monitor.AddLayers(LayerFactory.GetVector(LayerAsignados, true, StyleFactory.GetHandlePoint()));
            Monitor.AddLayers(LayerFactory.GetVector(LayerNoAsignados, true, StyleFactory.GetHandlePoint()));
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa != null ? EditObject.Empresa.Id : cbEmpresa.AllValue);
            cbLinea.SetSelectedValue(EditObject.Linea != null ? EditObject.Linea.Id : cbLinea.AllValue);
            cbTipoZona.SetSelectedValue(EditObject.TipoZona != null ? EditObject.TipoZona.Id : cbTipoZona.AllValue);
            txtDescripcion.Text = EditObject.Descripcion;
            txtCodigo.Text = EditObject.Codigo;
            txtPrioridad.Text = EditObject.Prioridad.ToString("#0");

            lstAsignadas.Items.Clear();
            Monitor.ClearLayer(LayerAsignados);
            Monitor.ClearLayer(LayerNoAsignados);

            if (EditObject.Referencias.Count > 0)
            {
                var referencias = EditObject.Referencias.Cast<ReferenciaGeografica>().OrderBy(r => r.Descripcion);
                foreach (var referencia in referencias)
                {
                    lstAsignadas.Items.Add(new ListItem(referencia.Descripcion, referencia.Id.ToString("#0")));
                    if (referencia.Poligono.Radio > 0)
                        AddCircle(referencia.Id, referencia.Poligono.Centro, referencia.Poligono.Radio, LayerAsignados, Color.Blue);
                    else
                        AddPolygon(referencia.Id, referencia.Poligono.ToPointFList(), LayerAsignados, Color.Blue);
                }
            }
        }

        protected override void OnDelete()
        {
            DAOFactory.ZonaDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Linea = cbLinea.Selected > 0 ? DAOFactory.LineaDAO.FindById(cbLinea.Selected) : null;
            EditObject.TipoZona = DAOFactory.TipoZonaDAO.FindById(cbTipoZona.Selected);
            EditObject.Descripcion = txtDescripcion.Text;
            EditObject.Codigo = txtCodigo.Text;
            EditObject.Prioridad = Convert.ToInt32(txtPrioridad.Text);

            EditObject.Referencias.Clear();
            foreach (ListItem item in lstAsignadas.Items)
            {
                var referencia = DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32(item.Value));
                EditObject.Referencias.Add(referencia);
            }

            DAOFactory.ZonaDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {   
            ValidateEmpty(txtDescripcion.Text, "DESCRIPCION");
            var code = ValidateEmpty(txtCodigo.Text, "CODE");
            var prioridad = ValidateInt32(txtPrioridad.Text, "PRIORIDAD");
            ValidateEntity(cbEmpresa.Selected, "PARENTI01");
            ValidateEntity(cbTipoZona.Selected, "PARENTI93");

            var byCode = DAOFactory.ZonaDAO.FindByCodigo(cbEmpresa.Selected, cbLinea.Selected, code);
            ValidateDuplicated(byCode, "CODE");

            var byPrioridad = DAOFactory.ZonaDAO.FindByPrioridad(cbEmpresa.Selected, cbLinea.Selected, prioridad);
            ValidateDuplicated(byPrioridad, "PRIORIDAD");
        }

        protected void BtnBuscarOnClick(object sender, EventArgs e)
        {
            var referencias = DAOFactory.ReferenciaGeograficaDAO.GetList(new[] {cbEmpresa.Selected},
                                                                         new[] {cbLinea.Selected},
                                                                         new[] {cbTipoReferencia.Selected})
                                                                .Where(r => r.Poligono != null).ToList();
            if (!txtGeoRef.Text.Trim().Equals(string.Empty))
                referencias = referencias.Where(r => r.Descripcion.ToLowerInvariant().Contains(txtGeoRef.Text.Trim().ToLowerInvariant())).ToList();

            referencias = DAOFactory.ZonaDAO.FilterAsignadas(cbEmpresa.Selected, cbLinea.Selected, referencias);

            lstNoAsignadas.Items.Clear();

            if (referencias.Count > 0)
            {
                var items = referencias.Select(r => new ListItem(r.Descripcion, r.Id.ToString("#0"))).OrderBy(l => l.Text).ToArray();
                lstNoAsignadas.Items.AddRange(items);
                foreach (ListItem item in lstAsignadas.Items)
                {
                    lstNoAsignadas.Items.Remove(item);
                }
            }
        }

        protected void BtnAgregarOnClick(object sender, EventArgs e)
        {
            Monitor.ClearLayer(LayerNoAsignados);
            var indices = lstNoAsignadas.GetSelectedIndices().OrderByDescending(i => i);
            foreach (var indice in indices)
            {
                var item = lstNoAsignadas.Items[indice];
                lstAsignadas.Items.Add(item);
                lstNoAsignadas.Items.RemoveAt(indice);
                var referencia = DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32(item.Value));
                if (referencia.Poligono.Radio > 0)
                    AddCircle(referencia.Id, referencia.Poligono.Centro, referencia.Poligono.Radio, LayerAsignados, Color.Blue);
                else
                    AddPolygon(referencia.Id, referencia.Poligono.ToPointFList(), LayerAsignados, Color.Blue);
            }

            lstAsignadas.SelectedIndex = -1;
        }

        protected void BtnEliminarOnClick(object sender, EventArgs e)
        {
            var indices = lstAsignadas.GetSelectedIndices().OrderByDescending(i => i);
            foreach (var indice in indices)
            {
                var item = lstAsignadas.Items[indice];
                lstNoAsignadas.Items.Add(item);
                lstAsignadas.Items.RemoveAt(indice);
                Monitor.RemoveGeometries(LayerAsignados, item.Value);
            }

            lstNoAsignadas.SelectedIndex = -1;
        }

        protected void LstNoAsignadasOnSelectedIndexChanged(object sender, EventArgs e)
        {
            Monitor.ClearLayer(LayerNoAsignados);
            var indices = lstNoAsignadas.GetSelectedIndices();
            
            foreach (var indice in indices)
            {
                var item = lstNoAsignadas.Items[indice];
                var referencia = DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32(item.Value));
                
                if (referencia.Poligono.Radio > 0)
                    AddCircle(referencia.Id, referencia.Poligono.Centro, referencia.Poligono.Radio, LayerNoAsignados, Color.Red);
                else
                    AddPolygon(referencia.Id, referencia.Poligono.ToPointFList(), LayerNoAsignados, Color.Red);
            }
        }

        protected void LstAsignadasOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var indices = lstAsignadas.GetSelectedIndices();

            foreach (var indice in indices)
            {
                var item = lstAsignadas.Items[indice];
                var referencia = DAOFactory.ReferenciaGeograficaDAO.FindById(Convert.ToInt32(item.Value));

                if (referencia.Poligono.Radio > 0)
                    Monitor.SetCenter(referencia.Poligono.Centro.Y, referencia.Poligono.Centro.X);
                else
                    Monitor.SetCenter(referencia.Poligono.ToPointFList().Last().Y, referencia.Poligono.ToPointFList().Last().X);
            }
        }

        protected void AddPolygon(int id, List<PointF> points, string layer, Color color)
        {
            if (points == null || points.Count == 0) return;

            var poly = new Polygon(id.ToString("#0"), StyleFactory.GetPointFromColor(color));

            for (var i = 0; i < points.Count; i++) poly.AddPoint(new Point(i.ToString("#0"), points[i].X, points[i].Y));

            Monitor.AddGeometries(layer, poly);
            Monitor.SetCenter(points.Last().Y, points.Last().X);
        }
        protected void AddCircle(int id, PointF point, int radio, string layer, Color color)
        {
            if (radio <= 0) return;

            var poly = new Point(id.ToString("#0"), point.X, point.Y, radio, StyleFactory.GetPointFromColor(color));

            Monitor.AddGeometries(layer, poly);
            Monitor.SetCenter(point.Y, point.X);
        }
    }
}
