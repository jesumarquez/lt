using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class CaudalimetroVo
    {
        public const int IndexDescripcion = 0;
        public const int IndexTanque = 1;

        public int Id { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexTanque, ResourceName = "Entities", VariableName = "PARENTI36", IncludeInSearch = true)]
        public string Tanque { get; set;}

        [GridMapping(Index = 2, ResourceName = "Entities", VariableName = "PARENTI19", IncludeInSearch = true)]
        public string Equipo { get; set;}

        public CaudalimetroVo(Caudalimetro caudalimetro)
        {
            Id = caudalimetro.Id;
            Descripcion = caudalimetro.Descripcion;
            Tanque = caudalimetro.Tanque != null ? caudalimetro.Tanque.Descripcion : "Sin Asignar";
            Equipo = caudalimetro.Equipo != null ? caudalimetro.Equipo.Descripcion : "Sin Asignar";
        }
    }
}
