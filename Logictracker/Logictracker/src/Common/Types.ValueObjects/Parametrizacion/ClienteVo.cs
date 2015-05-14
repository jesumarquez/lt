using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class ClienteVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true, Width = "20%")]
        public string Codigo { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        public bool Nomenclado { get; set;}

        public ClienteVo(Cliente cliente)
        {
            Id = cliente.Id;
            Codigo = cliente.Codigo;
            Descripcion = cliente.Descripcion;
            Nomenclado = cliente.Nomenclado;
        }
    }
}
