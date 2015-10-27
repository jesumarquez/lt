using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Geocoder.Core.VO;
using Logictracker.Services.Helpers;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;

namespace Logictracker.App_Controls
{
    public partial class DireccionSearch : UserControl
    {
        public event EventHandler DireccionSelected;

        protected virtual void OnDireccionSelected()
        {
            if (DireccionSelected != null) DireccionSelected(this, EventArgs.Empty);
        }

        private IList<DireccionVO> Direcciones
        {
            get { return (IList<DireccionVO>)(ViewState["Direcciones"] ?? new List<DireccionVO>()); }
            set { ViewState["Direcciones"] = value; }
        }

        public Direccion Selected
        {
            get { return ViewState["Selected"] as Direccion; }
            private set { ViewState["Selected"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProvincias();
                SetDireccion(Selected);
            }
        }

        private void BindProvincias()
        {
            var provincias = GeocoderHelper.BuscarProvincias();

            cbProvincia.DataSource = provincias;
            cbProvincia.DataBind();
        }

        protected void BtSmartSearchClick(object sender, EventArgs e)
        {
            var frase = txtSmartSearch.Text.Trim();
            if(string.IsNullOrEmpty(frase)) return;
            var result = GeocoderHelper.GetDireccionSmartSearch(frase);
            SetResults(result);
        }

        protected void BtNormalSearchClick(object sender, EventArgs e)
        {
            var calle = txtCalle.Text.Trim();
            int altura;

            if (!int.TryParse(txtAltura.Text.Trim(), out altura)) altura = -1;

            var esquina = txtEsquina.Text.Trim();
            var partido = txtPartido.Text.Trim();
            var provincia = Convert.ToInt32(cbProvincia.SelectedValue);

            if (string.IsNullOrEmpty(calle)) return;
            if (string.IsNullOrEmpty(esquina) && altura < 0) return;

            var result = GeocoderHelper.GetDireccion(calle, altura, esquina, partido, provincia);

            SetResults(result);
        }

        protected void BtLatLonClick(object sender, EventArgs e)
        {
            double lat, lon;

            if (!double.TryParse(txtLatitud.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lat)
                || !double.TryParse(txtLongitud.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lon)) return;

            FindByLatLon(lat, lon);
        }

        public void FindByLatLon(double latitud, double longitud)
        {
            var result = GeocoderHelper.GetEsquinaMasCercana(latitud, longitud) 
                         ?? new DireccionVO
                                {
                                    Altura = -1,
                                    Calle = string.Empty,
                                    Direccion = string.Format("({0}, {1})", latitud, longitud),
                                    IdEsquina = -1,
                                    IdMapaUrbano = -1,
                                    IdPoligonal = -1,
                                    IdProvincia = -1,
                                    Partido = string.Empty,
                                    Provincia = string.Empty,
                                };

            result.Latitud = latitud;
            result.Longitud = longitud;
            SetDireccion(DireccionFromVo(result));

            updControl.Update();
        }

        private void SetResults(IList<DireccionVO> result)
        {
            if (result.Count == 0) return;

            if (result.Count == 1) SetDireccion(DireccionFromVo(result[0]));
            else
            {
                Direcciones = result;
                cbResults.Items.Clear();

                foreach (var vo in result) cbResults.Items.Add(vo.Direccion);

                SetResultView(true);
                SwitchView(4);
            }
        }

        protected void BtAceptarClick(object sender, EventArgs e)
        {
            if (cbResults.SelectedIndex < 0) return;

            SetDireccion(DireccionFromVo(Direcciones[cbResults.SelectedIndex]));
            Direcciones = null;
        }

        protected void BtCancelarClick(object sender, EventArgs e)
        {
            SetResultView(false);
            Direcciones = null;
        }

        public void SetDireccion(Direccion direccion)
        {
            Selected = direccion;

            if(direccion != null)
            {
                lblDireccion.Text = direccion.Descripcion;
                lblLatitud.Text = string.Format("Latitud: {0}", direccion.Latitud);
                lblLongitud.Text = string.Format("Longitud: {0}", direccion.Longitud);
                SwitchView(0);
            }
            else
            {
                lblDireccion.Text = "No se ha seleccionado Dirección";
                lblLatitud.Text = string.Empty;
                lblLongitud.Text = string.Empty;
                SwitchView(0);
            }

            SetResultView(false);

            OnDireccionSelected();
        }

        private void SetResultView(bool state)
        {
            tab.ActiveTabIndex = 0;

            tabNormalSearch.Enabled = tabSmartSearch.Enabled = tabLatLon.Enabled = tabDireccion.Enabled = !state;
            tabResult.Enabled = state;
            
            updControl.Update();
        }

        private static Direccion DireccionFromVo(DireccionVO direccion)
        {
            var dir = new Direccion
                          {
                              Altura = direccion.Altura,
                              Calle = direccion.Calle,
                              Descripcion = direccion.Direccion,
                              IdCalle = direccion.IdPoligonal,
                              IdEntrecalle = (-1),
                              IdEsquina = direccion.IdEsquina,
                              IdMapa = ((short) direccion.IdMapaUrbano),
                              Latitud = direccion.Latitud,
                              Longitud = direccion.Longitud,
                              Pais = "Argentina",
                              Partido = direccion.Partido,
                              Provincia = direccion.Provincia,
                              Vigencia = new Vigencia()
                          };
            return dir;
        }

        public Unit Width
        {
            get { return panelControl.Width; }
            set { panelControl.Width = tab.Width =  value; }
        }

        public Unit Height
        {
            get { return panelControl.Height; }
            set { panelControl.Height = tab.Height = value; }
        }

        private void SwitchView(int idx) { tab.ActiveTabIndex = idx; }
    }
}
