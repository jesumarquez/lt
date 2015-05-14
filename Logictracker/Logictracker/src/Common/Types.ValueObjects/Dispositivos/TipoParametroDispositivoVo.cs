using System;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Types.ValueObjects.Dispositivos
{
    [Serializable]
    public class TipoParametroDispositivoVo
    {
        public const int IndexNombre = 0;
        public const int IndexDescripcion = 1;
        public const int IndexTipoDato = 2;
        public const int IndexConsumidor = 3;
        public const int IndexValorInicial = 4;
        public const int IndexEditable = 5;
        public const int IndexRequiereReset = 6;

        public int Id { get; set; }

        [GridMapping(Index = IndexNombre, ResourceName = "Labels", VariableName = "NAME", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Nombre { get; set;}

        [GridMapping(Index = IndexDescripcion, ResourceName = "Labels", VariableName = "DESCRIPCION", AllowGroup = false, IncludeInSearch = true)]
        public string Descripcion { get; set;}

        [GridMapping(Index = IndexTipoDato, ResourceName = "Labels", VariableName = "TIPO_DATO")]
        public string TipoDato { get; set;}

        [GridMapping(Index = IndexConsumidor, ResourceName = "Labels", VariableName = "CONSUMIDOR")]
        public string Consumidor { get; set;}

        [GridMapping(Index = IndexValorInicial, ResourceName = "Labels", VariableName = "VALOR_INICIAL")]
        public string ValorInicial { get; set;}

        [GridMapping(Index = IndexEditable, ResourceName = "Labels", VariableName = "EDITABLE")]
        public string Editable { get; set;}

        [GridMapping(Index = IndexRequiereReset, ResourceName = "Labels", VariableName = "REQUIRES_RESET")]
        public string RequiereReset { get; set;} 

        public TipoParametroDispositivoVo(TipoParametroDispositivo tipoParametroDispositivo)
        {
            Id = tipoParametroDispositivo.Id;
            Nombre = tipoParametroDispositivo.Nombre;
            Descripcion = tipoParametroDispositivo.Descripcion;
            TipoDato = tipoParametroDispositivo.TipoDato;
            Consumidor = tipoParametroDispositivo.Consumidor;
            ValorInicial = tipoParametroDispositivo.ValorInicial;
            Editable = tipoParametroDispositivo.Editable ? "X" : string.Empty;
            RequiereReset = tipoParametroDispositivo.RequiereReset ? "X" : string.Empty;
        }
    }
}
