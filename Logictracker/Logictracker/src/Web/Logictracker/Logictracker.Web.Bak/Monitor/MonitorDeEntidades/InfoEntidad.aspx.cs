using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using Logictracker.Messaging;
using Logictracker.Services.Helpers;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Web;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Web.CustomWebControls.Labels;

namespace Logictracker.Monitor.MonitorDeEntidades
{
    public partial class OperacionInfoEntidad : OnLineSecuredPage
    {
        protected override InfoLabel LblInfo { get { return null; } }
        protected override string GetRefference() { return "OPE_MON_ENTIDAD"; }

        protected int IdDispositivo
        {
            get{ return (int)(ViewState["IdDispositivo"]??0);}
            set { ViewState["IdDispositivo"] = value; }
        }
        protected int IdEntidad
        {
            get { return (int)(ViewState["IdEntidad"] ?? 0); }
            set { ViewState["IdEntidad"] = value; }
        }
        protected List<int> IdsLineas
        {
            get { return (List<int>)(ViewState["IdsLineas"] ?? new List<int>()); }
            set { ViewState["IdsLineas"] = value; }
        }
        protected List<int> IdsEmpresas
        {
            get { return (List<int>)(ViewState["IdsEmpresas"] ?? new List<int>()); }
            set { ViewState["IdsEmpresas"] = value; }
        }
        protected int LoadStep
        {
            get { return (int)(ViewState["LoadStep"] ?? 0); }
            set { ViewState["LoadStep"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (Request.QueryString["c"] == null)
            {
                lblDescripcion.Text = CultureManager.GetError("NO_VEHICLE_INFO");
                return;
            }

            IdEntidad = Convert.ToInt32(Request.QueryString["c"]);

            IdsLineas = Request.QueryString["l"].Split(' ').Select(l => Convert.ToInt32(l)).ToList();
            IdsEmpresas = !string.IsNullOrEmpty(Request.QueryString["e"])
                              ? Request.QueryString["e"].Split(' ').Select(l => Convert.ToInt32(l)).ToList()
                              : new List<int> { -1 };

            var entidad = DAOFactory.EntidadDAO.FindById(IdEntidad);

            IdDispositivo = entidad.Dispositivo != null ? entidad.Dispositivo.Id : -1;
        
            lblDescripcion.Text = entidad.Descripcion;
            lblTipo.Text = entidad.TipoEntidad.Descripcion;
            imgTipo.ImageUrl = IconDir + entidad.ReferenciaGeografica.Icono.PathIcono;

            var subEntidades = DAOFactory.SubEntidadDAO.GetList(new[] { entidad.Empresa.Id },
                                                                new[] { entidad.Linea != null ? entidad.Linea.Id : -1 },
                                                                new[] { entidad.TipoEntidad.Id },
                                                                new[] { entidad.Id },
                                                                new[] { entidad.Dispositivo != null ? entidad.Dispositivo.Id : -1 },
                                                                new[] { -1 });

            var fechaUltimaMedicion = DateTime.MinValue;
        
            foreach (var subEntidad in subEntidades)
            {
                var ultimaMedicion = DAOFactory.MedicionDAO.GetUltimaMedicionHasta(subEntidad.Sensor.Dispositivo.Id, subEntidad.Sensor.Id, DateTime.UtcNow);

                var label = new Label { Text = subEntidad.Descripcion + @": " };
                divValor.Controls.Add(label);
                label = new Label { Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };

                switch (subEntidad.Sensor.TipoMedicion.Codigo)
                {
                    case "TEMP":
                        if (ultimaMedicion != null)
                        {
                            fechaUltimaMedicion = ultimaMedicion.FechaMedicion > fechaUltimaMedicion
                                                      ? ultimaMedicion.FechaMedicion
                                                      : fechaUltimaMedicion;

                            label.Text = ultimaMedicion.Valor;
                            divValor.Controls.Add(label);

                            //EVALUO CONEXION
                            label = new Label { Text = @" Energía: " };
                            divValor.Controls.Add(label);
                            label = new Label { Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };

                            var ultimoEventoConexion = DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                                         new[] { Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString(),
                                                                                                                 Convert.ToInt32(MessageIdentifier.TemperaturePowerReconected).ToString() });

                            if (ultimoEventoConexion != null &&
                                ultimoEventoConexion.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperaturePowerDisconected).ToString())
                                label.Text = @"Off";
                            else
                                label.Text = @"On";
                            divValor.Controls.Add(label);

                            //EVALUO DESCONGELAMIENTO
                            label = new Label { Text = @" Desc: " };
                            divValor.Controls.Add(label);
                            label = new Label { Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };

                            var ultimoEventoDescongelamiento =
                                DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                  new[] { Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString(),
                                                                                          Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonUnpressed).ToString() });

                            if (ultimoEventoDescongelamiento != null &&
                                ultimoEventoDescongelamiento.Mensaje.Codigo == Convert.ToInt32(MessageIdentifier.TemperatureThawingButtonPressed).ToString())
                                label.Text = @"On";
                            else
                                label.Text = @"Off";
                            divValor.Controls.Add(label);
                        }
                        else
                        {
                            label.Text = @"-";
                            divValor.Controls.Add(label);

                            //EVALUO CONEXION
                            label = new Label { Text = @" Energía: " };
                            divValor.Controls.Add(label);
                            label = new Label { Text = @"-", Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };
                            divValor.Controls.Add(label);
                            //EVALUO DESCONGELAMIENTO
                            label = new Label { Text = @" Desc: " };
                            divValor.Controls.Add(label);
                            label = new Label { Text = @"-", Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };
                            divValor.Controls.Add(label);
                        }
                        break;
                    case "EST":
                        if (ultimaMedicion != null)
                        {
                            fechaUltimaMedicion = ultimaMedicion.FechaMedicion > fechaUltimaMedicion
                                                      ? ultimaMedicion.FechaMedicion
                                                      : fechaUltimaMedicion;


                            //EVALUO CHECK
                            var ultimoCheckPoint =
                                DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                  new[] { Convert.ToInt32(MessageIdentifier.CheckpointReached).ToString() });

                            if (ultimoCheckPoint != null)
                            {
                                label = new Label { Text = @"Último Control: ", ForeColor = Color.FromArgb(66, 66, 66) };
                                divValor.Controls.Add(label);
                                label = new Label
                                            {
                                                Height = Unit.Pixel(10),
                                                ForeColor = Color.FromArgb(66, 66, 66),
                                                Text = ultimoCheckPoint.Fecha.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm")
                                            };
                                divValor.Controls.Add(label);
                            }
                            else
                            {
                                //EVALUO 911
                                label = new Label { Text = @" 911: ", ForeColor = Color.FromArgb(66, 66, 66) };
                                divValor.Controls.Add(label);
                                label = new Label { Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };

                                var ultimoEvento911 =
                                    DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                      new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton1).ToString() });

