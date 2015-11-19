namespace Logictracker.Tracker.Application.WebServiceConsumer
{
    public class VehicleDataSos
    {
        public VehicleDataSos(string patente, string color, string marca)
        {
            Patente = patente;
            Color = color;
            Marca = marca;
        }

        public string Patente { get; set; }
        public string Color { get; set; }
        public string Marca { get; set; }
    }
}