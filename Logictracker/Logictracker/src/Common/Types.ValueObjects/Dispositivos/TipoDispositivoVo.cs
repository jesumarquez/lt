using System;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Types.ValueObjects.Dispositivos
{
    [Serializable]
    public class TipoDispositivoVo
    {
        public const int IndexModelo = 0;
        public const int IndexFabricante = 1;

        public int Id { get; set; }

        [GridMapping(Index = IndexModelo, ResourceName = "Labels", VariableName = "MODELO", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Modelo { get; set;}

        [GridMapping(Index = IndexFabricante, ResourceName = "Labels", VariableName = "FABRICANTE", IncludeInSearch = true)]
        public string Fabricante { get; set;}

        public TipoDispositivoVo(TipoDispositivo tipoDispositivo)
        {
            Id = tipoDispositivo.Id;
            Modelo = tipoDispositivo.Modelo;
            Fabricante = tipoDispositivo.Fabricante;
        }
    }
}
