using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class ProveedorVo
    {
        public const int IndexCodigo = 1;
        public const int IndexDescripcion = 2;
        public const int IndexTipoProveedor = 3;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexTipoProveedor, ResourceName = "Entities", VariableName = "PARENTI86", InitialSortExpression = true, AllowGroup = true, IncludeInSearch = true)]
        public string TipoProveedor { get; set; }

        public Empresa Empresa { get; set; }
        public Linea Linea { get; set; }
        
        public ProveedorVo(Proveedor proveedor)
        {
            Id = proveedor.Id;
            Descripcion = proveedor.Descripcion;
            Codigo = proveedor.Codigo;
            Empresa = proveedor.Empresa;
            Linea = proveedor.Linea;
            TipoProveedor = proveedor.TipoProveedor != null ? proveedor.TipoProveedor.Descripcion : string.Empty;
        }
    }
}
