using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class ZonaVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexPrioridad = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, InitialSortExpression = true)]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexPrioridad, ResourceName = "Labels", VariableName = "PRIORIDAD", AllowGroup = true)]
        public string Prioridad { get; set; }
        
        public ZonaVo(Zona zona)
        {
            Id = zona.Id;
            Codigo = zona.Codigo;
            Descripcion = zona.Descripcion;
            Prioridad = zona.Prioridad.ToString("#0");
        }
    }
}
