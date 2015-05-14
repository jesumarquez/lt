using System;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.ReportObjects.Datamart;

namespace Logictracker.Scheduler.Tasks.Mantenimiento.Util
{
    public static class EstadoMotor
    {
		public static String FromPosicion(LogPosicionBase posicion)
        {
            return posicion.MotorOn.HasValue
                       ? (posicion.MotorOn.Value ? Datamart.EstadosMotor.Encendido : Datamart.EstadosMotor.Apagado)
                       : Datamart.EstadosMotor.Indefinido;
        }
    }
}
