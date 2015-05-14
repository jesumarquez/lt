using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobilePoiVo
    {
        public const int IndexPuntoDeInteres = 0;
        public const int IndexInterno = 1;
        public const int IndexTipoVehiculo = 2;
        public const int IndexChofer = 3;
        public const int IndexResponsable = 4;
        public const int IndexDistancia = 5;
        public const int IndexEsquina = 6;

        [GridMapping(Index = IndexPuntoDeInteres, ResourceName = "Labels", VariableName = "GEOCERCA", IsInitialGroup = true)]
        public string PuntoDeInteres { get; set; }

        [GridMapping(Index = IndexInterno, ResourceName = "Entities", VariableName = "PARENTI03")]
        public string Interno { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17")]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexChofer, ResourceName = "Labels", VariableName = "CHOFER")]
        public string Chofer { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE")]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexDistancia, ResourceName = "Labels", VariableName = "DISTANCIA", AllowGroup = false, InitialSortExpression = true)]
        public string Distancia { get; set; }

        [GridMapping(Index = IndexEsquina, ResourceName = "Labels", VariableName = "ESQUINA", AllowGroup = false)]
        public string Esquina { get; set; }

        public int Velocidad { get; set; }

        public MobilePoiVo(MobilePoi mobilePoi)
        {
            Interno = mobilePoi.Interno;
            PuntoDeInteres = mobilePoi.PuntoDeInteres;
            TipoVehiculo = mobilePoi.TipoVehiculo;
            Chofer = mobilePoi.Chofer;
            Responsable = mobilePoi.Responsable;
            Distancia = string.Format("{0:0.00}m",mobilePoi.Distancia);
            Esquina = mobilePoi.Esquina;
            Velocidad = mobilePoi.Velocidad;
        }
    }
}
