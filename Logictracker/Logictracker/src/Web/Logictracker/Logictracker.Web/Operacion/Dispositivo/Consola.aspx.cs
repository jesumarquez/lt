using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Logictracker.Configuration;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;
using Logictracker.Web.Monitor;
using Logictracker.Web.Monitor.Markers;
using System.Globalization;
using Logictracker.Culture;
using System.IO;
using AjaxControlToolkit;
using NHibernate.Util;

namespace Logictracker.Operacion.Dispositivo
{
    public partial class Consola : ApplicationSecuredPage
    {
        public enum FilterViews
        {
            Dispositivo,
            Vehiculo
        }

        protected override string GetRefference() { return "OPE_CONSOLA_DISPOSITIVOS"; }
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        protected DateTime MinDate { get { return DateTime.UtcNow.AddDays(-1); } }
        protected FilterViews CurrentFilterView { get { return panelTipoDispositivo.Visible ? FilterViews.Dispositivo : FilterViews.Vehiculo; } }
        protected VsProperty<int> TabCount { get { return this.CreateVsProperty("TabCount", 0); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var googleMapsEnabled = true;
                var usuario = DAOFactory.UsuarioDAO.FindById(WebSecurity.AuthenticatedUser.Id);
                if (usuario != null && usuario.PorEmpresa && usuario.Empresas.Count == 1)
                {
                    var empresa = usuario.Empresas.First() as Empresa;
                    if (empresa != null)
                        googleMapsEnabled = empresa.GoogleMapsEnabled;
                }

                Monitor1.Initialize(googleMapsEnabled);
                Monitor1.AddLayers(LayerFactory.GetMarkers("Posicion", true));
                
            }
        }
        protected void btMapa_Command(object sender, CommandEventArgs e)
        {
            var pos = e.CommandArgument.ToString().Split(',');
            var lat = Convert.ToDouble(pos[0], CultureInfo.InvariantCulture);
            var lon = Convert.ToDouble(pos[1], CultureInfo.InvariantCulture);
            var icn = string.Concat(ImagesDir, "salida.png");
            var size = DrawingFactory.GetSize(40, 40);
            var offset = DrawingFactory.GetOffset(-20, -32);
            var marker = new Marker("0", icn, lat, lon, size, offset);
            Monitor1.AddMarkers("Posicion", marker);
            Monitor1.SetCenter(lat, lon, 12);
            modalMapa.Show();
            ScriptManager.RegisterStartupScript(this, typeof(string), "hidemap", string.Format("$get('{0}').style.visibility = '';", divMapa.ClientID), true);
        }

        protected void btChangeMode_Click(object sender, EventArgs e)
        {
            var view = CurrentFilterView == FilterViews.Dispositivo ? FilterViews.Vehiculo : FilterViews.Dispositivo;
            SetFilterView(view);
        }
        protected void SetFilterView(FilterViews view)
        {
            panelTipoDispositivo.Visible = panelDispositivo.Visible = view == FilterViews.Dispositivo;
            panelTipoVehiculo.Visible = panelVehiculo.Visible = view == FilterViews.Vehiculo;
        }

        protected void BtnSearchClick(object sender, EventArgs e)
        {
            var dispositivo = CurrentFilterView == FilterViews.Vehiculo 
                ? DAOFactory.CocheDAO.FindById(cbVehiculo.Selected).Dispositivo 
                : DAOFactory.DispositivoDAO.FindById(cbDispositivo.Selected);
            if(dispositivo == null)
            {
                ShowError(CultureManager.GetError("VEHICLE_NO_DEVICE_ASSIGNED"));
                panelResult.Visible = false;
                return;
            }
            ShowDatosGenerales(dispositivo);
            ShowUltimaPosicion(dispositivo);
            ShowEventos(dispositivo);
            ShowPosicionesDescartadas(dispositivo);
            ShowEventosDescartados(dispositivo);
            ShowErrores(dispositivo);
            ShowFota(dispositivo);
            panelResult.Visible = true;
        }

