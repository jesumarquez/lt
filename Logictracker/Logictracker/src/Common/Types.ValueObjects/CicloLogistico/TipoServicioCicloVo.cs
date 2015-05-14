using System;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class TipoServicioCicloVo
    {
        public const int IndexEmpresa = 0;
        public const int IndexLinea = 1;
        public const int IndexCodigo = 2;
        public const int IndexDescripcion = 3;
        public const int IndexDemora = 4;
        public const int IndexDefault = 5;

        public int Id { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", AllowGroup = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02", AllowGroup = true)]
        public string Linea { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, InitialSortExpression = true, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexDemora, ResourceName = "Labels", VariableName = "DEMORA", AllowGroup = false)]
        public int Demora { get; set; }

        [GridMapping(Index = IndexDefault, IsTemplate = true, ResourceName = "Labels", VariableName = "DEFAULT", AllowGroup = false)]
        public bool Default { get; set; }

        public TipoServicioCicloVo(TipoServicioCiclo tipoServicio)
        {
            Id = tipoServicio.Id;
            Codigo = tipoServicio.Codigo;
            Empresa = tipoServicio.Empresa != null ? tipoServicio.Empresa.RazonSocial : string.Empty;
            Linea = tipoServicio.Linea != null ? tipoServicio.Linea.Descripcion : string.Empty;
            Descripcion = tipoServicio.Descripcion;
            Demora = tipoServicio.Demora;
            Default = tipoServicio.Default;
        }
    }
}
