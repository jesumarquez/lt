using System;
using System.Collections.Generic;
using System.Globalization;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Tickets;

namespace Logictracker.Types.ValueObjects.ReportObjects.CicloLogistico
{
    [Serializable]
    public class DuracionEstadosVo : IDynamicData
    {
        public const int IndexFecha = 0;
        public const int IndexCodigoPedido = 1;
        public const int IndexCodigo = 2;
        public const int IndexCliente = 3;
        public const int IndexPuntoEntrega = 4;
        public const int IndexVehiculo = 5;
        public const int IndexCantidadViajes = 6;
        public const int IndexCarga = 7;
        public const int IndexCargaReal = 8;
        public const int IndexDiferenciaCarga = 9;
        public const int IndexPorcentajeLlenado = 10;
        public const int IndexDistanciaAObra = 11;
        public const int IndexTiempoEnObra = 12;
        public const int IndexTiempoCiclo = 13;
        public const int IndexBaseOrigen = 14;
        public const int IndexBaseDestino = 15;
        public const int IndexDynamicColumns = 16;

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", IsAggregate = false, AllowGroup = false, InitialSortExpression = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexCodigoPedido, ResourceName = "Entities", VariableName = "PEDIDO", IsAggregate = false)]
        public string CodigoPedido { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", IsAggregate = false)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexCliente, ResourceName = "Entities", VariableName = "CLIENT", IsInitialGroup = true, AllowGroup = true, AllowMove = true)]
        public string Cliente { get; set; }

        [GridMapping(Index = IndexPuntoEntrega, ResourceName = "Entities", VariableName = "PARENTI44", IsInitialGroup = true, AllowGroup = true, AllowMove = true)]
        public string PuntoEntrega { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true, AllowMove = true)]
        public string Vehiculo { get; set; }

        //[GridMapping(Index = IndexTotal, ResourceName = "Labels", VariableName = "TOTAL", AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}")]
        public int Total { get; set; }

