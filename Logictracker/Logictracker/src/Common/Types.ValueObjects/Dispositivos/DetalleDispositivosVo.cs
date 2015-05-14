using System;
using Logictracker.Types.BusinessObjects.Dispositivos;

namespace Logictracker.Types.ValueObjects.Dispositivos
{
    [Serializable]
    public class DetalleDispositivosVo
    {
        public const int IndexCodigoDispositivo = 0;
        public const int IndexVehiculo = 1;
        public const int IndexTelefono = 2;
        public const int IndexImei = 3;
        public const int IndexConfig = 4;
        public const int IndexParam = 5;
        public const int IndexValor = 6;

        public int Id { get; set; }

        [GridMapping(Index = IndexCodigoDispositivo, ResourceName = "Labels", VariableName = "CODE", InitialSortExpression = true, IncludeInSearch = true)]
        public string CodigoDispositivo { get; set; }

        [GridMapping(Index = IndexVehiculo, ResourceName = "Entities", VariableName = "PARENTI03", IncludeInSearch = true)]
        public string Vehiculo { get; set; }

        [GridMapping(Index = IndexTelefono, ResourceName = "Labels", VariableName = "TELEFONO", IncludeInSearch = true)]
        public string Telefono { get; set; }

        [GridMapping(Index = IndexImei, ResourceName = "Labels", VariableName = "IMEI", IncludeInSearch = true)]
        public string Imei { get; set; }

        [GridMapping(Index = IndexConfig, ResourceName = "Labels", VariableName = "CONFIGURACION", IncludeInSearch = true)]
        public string Config { get; set; }

        [GridMapping(Index = IndexParam, ResourceName = "Labels", VariableName = "PARAMETRO", IncludeInSearch = true, IsInitialGroup = true)]
        public string Param { get; set; }

        [GridMapping(Index = IndexValor, ResourceName = "Labels", VariableName = "VALOR", IncludeInSearch = true)]
        public string Valor { get; set; }

        public DetalleDispositivosVo(DetalleDispositivo detalle, string vehiculo)
        {
            Id = detalle.Dispositivo.Id;

            CodigoDispositivo = detalle.Dispositivo.Codigo;
            Vehiculo = vehiculo ?? string.Empty;
            Telefono = detalle.Dispositivo.Telefono;
            Imei = detalle.Dispositivo.Imei;
            Config = detalle.Dispositivo.ConfiguracionAsString;
            if (Config.Length > 64) Config = Config.Substring(0, 64);
            Param = detalle.TipoParametro.Nombre;
            Valor = detalle.Valor;
        }
    }
}