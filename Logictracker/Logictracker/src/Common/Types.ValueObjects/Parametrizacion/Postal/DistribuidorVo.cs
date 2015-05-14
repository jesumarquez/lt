using System;
using Logictracker.Types.BusinessObjects.Postal;

namespace Logictracker.Types.ValueObjects.Parametrizacion.Postal
{
    [Serializable]
    public class DistribuidorVo
    {
        public const int IndexCodigo = 0;
        public const int IndexUsuario = 1;
        public const int IndexNombre = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexUsuario, ResourceName = "Entities", VariableName = "USUARIO", AllowGroup = false, IncludeInSearch = true)]
        public string Usuario { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NOMBRE", AllowGroup = false, IncludeInSearch = true)]
        public string Nombre { get; set; }

        public DistribuidorVo(Distribuidor distribuidor)
        {
            Id = distribuidor.Id;
            Usuario = distribuidor.Usuario;
            Codigo = distribuidor.Codigo;
            Nombre = distribuidor.Nombre;
        }
    }
}
