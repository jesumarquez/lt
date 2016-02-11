using System;
using System.Linq;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Web.Monitor;
using Logictracker.Culture;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Collections.Generic;
using Logictracker.Web.Monitor.Geometries;
using Point = Logictracker.Web.Monitor.Geometries.Point;
using Logictracker.Services.Helpers;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class ViajeProgramadoAlta : SecuredAbmPage<ViajeProgramado>
    {
        protected const int MinZoomLevel = 7;
        protected const string LayerAsignados = "Asignados";
        protected const string LayerNoAsignados = "No Asignados";
        protected const string LayerRecorrido = "Recorrido";
        protected override String RedirectUrl { get { return "ViajeProgramadoLista.aspx"; } }
        protected override String VariableName { get { return "PAR_VIAJE_PROGRAMADO"; } }
        protected override String GetRefference() { return "PAR_VIAJE_PROGRAMADO"; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (IsPostBack) return;

            monitor.Initialize(true);
            monitor.AddLayers(LayerFactory.GetVector(LayerAsignados, true, StyleFactory.GetHandlePoint()));
            monitor.AddLayers(LayerFactory.GetVector(LayerNoAsignados, true, StyleFactory.GetHandlePoint()));
            monitor.AddLayers(LayerFactory.GetVector(LayerRecorrido, true, StyleFactory.GetHandlePoint()));
        }

        protected override void Bind()
        {
            cbEmpresa.SetSelectedValue(EditObject.Empresa.Id);
            cbTransportista.SetSelectedValue(EditObject.Transportista != null ? EditObject.Transportista.Id : cbTransportista.AllValue);
            txtCodigo.Text = EditObject.Codigo;
            var ts = new TimeSpan(0, 0, (int)(EditObject.Horas * 3600));
            txtHoras.Text = ((int)Math.Truncate(ts.TotalHours)).ToString("00") + ":" + ((int)(ts.Minutes)).ToString("00") + ":" + ((int)(ts.Seconds)).ToString("00");
            txtKm.Text = EditObject.Km.ToString("#0.00");

            lstAsignadas.Items.Clear();
            monitor.ClearLayer(LayerAsignados);
            monitor.ClearLayer(LayerNoAsignados);
            monitor.ClearLayer(LayerRecorrido);

            if (EditObject.Detalles.Count > 0)
            {
                var puntos = EditObject.Detalles.OrderBy(d => d.Orden).Select(d => d.PuntoEntrega);
                foreach (var punto in puntos)
                {
                    lstAsignadas.Items.Add(new ListItem(punto.Descripcion, punto.Id.ToString("#0")));
                    if (punto.ReferenciaGeografica.Poligono.Radio > 0)
                        AddCircle(punto.Id, punto.ReferenciaGeografica.Poligono.Centro, punto.ReferenciaGeografica.Poligono.Radio, LayerAsignados, Color.Blue);
                    else
                        AddPolygon(punto.Id, punto.ReferenciaGeografica.Poligono.ToPointFList(), LayerAsignados, Color.Blue);
                }
            }

            ShowRecorrido(false);
        }

        protected override void OnDelete()
        {
            DAOFactory.ViajeProgramadoDAO.Delete(EditObject);
        }

        protected override void OnSave()
        {
            EditObject.Empresa = DAOFactory.EmpresaDAO.FindById(cbEmpresa.Selected);
            EditObject.Transportista = cbTransportista.Selected > 0 ? DAOFactory.TransportistaDAO.FindById(cbTransportista.Selected) : null;
            EditObject.Codigo = txtCodigo.Text;
            
            var km = 0.0;
            double.TryParse(txtKm.Text, out km);            
            var horas = 0;
            var minutos = 0;
            var segundos = 0;
            int.TryParse(txtHoras.Text.Split(':')[0], out horas);
            int.TryParse(txtHoras.Text.Split(':')[1], out minutos);
            int.TryParse(txtHoras.Text.Split(':')[2], out segundos);
            var ts = new TimeSpan(horas, minutos, segundos);
            
            EditObject.Horas = ts.TotalHours;
            EditObject.Km = km;

            EditObject.Detalles.Clear();
            for (var i = 0; i < lstAsignadas.Items.Count; i++)
            {
                var item = lstAsignadas.Items[i];
                var punto = DAOFactory.PuntoEntregaDAO.FindById(Convert.ToInt32(item.Value));
                var entrega = new EntregaProgramada();
                entrega.PuntoEntrega = punto;
                entrega.Orden = i;
                entrega.ViajeProgramado = EditObject;
                EditObject.Detalles.Add(entrega);
            }

            DAOFactory.ViajeProgramadoDAO.SaveOrUpdate(EditObject);
        }

        protected override void ValidateSave()
        {
            var descripcion = ValidateEmpty(txtCodigo.Text, "CODIGO");

            var byCodigo = DAOFactory.ViajeProgramadoDAO.FindByCodigo(cbEmpresa.Selected, txtCodigo.Text);
            ValidateDuplicated(byCodigo, "CODIGO");
        }

        protected void BtnBuscarOnClick(object sender, EventArgs e)
        {
            var clientes = new List<int>();
            if (cbCliente.Selected > 0) clientes = cbCliente.SelectedValues;
            else clientes = DAOFactory.ClienteDAO.GetList(new[] { cbEmpresa.Selected }, new[] { -1 }).Select(c => c.Id).ToList();
            
            var puntos = DAOFactory.PuntoEntregaDAO.GetList(new[] { cbEmpresa.Selected },
                                                            new[] { -1 },
                                                            clientes)
                                                   .ToList();
            if (!txtPunto.Text.Trim().Equals(string.Empty))
                puntos = puntos.Where(r => r.Descripcion.ToLowerInvariant().Contains(txtPunto.Text.Trim().ToLowerInvariant())).ToList();

            lstNoAsignadas.Items.Clear();

            if (puntos.Count > 0)
            {
                var items = puntos.Select(r => new ListItem(r.Descripcion, r.Id.ToString("#0"))).OrderBy(l => l.Text).ToArray();
                lstNoAsignadas.Items.AddRange(items);
                foreach (ListItem item in lstAsignadas.Items)
                {
                    lstNoAsignadas.Items.Remove(item);
                }
            }
        }

        protected void BtnAgregarOnClick(object sender, EventArgs e)
        {
            monitor.ClearLayer(LayerNoAsignados);
            var indices = lstNoAsignadas.GetSelectedIndices().OrderByDescending(i => i);
            foreach (var indice in indices)
            {
                var item = lstNoAsignadas.Items[indice];
                lstAsignadas.Items.Add(item);
                lstNoAsignadas.Items.RemoveAt(indice);
                var punto = DAOFactory.PuntoEntregaDAO.FindById(Convert.ToInt32(item.Value));
                if (punto.ReferenciaGeografica.Poligono.Radio > 0)
                    AddCircle(punto.Id, punto.ReferenciaGeografica.Poligono.Centro, punto.ReferenciaGeografica.Poligono.Radio, LayerAsignados, Color.Blue);
                else
                    AddPolygon(punto.Id, punto.ReferenciaGeografica.Poligono.ToPointFList(), LayerAsignados, Color.Blue);
            }

            lstAsignadas.SelectedIndex = -1;

            ShowRecorrido(true);
        }

        protected void BtnEliminarOnClick(object sender, EventArgs e)
        {
            var indices = lstAsignadas.GetSelectedIndices().OrderByDescending(i => i);
            foreach (var indice in indices)
            {
                var item = lstAsignadas.Items[indice];
                lstNoAsignadas.Items.Add(item);
                lstAsignadas.Items.RemoveAt(indice);
                monitor.RemoveGeometries(LayerAsignados, item.Value);
            }

            lstNoAsignadas.SelectedIndex = -1;

            ShowRecorrido(true);
        }

        protected void LstNoAsignadasOnSelectedIndexChanged(object sender, EventArgs e)
        {
            monitor.ClearLayer(LayerNoAsignados);
            var indices = lstNoAsignadas.GetSelectedIndices();

            foreach (var indice in indices)
            {
                var item = lstNoAsignadas.Items[indice];
                var punto = DAOFactory.PuntoEntregaDAO.FindById(Convert.ToInt32(item.Value));

                if (punto.ReferenciaGeografica.Poligono.Radio > 0)
                    AddCircle(punto.Id, punto.ReferenciaGeografica.Poligono.Centro, punto.ReferenciaGeografica.Poligono.Radio, LayerNoAsignados, Color.Red);
                else
                    AddPolygon(punto.Id, punto.ReferenciaGeografica.Poligono.ToPointFList(), LayerNoAsignados, Color.Red);
            }
        }

        protected void LstAsignadasOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var indices = lstAsignadas.GetSelectedIndices();

            foreach (var indice in indices)
            {
                var item = lstAsignadas.Items[indice];
                var punto = DAOFactory.PuntoEntregaDAO.FindById(Convert.ToInt32(item.Value));

                if (punto.ReferenciaGeografica.Poligono.Radio > 0)
                    monitor.SetCenter(punto.ReferenciaGeografica.Poligono.Centro.Y, punto.ReferenciaGeografica.Poligono.Centro.X);
                else
                    monitor.SetCenter(punto.ReferenciaGeografica.Poligono.ToPointFList().Last().Y, punto.ReferenciaGeografica.Poligono.ToPointFList().Last().X);
            }
        }

        protected void AddPolygon(int id, List<PointF> points, string layer, Color color)
        {
            if (points == null || points.Count == 0) return;

            var poly = new Polygon(id.ToString("#0"), StyleFactory.GetPointFromColor(color));

            for (var i = 0; i < points.Count; i++) poly.AddPoint(new Point(i.ToString("#0"), points[i].X, points[i].Y));

            monitor.AddGeometries(layer, poly);
            monitor.SetCenter(points.Last().Y, points.Last().X);
        }
        protected void AddCircle(int id, PointF point, int radio, string layer, Color color)
        {
            if (radio <= 0) return;

            var poly = new Point(id.ToString("#0"), point.X, point.Y, radio, StyleFactory.GetPointFromColor(color));

            monitor.AddGeometries(layer, poly);
            monitor.SetCenter(point.Y, point.X);
        }

        private void ShowRecorrido(bool recalcularValores)
        {
            monitor.ClearLayer(LayerRecorrido);
            var count = lstAsignadas.Items.Count;
            if (count > 1)
            {
                var primero = DAOFactory.PuntoEntregaDAO.FindById(Convert.ToInt32(lstAsignadas.Items[0].Value));
                var ultimo = DAOFactory.PuntoEntregaDAO.FindById(Convert.ToInt32(lstAsignadas.Items[count - 1].Value));
                var origen = new LatLon(primero.ReferenciaGeografica.Latitude, primero.ReferenciaGeografica.Longitude);
                var destino = new LatLon(ultimo.ReferenciaGeografica.Latitude, ultimo.ReferenciaGeografica.Longitude);
                var waypoints = new List<LatLon>();
                for (int i = 1; i < count - 1; i++)
                {
                    var punto = DAOFactory.PuntoEntregaDAO.FindById(Convert.ToInt32(lstAsignadas.Items[i].Value));
                    var waypoint = new LatLon(punto.ReferenciaGeografica.Latitude, punto.ReferenciaGeografica.Longitude);
                    waypoints.Add(waypoint);
                }

                var directions = GoogleDirections.GetDirections(origen, destino, GoogleDirections.Modes.Driving, string.Empty, waypoints.ToArray());
                var posiciones = directions.Legs.SelectMany(l => l.Steps.SelectMany(s => s.Points));
                var line = new Line("D:" + Color.Red.ToArgb(), StyleFactory.GetLineFromColor(Color.Red, 4, 0.5));
                line.AddPoints(posiciones.Select(p => new Point("", p.Longitud, p.Latitud)));
                monitor.AddGeometries(LayerRecorrido, line);

                if (recalcularValores)
                {
                    var ts = directions.Duration;
                    txtHoras.Text = ((int)Math.Truncate(ts.TotalHours)).ToString("00") + ":" + ((int)(ts.Minutes)).ToString("00") + ":" + ((int)(ts.Seconds)).ToString("00");
                    txtKm.Text = (directions.Distance / 1000.00).ToString("#0.00");
                }
            }
            else
            {
                if (recalcularValores)
                {
                    txtHoras.Text = "00:00:00";
                    txtKm.Text = "0.00";
                }
            }
        }
    }
}
