using System;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.InterfacesAndBaseClasses;

namespace Logictracker.Types.BusinessObjects.CicloLogistico
{
    public class AgendaVehicular: IAuditable, ISecurable, IHasVehiculo, IHasDepartamento
    {
        public static class Estados
        {
            public const short Cancelado = -1;
            public const short Reservado = 0;
            public const short EnCurso = 1;
            public const short Cerrado = 9;

            public static string GetLabelVariableName(short estado)
            {
                switch (estado)
                {
                    case Cancelado: return "AGENDA_STATE_CANCELADO";
                    case Reservado: return "AGENDA_STATE_RESERVADO";
                    case EnCurso: return "AGENDA_STATE_ENCURSO";
                    case Cerrado: return "AGENDA_STATE_CERRADO";
                    default: return "AGENDA_STATE_PENDIENTE";
                }
            }
        }

        public virtual Type TypeOf() { return GetType(); }

        public virtual int Id { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Linea Linea { get; set; }
        public virtual Departamento Departamento { get; set; }
        public virtual Coche Vehiculo { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Shift Turno { get; set; }
        
        public virtual DateTime FechaDesde { get; set; }
        public virtual DateTime FechaHasta { get; set; }
        public virtual int Estado { get; set; }
    }
}
