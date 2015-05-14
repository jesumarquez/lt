using System;
using Logictracker.Types.BusinessObjects.Entidades;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class SensorVo
    {   
        public const int IndexCodigo = 0;
        public const int IndexDispositivo = 1;
        public const int IndexDescripcion = 2;
        public const int IndexTipoMedicion = 3;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexCodigo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Codigo { get; set; }

        [GridMapping(Index = IndexDispositivo, ResourceName = "Entities", VariableName = "PARENTI08", InitialSortExpression = true, AllowGroup = true, IsInitialGroup = true, IncludeInSearch = true)]
        public string Dispositivo { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }
        
        [GridMapping(Index = IndexTipoMedicion, ResourceName = "Entities", VariableName = "PARENTI77", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string TipoMedicion { get; set; }

        public SensorVo(Sensor sensor)
        {
            Id = sensor.Id;
            Codigo = sensor.Codigo;
            Descripcion = sensor.Descripcion;
            Dispositivo = sensor.Dispositivo.Codigo;
            TipoMedicion = sensor.TipoMedicion.ToString();
        }
    }
}
