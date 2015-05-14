using System;
using Logictracker.Types.BusinessObjects;

namespace Logictracker.Types.ValueObjects.Empleados
{
    [Serializable]
    public class TarjetaVo
    {
        public const int IndexNumero = 0;
        public const int IndexPin = 1;
        public const int IndexPinHexa = 2;
        public const int IndexChofer = 3;

        public int Id { get; set; }

        [GridMapping(Index = IndexNumero, ResourceName = "Labels", VariableName = "NUMERO", InitialSortExpression = true, AllowGroup = false, IncludeInSearch = true)]
        public string Numero { get; set; }

        [GridMapping(Index = IndexPin, ResourceName = "Labels", VariableName = "PIN", AllowGroup = false, IncludeInSearch = true)]
        public string Pin { get; set; }

        [GridMapping(Index = IndexPinHexa, ResourceName = "Labels", VariableName = "PIN_HEXA", AllowGroup = false, IncludeInSearch = true)]
        public string PinHexa { get; set; }

        [GridMapping(Index = IndexChofer, ResourceName = "Labels", VariableName = "CHOFER_ASIGNADO", AllowGroup = false, IncludeInSearch = true)]
        public string Chofer { get; set; }
               
        public TarjetaVo(Tarjeta tarjeta, Empleado empleado)
        {
            Id = tarjeta.Id;
            Numero = tarjeta.Numero;
            Pin = tarjeta.Pin;
            PinHexa = tarjeta.PinHexa;
            Chofer = empleado != null ? empleado.Entidad != null ? empleado.Entidad.Descripcion : "Desconocido" : "Sin Asignar";
        }
    }
}