using C1.Web.UI.Controls.C1GridView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Messaging;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Entidades;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Reportes.M2M
{
    public partial class ResumenDeEntidades : ApplicationSecuredPage
    {
        protected override string GetRefference() { return "M2M_RESUMEN_ENTIDADES"; }
        protected override InfoLabel LblInfo { get { return null; } }
        
        private readonly string _codigoPolicia = MessageCode.KeyboardButton1.GetMessageCode();
        private readonly string _codigoAmbulancia = MessageCode.KeyboardButton2.GetMessageCode();
        private readonly string _codigoBomberos = MessageCode.KeyboardButton3.GetMessageCode();
        private readonly string _codigoExcesoTemperatura = MessageCode.SensorUpperLimitExceeded.GetMessageCode();
        private readonly string _codigoBajaTemperatura = MessageCode.SensorLowerLimitExceeded.GetMessageCode();
        private readonly string _codigoDescongelamiento = MessageCode.TemperatureThawingButtonPressed.GetMessageCode();
        private readonly string _codigoDesconexionTemperatura = MessageCode.TemperaturePowerDisconected.GetMessageCode();

        private int _totalInactivos;
        private int _totalActivos;
        private int _totalConAlarmas;
        private int _totalAlarmas;

        protected void BtnActualizar_OnClick(object sender, EventArgs e) 
        {
            CalcularEstadisticas(string.Empty);
            lblTitAlarma.Visible = pnlDetalles.Visible = pnlSubEntidades.Visible = false;
            LoadLabels();
            LoadGauges();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                dtDesde.SelectedDate = DateTime.UtcNow.Date;
                dtHasta.SelectedDate = DateTime.UtcNow.Date.AddDays(1).AddMinutes(-1);

                CalcularEstadisticas(string.Empty);
        
                LoadLabels();
                LoadGauges();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            SetExpiration();
        }
        
        private void SetExpiration()
        {
            gaugeActInact.AbsoluteExpiration = gaugeAlarmas.AbsoluteExpiration = DateTime.Today.AddDays(-1);
            gaugeActInact.SlidingExpiration = gaugeAlarmas.SlidingExpiration = new TimeSpan(0, 0, 1);
        }

        private void CalcularEstadisticas(string estado)
        {
            var entidades = DAOFactory.EntidadDAO.GetList(new[] { ddlEmpresa.Selected },
                                                          new[] { ddlPlanta.Selected },
                                                          new[] { -1 },
                                                          new[] { ddlTipoEntidad.Selected })
                                                 .OrderBy(e => e.TipoEntidad.Descripcion)
                                                 .ToList();

            var entidadesFiltradas = new List<EntidadPadre>();

            if (estado.Equals(string.Empty))
            {
                entidadesFiltradas = entidades;
            }
            else
            {
                foreach (var entidad in entidades)
                {
                    var str = GetIcon(entidad, dtHasta.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value) : DateTime.MinValue, -1).Split('|');
                    
                    if (estado == "Alarma")
                    {
                        if (str[0].Contains("red") || str[0].Contains("blue") || str[0].Contains("black"))
                            entidadesFiltradas.Add(entidad);
                    }
                    else
                    {
                        if (estado == str[1])
                            entidadesFiltradas.Add(entidad);

                        if (estado == "Activo" && (str[0].Contains("red") || str[0].Contains("blue") || str[0].Contains("black")))
                            entidadesFiltradas.Add(entidad);
                    }
                }
            }

            gridEntidades.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI76");
            gridEntidades.Columns[1].HeaderText = CultureManager.GetLabel("DESCRIPCION");
            gridEntidades.Columns[2].HeaderText = CultureManager.GetLabel("ULTIMO_REPORTE");
            gridEntidades.Columns[3].HeaderText = CultureManager.GetLabel("VALOR");
            gridEntidades.Columns[4].HeaderText = CultureManager.GetLabel("ESTADO");
            gridEntidades.DataSource = entidadesFiltradas;
            gridEntidades.DataBind();

            gridEntidades.Visible = entidadesFiltradas.Count > 0;
            lblSinEntidades.Visible = entidadesFiltradas.Count == 0;
            lblTitEntidades.Visible = estado != string.Empty;
            switch (estado)
            {
                case "Alarma":
                    lblTitEntidades.Text = lnkAlarmas.Text + @":";
                    break;
                case "Activo":
                    lblTitEntidades.Text = lnkActivos.Text + @":";
                    break;
                case "Inactivo":
                    lblTitEntidades.Text = lnkInactivos.Text + @":";
                    break;
                default:
                    break;
            }

            var entidadesId = entidadesFiltradas.Select(e => e.Id).ToList();
            var alarmas = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(entidadesId, 
                                                                        new List<string> { _codigoPolicia, _codigoBomberos, _codigoAmbulancia, _codigoExcesoTemperatura, _codigoBajaTemperatura, _codigoDesconexionTemperatura, _codigoDescongelamiento },
                                                                        dtDesde.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtDesde.SelectedDate.Value) : DateTime.MinValue,
                                                                        dtHasta.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value) : DateTime.MinValue);
            var validas = alarmas.Where(alarma => alarma.Sensor != null).OrderByDescending(a => a.Fecha).ToList();

            _totalAlarmas = validas.Count;
            
            gridAlarmas.Columns[0].HeaderText = CultureManager.GetLabel("DATE");
            gridAlarmas.Columns[1].HeaderText = CultureManager.GetEntity("PARENTI79");
            gridAlarmas.Columns[2].HeaderText = CultureManager.GetEntity("PARENTI81");
            gridAlarmas.Columns[3].HeaderText = CultureManager.GetLabel("TEXTO");
            gridAlarmas.DataSource = validas;
            gridAlarmas.DataBind();

            gridAlarmas.Visible = validas.Count > 0;
            lblSinAlarmas.Visible = validas.Count == 0;
        }

        private void LoadLabels()
        {
            lblActivas.Text = (_totalActivos + _totalConAlarmas).ToString();
            lblInactivas.Text = _totalInactivos.ToString();
            lblTotal.Text = (_totalActivos + _totalInactivos + _totalConAlarmas).ToString();
            lblConAlarma.Text = _totalConAlarmas.ToString();
            lblAlarmas.Text = _totalAlarmas.ToString();
        }

        private void LoadGauges()
        {
            var maxActInact = _totalActivos > _totalInactivos ? (Convert.ToInt32(_totalActivos / 10) + 1) * 10
                                                              : (Convert.ToInt32(_totalInactivos / 10) + 1) * 10;
            gaugeActInact.Gauges[0].Maximum = maxActInact;
            gaugeActInact.Gauges[0].Value = _totalConAlarmas;
            gaugeActInact.Gauges[0].MorePointers[0].Value = _totalInactivos;
            gaugeActInact.Gauges[0].MorePointers[1].Value = _totalActivos;

            var maxAlarmas = (Convert.ToInt32(_totalAlarmas / 10) + 1) * 10;
            gaugeAlarmas.Gauges[0].Maximum = maxAlarmas;
            gaugeAlarmas.Gauges[0].Value = _totalAlarmas;
        }

        protected void GridEntidadesOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var entidad = e.Row.DataItem as EntidadPadre;
                if (entidad != null)
                {
                    var lbl = e.Row.FindControl("lblTipoEntidad") as Label;
                    if (lbl != null)
                        lbl.Text = entidad.TipoEntidad.Descripcion;

                    var lnk = e.Row.FindControl("lblDescripcion") as LinkButton;
                    if (lnk != null)
                    {
                        lnk.Text = entidad.Descripcion;
                        lnk.Attributes.Add("ID_ENTIDAD", entidad.Id.ToString());
                    }

                    var ultimaMedicion = GetUltimaMedicion(entidad, dtHasta.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value) : DateTime.MinValue, -1);
                    lbl = e.Row.FindControl("lblFechaUltimoReporte") as Label;
                    if (lbl != null)
                        lbl.Text = ultimaMedicion != null ? ultimaMedicion.FechaMedicion.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : "";

                    lbl = e.Row.FindControl("lblValorUltimoReporte") as Label;
                    if (lbl != null)
                        lbl.Text = ultimaMedicion != null ? ultimaMedicion.Valor : "";

                    var icono = GetIcon(entidad, dtHasta.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value) : DateTime.MinValue, -1);
                    var img = e.Row.FindControl("imgEstado") as Image;
                    if (img != null)
                    {
                        img.ImageUrl = "~/images/" + icono.Split('|')[0];
                        img.ToolTip = icono.Split('|')[1];
                    }

                    var btnEnt = e.Row.FindControl("btnMonitorEntidades") as ImageButton;
                    if (btnEnt != null)
                    {
                        btnEnt.Attributes.Add("ID_ENTIDAD", entidad.Id.ToString());
                        btnEnt.ToolTip = CultureManager.GetMenu("OPE_MON_ENTIDAD");
                        btnEnt.ImageUrl = ResolveUrl("~/OPE_MON_ENTIDAD.image");
                    }

                    var btnSub = e.Row.FindControl("btnMonitorSubEntidades") as ImageButton;
                    if (btnSub != null)
                    {
                        btnSub.Attributes.Add("ID_ENTIDAD", entidad.Id.ToString());
                        btnSub.ToolTip = CultureManager.GetMenu("OPE_MON_SUBENTIDAD");
                        btnSub.ImageUrl = ResolveUrl("~/OPE_MON_SUBENTIDAD.image");
                    }
                }
            }
        }

        protected void GridAlarmasOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var alarma = e.Row.DataItem as LogEvento;
                if (alarma != null)
                {
                    var lbl = e.Row.FindControl("lblFecha") as Label;
                    if (lbl != null)
                        lbl.Text = alarma.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm");

                    lbl = e.Row.FindControl("lblSubentidad") as Label;
                    if (lbl != null)
                        lbl.Text = alarma.SubEntidad.Descripcion;

                    lbl = e.Row.FindControl("lblTexto") as Label;
                    if (lbl != null)
                        lbl.Text = alarma.Texto;

                    var entidades = DAOFactory.EntidadDAO.GetList(new[] {ddlEmpresa.Selected},
                                                                  new[] {ddlPlanta.Selected},
                                                                  new[] {alarma.Sensor.Dispositivo.Id},
                                                                  new[] {ddlTipoEntidad.Selected});
                    if (entidades.Count > 0)
                    {
                        lbl = e.Row.FindControl("lblEntidad") as Label;
                        if (lbl != null)
                            lbl.Text = entidades[0].Descripcion;
                    }
                }
            }
        }

        protected void GridDetallesOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var detalle = e.Row.DataItem as DetalleValor;
                if (detalle != null)
                {
                    var lbl = e.Row.FindControl("lblDescripcion") as Label;
                    if (lbl != null)
                        lbl.Text = detalle.Detalle.Nombre;

                    lbl = e.Row.FindControl("lblValor") as Label;
                    if (lbl != null)
                    {
                        switch (detalle.Detalle.Tipo)
                        {
                            case 1:
                                lbl.Text = detalle.ValorStr;
                                break;
                            case 2:
                                lbl.Text = detalle.ValorNum.ToString();
                                break;
                            case 3:
                                lbl.Text = detalle.ValorDt.HasValue ? detalle.ValorDt.Value.ToString("dd/MM/yyyy HH:mm") : "";
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        protected void GridSubEntidadesOnRowDataBound(object sender, C1GridViewRowEventArgs e)
        {
            if (e.Row.RowType == C1GridViewRowType.DataRow)
            {
                var subentidad = e.Row.DataItem as SubEntidad;
                if (subentidad != null)
                {
                    var lbl = e.Row.FindControl("lblEntidad") as Label;
                    if (lbl != null)
                        lbl.Text = subentidad.Entidad.Descripcion;

                    var lnk = e.Row.FindControl("lblDescripcion") as LinkButton;
                    if (lnk != null)
                    {
                        lnk.Text = subentidad.Descripcion;
                        lnk.Attributes.Add("ID_SUBENTIDAD", subentidad.Id.ToString());
                    }

                    var ultimaMedicion = GetUltimaMedicion(subentidad.Entidad, dtHasta.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value) : DateTime.MinValue, subentidad.Sensor.Id);
                    lbl = e.Row.FindControl("lblFechaUltimoReporte") as Label;
                    if (lbl != null)
                        lbl.Text = ultimaMedicion != null ? ultimaMedicion.FechaMedicion.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm") : "";

                    lbl = e.Row.FindControl("lblValorUltimoReporte") as Label;
                    if (lbl != null)
                        lbl.Text = ultimaMedicion != null
                                       ? ultimaMedicion.TipoMedicion.Codigo.Equals("TIME")
                                             ? new TimeSpan(0,0,0, int.Parse(ultimaMedicion.Valor)).ToString()
                                             : ultimaMedicion.Valor
                                       : "";

                    var icono = GetIcon(subentidad.Entidad, dtHasta.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value) : DateTime.MinValue, subentidad.Sensor.Id);
                    var img = e.Row.FindControl("imgEstado") as Image;
                    if (img != null)
                    {
                        img.ImageUrl = "~/images/" + icono.Split('|')[0];
                        img.ToolTip = icono.Split('|')[1];
                    }

                    var btnSub = e.Row.FindControl("btnMonitorSubEntidades") as ImageButton;
                    if (btnSub != null)
                    {
                        btnSub.Attributes.Add("ID_SUBENTIDAD", subentidad.Id.ToString());
                        btnSub.ToolTip = CultureManager.GetMenu("OPE_MON_SUBENTIDAD");
                        btnSub.ImageUrl = ResolveUrl("~/OPE_MON_SUBENTIDAD.image");
                    }
                }
            }
        }

        protected void BtnMonitorEntidadesOnClick(object sender, EventArgs e)
        {
            var btn = sender as ImageButton;
            if (btn != null)
                this.RegisterStartupJScript("MonitorEntidades", "window.open('../../Monitor/MonitorDeEntidades/MonitorEntidades.aspx?ID_ENTIDAD=" + btn.Attributes["ID_ENTIDAD"] + "','MonitorEntidades');");
        }

        protected void BtnMonitorSubEntidadesOnClick(object sender, EventArgs e)
        {
            var btn = sender as ImageButton;
            if (btn != null)
                this.RegisterStartupJScript("MonitorSubEntidades", "window.open('../../Monitor/MonitorDeSubEntidades/MonitorSubEntidades.aspx?ID_ENTIDAD=" + btn.Attributes["ID_ENTIDAD"] + "','MonitorSubEntidades');");
        }

        protected void BtnMonitorSubEntidades2OnClick(object sender, EventArgs e)
        {
            var btn = sender as ImageButton;
            if (btn != null)
                this.RegisterStartupJScript("MonitorSubEntidades", "window.open('../../Monitor/MonitorDeSubEntidades/MonitorSubEntidades.aspx?ID_SUBENTIDAD=" + btn.Attributes["ID_SUBENTIDAD"] + "','MonitorSubEntidades');");
        }

        protected void LblDescripcionEntidadOnClick(object sender, EventArgs e)
        {
            var lbl = sender as LinkButton;
            if (lbl == null) return;
            
            var idEntidad = Convert.ToInt32(lbl.Attributes["ID_ENTIDAD"]);
            var entidad = DAOFactory.EntidadDAO.FindById(idEntidad);

            LoadAlarmas(entidad, null);
            LoadDetalles(entidad);
            LoadSubentidades(entidad);
        }

        protected void LblDescripcionSubEntidadOnClick(object sender, EventArgs e)
        {
            var lbl = sender as LinkButton;
            if (lbl == null) return;

            var idSubEntidad = Convert.ToInt32(lbl.Attributes["ID_SUBENTIDAD"]);
            var subentidad = DAOFactory.SubEntidadDAO.FindById(idSubEntidad);

            LoadAlarmas(subentidad.Entidad, subentidad);
        }

        protected void LnkActivosOnClick(object sender, EventArgs e)
        {
            CalcularEstadisticas("Activo");
            pnlDetalles.Visible = false;
            pnlSubEntidades.Visible = false;
            lblTitAlarma.Visible = false;
        }

        protected void LnkInactivosOnClick(object sender, EventArgs e)
        {
            CalcularEstadisticas("Inactivo");
            pnlDetalles.Visible = false;
            pnlSubEntidades.Visible = false;
            lblTitAlarma.Visible = false;
        }

        protected void LnkAlarmasOnClick(object sender, EventArgs e)
        {
            CalcularEstadisticas("Alarma");
            pnlDetalles.Visible = false;
            pnlSubEntidades.Visible = false;
            lblTitAlarma.Visible = false;
        }

        protected void LnkTotalOnClick(object sender, EventArgs e)
        {
            CalcularEstadisticas(string.Empty);
            pnlDetalles.Visible = false;
            pnlSubEntidades.Visible = false;
            lblTitAlarma.Visible = false;
        }

        private Medicion GetUltimaMedicion(EntidadPadre entidad, DateTime fecha, int idSensor)
        {
            var subEntidades = DAOFactory.SubEntidadDAO.GetList(new[] { entidad.Empresa.Id },
                                                                new[] { entidad.Linea != null ? entidad.Linea.Id : -1 },
                                                                new[] { entidad.TipoEntidad != null ? entidad.TipoEntidad.Id : -1 },
                                                                new[] { entidad.Id },
                                                                new[] { entidad.Dispositivo != null ? entidad.Dispositivo.Id : -1 },
                                                                new[] { idSensor });
            Medicion ultimaMedicion = null;

            foreach (var subEntidad in subEntidades)
            {
                if (subEntidad.Sensor == null) return null;

                var medicion = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subEntidad.Sensor.Dispositivo.Id, subEntidad.Sensor.Id, fecha);

                if (medicion != null && (ultimaMedicion == null || ultimaMedicion.FechaMedicion < medicion.FechaMedicion))
                    ultimaMedicion = medicion;
            }

            return ultimaMedicion;
        }

        private string GetIcon(EntidadPadre entidad, DateTime fecha, int idSensor)
        {
            var subEntidades = DAOFactory.SubEntidadDAO.GetList(new[] { entidad.Empresa.Id },
                                                                new[] { entidad.Linea != null ? entidad.Linea.Id : -1 },
                                                                new[] { entidad.TipoEntidad != null ? entidad.TipoEntidad.Id : -1 },
                                                                new[] { entidad.Id },
                                                                new[] { entidad.Dispositivo != null ? entidad.Dispositivo.Id : -1 },
                                                                new[] { idSensor });
            var style = "";

            foreach (var subEntidad in subEntidades)
            {
                if (subEntidad.Sensor == null) continue;

                var ultimaMedicion = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subEntidad.Sensor.Dispositivo.Id, subEntidad.Sensor.Id, fecha);
                
                // DESCONGELAMIENTO
                var ultimoEventoDescongelamiento = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                                     new[] { Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString(),
                                                                                                             Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonUnpressed).ToString() });
                var enDescongelamiento = ultimoEventoDescongelamiento != null
                                      && ultimoEventoDescongelamiento.Mensaje != null
                                      && ultimoEventoDescongelamiento.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString();
                // CONEXION
                var ultimoEventoConexion = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                             new[] { Convert.ToInt32(MessageIdentifier.TemperaturePowerReconected).ToString(),
                                                                                                     Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString() });
                var energiaDesconectada = ultimoEventoConexion != null
                       && ultimoEventoConexion.Mensaje != null
                       && ultimoEventoConexion.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString();
                // ALARMA BOTONERA
                var ultimoEventoBotonera = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                             new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton1).ToString(),
                                                                                                     Convert.ToInt32(MessageIdentifier.KeyboardButton2).ToString(),
                                                                                                     Convert.ToInt32(MessageIdentifier.KeyboardButton3).ToString() });
                var enEmergencia = ultimoEventoBotonera != null && ultimoEventoBotonera.Fecha > DateTime.UtcNow.AddMinutes(-10);
                // RONDIN
                var ultimoEventoRondin = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                           new[] { Convert.ToInt32(MessageIdentifier.CheckpointReached).ToString(),
                                                                                                   Convert.ToInt32(MessageIdentifier.BateryDisconected).ToString(),
                                                                                                   Convert.ToInt32(MessageIdentifier.BateryReConected).ToString(),
                                                                                                   Convert.ToInt32(MessageIdentifier.BateryLow).ToString(),
                                                                                                   Convert.ToInt32(MessageIdentifier.DeviceOpenned).ToString() });

                if (ultimaMedicion != null)
                {
                    //MEDICION DE TEMPERATURA
                    if (ultimaMedicion.TipoMedicion.Codigo.Equals("TEMP"))
                    {
                        if (enDescongelamiento)
                            style = "button-blue.png|En Descongelamiento";
                        else
                            if (energiaDesconectada)
                            {
                                _totalConAlarmas++;
                                return "button-black.png|Desconectado";
                            }
                            else
                            {
                                if (ultimaMedicion.ValorDouble > subEntidad.Maximo && subEntidad.ControlaMaximo)
                                {
                                    _totalConAlarmas++;
                                    return "button-red.png|Exceso Temperatura";
                                }
                                if (ultimaMedicion.ValorDouble < subEntidad.Minimo && subEntidad.ControlaMinimo)
                                {
                                    _totalConAlarmas++;
                                    return "button-red.png|Exceso Temperatura";
                                }

                                if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-3))
                                {
                                    if (style.Split('|')[0] != "button-blue.png")
                                        style = "button-grey.png|Inactivo";
                                }
                                else
                                {
                                    if (style == "")
                                        style = "button-green.png|Activo";
                                }
                            }
                        
                        continue;
                    }
                    
                    //MEDICION DE ESTADO
                    if (ultimaMedicion.TipoMedicion.Codigo.Equals("EST"))
                    {
                        if (enEmergencia)
                        {
                            _totalConAlarmas++;
                            return "button-red.png|En Emergencia";
                        }

                        if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-3))
                        {
                            if (style.Split('|')[0] != "button-blue.png")
                                style = "button-grey.png|Inactivo";
                        }
                        else
                        {
                            if (style == "")
                                style = "button-green.png|Activo";
                        }

                        if (ultimoEventoRondin != null && ultimoEventoRondin.Mensaje != null)
                        {
                            var msg = ultimoEventoRondin.Mensaje;
                            if (msg.Codigo == Convert.ToInt32(MessageIdentifier.BateryReConected).ToString()
                                || msg.Codigo == Convert.ToInt32(MessageIdentifier.BateryLow).ToString()
                                || msg.Codigo == Convert.ToInt32(MessageIdentifier.DeviceOpenned).ToString())
                                if (style.Split('|')[0] == "button-green.png" || style == "")
                                    style = "button-yellow.png|Activo";
                        }

                        if (style == "")
                            style = "button-green.png|Activo";

                        continue;
                    }
                    
                    //MEDICION NUMERICA
                    if (ultimaMedicion.TipoMedicion.Codigo.Equals("NU"))
                    {
                        if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-2))
                        {
                            if (style.Split('|')[0] != "button-blue.png")
                                style = "button-grey.png|Inactivo";
                        }
                        else
                        {
                            if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-1))
                            {
                                if (style.Split('|')[0] == "button-green.png" || style == "")
                                    style = "button-yellow.png|Activo";
                            }
                            else
                            {
                                if (style == "")
                                    style = "button-green.png|Activo";
                            }
                        }

                        continue;
                    }

                    if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-2))
                    {
                        if (style.Split('|')[0] != "button-blue.png")
                            style = "button-grey.png|Inactivo";
                    }
                    else
                    {
                        if (ultimaMedicion.FechaMedicion < DateTime.UtcNow.AddHours(-1))
                        {
                            if (style.Split('|')[0] == "button-green.png" || style == "")
                                style = "button-yellow.png|Activo";
                        }
                        else
                        {
                            if (style == "")
                                style = "button-green.png|Activo";
                        }
                    }
                }
                else
                    style = "button-grey.png|Inactivo";
            }

            if (style == "")
                style = "button-grey.png|Inactivo";

            if (style.Contains("green") || style.Contains("yellow"))
                _totalActivos++;
            else
            {
                if (style.Contains("grey")) _totalInactivos++;
                if (style.Contains("blue")) _totalConAlarmas++;
            }

            return style;
        }

        private void LoadAlarmas(EntidadPadre entidad, SubEntidad subEntidad)
        {
            var alarmas = DAOFactory.LogEventoDAO.GetByEntitiesAndCodes(new List<int> { entidad.Id }, 
                                                                        new List<string> { _codigoPolicia, _codigoBomberos, _codigoAmbulancia, _codigoExcesoTemperatura, _codigoBajaTemperatura, _codigoDesconexionTemperatura },
                                                                        dtDesde.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtDesde.SelectedDate.Value) : DateTime.MinValue, 
                                                                        dtHasta.SelectedDate.HasValue ? SecurityExtensions.ToDataBaseDateTime(dtHasta.SelectedDate.Value) : DateTime.MinValue);
            var validas = alarmas.Where(alarma => alarma.Sensor != null
                                               && (subEntidad == null 
                                                  || alarma.SubEntidad.Id == subEntidad.Id))
                                 .ToList();

            gridAlarmas.Columns[0].HeaderText = CultureManager.GetLabel("DATE");
            gridAlarmas.Columns[1].HeaderText = CultureManager.GetEntity("PARENTI79");
            gridAlarmas.Columns[2].HeaderText = CultureManager.GetEntity("PARENTI81");
            gridAlarmas.Columns[3].HeaderText = CultureManager.GetLabel("TEXTO");
            gridAlarmas.DataSource = validas;
            gridAlarmas.DataBind();

            lblTitAlarma.Text = subEntidad != null 
                                    ? CultureManager.GetLabel("ALARMAS_SUBENTIDAD") + @" " + subEntidad.Descripcion + @":" 
                                    : CultureManager.GetLabel("ALARMAS_ENTIDAD") + @" " + entidad.Descripcion + @":";
            lblTitAlarma.Visible = true;
            gridAlarmas.Visible = validas.Count > 0;
            lblSinAlarmas.Visible = validas.Count == 0;
        }

        private void LoadDetalles(EntidadPadre entidad)
        {
            var detalles = entidad.Detalles.Cast<DetalleValor>().Where(d => !d.Detalle.Baja);
            gridDetalles.Columns[0].HeaderText = CultureManager.GetLabel("DESCRIPCION");
            gridDetalles.Columns[1].HeaderText = CultureManager.GetLabel("VALOR");
            gridDetalles.DataSource = detalles;
            gridDetalles.DataBind();

            lblTitDetalles.Text = CultureManager.GetLabel("DETALLES_ENTIDAD") + @" " + entidad.Descripcion + @":";
            lblTitDetalles.Visible = true;
            pnlDetalles.Visible = true;
            gridDetalles.Visible = detalles != null && detalles.Count() > 0;
            lblSinDetalles.Visible = detalles == null || detalles.Count() == 0;
        }

        private void LoadSubentidades(EntidadPadre entidad)
        {
            var subentidades = DAOFactory.SubEntidadDAO.GetList(ddlEmpresa.SelectedValues,
                                                                ddlPlanta.SelectedValues,
                                                                ddlTipoEntidad.SelectedValues,
                                                                new[] { entidad.Id },
                                                                new[] { entidad.Dispositivo != null ? entidad.Dispositivo.Id : -1 },
                                                                new[] { -1 });

            gridSubEntidades.Columns[0].HeaderText = CultureManager.GetEntity("PARENTI79");
            gridSubEntidades.Columns[1].HeaderText = CultureManager.GetLabel("DESCRIPCION");
            gridSubEntidades.Columns[2].HeaderText = CultureManager.GetLabel("ULTIMO_REPORTE");
            gridSubEntidades.Columns[3].HeaderText = CultureManager.GetLabel("VALOR");
            gridSubEntidades.Columns[4].HeaderText = CultureManager.GetLabel("ESTADO");
            gridSubEntidades.DataSource = subentidades;
            gridSubEntidades.DataBind();

            pnlSubEntidades.Visible = subentidades.Count > 0;
            lblSinSubEntidades.Visible = subentidades.Count == 0;
        }
    }
}
