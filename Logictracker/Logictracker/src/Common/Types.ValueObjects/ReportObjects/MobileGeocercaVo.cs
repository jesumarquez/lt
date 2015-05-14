#region Usings

using System;
using Logictracker.Types.ReportObjects;

#endregion

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class MobileGeocercaVo
    {
        public const int KeyIndexIdMovil = 0;

        public const int IndexInterno = 0;
        public const int IndexGeocerca = 1;
        public const int IndexTipoGeocerca = 2;
        public const int IndexEntrada = 3;
        public const int IndexSalida = 4;
        public const int IndexDuracion = 5;
        public const int IndexProximaGeocerca = 6;
        public const int IndexRecorrido = 7;

        [GridMapping(Index = IndexInterno, ResourceName = "Entities", VariableName = "PARENTI03", IsInitialGroup = true)]
        public string Interno { get; set; }

        [GridMapping(Index = IndexGeocerca, ResourceName = "Labels", VariableName = "GEOCERCA")]
        public string Geocerca { get; set; }

        [GridMapping(Index = IndexTipoGeocerca, ResourceName = "Labels", VariableName = "TYPE")]
        public string TipoGeocerca { get; set; }

        [GridMapping(Index = IndexEntrada, ResourceName = "Labels", VariableName = "INICIO", AllowGroup = false, InitialSortExpression = true)]
        public string Entrada { get; set; }

        [GridMapping(Index = IndexSalida, ResourceName = "Labels", VariableName = "FIN", AllowGroup = false)]
        public string Salida { get; set; }

        [GridMapping(Index = IndexDuracion, ResourceName = "Labels", VariableName = "DURACION", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public TimeSpan Duracion { get; set; }

        [GridMapping(Index = IndexProximaGeocerca, ResourceName = "Labels", VariableName = "A_PROXIMA_GEOCERCA", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public TimeSpan ProximaGeocerca { get; set; }


        [GridMapping(Index = IndexRecorrido, ResourceName = "Labels", VariableName = "RECORRIDO", AllowGroup = false, IsAggregate = true, DataFormatString = "{0: 0.00}" ,AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 0.00}")]
        public double Recorrido { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdMovil { get; set; }

        public MobileGeocercaVo(MobileGeocerca mobileGeocerca)
        {
            Interno = mobileGeocerca.Interno + " - " + mobileGeocerca.Patente;
            Geocerca = mobileGeocerca.Geocerca;
            TipoGeocerca = mobileGeocerca.TipoGeocerca;
            Entrada = mobileGeocerca.Entrada.Equals(DateTime.MinValue)
                          ? string.Empty
                          : string.Format("{0} {1}", mobileGeocerca.Entrada.ToShortDateString(),
                                          mobileGeocerca.Entrada.TimeOfDay);
            Salida = mobileGeocerca.Salida.Equals(DateTime.MinValue) ? string.Empty
                : string.Format("{0} {1}", mobileGeocerca.Salida.ToShortDateString(), mobileGeocerca.Salida.TimeOfDay);
            Duracion = mobileGeocerca.Duracion.Equals(TimeSpan.MinValue) ? new TimeSpan() : mobileGeocerca.Duracion;
            ProximaGeocerca = mobileGeocerca.ProximaGeocerca.Equals(TimeSpan.MinValue) ||
                              mobileGeocerca.ProximaGeocerca < TimeSpan.Zero
                                  ? new TimeSpan()
                                  : mobileGeocerca.ProximaGeocerca;
            Recorrido = ProximaGeocerca.TotalSeconds > 0.0
                            ? mobileGeocerca.Recorrido
                            : 0.0;
            IdMovil = mobileGeocerca.IdMovil;
        }
    }
}
