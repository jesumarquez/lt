using System;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Types.ValueObjects.Dispositivos
{
    [Serializable]
    public class PrecintoVo
    {
        public const int IndexCodigo = 0;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        
        public PrecintoVo(Precinto precinto)
        {
            if (precinto == null) return;

            Id = precinto.Id;
            Codigo = precinto.Codigo;
        }
    }
}