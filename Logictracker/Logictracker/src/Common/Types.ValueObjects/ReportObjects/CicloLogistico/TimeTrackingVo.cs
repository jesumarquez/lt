using System;
using System.Globalization;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.TimeTracking;

namespace Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico
{
    [Serializable]
    public class TimeTrackingVo
    {
        public static class Index
        {
            public const int Viaje = 0;
            public const int Tipo = 1;
            public const int Vehiculo = 2;
            public const int Empleado = 3;
            public const int ReferenciaGeografica = 4;
            public const int Fecha = 5;
            public const int Duracion = 6;
            public const int TiempoViaje = 7;
            public const int KmRecorridos = 8;
        }
        public static class KeyIndex
        {
            public const int IdEntrada = 0;
            public const int IdSalida = 1;
            public const int FechaDesde = 2;
            public const int FechaHasta = 3;
            public const int IdVehiculo = 4;
        }
        [GridMapping(Index = KeyIndex.IdEntrada, IsDataKey = true)]
        public int IdEntrada { get; set; }

        [GridMapping(Index = KeyIndex.IdSalida, IsDataKey = true)]
        public int IdSalida { get; set; }

        [GridMapping(Index = KeyIndex.FechaDesde, IsDataKey = true)]
        public DateTime FechaDesde { get; set; }

        [GridMapping(Index = KeyIndex.FechaHasta, IsDataKey = true)]
        public DateTime FechaHasta { get; set; }

        [GridMapping(Index = KeyIndex.IdVehiculo, IsDataKey = true)]
        public int IdVehiculo { get; set; }

        [GridMapping(Index = Index.Viaje, ResourceName = "Labels", VariableName = "VIAJE", IsAggregate = false, IsInitialGroup = true)]
        public string Viaje { get; set; }

        [GridMapping(Index = Index.Tipo, IsTemplate = true, Width = "30px", IsAggregate = false, AllowGroup = false, AllowMove = false)]
        public int Tipo { get; set; }

        [GridMapping(Index = Index.Vehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IsAggregate = false)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = Index.Vehiculo, ResourceName = "Entities", VariableName = "PARENTI09", IsAggregate = false)]
        public string Empleado { get; set; }

        [GridMapping(Index = Index.ReferenciaGeografica, ResourceName = "Entities", VariableName = "PARENTI05", IsAggregate = false)]
        public string ReferenciaGeografica { get; set; }

        [GridMapping(Index = Index.Fecha, ResourceName = "Labels", VariableName = "FECHA", IsAggregate = false, AllowGroup = false, InitialSortExpression = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = Index.Duracion, ResourceName = "Labels", VariableName = "TIEMPO_GEOCERCA", IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public TimeSpan Duracion { get; set; }

        [GridMapping(Index = Index.TiempoViaje, ResourceName = "Labels", VariableName = "TIEMPO_VIAJE", IsAggregate = true, AggregateType = GridAggregateType.Sum)]
        public TimeSpan TiempoViaje { get; set; }

        private double _km = -1;

        [GridMapping(Index = Index.KmRecorridos, ResourceName = "Labels", VariableName = "KM_RECORRIDOS", IsAggregate = true, AggregateType = GridAggregateType.Sum, DataFormatString = "{0:0.00}", AggregateTextFormat = "{0:0.00}")]
        public double KmRecorridos
        {
            get
            {
                if (_km.Equals(-1))
                {
                    _km = Anterior != null ? DaoFactory.CocheDAO.GetDistance(IdVehiculo, Anterior.Fecha.AddSeconds(-1), Fecha.Add(Duracion).AddSeconds(1).ToDataBaseDateTime()) : 0;
                }
                return _km;
            }
        }

        protected EventoViaje Anterior { get; set; }
        protected DAOFactory DaoFactory { get; set; }

        public TimeTrackingVo(EventoViaje entrada, EventoViaje salida, EventoViaje anterior, int numeroViaje, DAOFactory daoFactory)
        {
            IdEntrada = entrada != null ? entrada.Id : 0;
            IdSalida = salida != null ? salida.Id : 0;
            Viaje = numeroViaje.ToString(CultureInfo.InvariantCulture);
            Tipo = anterior == null && entrada == null && salida != null && salida.EsInicio ? 0
                : entrada != null && salida == null && entrada.EsFin ? 2
                : 1;
            var evento = entrada ?? salida;
            if (evento == null) return;
            Vehiculo = evento.Vehiculo.Interno;
            Empleado = evento.Empleado != null ? evento.Empleado.Entidad.Descripcion : string.Empty;
            ReferenciaGeografica = evento.ReferenciaGeografica.Descripcion;
          
            Fecha = evento.Fecha.ToDisplayDateTime();
            Duracion = entrada != null && salida != null ? salida.Fecha.Subtract(entrada.Fecha) : TimeSpan.Zero;
            TiempoViaje = entrada != null && anterior != null ? entrada.Fecha.Subtract(anterior.Fecha) : TimeSpan.Zero;
            DaoFactory = daoFactory;
            IdVehiculo = evento.Vehiculo.Id;
            Anterior = anterior;
        }

    }
}
