using System;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class ReporteViajeVo
    {
        public const int IndexTransportista = 0;
        public const int IndexInterno = 2;
        public const int IndexParadasVisitadas = 3;
        public const int IndexTiempoEntrega = 4;
        public const int IndexPromedioMin = 5;
        public const int IndexPromedioHora = 6;
        public const int IndexTiempoTotal = 7;
        public const int IndexTotalMovimiento = 8;
        public const int IndexTotalDetencion = 9;

        [GridMapping(Index = IndexTransportista, ResourceName = "Entities", VariableName = "PARENTI07", IsInitialGroup = true)]
        public string Transportista { get; set; }

        [GridMapping(Index = IndexInterno, ResourceName = "Labels", VariableName = "INTERNO")]
        public string Interno { get; set; }

        [GridMapping(Index = IndexParadasVisitadas, ResourceName = "Labels", VariableName = "REALIZADO")]
        public int Realizadas { get; set; }

        [GridMapping(Index = IndexTiempoEntrega, ResourceName = "Labels", VariableName = "TIEMPO_ENTREGA", DataFormatString = "{0:d}")]
        public TimeSpan TiempoEntrega { get; set; }

        [GridMapping(Index = IndexPromedioMin, ResourceName = "Labels", VariableName = "PROMEDIO_MIN", DataFormatString = "{0:0.00}")]
        public double PromedioMin { get; set; }

        [GridMapping(Index = IndexPromedioHora, ResourceName = "Labels", VariableName = "PROMEDIO_HORA", DataFormatString = "{0:0.00}")]
        public double PromedioHora { get; set; }

        [GridMapping(Index = IndexTiempoTotal, ResourceName = "Labels", VariableName = "TIEMPO_TOTAL", DataFormatString = "{0:d}")]
        public TimeSpan TiempoTotal { get; set; }

        [GridMapping(Index = IndexTotalMovimiento, ResourceName = "Labels", VariableName = "EN_MOVIMIENTO", DataFormatString = "{0:d}")]
        public TimeSpan TotalMovimiento { get; set; }

        [GridMapping(Index = IndexTotalDetencion, ResourceName = "Labels", VariableName = "EN_DETENCION", DataFormatString = "{0:d}")]
        public TimeSpan TotalDetencion { get; set; }

        public int IdMovil { get; set; }

        public ReporteViajeVo(Coche coche, int realizadas, TimeSpan tiempoEntrega, TimeSpan tiempoTotal, TimeSpan tiempoDetenido)
        {
            IdMovil = coche.Id;
            Transportista = coche.Transportista != null ? coche.Transportista.Descripcion : string.Empty;
            Interno = coche.Interno;
            Realizadas = realizadas;
            TiempoEntrega = tiempoEntrega;
            var promedioMin = realizadas > 0 ? tiempoEntrega.TotalMinutes/realizadas : 0.00;
            var promedioHora = realizadas > 0 ? tiempoEntrega.TotalHours/realizadas : 0.00;
            PromedioMin = promedioMin;
            PromedioHora = promedioHora;
            TiempoTotal = tiempoTotal;
            TotalDetencion = tiempoDetenido;
            TotalMovimiento = tiempoTotal.Subtract(tiempoDetenido);
        }
    }
}
