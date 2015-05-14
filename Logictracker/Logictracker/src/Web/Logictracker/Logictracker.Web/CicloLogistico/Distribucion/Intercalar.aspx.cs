using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using C1.Web.UI.Controls.C1GridView;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Mailing;
using Logictracker.Process.CicloLogistico.Distribucion;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.BaseClasses.Util;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Helpers.ColorHelpers;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Geometries;
using AjaxControlToolkit;
using NHibernate.Util;
using Point = Logictracker.Web.Monitor.Geometries.Point;

namespace Logictracker.CicloLogistico.Distribucion
{
    public partial class Intercalar : OnLineSecuredPage
    {
        protected override string GetRefference() { return "CLOG_INTERCALADOR"; }
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        private const string LayerEntregas = "Entregas";
        private const string LayerRecorrido = "Recorrido";

        private VsProperty<List<Intercalado>> Intercalados
        {
            get { return this.CreateVsProperty<List<Intercalado>>("Intercalados", null); }
        }

        private bool[] Horas {get { return hidHourSelected.Value.Select(c => c == '1').ToArray(); }}

        private bool InformarPorMail { get { return hidMail.Value == "1"; } }

        #region Colores

        private ColorGenerator _colorGenerator;

        private ColorGenerator ColorGenerator
        {
            get { return _colorGenerator ?? (_colorGenerator = new ColorGenerator()); }
        }

        private VsProperty<Dictionary<int, Color>> ColorViaje
        {
            get { return this.CreateVsProperty("ColorViaje", new Dictionary<int, Color>()); }
        }

        private Color GetColor(int idViaje)
        {
            var colorViaje = ColorViaje.Get();
            if (!colorViaje.ContainsKey(idViaje))
            {
                colorViaje.Add(idViaje, ColorGenerator.GetNextColor());
                ColorViaje.Set(colorViaje);
            }
            return colorViaje[idViaje];
        }

        #endregion

        #region GetImageUrl
        public string GetImageUrl(Color color, object text)
        {
            return GetImageUrl(color, text, "cl");
        }
        public string GetImageUrl(Color color, object text, string shape)
        {
            return string.Format("{0}?color={1}&no={2}&shape={3}", Request.Path, color.ToArgb(), text, shape);
        } 
        #endregion

        #region Page Events
        protected override void OnInit(EventArgs e)
        {
            if (Request.QueryString["color"] != null)
            {
                CreateImage();
                return;
            }
            base.OnInit(e);

            if (!IsPostBack)
            {
                var googleMapsEnabled = true;
                var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
                if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
                {
                    var empresa = usuario.Empresas.First() as Empresa;
                    if (empresa != null)
                        googleMapsEnabled = empresa.GoogleMapsEnabled;
                }

                Monitor1.EnableTimer = false;
                Monitor1.Initialize(googleMapsEnabled);
                Monitor1.AddLayers(LayerFactory.GetVector(LayerRecorrido, true),
                                   LayerFactory.GetMarkers(LayerEntregas, true));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterExtJsStyleSheet();
                dtFecha.SelectedDate = DateTime.Today;
            }
        } 
        #endregion

