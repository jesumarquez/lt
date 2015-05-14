using System;
using Urbetrack.Common.Security;
using Urbetrack.Types.BusinessObjects.Dispositivos;

namespace Urbetrack.Types.ValueObjects.Dispositivos
{
    [Serializable]
    public class TemperatureSensorEntropyVo
    {
        public const int IndexFecha = 0;
        public const int IndexTemperatura = 1;
        public const int IndexConectado = 2;

        public int Id { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "DATE", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Fecha { get; set; }

        [GridMapping(Index = IndexTemperatura, ResourceName = "Labels", VariableName = "TEMPERATURA", IncludeInSearch = true)]
        public string Temperatura { get; set; }

        [GridMapping(Index = IndexConectado, ResourceName = "Labels", VariableName = "CONECTADO", AllowGroup = true, IncludeInSearch = true)]
        public string Conectado { get; set; }

        public TemperatureSensorEntropyVo(TemperatureSensorEntropy temperatureSensorEntropy)
        {
            Id = temperatureSensorEntropy.Id;
            Conectado = temperatureSensorEntropy.Connected ? "SI" : "NO";
            Fecha = temperatureSensorEntropy.DateTime.Date.ToDisplayDateTime().ToString("dd/MM/yyyy HH:mm:ss");
            Temperatura = temperatureSensorEntropy.Temperature.ToString("#0.00");
        }
    }
}
