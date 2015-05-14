using Geocoder.Core.VO;

namespace Logictracker.Services.Helpers.Types
{
    public class Direccion
    {
        public Direccion()
        {
            IdEsquina = -1;
            Altura = -1;
        }
        public Direccion(DireccionVO direccion)
        {
            IdMapa = (short)direccion.IdMapaUrbano;
            IdPoligonal = direccion.IdPoligonal;
            IdEsquina = direccion.IdEsquina;
            Altura = direccion.Altura;
        }

        public short IdMapa { get; set; }
        public int IdPoligonal { get; set; }
        public int IdEsquina { get; set; }
        public int Altura { get; set; }
    }
}
