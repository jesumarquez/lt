#region Usings

using System;
using Logictracker.Types.BusinessObjects;

#endregion

namespace Logictracker.Types.ValueObjects.Parametrizacion
{
    [Serializable]
    public class EmpresaVo
    {
        public const int IndexCodigo = 0;
        public const int IndexNombre = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Nombre { get; set; }

        public EmpresaVo(Empresa empresa)
        {
            Id = empresa.Id;
            Codigo = empresa.Codigo;
            Nombre = empresa.RazonSocial;
        }
    }
}
