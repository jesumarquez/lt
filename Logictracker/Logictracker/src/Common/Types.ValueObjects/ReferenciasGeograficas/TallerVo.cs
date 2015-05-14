#region Usings

using System;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Types.ValueObjects.ReferenciasGeograficas
{
    [Serializable]
    public class TallerVo
    {
        public const int IndexIconUrl = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexIconUrl, IsTemplate = true, Width = "40px", AllowGroup = false)]
        public string IconUrl { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public TallerVo(Taller taller)
        {
            Id = taller.Id;
            IconUrl = taller.ReferenciaGeografica.Icono != null ? taller.ReferenciaGeografica.Icono.PathIcono : string.Empty;
            Descripcion = taller.Descripcion;
        }
    }
}
