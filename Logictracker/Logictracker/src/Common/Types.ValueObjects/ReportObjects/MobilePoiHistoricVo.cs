using System;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobilePoiHistoricVo
    {
        public const int IndexInterno = 0;
        public const int IndexTipoVehiculo = 1;
        public const int IndexChofer = 2;
        public const int IndexResponsable = 3;
        public const int IndexFecha = 4;
        public const int IndexDistancia = 5;
        public const int IndexEsquina = 6;

        [GridMapping(Index = IndexInterno, ResourceName = "Entities", VariableName = "PARENTI03")]
        public string Interno { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17")]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexChofer, ResourceName = "Labels", VariableName = "CHOFER")]
        public string Chofer { get; set; }

        [GridMapping(Index = IndexResponsable, ResourceName = "Labels", VariableName = "RESPONSABLE")]
        public string Responsable { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:G}", AllowGroup = false)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexDistancia, ResourceName = "Labels", VariableName = "DISTANCIA", InitialSortExpression = true, AllowGroup = false)]
        public double Distancia { get; set; }

        [GridMapping(Index = IndexEsquina, ResourceName = "Labels", VariableName = "ESQUINA", AllowGroup = false)]
        public string Esquina { get; set; }

        public int Velocidad { get; set; }

        public MobilePoiHistoricVo(MobilePoiHistoric mobilePoiHistoric)
        {
            Interno = mobilePoiHistoric.Interno;
            TipoVehiculo = mobilePoiHistoric.TipoVehiculo;
            Chofer = mobilePoiHistoric.Chofer;
            Responsable = mobilePoiHistoric.Responsable;
            Fecha = mobilePoiHistoric.Fecha.ToDisplayDateTime();
            Distancia = mobilePoiHistoric.Distancia;
            Esquina = mobilePoiHistoric.Esquina;
            Velocidad = mobilePoiHistoric.Velocidad;
        }
    }
}