                                if (ultimoEvento911 != null &&
                                    ultimoEvento911.Fecha > DateTime.UtcNow.AddMinutes(-10))
                                    label.Text = @"On";
                                else
                                    label.Text = @"Off";
                                divValor.Controls.Add(label);

                                //EVALUO AMBULANCIA
                                label = new Label { Text = @" Amb: ", ForeColor = Color.FromArgb(66, 66, 66) };
                                divValor.Controls.Add(label);
                                label = new Label { Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };

                                var ultimoEventoAmbulancia =
                                    DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                      new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton2).ToString() });

                                if (ultimoEventoAmbulancia != null &&
                                    ultimoEventoAmbulancia.Fecha > DateTime.UtcNow.AddMinutes(-10))
                                    label.Text = @"On";
                                else
                                    label.Text = @"Off";
                                divValor.Controls.Add(label);

                                //EVALUO BOMBEROS
                                label = new Label { Text = @" Bomb: ", ForeColor = Color.FromArgb(66, 66, 66) };
                                divValor.Controls.Add(label);
                                label = new Label { Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };

                                var ultimoEventoBombero =
                                    DAOFactory.LogEventoDAO.GetLastBySensoresAndCodes(new[] { subEntidad.Sensor.Id },
                                                                                      new[] { Convert.ToInt32(MessageIdentifier.KeyboardButton3).ToString() });

                                if (ultimoEventoBombero != null &&
                                    ultimoEventoBombero.Fecha > DateTime.UtcNow.AddMinutes(-10))
                                    label.Text = @"On";
                                else
                                    label.Text = @"Off";
                                divValor.Controls.Add(label);
                            }
                        }
                        else
                        {
                            label = new Label { Text = @" 911: - Amb: - Bomb: -", Height = Unit.Pixel(10), ForeColor = Color.FromArgb(66, 66, 66) };
                            divValor.Controls.Add(label);
                        }
                        break;
                    case "NU":
                        if (ultimaMedicion != null)
                        {
                            fechaUltimaMedicion = ultimaMedicion.FechaMedicion > fechaUltimaMedicion
                                                      ? ultimaMedicion.FechaMedicion
                                                      : fechaUltimaMedicion;

                            label.Text = ultimaMedicion.Valor;
                            divValor.Controls.Add(label);
                        }
                        else
                        {
                            label.Text = @"-";
                            divValor.Controls.Add(label);
                        }
                        break;
                    case "TIME":
                        if (ultimaMedicion != null)
                        {
                            fechaUltimaMedicion = ultimaMedicion.FechaMedicion > fechaUltimaMedicion
                                                      ? ultimaMedicion.FechaMedicion
                                                      : fechaUltimaMedicion;

                            var time = new TimeSpan(0, 0, 0, int.Parse(ultimaMedicion.Valor));
                            label.Text = time.ToString();
                            divValor.Controls.Add(label);
                        }
                        else
                        {
                            label.Text = @"-";
                            divValor.Controls.Add(label);
                        }
                        break;
                    default:
                        if (ultimaMedicion != null)
                        {
                            fechaUltimaMedicion = ultimaMedicion.FechaMedicion > fechaUltimaMedicion
                                                      ? ultimaMedicion.FechaMedicion
                                                      : fechaUltimaMedicion;

                            label.Text = ultimaMedicion.Valor;
                            divValor.Controls.Add(label);
                        }
                        else
                        {
                            label.Text = @"-";
                            divValor.Controls.Add(label);
                        }
                        break;
                }

                label = new Label { Text = @"<br/>" };
                divValor.Controls.Add(label);
            }
               
            lblFechaPosicion.Text = fechaUltimaMedicion != DateTime.MinValue ? fechaUltimaMedicion.ToDisplayDateTime().ToString("dddd dd \"de\" MMMM \"de\" yyyy HH:mm:ss") : "-";
        
            try
            {
                lblPosicion.Text = GeocoderHelper.GetDescripcionEsquinaMasCercana(entidad.ReferenciaGeografica.Latitude, entidad.ReferenciaGeografica.Longitude);
            }
            catch
            {
                lblPosicion.Text = @"Posición erronea.";
            }
        }

        protected void LnkSubEntidadesOnClick(object sender, EventArgs e)
        {
            this.RegisterStartupJScript("MonitorSubEntidades", "window.open('../../Monitor/MonitorDeSubEntidades/MonitorSubEntidades.aspx?ID_ENTIDAD=" + IdEntidad + "','MonitorSubEntidades');");
        }
    }
}
