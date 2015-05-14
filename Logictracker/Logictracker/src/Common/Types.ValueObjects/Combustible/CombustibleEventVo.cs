using System;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

namespace Logictracker.Types.ValueObjects.Combustible
{
    [Serializable]
    public class CombustibleEventVo
    {
        public const int IndexKeyIdAccion = 0;

        public const int IndexMotorDescri = 0;
        public const int IndexFecha = 1;
        public const int IndexMensaje = 2;

        [GridMapping(Index = IndexMotorDescri, ResourceName = "Entities", VariableName = "PARENTI08", IsInitialGroup = true)]
        public string MotorDescri { get; set; }

        [GridMapping(Index = IndexFecha, ResourceName = "Labels", VariableName = "FECHA", InitialSortExpression = true, DataFormatString = "{0:G}", SortDirection = GridSortDirection.Descending)]
        public DateTime Fecha { get; set; }

        [GridMapping(Index = IndexMensaje, ResourceName = "Labels", VariableName = "MENSAJE")]
        public string Mensaje { get; set; }

        [GridMapping(IsDataKey = true)]
        public int IdAccion { get; set; }

         public CombustibleEventVo(CombustibleEvent e)
        {
             MotorDescri = e.MotorDescri;
             IdAccion = e.IDAccion;
             Fecha = e.Fecha;
             Mensaje = e.Mensaje;
        }
    }
}