        protected void ShowDatosGenerales(Logictracker.Types.BusinessObjects.Dispositivos.Dispositivo dispositivo)
        {
            lblTipoDispositivo.Text = dispositivo.TipoDispositivo.Modelo;
            lblDispositivoCodigo.Text = dispositivo.Codigo;
            lblDispositivoId.Text = dispositivo.Id.ToString();

            var coche = DAOFactory.CocheDAO.FindMobileByDevice(dispositivo.Id);
            lblVehiculo.Text = coche != null ? coche.Interno : "(sin vehiculo asignado)";
            lblTipoVehiculo.Text = coche != null ? coche.TipoCoche.Descripcion : string.Empty;
        }

        protected void ShowUltimaPosicion(Logictracker.Types.BusinessObjects.Dispositivos.Dispositivo dispositivo)
        {
            panelPosicion.Visible = false;
            lblFecha.Text = string.Empty;
            btMapa.Text = string.Empty;
            lblVelocidad.Text = string.Empty;

            try
            {
                var coche = DAOFactory.CocheDAO.FindMobileByDevice(dispositivo.Id);
                if (coche == null)
                {
                    lblFecha.Text = "(no hay vehiculo asignado)";
                    return;
                }

                var posicion = DAOFactory.LogPosicionDAO.GetLastOnlineVehiclePosition(coche);
                if (posicion == null)
                {
                    lblFecha.Text = "(no se encontró posición)";
                    return;
                }

                if (posicion.IdDispositivo != dispositivo.Id) return;

                var fecha = posicion.FechaMensaje.ToDisplayDateTime();
                lblFecha.Text = fecha.ToString("dd/MM/yyyy HH:mm:ss");
                lblVelocidad.Text = posicion.Velocidad.ToString(CultureInfo.InvariantCulture);
                var latlon = string.Concat(posicion.Latitud.ToString(CultureInfo.InvariantCulture), ",",posicion.Longitud.ToString(CultureInfo.InvariantCulture));
                btMapa.Text = string.Format("({0})",latlon);
                btMapa.CommandArgument = latlon;
            }
            catch (Exception ex)
            {
                lblFecha.Text = ex.Message;
            }
            panelPosicion.Visible = true;
        }
        protected void ShowEventos(Logictracker.Types.BusinessObjects.Dispositivos.Dispositivo dispositivo)
        {
            panelEventos.Visible = false;
            lblEnventos.Text = string.Empty;

            try
            {
            var coche = DAOFactory.CocheDAO.FindMobileByDevice(dispositivo.Id);
            if (coche == null) return;

            var maxMonths = coche.Empresa != null ? coche.Empresa.MesesConsultaPosiciones : 3;
            var eventos = DAOFactory.LogMensajeDAO.GetLastEvents(coche.Id, MinDate, 20, maxMonths);

            if (eventos.Count > 0)
            {
                foreach (var evento in eventos)
                {
                    if (evento.Dispositivo.Id != dispositivo.Id) continue;
                    lblEnventos.Text += string.Format("<div style='padding: 5px; margin-bottom: 3px; background-color: #ffffff; border: solid 1px #cccccc;'>{0}<br/><b>{1}</b></div>",
                                                      evento.Fecha.ToDisplayDateTime().ToString("dd-MM-yyy HH:mm"), evento.Texto);
                }
            }
            else
            {
                lblEnventos.Text = "(no se encontraron eventos)";
            }
}
            catch(Exception ex)
            {
                lblEnventos.Text = ex.Message;
            }
            panelEventos.Visible = true;
        }
        protected void ShowPosicionesDescartadas(Logictracker.Types.BusinessObjects.Dispositivos.Dispositivo dispositivo)
        {
            panelPosicionesDescartadas.Visible = false;
            lblPosicionesDescartadas.Text = string.Empty;

            try
            {
            var eventos = DAOFactory.LogPosicionDescartadaDAO.GetLastPositions(dispositivo.Id, 5);

            foreach (var evento in eventos)
            {
                if (evento.FechaRecepcion < MinDate) continue;
                lblPosicionesDescartadas.Text += string.Format("<div style='padding: 5px; margin-bottom: 3px; background-color: #ffffff; border: solid 1px #cccccc;'>{0}<br/><b>{1}</b></div>",
                                                  evento.FechaMensaje.ToDisplayDateTime().ToString("dd-MM-yyy HH:mm"), DAOFactory.LogPosicionDescartadaDAO.GetMotivoDescarte(evento.MotivoDescarte));
            }

            if (string.IsNullOrEmpty(lblPosicionesDescartadas.Text))
            {
                lblPosicionesDescartadas.Text = "(no se encontraron posiciones descartadas)";
            }
                }
            catch(Exception ex)
            {
                lblPosicionesDescartadas.Text = ex.Message;
            }
            panelPosicionesDescartadas.Visible = true;
        }
        protected void ShowEventosDescartados(Logictracker.Types.BusinessObjects.Dispositivos.Dispositivo dispositivo)
        {
            panelEventosDescartados.Visible = false;
            lblEventosDescartados.Text = string.Empty;

            try
            {
            var coche = DAOFactory.CocheDAO.FindMobileByDevice(dispositivo.Id);
            if (coche == null) return;

            var eventos = DAOFactory.LogMensajeDescartadoDAO.GetLastEvents(coche.Id, 5);

            foreach (var evento in eventos)
            {
                if (evento.Fecha < MinDate) continue;
                if (evento.Dispositivo.Id != dispositivo.Id) continue;
                lblEventosDescartados.Text += string.Format("<div style='padding: 5px; margin-bottom: 3px; background-color: #ffffff; border: solid 1px #cccccc;'>{0}<br/><b>{1}</b><br/>{2}</div>",
                                                  evento.Fecha.ToDisplayDateTime().ToString("dd-MM-yyy HH:mm"), evento.Texto,
                                                  DAOFactory.LogPosicionDescartadaDAO.GetMotivoDescarte(evento.MotivoDescarte));
            }
            
            if (string.IsNullOrEmpty(lblEventosDescartados.Text))
            {
                lblEventosDescartados.Text = "(no se encontraron posiciones descartadas)";
            }
                }
            catch(Exception ex)
            {
                lblEventosDescartados.Text = ex.Message;
            }
            panelEventosDescartados.Visible = true;
        }

