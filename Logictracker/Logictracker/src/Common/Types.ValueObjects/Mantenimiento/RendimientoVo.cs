using System;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class RendimientoVo
    {
        public const int IndexTipoVehiculo = 0;
        public const int IndexVehiculo = 1;
        public const int IndexChofer = 2;
        public const int IndexConsumo = 3;
        public const int IndexDespacho = 4;
        public const int IndexHorasMarcha = 5;
        public const int IndexKmRecorridos = 6;
        public const int IndexConsumoXKm = 7;
        public const int IndexDespachoXKm = 8;
        public const int IndexViajes = 9;
        public const int IndexConsumoXViaje = 10;
        public const int IndexDespachoXViaje = 11;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true, IsInitialGroup = true, IncludeInSearch = true)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexChofer, ResourceName = "Entities", VariableName = "PARENTI09", IncludeInSearch = true)]
        public string Chofer { get; set; }

        [GridMapping(Index = IndexConsumo, ResourceName = "Labels", VariableName = "LITROS_CONS", DataFormatString = "{0:#0.00}")]
        public double Consumo { get; set; }

        [GridMapping(Index = IndexDespacho, ResourceName = "Labels", VariableName = "LITROS_DESPACHADOS", DataFormatString = "{0:#0.00}")]
        public double Despacho { get; set; }

        [GridMapping(Index = IndexHorasMarcha, ResourceName = "Labels", VariableName = "HS_MARCHA")]
        public string HsMarcha { get; set; }

        [GridMapping(Index = IndexKmRecorridos, ResourceName = "Labels", VariableName = "KM", DataFormatString = "{0:#0.00}")]
        public double Km { get; set; }

        [GridMapping(Index = IndexConsumoXKm, ResourceName = "Labels", VariableName = "CONSUMO_X_KM", DataFormatString = "{0:#0.00}")]
        public double ConsumoXKm { get; set; }

        [GridMapping(Index = IndexDespachoXKm, ResourceName = "Labels", VariableName = "DESPACHO_X_KM", DataFormatString = "{0:#0.00}")]
        public double DespachoXKm { get; set; }

        [GridMapping(Index = IndexViajes, ResourceName = "Labels", VariableName = "CANTIDAD_VIAJES", DataFormatString = "{0:#0}")]
        public int Viajes { get; set; }

        [GridMapping(Index = IndexConsumoXViaje, ResourceName = "Labels", VariableName = "CONSUMO_X_VIAJE", DataFormatString = "{0:#0.00}")]
        public double ConsumoXViaje { get; set; }

        [GridMapping(Index = IndexDespachoXViaje, ResourceName = "Labels", VariableName = "DESPACHO_X_VIAJE", DataFormatString = "{0:#0.00}")]
        public double DespachoXViaje { get; set; }
        
        public RendimientoVo(Coche coche, DateTime desde, DateTime hasta, double despachos, int viajes, bool controlaViajes)
        {
            Id = coche.Id;
            TipoVehiculo = coche.TipoCoche != null ? coche.TipoCoche.ToString() : string.Empty;
            Vehiculo = coche.TipoCoche != null && coche.TipoCoche.SeguimientoPersona && coche.Chofer != null
                           ? coche.Chofer.Entidad.Descripcion
                           : coche.Interno;
            Chofer = coche.Chofer != null && coche.Chofer.Entidad != null ? coche.Chofer.Entidad.Descripcion : string.Empty;
            
            var dao = new DAOFactory();
            var datamart = dao.DatamartDAO.GetSummarizedDatamart(desde, hasta, coche.Id);
            
            var hsMarcha = datamart.HsMarcha;
            HsMarcha = TimeSpan.FromHours(hsMarcha).ToString();
            Km = datamart.Kilometros;

            if (coche.Modelo != null)
            {
                var litros = (datamart.Kilometros/100.0)*coche.Modelo.Rendimiento;
                var hsRalenti = datamart.HsMarcha - datamart.HsMovimiento;
                if (hsRalenti < 0.0) hsRalenti = 0.0;

                litros += hsRalenti*coche.Modelo.RendimientoRalenti;
                Consumo = litros;
            }
            else
            {
                Consumo = 0.0;
            }

            ConsumoXKm = Consumo > 0.0 && Km > 0.0 ? (Consumo / Km) : 0.00;

            Despacho = despachos;
            DespachoXKm = Despacho > 0.0 && Km > 0.0 ? (Despacho / Km) : 0.00;

            if (controlaViajes)
            {
                Viajes = viajes;
                ConsumoXViaje = Consumo > 0.0 && Viajes > 0 ? (Consumo / Viajes) : 0.00;
                DespachoXViaje = Despacho > 0.0 && Viajes > 0 ? (Despacho / Viajes) : 0.00;
            }
        }
    }
}
