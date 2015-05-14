using System;
using Urbetrack.Postal.Enums;

namespace Urbetrack.Postal
{
    public class Ruta
    {
        public int Id { get; set; }
        public int TipoServicio { get; set; }
        public String TipoServicioDesc { get; set; }
        public String TipoServicioDescCorta { get; set; }
        public int Cliente { get; set; }
        public string ClienteDesc { get; set; }
        public String Destinatario { get; set; }
        public String Direccion { get; set; }
        public String Pieza { get; set; }       
        public EstadoServicio Estado { get; set; }
        public Motivo Motivo { get; set; }
        public string Lateral1 { get; set; }
        public string Lateral2 { get; set; }
        public string Referencia { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public byte[] Foto { get; set; }
        public TipoValidacion Confirma { get; set; }
        public TipoValidacion ConFoto { get; set; }
        public bool TieneFoto { get; set; }
        public bool LlevaFoto { get; set; }
        public TipoValidacion ConLaterales { get; set; }
        public bool TieneLaterales { get; set; }
        public bool LlevaLaterales { get; set; }
        public TipoValidacion ConReferencia { get; set; }
        public bool TieneReferencia { get; set; }
        public bool LlevaReferencia { get; set; }
        public TipoValidacion ConGPS { get; set; }
        public bool TieneGPS { get; set; }
        public bool LlevaGPS { get; set; }
        public bool Selected { get; set; }

        public string Faltantes { get
        {
            CalcularEstados();

            string s = "";
            
            if (Motivo==null)
            {
                s = "-";
            }
            else
            {
                if (LlevaLaterales && !TieneLaterales) s = String.Concat(s, "L");
                if (LlevaReferencia && !TieneReferencia) s = String.Concat(s, "R");
                if (LlevaGPS && !TieneGPS) s = String.Concat(s, "G");
                if (LlevaFoto && !TieneFoto) s = String.Concat(s, "F");
            }
            return s;
        }}

        private bool filledString(String s)
        {
            if (s == null) return true;
            if (s != "") return true;
            return false;
        }
        
        public void CalcularEstados()
        {
            if (Motivo == null)
            {
                Estado = EstadoServicio.Pendiente;
            }
            else
            {
                Estado = EstadoServicio.EnCurso;

                LlevaLaterales = ConLaterales.Validate(Motivo.EsEntrega);
                TieneLaterales = filledString(Lateral1) && filledString(Lateral2);

                LlevaReferencia = ConReferencia.Validate(Motivo.EsEntrega);
                TieneReferencia = filledString(Referencia);

                LlevaGPS = ConGPS.Validate(Motivo.EsEntrega);
                TieneGPS = Longitud.HasValue && Latitud.HasValue;

                LlevaFoto = ConFoto.Validate(Motivo.EsEntrega);
                TieneFoto = Foto.Length > 0;

                var laterales = (LlevaLaterales && TieneLaterales) || !LlevaLaterales;
                var referencia = (LlevaReferencia && TieneReferencia) || !LlevaReferencia;
                var gps = (LlevaGPS && TieneGPS) || !LlevaGPS;
                var foto = (LlevaFoto && TieneFoto) || !LlevaFoto;

                if (laterales && referencia && gps && foto) Estado = EstadoServicio.Terminado;
            }
        }
    }
}
