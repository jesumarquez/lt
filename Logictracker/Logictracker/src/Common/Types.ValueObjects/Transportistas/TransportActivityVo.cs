#region Usings

using System;
using Logictracker.Types.ReportObjects;

#endregion

namespace Logictracker.Types.ValueObjects.Transportistas
{
    [Serializable]
    public class TransportActivityVo
    {

        public const int IndexTransport = 0;
        public const int IndexCentroDeCostos = 1;
        public const int IndexMovil = 2;
        public const int IndexTipoVehiculo = 3;
        public const int IndexCantidadViajes = 4;
        public const int IndexRecorrido = 5;
        public const int IndexHorasActivo = 6;
        public const int IndexHorasDetenido = 7;
        public const int IndexInfracciones = 8;
        public const int IndexHorasInfraccion = 9;
        public const int IndexVelocidadPromedio = 10;
        public const int IndexVelocidadMaxima = 11;

        [GridMapping(Index = IndexTransport, ResourceName = "Entities", VariableName = "PARENTI07", AllowGroup = true, IsInitialGroup = true)]
        public string Transport { get; set; }

        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true)]
        public string CentroDeCostos { get; set; }

        [GridMapping(Index = IndexMovil, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = false, InitialSortExpression = true, IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "Vehiculos: {0}")]
        public string Movil { get; set; }

        [GridMapping(Index = IndexTipoVehiculo, ResourceName = "Entities", VariableName = "PARENTI17", AllowGroup = true)]
        public string TipoVehiculo { get; set; }

        [GridMapping(Index = IndexCantidadViajes, ResourceName = "Labels", VariableName = "VIAJES_REALIZADOS", AllowGroup = false)]
        public int CantidadViajes { get; set; }

        [GridMapping(Index = IndexRecorrido, ResourceName = "Labels", VariableName = "KILOMETROS", DataFormatString = "{0:2} km", AllowGroup = false, IsAggregate = true, AggregateTextFormat = "Kilometros: {0:0.00}")]
        public double Recorrido { get; set; }

        [GridMapping(Index = IndexHorasActivo, ResourceName = "Labels", VariableName = "MOVIMIENTO", AllowGroup = false)]
        public string HorasActivo { get; set; }

        [GridMapping(Index = IndexHorasDetenido, ResourceName = "Labels", VariableName = "DETENCION", AllowGroup = false)]
        public string HorasDetenido { get; set; }

        [GridMapping(Index = IndexInfracciones, ResourceName = "Labels", VariableName = "INFRACCIONES", AllowGroup = false)]
        public int Infracciones { get; set; }

        [GridMapping(Index = IndexHorasInfraccion, ResourceName = "Labels", VariableName = "TIEMPO_INFRACCION", AllowGroup = false)]
        public string HorasInfraccion { get; set; }

        [GridMapping(Index = IndexVelocidadPromedio, ResourceName = "Labels", VariableName = "VELOCIDAD_PROMEDIO", DataFormatString = "{0:0} km/h", AllowGroup = false)]
        public double VelocidadPromedio { get; set; }

        [GridMapping(Index = IndexVelocidadMaxima, ResourceName = "Labels", VariableName = "VELOCIDAD_MAXIMA", DataFormatString = "{0:0} km/h", AllowGroup = false)]
        public double VelocidadMaxima { get; set; }

        public TransportActivityVo(TransportActivity t)
        {
            Transport = t.Transport;
            VelocidadMaxima = t.VelocidadMaxima;
            VelocidadPromedio = t.VelocidadPromedio;
            Infracciones = t.Infracciones;
            Movil = t.Movil;
            CentroDeCostos = t.CentroDeCostos;
            TipoVehiculo = t.TipoVehiculo;
            CantidadViajes = t.CantidadViajes;
            Recorrido = t.Recorrido;
            HorasActivo = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasActivo.Days, t.HorasActivo.Hours,t.HorasActivo.Minutes, t.HorasActivo.Seconds);
            HorasDetenido = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasDetenido.Days, t.HorasDetenido.Hours, t.HorasDetenido.Minutes, t.HorasDetenido.Seconds);
            HorasInfraccion = String.Format("Días:{0} - Horas {1}:{2}:{3}", t.HorasInfraccion.Days, t.HorasInfraccion.Hours, t.HorasInfraccion.Minutes, t.HorasInfraccion.Seconds);
        }
    }
}