        protected void ShowErrores(Logictracker.Types.BusinessObjects.Dispositivos.Dispositivo dispositivo)
        {
            panelErrores.Visible = false;
            lblErrores.Text = string.Empty;

            try
            {
                var reader = new Reader();

                var eventos = reader.GetLastEntries(dispositivo.Id, 1, MinDate, 5);

                foreach (var evento in eventos)
                {
                    lblErrores.Text +=
                        string.Format(
                            "<div style='padding: 5px; margin-bottom: 3px; background-color: #ffffff; border: solid 1px #cccccc;'>{0} <br/>{1} - {2}<br/><b>{3}</b></div>",
                            evento.DateTime.ToDisplayDateTime().ToString("dd-MM-yyy HH:mm"),
                            evento.Module,
                            evento.Component,
                            evento.Message);
                }

                if (string.IsNullOrEmpty(lblErrores.Text))
                {
                    lblErrores.Text = "(no se encontraron errores)";
                }

            }
            catch(Exception ex)
            {
                lblErrores.Text = ex.Message;
            }
            panelErrores.Visible = true;
        }
        protected void ShowFota(Logictracker.Types.BusinessObjects.Dispositivos.Dispositivo dispositivo)
        {
            try
            {
                var tabFota = new TabContainer
                                    {
                                        Height = Unit.Pixel(260),
                                        Width = Unit.Percentage(100),
                                        ScrollBars = ScrollBars.Auto
                                    };

                var path = Config.Directory.FotaDirectory;
                if (!Directory.Exists(path))
                {
                    throw new ApplicationException("No existe el directorio." + path);
                }
                var files = Directory.GetFiles(path, string.Format("{0}.*", dispositivo.Id.ToString().PadLeft(4,'0')));
                TabCount.Set(files.Length);
                foreach (var file in files)
                {
                    var tab = new TabPanel();
                    tab.HeaderText = Path.GetExtension(file);
                    var panel = new Panel {ScrollBars = ScrollBars.Auto};
                    var lblContent = new Label();
                    panel.Controls.Add(lblContent);
                    tab.Controls.Add(panel);
                    var info = new FileInfo(file);
                    var size = info.Length;
                    if (size < 500000)
                    {
                        lblContent.Text = "<pre>" + File.ReadAllText(file) + "</pre>";
                    }
                    else
                    {
                        lblContent.Text = "(El archivo es muy grande)";
                    }
                    tabFota.Tabs.Add(tab);
                }
                panelFota.Controls.Add(tabFota);
            }
            catch(Exception ex)
            {
                lblErrores.Text = ex.Message;
            }
        }
    }
}