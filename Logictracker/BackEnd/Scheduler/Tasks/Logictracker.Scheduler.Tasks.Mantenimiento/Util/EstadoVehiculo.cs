using System;
using Logictracker.Types.BusinessObjects.BaseObjects;
using Logictracker.Types.ReportObjects.Datamart;

namespace Logictracker.Scheduler.Tasks.Mantenimiento.Util
{
    public static class EstadoVehiculo
    {
		public static String FromPosicion(LogPosicionBase posicion)
        {
            return posicion.Velocidad > 0 ? Datamart.Estadosvehiculo.EnMovimiento : Datamart.Estadosvehiculo.Detenido;
        }
    }
}