        protected void BtnSearchClick(object sender, EventArgs e)
        {
            try
            {
                if (!panelDisableFilters.Visible)
                {
                    var empresa = cbEmpresa.Selected;
                    var linea = cbLinea.Selected;
                    var fecha = dtFecha.SelectedDate.Value.Date;
                    var desde = fecha.ToDataBaseDateTime();
                    var hasta = desde.AddDays(1);
                    var radio = ValidateInt32(txtRadio.Text, "Radio");
                    var puntoEntrega = DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected);

                    var intercalador = new Intercalador(DAOFactory);
                    intercalador.Load(empresa, linea, desde, hasta, Horas);
                    intercalador.CalcularCostos(puntoEntrega, radio);

                    SetResults(intercalador.Intercalados);

                    panelSave.Visible = grid.Rows.Count > 0;
                }
                else
                {
                    SetResults(null);
                    panelSave.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            
            panelDisableFilters.Visible = !panelDisableFilters.Visible;
            btSearch.VariableName = !panelDisableFilters.Visible ? "BUTTON_SEARCH" : "BUTTON_CANCEL";
        }

        protected void btGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var intercalados = Intercalados.Get();
                var id = grid.SelectedIndex > -1 && grid.SelectedIndex < grid.DataKeys.Count
                             ? Convert.ToInt32(grid.DataKeys[grid.SelectedIndex].Value)
                             : -1;
                if (id <= 0) return;


                var viaje = DAOFactory.ViajeDistribucionDAO.FindById(id);
                Intercalado intercalado = intercalados.FirstOrDefault(i => i.Id == id);

                var nuevoDetalle = new EntregaDistribucion
                                       {
                                           Viaje = viaje,
                                           PuntoEntrega = DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected)
                                       };
                nuevoDetalle.Cliente = nuevoDetalle.PuntoEntrega.Cliente;
                nuevoDetalle.Descripcion = nuevoDetalle.PuntoEntrega.Descripcion;
                nuevoDetalle.Orden = intercalado.Index;
                nuevoDetalle.Programado = intercalado.Hora;
                nuevoDetalle.TipoServicio = DAOFactory.TipoServicioCicloDAO.FindById(cbTipoServicio.Selected);
                viaje.InsertarEntrega(intercalado.Index, nuevoDetalle);

