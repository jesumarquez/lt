using System;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace Logictracker.Types.ValueObjects.Vehiculos
{
    [Serializable]
    public class MarcaVo
    {
        public const int IndexDescripcion = 0;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public MarcaVo(Marca marca)
        {
            Id = marca.Id;
            Descripcion = marca.Descripcion;
        }
    }
}
