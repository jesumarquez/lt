using System;
using System.Linq;
using Logictracker.Types.BusinessObjects.CicloLogistico;

namespace Logictracker.Types.ValueObjects.CicloLogistico
{
    [Serializable]
    public class RecorridoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexNombre = 1;
        public const int IndexDistancia = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NAME", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Nombre { get; set; }

        [GridMapping(Index = IndexDistancia, ResourceName = "Labels", VariableName = "DISTANCIA", AllowGroup = true, DataFormatString = "{0:0.00}")]
        public double Distancia { get; set; }       

        public RecorridoVo(Recorrido recorrido)
        {
            Id = recorrido.Id;
            Codigo = recorrido.Codigo;
            Nombre = recorrido.Nombre;
            Distancia = recorrido.Detalles.Sum(d=>d.Distancia);
        }
    }
}
