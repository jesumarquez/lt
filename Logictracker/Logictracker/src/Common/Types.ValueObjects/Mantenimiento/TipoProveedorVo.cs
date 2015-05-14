using System;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class TipoProveedorVo
    {
        public const int IndexCodigo = 1;
        public const int IndexDescripcion= 2;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public Empresa Empresa { get; set; }
        public Linea Linea { get; set; }

        public TipoProveedorVo(TipoProveedor tipoProveedor)
        {
            Id = tipoProveedor.Id;
            Descripcion = tipoProveedor.Descripcion;
            Codigo = tipoProveedor.Codigo;
            Empresa = tipoProveedor.Empresa;
            Linea = tipoProveedor.Linea;
        }
    }
}
