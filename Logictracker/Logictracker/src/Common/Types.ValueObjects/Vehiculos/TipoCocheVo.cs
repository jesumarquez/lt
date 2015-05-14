#region Usings

using System;
using Logictracker.Types.BusinessObjects.Vehiculos;

#endregion

namespace Logictracker.Types.ValueObjects.Vehiculos
{
    [Serializable]
    public class TipoCocheVo
    {
        public const int IndexIconUrl = 0;
        public const int IndexCodigo = 1;
        public const int IndexDescripcion = 2;
        public const int IndexEmpresa = 3;
        public const int IndexLinea = 4;

        public int Id { get; set; }

        [GridMapping(Index = IndexIconUrl, IsTemplate = true, Width = "32px", AllowGroup = false)]
        public string IconUrl { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", IncludeInSearch = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02", IncludeInSearch = true)]
        public string Linea { get; set; }

        public TipoCocheVo(TipoCoche tipoCoche)
        {
            Id = tipoCoche.Id;
            IconUrl = tipoCoche.IconoDefault != null ? tipoCoche.IconoDefault.PathIcono : string.Empty;
            Codigo = tipoCoche.Codigo;
            Descripcion = tipoCoche.Descripcion;
            Empresa = tipoCoche.Empresa != null ? tipoCoche.Empresa.RazonSocial : string.Empty;
            Linea = tipoCoche.Linea != null ? tipoCoche.Linea.Descripcion : string.Empty;
        }
    }
}
