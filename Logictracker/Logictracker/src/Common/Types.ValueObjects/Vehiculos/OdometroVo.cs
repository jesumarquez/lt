using System;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.Vehiculos
{
    [Serializable]
    public class OdometroVo
    {
        public const int IndexDescripcion = 0;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public OdometroVo(Odometro odometro)
        {
            Id = odometro.Id;
            Descripcion = odometro.Descripcion;
        }
    }
}
