using System;
using Logictracker.Types.BusinessObjects.Mantenimiento;

namespace Logictracker.Types.ValueObjects.Mantenimiento
{
    [Serializable]
    public class DepositoVo
    {
        public const int IndexCodigo = 1;
        public const int IndexDescripcion= 2;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false)]
        public string Descripcion { get; set; }

        public DepositoVo(Deposito deposito)
        {
            Id = deposito.Id;
            Descripcion = deposito.Descripcion;
            Codigo = deposito.Codigo;
        }
    }
}
