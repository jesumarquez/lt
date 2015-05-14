using System;
using Logictracker.Types.BusinessObjects.Entidades;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class SubEntidadVo
    {   
        public const int IndexCodigo = 0;
        public const int IndexDescripcion = 1;
        public const int IndexTipoEntidad = 2;
        public const int IndexEntidad = 3;
        public const int IndexTipoMedicion = 4;
        public const int IndexSensor = 5;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        [GridMapping(Index = IndexTipoEntidad, ResourceName = "Entities", VariableName = "PARENTI76", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string TipoEntidad { get; set; }

        [GridMapping(Index = IndexEntidad, ResourceName = "Entities", VariableName = "PARENTI79", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string Entidad { get; set; }

        [GridMapping(Index = IndexTipoMedicion, ResourceName = "Entities", VariableName = "PARENTI77", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string TipoMedicion { get; set; }

        [GridMapping(Index = IndexSensor, ResourceName = "Entities", VariableName = "PARENTI80", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string Sensor { get; set; }

        public SubEntidadVo(SubEntidad subEntidad)
        {
            Id = subEntidad.Id;
            Codigo = subEntidad.Codigo;
            Descripcion = subEntidad.Descripcion;
            TipoEntidad = subEntidad.Entidad.TipoEntidad.ToString();
            Entidad = subEntidad.Entidad.ToString();
            TipoMedicion = subEntidad.Sensor != null ? subEntidad.Sensor.TipoMedicion.ToString() : "";
            Sensor = subEntidad.Sensor != null ? subEntidad.Sensor.ToString() : "";
        }
    }
}
