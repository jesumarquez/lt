using System;
using Logictracker.Types.ReportObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class KilometrosDiariosVo
    {   
        public const int IndexPatente = 0;        
        public const int IndexKmTotales = 1;
        public const int IndexKmEnRuta = 2;
        public const int IndexHorasDeMarcha = 3;
        public const int IndexHorasMovimiento = 4;
        public const int IndexHorasDetenido = 5;
        public const int IndexHorasSinReportar = 6;
        public const int IndexHorasDetenidoEnTaller = 7;
        public const int IndexHorasDetenidoEnBase = 8;
        public const int IndexHorasDetenidoConTarea = 9;
        public const int IndexHorasDetenidoSinTarea = 10;

        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE", InitialSortExpression = true)]
        public string Patente { get; set; }
        
        [GridMapping(Index = IndexKmTotales, ResourceName = "Labels", VariableName = "KM_TOTALES", DataFormatString = "{0: 0.00}")]
        public double KmTotales { get; set; }

        [GridMapping(Index = IndexKmEnRuta, ResourceName = "Labels", VariableName = "KM_EN_RUTA", DataFormatString = "{0: 0.00}")]
        public double KmEnRuta { get; set; }

        [GridMapping(Index = IndexHorasDeMarcha, ResourceName = "Labels", VariableName = "HS_MARCHA")]
        public TimeSpan HorasDeMarcha { get; set; }

        [GridMapping(Index = IndexHorasMovimiento, ResourceName = "Labels", VariableName = "HS_MOVIMIENTO")]
        public TimeSpan HorasMovimiento { get; set; }

        [GridMapping(Index = IndexHorasDetenido, ResourceName = "Labels", VariableName = "HS_DETENIDO")]
        public TimeSpan HorasDetenido { get; set; }

        [GridMapping(Index = IndexHorasSinReportar, ResourceName = "Labels", VariableName = "HS_SIN_REPORTAR")]
        public TimeSpan HorasSinReportar { get; set; }

        [GridMapping(Index = IndexHorasDetenidoEnTaller, ResourceName = "Labels", VariableName = "HS_DETENIDO_TALLER")]
        public TimeSpan HorasDetenidoEnTaller { get; set; }

        [GridMapping(Index = IndexHorasDetenidoEnBase, ResourceName = "Labels", VariableName = "HS_DETENIDO_BASE")]
        public TimeSpan HorasDetenidoEnBase { get; set; }

        [GridMapping(Index = IndexHorasDetenidoConTarea, ResourceName = "Labels", VariableName = "HS_DETENIDO_CON_TAREA")]
        public TimeSpan HorasDetenidoConTarea { get; set; }

        [GridMapping(Index = IndexHorasDetenidoSinTarea, ResourceName = "Labels", VariableName = "HS_DETENIDO_SIN_TAREA")]
        public TimeSpan HorasDetenidoSinTarea { get; set; }

        public int IdVehiculo { get; set; }

        public KilometrosDiariosVo(KilometrosDiarios kilometrosDiarios)
        {
            IdVehiculo = kilometrosDiarios.IdVehiculo;
            Patente = kilometrosDiarios.Patente;
            KmTotales = kilometrosDiarios.KmTotales;
            KmEnRuta = kilometrosDiarios.KmEnRuta;
            HorasDeMarcha = new TimeSpan(0, 0, (int)(kilometrosDiarios.HorasDeMarcha * 3600));
            HorasMovimiento = new TimeSpan(0, 0, (int)(kilometrosDiarios.HorasMovimiento * 3600));
            HorasDetenido = new TimeSpan(0, 0, (int)(kilometrosDiarios.HorasDetenido * 3600));
            HorasSinReportar = new TimeSpan(0, 0, (int)(kilometrosDiarios.HorasSinReportar * 3600));
            HorasDetenidoEnTaller = new TimeSpan(0, 0, (int)(kilometrosDiarios.HorasDetenidoEnTaller * 3600));
            HorasDetenidoEnBase = new TimeSpan(0, 0, (int)(kilometrosDiarios.HorasDetenidoEnBase * 3600));
            HorasDetenidoConTarea = new TimeSpan(0, 0, (int)(kilometrosDiarios.HorasDetenidoConTarea * 3600));
            HorasDetenidoSinTarea = new TimeSpan(0, 0, (int)(kilometrosDiarios.HorasDetenidoSinTarea * 3600));
        }
    }
}