        [GridMapping(Index = IndexCarga, ResourceName = "Labels", VariableName = "CANTIDAD_CARGA", DataFormatString = "{0:0.00}", AllowGroup = false, AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}")]
        public double Carga { get; set; }

        [GridMapping(Index = IndexCargaReal, ResourceName = "Labels", VariableName = "CANTIDAD_CARGA_REAL", DataFormatString = "{0:0.00}", AllowGroup = false, AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}")]
        public double CargaReal { get; set; }

        [GridMapping(Index = IndexDiferenciaCarga, ResourceName = "Labels", VariableName = "DIFERENCIA_CARGA", DataFormatString = "{0:0.00}", AllowGroup = false, AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00}")]
        public double DiferenciaCarga { get; set; }

        //[GridMapping(Index = IndexCapacidad, ResourceName = "Labels", VariableName = "CAPACIDAD_CARGA", AllowGroup = false, AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}")]
        public int Capacidad { get; set; }

        [GridMapping(Index = IndexPorcentajeLlenado, ResourceName = "Labels", VariableName = "PORCENTAJE_LLENADO", AllowGroup = false, AllowMove = false, DataFormatString = "{0:0.00}%", IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}%")]
        public double PorcentajeLlenado { get; set; }

        private double _distanciaAObra;
        [GridMapping(Index = IndexDistanciaAObra, ResourceName = "Labels", VariableName = "DISTANCIA_A_OBRA", DataFormatString = "{0:0.00}", AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.AvgNonZero, AggregateTextFormat = "{0:0.00}")]
        public double DistanciaAObra
        {
            get
            {
                if (!_calculated) Calculate();
                return _distanciaAObra;
            }
            set { _distanciaAObra = value; }
        }

        private TimeSpan _tiempoEnObra;
        [GridMapping(Index = IndexTiempoEnObra, ResourceName = "Labels", VariableName = "TIEMPO_EN_OBRA", AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: HH:mm:ss}")]
        public TimeSpan TiempoEnObra
        {
            get
            {
                if (!_calculated) Calculate();
                return _tiempoEnObra;
            }
            set { _tiempoEnObra = value; }
        }

        private double _distanciaTotal;
        //[GridMapping(Index = IndexDistanciaTotal, ResourceName = "Labels", VariableName = "DISTANCIA_TOTAL", DataFormatString = "{0:0.00}", AllowGroup = false, AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0:0.00}")]
        public double DistanciaTotal
        {
            get
            {
                if (!_calculated) Calculate();
                return _distanciaTotal;
            }
            set { _distanciaTotal = value; }
        }

        private TimeSpan _tiempoCiclo;
        [GridMapping(Index = IndexTiempoCiclo, ResourceName = "Labels", VariableName = "PEDIDO_TIEMPO_CICLO", AllowGroup = false, AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Avg, AggregateTextFormat = "{0: HH:mm:ss}")]
        public TimeSpan TiempoCiclo
        {
            get
            {
                if (!_calculated) Calculate();
                return _tiempoCiclo;
            }
            set { _tiempoCiclo = value; }
        }

        [GridMapping(Index = IndexCantidadViajes, ResourceName = "Labels", VariableName = "CANTIDAD_VIAJES", AllowGroup = false, AllowMove = false, IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "{0: 0}")]
        public int CantidadViajes { get; set; }

        [GridMapping(Index = IndexBaseOrigen, ResourceName = "Labels", VariableName = "BASE_ORIGEN", AllowGroup = true)]
        public string BaseOrigen { get; set; }

        [GridMapping(Index = IndexBaseDestino, ResourceName = "Labels", VariableName = "BASE_DESTINO", AllowGroup = true)]
        public string BaseDestino { get; set; }

        private Dictionary<string, object> _dynamicData;
        public Dictionary<string, object> DynamicData
        {
            get
            {
                if (!_calculated) Calculate();
                return _dynamicData;
            }
            set { _dynamicData = value; }
        }

        public int Id { get; set; }

        public int Estado { get; set; }

        private Ticket Ticket { get; set; }
        private bool _calculated;

        public DuracionEstadosVo(Ticket ticket)
        {
            Id = ticket.Id;
            Codigo = ticket.Codigo;
            Cliente = ticket.Cliente.Codigo + " - " + ticket.Cliente.Descripcion;
            PuntoEntrega = ticket.PuntoEntrega.Codigo + " - " + ticket.PuntoEntrega.Descripcion;
            Vehiculo = ticket.Vehiculo != null ? ticket.Vehiculo.Interno : string.Empty;
            CodigoPedido = ticket.Pedido != null ? ticket.Pedido.Codigo : string.Empty;
            Estado = ticket.Estado;
            if (ticket.FechaTicket != null)
                Fecha = ticket.FechaTicket.Value.ToDisplayDateTime();
            CantidadViajes = 1;

            int total;
            double carga, cargaReal = 0;
            int.TryParse(ticket.CantidadPedido, out total);
            double.TryParse(ticket.CantidadCarga.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out carga);
            if (!string.IsNullOrEmpty(ticket.CantidadCargaReal))
                double.TryParse(ticket.CantidadCargaReal.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out cargaReal);
            Total = total;
            Carga = carga;
            CargaReal = cargaReal;
            DiferenciaCarga = carga - cargaReal;

            Capacidad = ticket.Vehiculo != null ? ticket.Vehiculo.Capacidad : 0;
            PorcentajeLlenado = Capacidad > 0 ? Carga * 100 / Capacidad : 100;

            Ticket = ticket;

            BaseOrigen = ticket.Linea != null ? ticket.Linea.Descripcion : string.Empty;
            BaseDestino = ticket.BaseLlegada != null ? ticket.BaseLlegada.Descripcion : string.Empty;
        }

        private void Calculate()
        {
            var daoFactory = new DAOFactory();
            _calculated = true;

            DetalleTicket estadoSalidaPlanta = null;
            DetalleTicket estadoLlegadaObra = null;
            DetalleTicket estadoSalidaObra = null;
            DetalleTicket estadoLlegadaPlanta = null;

            DynamicData = new Dictionary<string, object>(Ticket.Detalles.Count);

            var prev = DateTime.MinValue;
            var prevKey = string.Empty;
            foreach (DetalleTicket detalle in Ticket.Detalles)
            {
                var triggerEvent = detalle.EstadoLogistico.EsPuntoDeControl;
                switch (triggerEvent)
                {
                    case BusinessObjects.Estado.Evento.SaleDePlanta: estadoSalidaPlanta = detalle; break;
                    case BusinessObjects.Estado.Evento.LlegaAObra: estadoLlegadaObra = detalle; break;
                    case BusinessObjects.Estado.Evento.SaleDeObra: estadoSalidaObra = detalle; break;
                    case BusinessObjects.Estado.Evento.LlegaAPlanta: estadoLlegadaPlanta = detalle; break;
                }

                if (prev != DateTime.MinValue)
                {
                    if (detalle.Automatico.HasValue)
                    {
                        DynamicData.Add(prevKey, detalle.Automatico.Value.Subtract(prev));
                        prev = detalle.Automatico.Value;
                        prevKey = detalle.EstadoLogistico.Id.ToString();
                    }
                }
                else
                {
                    prev = Ticket.FechaTicket.Value;
                    prevKey = detalle.EstadoLogistico.Id.ToString();
                }
            }

            if (Ticket.Vehiculo == null) return;

            if (HasValue(estadoSalidaPlanta) && HasValue(estadoLlegadaObra))
                DistanciaAObra = daoFactory.CocheDAO.GetDistance(Ticket.Vehiculo.Id,
                                                                 estadoSalidaPlanta.Automatico.Value,
                                                                 estadoLlegadaObra.Automatico.Value);
            else
                if (HasValue(estadoSalidaObra) && HasValue(estadoLlegadaPlanta))
                    DistanciaAObra = daoFactory.CocheDAO.GetDistance(Ticket.Vehiculo.Id,
                                                                     estadoSalidaObra.Automatico.Value,
                                                                     estadoLlegadaPlanta.Automatico.Value);

            if (HasValue(estadoLlegadaObra) && HasValue(estadoSalidaObra))
                TiempoEnObra = estadoSalidaObra.Automatico.Value.Subtract(estadoLlegadaObra.Automatico.Value);

            if (HasValue(estadoSalidaPlanta) && HasValue(estadoLlegadaPlanta))
            {
                DistanciaTotal = daoFactory.CocheDAO.GetDistance(Ticket.Vehiculo.Id,
                                                                 estadoSalidaPlanta.Automatico.Value,
                                                                 estadoLlegadaPlanta.Automatico.Value);
                TiempoCiclo = estadoLlegadaPlanta.Automatico.Value.Subtract(estadoSalidaPlanta.Automatico.Value);
            }
        }

        private static bool HasValue(DetalleTicket detalle)
        {
            return detalle != null && detalle.Automatico.HasValue;
        }
    }
}
