using System;
using Logictracker.Culture;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.ReportObjects
{
    [Serializable]
    public class VerificadorEmpleadoVo
    {
        public const int IndexTipoZona = 0;
        public const int IndexZona = 1;
        public const int IndexEmpleado = 2;
        public const int IndexLegajo = 3;
        public const int IndexCentroCosto = 4;
        public const int IndexDepartamento = 5;
        public const int IndexCategoriaAcceso = 6;
        public const int IndexPuerta = 7;
        public const int IndexFecha = 8;
        public const int IndexHoras = 9;
        public const int IndexEnPeriodo = 10;

        [GridMapping(Index = IndexTipoZona, ResourceName = "Entities", VariableName = "PARENTI91", IsInitialGroup = true)]
        public string TipoZona { get; set; }

        [GridMapping(Index = IndexZona, ResourceName = "Entities", VariableName = "PARENTI92", IsInitialGroup = true, IncludeInSearch = true)]
        public string Zona { get; set; }

        [GridMapping(Index = IndexEmpleado, ResourceName = "Entities", VariableName = "PARENTI09", IncludeInSearch = true)]
        public string Empleado { get; set; }

        [GridMapping(Index = IndexLegajo, ResourceName = "Labels", VariableName = "LEGAJO", IncludeInSearch = true, IsAggregate = true, AggregateType = GridAggregateType.Count, AggregateTextFormat = "{0} Empleados")]
        public string Legajo { get; set; }

        [GridMapping(Index = IndexCentroCosto, ResourceName = "Entities", VariableName = "PARENTI37", IncludeInSearch = true)]
        public string CentroCosto { get; set; }

        [GridMapping(Index = IndexDepartamento, ResourceName = "Entities", VariableName = "PARENTI04", IncludeInSearch = true)]
        public string Departamento { get; set; }

        [GridMapping(Index = IndexCategoriaAcceso, ResourceName = "Entities", VariableName = "PARENTI15", IncludeInSearch = true)]
        public string CategoriaAcceso { get; set; }

        [GridMapping(Index = IndexPuerta, ResourceName = "Entities", VariableName = "PARENTI55", IncludeInSearch = true)]
        public string Puerta { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", AllowGroup = false, DataFormatString = "{0: dd/MM/yy HH:mm}", InitialSortExpression = true, SortDirection = GridSortDirection.Descending)]
        public DateTime? Fecha { get; set; }

        [GridMapping(Index = IndexHoras, ResourceName = "Labels", VariableName = "HORAS", AllowGroup = false)]
        public string Horas { get; set; }

        [GridMapping(Index = IndexEnPeriodo, ResourceName = "Labels", VariableName = "EN_PERIODO", AllowGroup = false, IsAggregate = true, AggregateType = GridAggregateType.Sum, AggregateTextFormat = "{0} en período")]
        public bool EnPeriodo { get; set; }
        
        public VerificadorEmpleadoVo(Empleado.VerificadorEmpleado verif, int periodo)
        {
            Zona = verif.ZonaAcceso != null ? verif.ZonaAcceso.Descripcion : CultureManager.GetLabel("SIN_ZONA");
            TipoZona = verif.ZonaAcceso != null ? verif.ZonaAcceso.TipoZonaAcceso.Descripcion : CultureManager.GetLabel("SIN_ZONA");
            Empleado = verif.Empleado != null && verif.Empleado.Entidad != null ? verif.Empleado.Entidad.Descripcion : string.Empty;
            Legajo = verif.Empleado != null ? verif.Empleado.Legajo : string.Empty;
            CentroCosto = verif.Empleado != null && verif.Empleado.CentroDeCostos != null ? verif.Empleado.CentroDeCostos.Descripcion : string.Empty;
            Departamento = verif.Empleado != null && verif.Empleado.Departamento != null ? verif.Empleado.Departamento.Descripcion : string.Empty;
            CategoriaAcceso = verif.Empleado != null && verif.Empleado.Categoria != null ? verif.Empleado.Categoria.Descripcion : string.Empty;
            Puerta = verif.PuertaAcceso != null ? verif.PuertaAcceso.Descripcion : string.Empty;
            Fecha = verif.Fecha.HasValue ? verif.Fecha.Value.ToDisplayDateTime() : verif.Fecha;
            var ts = verif.Fecha.HasValue ? DateTime.UtcNow.Subtract(verif.Fecha.Value) : new TimeSpan(0);
            var horas = Convert.ToInt32(ts.TotalHours) - (ts.Minutes > 30 ? 1 : 0);
            Horas = verif.Fecha.HasValue ? horas.ToString("#00") + ":" + ts.Minutes.ToString("00") : string.Empty;
            EnPeriodo = verif.Fecha.HasValue && ts.TotalHours < periodo;
        }
    }
}
