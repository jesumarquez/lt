using System;
using Logictracker.Types.BusinessObjects.Tickets;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class BocaDeCargaVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexLinea = 2;
        public const int IndexRendimiento = 3;
        public const int IndexHorasLaborales = 4;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02", AllowGroup = true)]
        public string Linea { get; set; }

        [GridMapping(Index = IndexRendimiento, ResourceName = "Labels", VariableName = "RENDIMIENTO", AllowGroup = true)]
        public int Rendimiento { get; set; }

        [GridMapping(Index = IndexHorasLaborales, ResourceName = "Labels", VariableName = "HORAS_LABORALES", AllowGroup = true)]
        public int HorasLaborales { get; set; }

        public BocaDeCargaVo(BocaDeCarga boca)
        {
            Id = boca.Id;
            Codigo = boca.Codigo;
            Descripcion = boca.Descripcion;
            Linea = boca.Linea.Descripcion;
            Rendimiento = boca.Rendimiento;
            HorasLaborales = boca.HorasLaborales;
        }
    }
}
