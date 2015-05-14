using System;
using System.Globalization;
using Compumap.Controls.Layers;

namespace HandlerTest
{
    public partial class Tester
    {
        private Markers Layer;

        public double Latitud
        {
            get { double d; return double.TryParse(txtLatitud.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out d) ? d : 0; }
            set { txtLatitud.Text = value.ToString(); }
        }
        public double Longitud
        {
            get { double d; return double.TryParse(txtLongitud.Text.Trim().Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out d) ? d : 0; }
            set { txtLongitud.Text = value.ToString(); }
        }

        private void InitializeMap()
        {
            mapControl1.AddLayers(Layer = new Markers("point"));
        }

        void mapControl1_Click(object sender, Compumap.Controls.MapMouseEventArgs e)
        {
            Latitud = e.Latitude;
            Longitud = e.Longitude;
            UpdateMarker();
        }

        private void txtLatLon_TextChanged(object sender, EventArgs e)
        {
            UpdateMarker();
        }

        private void UpdateMarker()
        {
            var lat = Latitud;
            var lon = Longitud;
            mapControl1.ClearFeatures(Layer);
            mapControl1.AddMarker(Layer, lat, lon);

        }
        private void GotoCurrent()
        {
            var pos = GetLastPosition(Dispositivo);
            if(pos == null) return;
            Latitud = pos.Latitud;
            Longitud = pos.Longitud;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            GotoCurrent();
        } 
    }
}
