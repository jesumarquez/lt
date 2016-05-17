using System;
using Logictracker.Types.BusinessObjects.CicloLogistico.Distribucion;

namespace Logictracker.Types.ValueObjects.CicloLogistico.Distribucion
{
    [Serializable]
    public class TipoCicloLogisticoVo
    {
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexDefault = 2;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexDefault, IsTemplate = true, ResourceName = "Labels", VariableName = "DEFAULT", AllowGroup = false)]
        public bool Default { get; set; }

        public TipoCicloLogisticoVo(TipoCicloLogistico tipo)
        {
            Id = tipo.Id;
            Codigo = tipo.Codigo;
            Descripcion = tipo.Descripcion;
            Default = tipo.Default;
        }
    }
}
