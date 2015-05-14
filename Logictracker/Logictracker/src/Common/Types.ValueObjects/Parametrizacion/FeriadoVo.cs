using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class FeriadoVo
    {
        public const int IndexFecha = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:dd/MM/yyyy}", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public FeriadoVo(Feriado feriado)
        {
            Id = feriado.Id;
            Fecha = feriado.Fecha;
            Descripcion = feriado.Descripcion;
        }
    }
}