                DAOFactory.ViajeDistribucionDAO.SaveOrUpdate(viaje);
                ShowInfo("La entrega se ha intercalado correctamente");
                if (InformarPorMail)
                {
                    try
                    {
                        SendMail();
                    }
                    catch (Exception ex)
                    {
                        STrace.Exception("Intercalador de Entregas", ex);
                        ShowError("La entrega se ha intercalado correctamente, pero no se pudo enviar el mail");
                    }
                }
                BtnSearchClick(sender, e);
            }
            catch(Exception ex)
            {
                ShowError(ex);
            }
        }
        protected void btInstructions_Click(object sender, EventArgs e)
        {
            multiDetalles.ActiveViewIndex = multiDetalles.ActiveViewIndex == 0 ? 1 : 0;
            btInstructions.VariableName = multiDetalles.ActiveViewIndex == 0 ? "Ver Hoja de Ruta" : "Ver Entregas";
            updDetalles.Update();
        }
        protected void grid_RowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            var dataItem = e.Row.DataItem as Intercalado;
            if (dataItem == null) return;
            var viaje = DAOFactory.ViajeDistribucionDAO.FindById(dataItem.Id);
            e.Row.MakeRowSelectable();
            var color = GetColor(viaje.Id);
            var backColor = new RGBColor { Color = color };
            var foreColor = new RGBColor { Color = GetForeColor(color) };
            e.Row.GetControl<Literal>("litColor").Text = string.Format("background-color: #{0}; color: #{1};", backColor.HexValue, foreColor.HexValue);
            e.Row.GetControl<Label>("lblCodigo").Text = viaje.Codigo;
            e.Row.GetControl<Label>("lblOrden").Text = (dataItem.Index + 1).ToString("0");
            e.Row.GetControl<Label>("lblVehiculo").Text = viaje.Vehiculo != null ? viaje.Vehiculo.Interno : string.Empty;
            e.Row.GetControl<Label>("lblCantidad").Text = viaje.EntregasTotalCount.ToString(CultureInfo.InvariantCulture);
            e.Row.GetControl<Label>("lblHora").Text = dataItem.Hora.ToString("HH:mm");
            e.Row.GetControl<Label>("lblMinutos").Text = string.Format("+{0} min", dataItem.CostoMinExtra.ToString("0"));
            e.Row.GetControl<Label>("lblDistancia").Text = string.Format("+{0} km", (dataItem.CostoKmExtra/1000).ToString("0.0"));

            var icono = e.Row.GetControl<System.Web.UI.WebControls.Image>("imgVehiculo");
            if (viaje.Vehiculo != null)
            {
                icono.ImageUrl = Path.Combine(IconDir, viaje.Vehiculo.TipoCoche.IconoNormal.PathIcono);
            }
            icono.Visible = viaje.Vehiculo != null;
        }

        protected void grid_SelectedIndexChanging(object sender, C1GridViewSelectEventArgs e)
        {
            grid.SelectedIndex = e.NewSelectedIndex;
            ShowSelected();
        }

        private int orden = 1;
        private int selectedIndex = 0;

        protected void reorderDetalles_ItemDataBound(object sender, ReorderListItemEventArgs e)
        {
            var data = e.Item.DataItem as EntregaDistribucion;
            if (data == null) return;
            var color = GetColor(data.Viaje.Id);
            var backColor = new RGBColor { Color = color };
            var foreColor = new RGBColor { Color = GetForeColor(color) };

            var panelHandle = e.Item.GetControl("panelHandle") as Panel;
            var panelItem = e.Item.GetControl("panelItem") as Panel;
            var intercalado = e.Item.DataItemIndex == selectedIndex;
            panelItem.Visible = !intercalado;
            panelHandle.Visible = intercalado;
            var panel = intercalado ? panelHandle : panelItem;
            (panel.GetControl("litColor") as Literal).Text = string.Format("background-color: #{0}; color: #{1};", backColor.HexValue, foreColor.HexValue);
            (panel.GetControl("lblOrden") as Label).Text = (orden++).ToString("0");
            (panel.GetControl("lblDescripcion") as Label).Text = data.Descripcion;
            (panel.GetControl("lblCliente") as Label).Text = data.Cliente != null ? data.Cliente.Descripcion : string.Empty;
            (panel.GetControl("lblDemora") as Label).Text = (data.TipoServicio != null ? data.TipoServicio.Demora : 0) + "min";
            (panel.GetControl("lblTipoServicio") as Label).Text = data.Linea != null ? string.Empty : ("[" + (data.TipoServicio != null ? data.TipoServicio.Descripcion : "ninguno") + "]");
            var lblHora = (panel.GetControl("lblHora") as Label);
            lblHora.Text = data.Programado.ToString("HH:mm");
            lblHora.ForeColor = Color.Green;
        }

        protected void reorderDetalles_ItemReorder(object sender, ReorderListItemReorderEventArgs e)
        {
            if (grid.SelectedIndex < 0) return;
            if (e.NewIndex == 0) return; // Salida de base
            
            var intercalados = Intercalados.Get();
            var selected = Convert.ToInt32(grid.DataKeys[grid.SelectedIndex].Value);
            var distribucion = DAOFactory.ViajeDistribucionDAO.FindById(selected);
            if (distribucion.RegresoABase && e.NewIndex == distribucion.EntregasTotalCountConBases) return; // Regreso a base

            var puntoEntrega = DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected);

            var intercalador = new Intercalador(DAOFactory);
            intercalador.Load(intercalados, Horas);
            intercalador.CambiarIndice(selected, puntoEntrega, e.NewIndex);


            Intercalados.Set(intercalador.Intercalados);
            ShowSelected();
        }

        protected void SetResults(List<Intercalado> intercalados)
        {
            ColorViaje.Set(null);
            Intercalados.Set(intercalados);

            grid.DataSource = intercalados;
            grid.DataBind();
            grid.SelectedIndex = intercalados == null ? -1 : 0;

            ShowSelected();
        }

        protected void ShowSelected()
        {
            var id = grid.SelectedIndex > -1 && grid.SelectedIndex < grid.DataKeys.Count ? Convert.ToInt32(grid.DataKeys[grid.SelectedIndex].Value) : -1;
            var intercalados = Intercalados.Get();
            ShowResults(intercalados, id);
        }

        protected void ShowResults(List<Intercalado> intercalados, int selectedId)
        {
            Monitor1.ClearLayer(LayerEntregas);
            Monitor1.ClearLayer(LayerRecorrido);
            lblInstructions.Text = string.Empty;

            IList<EntregaDistribucion> detalles = null;

            if (intercalados != null && selectedId > 0)
            {
                Intercalado selected = intercalados.FirstOrDefault(i => selectedId == i.Id);

                var viaje = DAOFactory.ViajeDistribucionDAO.FindById(selectedId);

                // nueva entrega
                var nuevoDetalle = new EntregaDistribucion
                                       {
                                           Viaje = viaje,
                                           PuntoEntrega = DAOFactory.PuntoEntregaDAO.FindById(cbPuntoEntrega.Selected)
                                       };
                nuevoDetalle.Cliente = nuevoDetalle.PuntoEntrega.Cliente;
                nuevoDetalle.Descripcion = nuevoDetalle.PuntoEntrega.Descripcion;
                nuevoDetalle.TipoServicio = cbTipoServicio.Selected > 0 ? 
                    DAOFactory.TipoServicioCicloDAO.FindById(cbTipoServicio.Selected)
                    : null;

                // intercalo la nueva entrega en el viaje seleccionado
                detalles = viaje.Detalles;

                viaje.InsertarEntrega(selected.Index, nuevoDetalle);
                if (selected.Index > 0) viaje.CalcularHorario(selected.Index, selected.ViajeIntercalado);
                if (selected.Index < detalles.Count) viaje.CalcularHorario(selected.Index, selected.ViajeIntercalado);


                // muestro todos los viajes en el mapa
                foreach (var costo in intercalados)
                {
                    ShowViaje(costo, selectedId);

                    // si el viaje está seleccionado muestro la nueva entrega con el mismo color
                    if (selectedId == costo.Id)
                    {
                        var colorNew = GetColor(costo.Id);
                        var imageUrlNew = GetImageUrl(colorNew, costo.Index + 1, "sq");
                        AddMarker(nuevoDetalle, imageUrlNew);
                        selected = costo;
                        
                        ShowInstructions(costo);
                    }
                }
                
                Monitor1.SetCenter(nuevoDetalle.ReferenciaGeografica.Latitude,
                                   nuevoDetalle.ReferenciaGeografica.Longitude);

                selectedIndex = selected.Index;
            }

            // bindeo los detalles con la nueva entrega intercalada
            reorderDetalles.DataSource = detalles;
            reorderDetalles.DataBind();

            updResult.Update();
            updDetalles.Update();
        }

        protected void ShowViaje(Intercalado viaje, int selectedId)
        {
            var color = GetColor(viaje.Id);
            var line = new Line("v" + viaje.Id, StyleFactory.GetLineFromColor(color, 4, 0.5));
            var distri = DAOFactory.ViajeDistribucionDAO.FindById(viaje.Id);

            // Agrego los markers de las entregas
            var ordenEntrega = 1;
            foreach (var entrega in distri.Detalles)
            {
                var imageUrl = GetImageUrl(color, ordenEntrega++);
                AddMarker(entrega, imageUrl);
            }

            // Agrego las lineas de recorrido
            var recorrido = viaje.Id == selectedId ? viaje.ViajeIntercalado : viaje.ViajeAnterior;
            if (recorrido != null)
            {
                var stepCount = 0;
                var steps = recorrido.Legs.SelectMany(l => l.Steps);
                var puntos = steps.SelectMany(s => s.Points)
                    .Select(p => new Point((stepCount++).ToString("0"), p.Longitud, p.Latitud));
                line.AddPoints(puntos);
                Monitor1.AddGeometries(LayerRecorrido, line);
            }
        }

        private void ShowInstructions(Intercalado viaje)
        {
            if (viaje.ViajeIntercalado == null) return;
            var distri = DAOFactory.ViajeDistribucionDAO.FindById(viaje.Id);
            var header = string.Format("<b>Hoja de Ruta: {0}</b> <span class='change'>({1})</span><br/>",
                                               distri.Codigo,
                                               distri.Vehiculo != null ? distri.Vehiculo.Interno : "sin vehiculo");

            const string destino = "<br/><div style='font-size: 1.1em'><span class='change'>DESTINO {3}: [{0}]</span> <b>{1}</b> {2}</div></br>";

            var stepNo = 1;
            var destNo = 1;
            var ins = header;
            for (int i = 0; i < viaje.ViajeIntercalado.Legs.Count; i++)
            {
                var entrega = distri.Detalles[i];
                ins += string.Format(destino, entrega.Programado.ToString("HH:mm"), entrega.Descripcion, destNo == viaje.Index + 1 ? "(NUEVO)" : "", destNo++);
                var leg = viaje.ViajeIntercalado.Legs[i];
                ins = leg.Steps.Aggregate(ins, (a, n) => string.Concat(a, "<b><span class='change'>", stepNo++, ".</span></b> ", n.Instructions, "<br/>"));
            }
            var last = distri.Detalles.Last();
            ins += string.Format(destino, last.Programado.ToString("HH:mm"), last.Descripcion, destNo == viaje.Index + 1 ? "(NUEVO)" : "", destNo);
            lblInstructions.Text = ins;
        }

        protected void AddMarker(EntregaDistribucion item, string imageUrl)
        {
            if (item.ReferenciaGeografica == null) return;
            var id = item.Id.ToString("0");
            var latitud = item.ReferenciaGeografica.Latitude;
            var longitud = item.ReferenciaGeografica.Longitude;
            var label = item.Programado.ToString("HH:mm");
            var marker = MarkerFactory.CreateLabeledMarker("m" + id, imageUrl, latitud, longitud, label);
            marker.Size = DrawingFactory.GetSize(20, 20);
            marker.Offset = DrawingFactory.GetOffset(-10, -10);
            Monitor1.AddMarkers(LayerEntregas, marker);
        }

        private void SendMail()
        {
            var mail = new MailSender(Server.MapPath("~/Logictracker.Mailing.Directions.config"));
            mail.SendMail(lblInstructions.Text);
        }

        #region Dynamic Icons
        private Color GetForeColor(Color backColor)
        {
            return (backColor.R + backColor.G + backColor.B) / 3 > 128 || backColor.GetBrightness() > 0.5 ? Color.Black : Color.White;
        }
        private void CreateImage()
        {
            var color = Color.FromArgb(Convert.ToInt32(Request.QueryString["color"]));
            var foreColor = GetForeColor(color);
            Response.Clear();
            Response.ContentType = "image/png";

            using (var bmp = new Bitmap(20, 20))
            {
                var g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var shape = Request.QueryString["shape"];
                if (shape != null && shape == "sq")
                {
                    g.FillRectangle(new SolidBrush(color), 0, 0, 19, 19);
                    g.DrawRectangle(new Pen(Color.Black), 0, 0, 19, 19);
                }
                else
                {
                    g.FillEllipse(new SolidBrush(color), 0, 0, 19, 19);
                    g.DrawEllipse(new Pen(Color.Black), 0, 0, 19, 19);
                }

                var f = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold, GraphicsUnit.Pixel);
                var fmt = StringFormat.GenericDefault;
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Center;
                g.DrawString(Request.QueryString["no"], f, new SolidBrush(foreColor), 9, 11, fmt);

                var mem = new MemoryStream();
                bmp.Save(mem, ImageFormat.Png);
                Response.OutputStream.Write(mem.ToArray(), 0, (int)mem.Length);
            }
            Response.End();
        } 
        #endregion

    }
}