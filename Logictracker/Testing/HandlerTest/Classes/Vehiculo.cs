using Logictracker.Types.BusinessObjects.Vehiculos;

namespace HandlerTest.Classes
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public bool Selected { get; set; }
        public string Empresa { get; set; }
        public string Linea { get; set; }
        public string Interno { get; set; }
        public string Dispositivo { get; set; }
        public Vehiculo(Coche coche)
        {
            Id = coche.Id;
            Empresa = coche.Empresa != null ? coche.Empresa.RazonSocial : string.Empty;
            Linea = coche.Linea != null ? coche.Linea.Descripcion : string.Empty;
            Interno = coche.Interno;
            Dispositivo = coche.Dispositivo.Codigo;
        }
    }
}
