using System;
using Logictracker.Types.BusinessObjects.Postal;

namespace Logictracker.Types.ValueObjects.Parametrizacion.Postal
{
    [Serializable]
    public class MotivoVo
    {
        public const int IndexDescripcion = 0;

        public int Id { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public MotivoVo(Motivo motivo)
        {
            Id = motivo.Id;
            Descripcion = motivo.Descripcion;
        }
    }
}
