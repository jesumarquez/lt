using System;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Organizacion
{
    [Serializable]
    public class FuncionVo
    {
        public const int IndexIconUrl = 0;
        public const int IndexDescripcion = 1;
        public const int IndexModulo = 2;
        public const int IndexSistema = 3;

        public int Id { get; set;}

        [GridMapping(Index = IndexIconUrl, IsTemplate = true, Width = "32px", AllowGroup = false)]
        public string IconUrl { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexModulo, HeaderText = "Modulo", IncludeInSearch = true)]
        public string Modulo { get; set;}

        [GridMapping(Index = IndexSistema, HeaderText = "Sistema", IncludeInSearch = true)]
        public string Sistema { get; set; }

        public FuncionVo(Funcion funcion)
        {
            Id = funcion.Id;
            Descripcion = CultureManager.GetString("Menu", funcion.Descripcion);
            Modulo = string.IsNullOrEmpty(funcion.Modulo) ? "" : CultureManager.GetString("Menu", funcion.Modulo);
            Sistema = CultureManager.GetString("Menu", funcion.Sistema.Descripcion);
            IconUrl = string.Concat("~/",funcion.Descripcion,".image");
        }
    }
}
