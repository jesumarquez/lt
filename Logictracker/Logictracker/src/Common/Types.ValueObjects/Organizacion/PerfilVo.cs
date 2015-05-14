using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Organizacion
{
    [Serializable]
    public class PerfilVo
    {
        public const int IndexDescripcion = 0;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public PerfilVo(Perfil perfil)
        {
            Id = perfil.Id;
            Descripcion = perfil.Descripcion;
        }
    }
}
