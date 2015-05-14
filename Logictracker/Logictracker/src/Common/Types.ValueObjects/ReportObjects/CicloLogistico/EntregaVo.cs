using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.Culture;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico
{
    [Serializable]
    public class EntregaVo
    {
        public const int IndexVehiculo = 0;
        public const int IndexViaje = 1;
        public const int IndexFecha = 2;
        public const int IndexOrden = 3;
        public const int IndexOrdenReal = 4;
        public const int IndexEntrega = 5;
        public const int IndexPuntoEntrega = 6;
        public const int IndexCantidadEntregas = 7;
        public const int IndexEstado = 8;
        public const int IndexKmRecorridos = 9;
        public const int IndexTiempoRecorrido = 10;
        public const int IndexTiempoEntrega = 11;
        public const int IndexHorarioEntrada = 12;
        public const int IndexHorarioSalida= 13;
        public const int IndexHorarioManual = 14;
        public const int IndexProgramado = 15;
        public const int IndexDesvio = 16;
        public const int IndexTieneFoto = 17;

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, AllowMove = true, IsInitialGroup = true)]
        public string Vehiculo { get; set; }
        
        [GridMapping(Index = IndexViaje, ResourceName = "Labels", VariableName = "VIAJE", AllowGroup = true, AllowMove = true, IsInitialGroup = true)]
        public string Viaje { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", IsAggregate = false, AllowGroup = false)]
        public string Fecha { get; set; }

        [GridMapping(Index = IndexOrden, ResourceName = "Labels", VariableName = "ORDEN", IsAggregate = false, AllowGroup = false, InitialSortExpression = true)]
        public int Orden { get; set; }

        [GridMapping(Index = IndexOrdenReal, ResourceName = "Labels", VariableName = "ORDEN_REAL", IsAggregate = false, AllowGroup = false, InitialSortExpression = true)]
        public int OrdenReal { get; set; }

        [GridMapping(Index = IndexEntrega, ResourceName = "Labels", VariableName = "ENTREGA", AllowGroup = false, IncludeInSearch = true)]
        public string Entrega { get; set; }

        [GridMapping(Index = IndexPuntoEntrega, ResourceName = "Entities", VariableName = "PARENTI44", AllowGroup = false)]
        public string PuntoEntrega { get; set; }

        [GridMapping(Index = IndexCantidadEntregas, ResourceName = "Labels", VariableName = "ENTREGAS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 0}")]
        public int Entregas { get; set; }

        [GridMapping(Index = IndexEstado, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = true, IncludeInSearch = true)]
        public string Estado { get; set; }

        [GridMapping(Index = IndexKmRecorridos, ResourceName = "Labels", VariableName = "KMS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 0.0}", DataFormatString = "{0: 0.0}")]
        public double Kms { get; set; }

        [GridMapping(Index = IndexTiempoRecorrido, ResourceName = "Labels", VariableName = "RECORRIDO", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0: 00:00:00}", DataFormatString = "{0: 00:00:00}")]
        public TimeSpan TiempoRecorrido { get; set; }

        [GridMapping(Index = IndexTiempoEntrega, ResourceName = "Labels", VariableName = "TIEMPO_ENTREGA", IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}", DataFormatString = "{0: 00:00:00}")]
        public TimeSpan TiempoEntrega { get; set; }

        [GridMapping(Index = IndexHorarioEntrada, ResourceName = "Labels", VariableName = "ENTRADA")]
        public string Entrada { get; set; }

        [GridMapping(Index = IndexHorarioSalida, ResourceName = "Labels", VariableName = "SALIDA")]
        public string Salida { get; set; }

        [GridMapping(Index = IndexHorarioManual, ResourceName = "Labels", VariableName = "MANUAL")]
        public string Manual { get; set; }

        [GridMapping(Index = IndexProgramado, ResourceName = "Labels", VariableName = "PROGRAMADO")]
        public string Programado { get; set; }

        [GridMapping(Index = IndexDesvio, ResourceName = "Labels", VariableName = "DESVIO", IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: 00:00:00}")]
        public TimeSpan Desvio { get; set; }

        [GridMapping(Index = IndexTieneFoto, HeaderText = "", IsTemplate = true)]
        public bool TieneFoto { get; set; }

        public int Id { get; set; }
        public int IdViaje { get; set; }
        public int IdDispositivo { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }

        public EntregaVo(int orden, EntregaDistribucion entregaDistribucion, TimeSpan tiempoRecorrido, double kms, DateTime? desde)
        {
            Id = entregaDistribucion.Id;
            IdViaje = entregaDistribucion.Viaje.Id;
            IdDispositivo = entregaDistribucion.Viaje.Vehiculo != null && entregaDistribucion.Viaje.Vehiculo.Dispositivo != null
                                ? entregaDistribucion.Viaje.Vehiculo.Dispositivo.Id
                                : 0;
            Vehiculo = entregaDistribucion.Viaje.Vehiculo != null ? entregaDistribucion.Viaje.Vehiculo.Interno : string.Empty;
            Viaje = entregaDistribucion.Viaje.Codigo;
            Fecha = entregaDistribucion.Viaje.Inicio.ToDisplayDateTime().ToString("dd/MM/yyyy");
            Orden = entregaDistribucion.Orden;
            OrdenReal = orden;
            Entrega = entregaDistribucion.Descripcion;
            PuntoEntrega = entregaDistribucion.PuntoEntrega != null ? entregaDistribucion.PuntoEntrega.Nombre : string.Empty;
            Entregas = entregaDistribucion.Linea != null ? 0 : 1;
            Estado = CultureManager.GetLabel(EntregaDistribucion.Estados.GetLabelVariableName(entregaDistribucion.Estado));
            TiempoRecorrido = tiempoRecorrido;
            var tiempoEntrega = entregaDistribucion.Entrada.HasValue && entregaDistribucion.Salida.HasValue
                                ? entregaDistribucion.Salida.Value.Subtract(entregaDistribucion.Entrada.Value)
                                : new TimeSpan(0);
            TiempoEntrega = tiempoEntrega.TotalSeconds >= 0 ? tiempoEntrega : new TimeSpan(0);
            Entrada = entregaDistribucion.Entrada.HasValue ? entregaDistribucion.Entrada.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty;
            Salida = entregaDistribucion.Salida.HasValue ? entregaDistribucion.Salida.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty;
            Manual = entregaDistribucion.Manual.HasValue ? entregaDistribucion.Manual.Value.ToDisplayDateTime().ToString("HH:mm") : string.Empty;
            Programado = entregaDistribucion.Programado.ToDisplayDateTime().ToString("HH:mm");
            Desvio = entregaDistribucion.Entrada.HasValue ? entregaDistribucion.Entrada.Value.Subtract(entregaDistribucion.Programado) : new TimeSpan(0,0,0);
            Kms = kms;
            
            if (entregaDistribucion.Manual.HasValue && entregaDistribucion.Viaje.Vehiculo != null)
            {
                Hasta = entregaDistribucion.Manual.Value;
                if (entregaDistribucion.Orden > 0 && desde.HasValue)
                {
                    Desde = desde.Value;
                    var dao = new DAOFactory();
                    var reportDao = new ReportFactory(dao);

                    var maxMonths = entregaDistribucion.Viaje.Vehiculo.Empresa != null ? entregaDistribucion.Viaje.Vehiculo.Empresa.MesesConsultaPosiciones : 3;
                    var eventos = reportDao.MobileEventDAO.GetMobilesEvents(new List<int> { entregaDistribucion.Viaje.Vehiculo.Id },
                                                                            new[] { 514 },
                                                                            new List<int> { 0 },
                                                                            Desde,
                                                                            Hasta,
                                                                            maxMonths);

                    TieneFoto = eventos.Any();
                }
            }
        }
    }
}
