using System;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Types.ValueObjects.Dispositivos
{
    [Serializable]
    public class ConfiguracionDispositivoVo
    {
        public const int IndexNombre = 0;
        public const int IndexDescripcion = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NAME", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Nombre { get; set; }

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set; }

        public ConfiguracionDispositivoVo(ConfiguracionDispositivo configuracionDispositivo)
        {
            Id = configuracionDispositivo.Id;
            Nombre = configuracionDispositivo.Nombre;
            Descripcion = configuracionDispositivo.Descripcion;
        }
    }
}
