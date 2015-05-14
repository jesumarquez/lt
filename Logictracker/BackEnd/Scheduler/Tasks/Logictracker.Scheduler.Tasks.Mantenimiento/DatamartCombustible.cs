#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Scheduler.Tasks.Mantenimiento
{
    /// <summary>
    /// Pre-calculates sumarized fuel information.
    /// </summary>
    public class DatamartCombustible : BaseTask
    {
        #region Protected Methods

        /// <summary>
        /// Performs taks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
			STrace.Trace(GetType().FullName, "Processing engines.");
            
            foreach(var m in DaoFactory.CaudalimetroDAO.FindAll()) ProcessAllCaudalimetrosMovements(m);

			STrace.Trace(GetType().FullName, "Processing tanks by location.");

            foreach (var t in DaoFactory.TanqueDAO.FindAllEnPlanta()) ProcessTankEnPlantaConsumptions(t);

			STrace.Trace(GetType().FullName, "Processing tanks by equipment.");

            foreach (var t in DaoFactory.TanqueDAO.FindAllEnEquipo()) ProcessTankEnEquipoConsumptions(t);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Precalcula los consumos de cada movimiento del motor (vienen en un contador global), para agilizar las consultas
        /// en la aplicación y resto del datamart.s
        /// </summary>
        /// <param name="caudalimetro"></param>
        private void ProcessAllCaudalimetrosMovements(Caudalimetro caudalimetro) { DaoFactory.MovimientoDAO.ProcessMovements(caudalimetro); }

        /// <summary>
        /// Procesa tanques por locacion.
        /// </summary>
        /// <param name="tanque"></param>
        private void ProcessTankEnPlantaConsumptions(Tanque tanque)
        {
            try
            {
                DaoFactory.VolumenHistoricoDAO.GenerateTanksByPlantaTeoricVolumes(tanque);

                var ultimoTeorico = DaoFactory.VolumenHistoricoDAO.GetLastTankTeoricLevel(tanque.Id);
                var volTeo = ultimoTeorico != null ? ultimoTeorico.Volumen : 0;

                tanque.VolTeorico = volTeo > 0 ? volTeo : 0;

                DaoFactory.TanqueDAO.SaveOrUpdate(tanque);
            }
            catch(Exception ex)
            {
				STrace.Exception(GetType().FullName, ex);

                throw;
            }
        }

        /// <summary>
        /// Procesa tanques por equipo.
        /// </summary>
        /// <param name="tanque"></param>
        private void ProcessTankEnEquipoConsumptions(Tanque tanque)
        {
            var lastTankMovement = DaoFactory.VolumenHistoricoDAO.GetLastTankTeoricLevel(tanque.Id);
            var lastDate = lastTankMovement != null ? lastTankMovement.Fecha : new DateTime(1900, 1, 1);

			STrace.Trace(GetType().FullName, "Processing real invalid volumes.");

            DaoFactory.VolumenHistoricoDAO.CheckInvalidTankVolumes(tanque,lastDate,null);

			STrace.Trace(GetType().FullName, "Generation teoric volumes.");

            DaoFactory.VolumenHistoricoDAO.GenerateTeoricVolumes(tanque);

			STrace.Trace(GetType().FullName, "Processing conciliations.");

            DaoFactory.VolumenHistoricoDAO.ProcessConciliaciones(tanque);
        }

        #endregion
    }
}
