
using System;
using Urbetrack.Postal.Enums;

namespace Urbetrack.Postal.DataSources
{
    public class ServicioView
    {
        public ServicioView(Ruta s)
        {
            Selected = s.Selected;
            Faltante = s.Faltantes;
            TipoServicio = s.TipoServicioDescCorta; 
            Direccion = s.Direccion;
            Estado = s.Estado;
            Id = s.Id;
        }

        public int Id { get; set; }
        public bool Selected { get; set; }
        public string Faltante { get; set; }
        public string TipoServicio { get; set; }
        public string Direccion { get; set; }
        public EstadoServicio Estado { get; set; }
    }
}
