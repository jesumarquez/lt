using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class InsumoVo
    {
        public const int IndexCodigo = 1;
        public const int IndexDescripcion= 2;
        public const int IndexTipoInsumo = 3;
        public const int IndexUnidadMedida = 4;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexTipoInsumo, ResourceName = "Entities", VariableName = "PARENTI60", AllowGroup = true, IncludeInSearch = true)]
        public string TipoInsumo { get; set; }

        [GridMapping(Index = IndexUnidadMedida, ResourceName = "Entities", VariableName = "PARENTI85", AllowGroup = true)]
        public string UnidadMedida { get; set; }

        public Empresa Empresa { get; set; }
        public Linea Linea { get; set; }
        
        public InsumoVo(Insumo insumo)
        {
            Id = insumo.Id;
            Descripcion = insumo.Descripcion;
            Codigo = insumo.Codigo;
            TipoInsumo = insumo.TipoInsumo.Descripcion;
            UnidadMedida = insumo.UnidadMedida != null ? insumo.UnidadMedida.Descripcion : string.Empty;
            Empresa = insumo.Empresa;
            Linea = insumo.Linea;
        }
    }
}
