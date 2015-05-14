using System;
using Logictracker.Types.BusinessObjects.Entidades;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class EntidadVo
    {   
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexDispositivo = 2;
        public const int IndexTipoEntidad = 3;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Dispositivo { get; set; }

        [GridMapping(Index = IndexTipoEntidad, ResourceName = "Entities", VariableName = "PARENTI76", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string TipoEntidad { get; set; }

        public EntidadVo(EntidadPadre entidad)
        {
            Id = entidad.Id;
            Codigo = entidad.Codigo;
            Descripcion = entidad.Descripcion;
            Dispositivo = entidad.Dispositivo.Codigo;
            TipoEntidad = entidad.TipoEntidad.ToString();
        }
    }
}
