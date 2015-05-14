using System;
using Logictracker.Security;
using Logictracker.Types.ValueObjects.Documentos.Partes;

namespace Logictracker.Types.ValueObjects.Documentos
{
    [Serializable]
    public class ParteReportVo
    {
        public const int IndexEquipo = 0;
        public const int IndexTipoServicio = 1;
        public const int IndexCentroCostos = 2;
        public const int IndexFecha = 3;
        public const int IndexEmpresa = 4;
        public const int IndexCodigo = 5;
        public const int IndexInterno = 6;
        public const int IndexSalida = 7;
        public const int IndexLlegada = 8;
        public const int IndexHorasReportadas = 9;
        public const int IndexHorasControladas = 10;
        public const int IndexDiffHoras = 11;
        public const int IndexKmTotal = 12;
        public const int IndexKmTotalCalculado = 13;
        public const int IndexDiffKmTotal = 14;
        public const int IndexImporte = 15;
        public const int IndexImporteControlado = 16;
        public const int IndexDiffImporte = 17;
        public const int IndexEstado = 18;
        

        public int Id { get; set; }

        [GridMapping(Index = IndexEquipo, ResourceName = "Entities", VariableName = "PARENTI19", AllowGroup = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string Equipo { get; set;}

        [GridMapping(Index = IndexTipoServicio, ResourceName = "Labels", VariableName = "TIPOSERVICIOPARTE", AllowGroup = true)]
        public string TipoServicio { get; set; }

        [GridMapping(Index = IndexCentroCostos, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true)]
        public string CentroCostos { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:G}", AllowGroup = true)]
        public DateTime Fecha { get; set;}

        [GridMapping(Index = IndexEmpresa, ResourceName = "Labels", VariableName = "EMPRESA", AllowGroup = true)]
        public string Empresa { get; set;}

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "PARTE")]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexInterno, ResourceName = "Entities", VariableName = "PARENTI03", AllowGroup = true)]
        public string Interno { get; set;}

        [GridMapping(Index = IndexSalida, ResourceName = "Labels", VariableName = "SALIDA", AllowGroup = true)]
        public string Salida { get; set; }

        [GridMapping(Index = IndexLlegada, ResourceName = "Labels", VariableName = "LLEGADA", AllowGroup = true)]
        public string Llegada { get; set; }

        [GridMapping(Index = IndexHorasReportadas, ResourceName = "Labels", VariableName = "HS_REPORTADAS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00} hs")]
        public TimeSpan HorasReportadas { get; set; }

        [GridMapping(Index = IndexHorasControladas, ResourceName = "Labels", VariableName = "HS_CONTROLADAS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00} hs")]
        public TimeSpan HorasControladas { get; set; }

        [GridMapping(Index = IndexDiffHoras, ResourceName = "Labels", VariableName = "DIFERENCIA_HS", IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0:0.00} hs")]
        public TimeSpan DiffHoras { get; set; }

        [GridMapping(Index = IndexKmTotal, ResourceName = "Labels", VariableName = "KM_REPORTADO", DataFormatString = "{0:0.00}", AllowGroup = true)]
        public double KmTotal { get; set; }

        [GridMapping(Index = IndexKmTotalCalculado, ResourceName = "Labels", VariableName = "KM_CONTROLADO", DataFormatString = "{0:0.00}", AllowGroup = true)]
        public double KmTotalCalculado { get; set; }

        [GridMapping(Index = IndexDiffKmTotal, ResourceName = "Labels", VariableName = "DIFERENCIA_KM", DataFormatString = "{0:0.00}", AllowGroup = true)]
        public double DiffKmTotal { get; set; }

        [GridMapping(Index = IndexImporte, ResourceName = "Labels", VariableName = "IMPORTE_REPORTADO", DataFormatString = "{0:2}", AllowGroup = true)]
        public string Importe { get; set; }

        [GridMapping(Index = IndexImporteControlado, ResourceName = "Labels", VariableName = "IMPORTE_CONTROLADO", DataFormatString = "{0:2}", AllowGroup = true)]
        public string ImporteControlado { get; set; }

        [GridMapping(Index = IndexDiffImporte, ResourceName = "Labels", VariableName = "DIFERENCIA_IMPORTE", DataFormatString = "{0:2}", AllowGroup = true)]
        public string DiffImporte { get; set; }

        [GridMapping(Index = IndexEstado, ResourceName = "Labels", VariableName = "ESTADO", AllowGroup = true)]
        public int Estado { get; set; }

        [GridMapping(IsDataKey = true)]
        public int DocumentId { get; set; }

        public ParteReportVo(PartePersonal parte)
        {
            Id = parte.Id;
            TipoServicio = parte.TipoServicio;
            Equipo = parte.Equipo;
            Fecha = parte.Fecha.ToDisplayDateTime();
            Empresa = parte.Empresa;
            Codigo = parte.Codigo;
            Interno = parte.Interno;
            Salida = parte.Salida;
            Llegada = parte.Llegada;
            HorasReportadas = parte.Horas;
            HorasControladas = parte.HorasControladas;
            DiffHoras = parte.DiffHoras;
            KmTotal = parte.KmTotal;
            KmTotalCalculado = parte.KmTotalCalculado;
            DiffKmTotal = parte.DiffKmTotal;
            Importe = String.Format("${0:000.00}", parte.Importe);
            ImporteControlado = String.Format("${0:000.00}", parte.ImporteControlado);
            DiffImporte = String.Format("${0:000.00}",parte.DiffImporte);
            Estado = parte.Estado;
            CentroCostos = parte.CentroCostos;
        }
    }
}
