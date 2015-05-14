using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Organizacion
{
    [Serializable]
    public class SistemaVo
    {
        public const int IndexIconUrl = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set;}

        [GridMapping(Index = IndexIconUrl, IsTemplate = true, Width = "32px", AllowGroup = false)]
        public string IconUrl { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        public SistemaVo(Sistema sistema)
        {
            Id = sistema.Id;
            Descripcion = CultureManager.GetString("Menu", sistema.Descripcion);
            IconUrl = string.Concat("~/",sistema.Descripcion, ".image");
        }
    }
}
