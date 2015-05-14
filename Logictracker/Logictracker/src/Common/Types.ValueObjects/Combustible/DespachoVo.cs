using System;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class DespachoVo
    {
        public const int IndexCentroDeCostos = 0;
        public const int IndexFecha = 1;
        public const int IndexInternoVehiculo = 2;
        public const int IndexPatente = 3;
        public const int IndexVolumen = 4;
        public const int IndexOperador = 5;

        [GridMapping(Index = IndexCentroDeCostos, ResourceName = "Entities", VariableName = "PARENTI37", AllowGroup = false)]
        public string CentroDeCostos { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", DataFormatString = "{0:G}", AllowGroup = false, InitialSortExpression = true)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexInternoVehiculo, ResourceName = "Labels", VariableName = "INTERNO", AllowGroup = true, IsInitialGroup = true, InitialSortExpression = true)]
        public string InternoVehiculo { get; set; }

        [GridMapping(Index = IndexPatente, ResourceName = "Labels", VariableName = "PATENTE", AllowGroup = false)]
        public string Patente { get; set; }

        [GridMapping(Index = IndexVolumen, ResourceName = "Labels", VariableName = "VOLUMEN", AllowGroup = false, DataFormatString = "{0:2} lit", IsAggregate = true, AggregateTextFormat = "Total Despachado: {0} lit")]
        public double Volumen { get; set; }

        [GridMapping(Index = IndexOperador, ResourceName = "Labels", VariableName = "CHOFER", AllowGroup = false)]
        public string Operador { get; set; }


        public DespachoVo(Movimiento m)
        {
            Fecha = m.Fecha;
            Operador = m.Empleado != null ? m.Empleado.Entidad.Descripcion : String.Empty;

            if(m.Coche == null) return;

            CentroDeCostos = m.Coche.CentroDeCostos != null ? m.Coche.CentroDeCostos.Descripcion : String.Empty;
            InternoVehiculo = m.Coche.Interno;
            Patente = m.Coche.Patente;
            Volumen = m.Volumen;
        }
    }
}
