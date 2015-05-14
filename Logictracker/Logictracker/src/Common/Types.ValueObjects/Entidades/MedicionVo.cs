using System;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects.Entidades;

namespace Logictracker.Types.ValueObjects.Entidades
{
    [Serializable]
    public class MedicionVo
    {   
        public const int IndexFecha = 0;
        public const int IndexValor = 1;
        public const int IndexTipoMedicion = 2;
        public const int IndexEntidad = 3;
        public const int IndexSubEntidad = 4;
        public const int IndexSensor = 5;
        public const int IndexDynamicColumns = 6;
        
        public int Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", InitialSortExpression = true, SortDirection = GridSortDirection.Descending, AllowGroup = false, IncludeInSearch = true)]
        public string Fecha { get; set; }

        [GridMapping(Index = IndexValor, ResourceName = "Labels", VariableName = "VALOR", InitialSortExpression = false, AllowGroup = false, IncludeInSearch = true)]
        public string Valor { get; set; }

        [GridMapping(Index = IndexTipoMedicion, ResourceName = "Entities", VariableName = "PARENTI77", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string TipoMedicion { get; set; }

        [GridMapping(Index = IndexEntidad, ResourceName = "Entities", VariableName = "PARENTI79", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string Entidad { get; set; }

        [GridMapping(Index = IndexSubEntidad, ResourceName = "Entities", VariableName = "PARENTI81", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string SubEntidad { get; set; }

        [GridMapping(Index = IndexSensor, ResourceName = "Entities", VariableName = "PARENTI80", InitialSortExpression = false, AllowGroup = true, IncludeInSearch = true)]
        public string Sensor { get; set; }

        public int IdDispositivo { get; set; }

        public MedicionVo(Medicion medicion)
        {
            Id = medicion.Id;
            Fecha = medicion.FechaMedicion.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss");
            Valor = medicion.Valor;
            TipoMedicion = medicion.TipoMedicion.ToString();
            Entidad = medicion.SubEntidad.Entidad.ToString();
            IdDispositivo = medicion.Dispositivo.Id;
            SubEntidad = medicion.SubEntidad.ToString();
            Sensor = medicion.Sensor.ToString();
        }
    }
}
