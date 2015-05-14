using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class SubCentroDeCostosVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexCentroDeCostos = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true, Width = "20%")]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = true)]
        public string CentroDeCostos { get; set; }

        public SubCentroDeCostosVo(SubCentroDeCostos subCentroDeCostos)
        {
            Id = subCentroDeCostos.Id;
            Codigo = subCentroDeCostos.Codigo;
            Descripcion = subCentroDeCostos.Descripcion;
            CentroDeCostos = subCentroDeCostos.CentroDeCostos != null ? subCentroDeCostos.CentroDeCostos.Descripcion : string.Empty;
        }
    }
}
