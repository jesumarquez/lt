using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class TanqueVo
    {
        public const int IndexDescripcion = 0;
        public const int IndexEquipo = 1;
        public const int IndexEmpresa = 2;
        public const int IndexLinea = 3;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexEquipo, ResourceName = "Entities", VariableName = "PARENTI19", IncludeInSearch = true)]
        public string Equipo { get; set; }

        [GridMapping(Index = IndexEmpresa, ResourceName = "Entities", VariableName = "PARENTI01", IncludeInSearch = true)]
        public string Empresa { get; set; }

        [GridMapping(Index = IndexLinea, ResourceName = "Entities", VariableName = "PARENTI02", IncludeInSearch = true)]
        public string Linea { get; set; }

        public TanqueVo(Tanque tanque)
        {
            Id = tanque.Id;
            Descripcion = tanque.Descripcion;
            Equipo = tanque.Equipo != null ? tanque.Equipo.Descripcion : string.Empty;
            Empresa = tanque.Linea != null ? tanque.Linea.Empresa.RazonSocial : string.Empty;
            Linea = tanque.Linea != null ? tanque.Linea.Descripcion : string.Empty;
        }
    }
}
