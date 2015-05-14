#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;

#endregion

namespace Logictracker.Scheduler.Tasks.VolumenTeorico
{
    /// <summary>
    /// Task for processing all teoric voume events.
    /// </summary>
    public class Task : BaseTask
    {
        #region Constants

        private const double HighValue = 99999999999;

		private const String CodeTTeoricoNegativo = "369";
		private const String CodeTTeoricoSuperaCapacidad = "370";
		private const String CodeTDifRealVsTeoricoSuperada = "373";

		private const String MsgTTeoricoNegativo = ". Volumen:{0} lt.";
		private const String MsgTTeoricoSuperaCapacidad = ". Capacidad:{0} lt. Volumen:{1} lt.";
		private const String MsgTDifRealVsTeoricoSuperada = ". Real:{0} lt. Teorico:{1} lt.";

        #endregion

        #region Protected Methods

        /// <summary>
        /// Proces tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            var fechas = new List<DateTime>();

            foreach (var tank in DaoFactory.TanqueDAO.FindAll())
            {
				STrace.Trace(GetType().FullName, String.Format("Processing tank: {0}", tank.Descripcion));
                
                var list = DaoFactory.VolumenHistoricoDAO.FindAllTeoricVolumeEvents(tank.Id);

                if (list == null || list.Count.Equals(0))
                {
					STrace.Trace(GetType().FullName, String.Format("No data to process for tank: {0}", tank.Descripcion));

                    continue;
                }
                
                var volumes = from vol in list orderby vol.Fecha ascending select vol;

                var actualVolume = volumes.First().Volumen;

                /*precalcula el volumen porcentual de diferencia que deberá haber entre el real y el teórico para generar evento*/
                var tankVolumeDifferenceAlarm = !tank.PorcentajeRealVsTeorico.HasValue ? HighValue : (double)tank.PorcentajeRealVsTeorico * tank.Capacidad/100;

                /*Cheks the boolean for the first member of the list*/
                var hasBeenBelowPercetageSinceLastEvent = HasBeenBelowPercentageSinceLastEvent(tank, volumes.First(), tankVolumeDifferenceAlarm);

                foreach (var vol in list)
                {
                    ControlsNegativeTeoricVolume(tank, actualVolume, vol);

                    ControlsTeoricVolumeExceededTankCapacity(tank, vol);

                    hasBeenBelowPercetageSinceLastEvent = ControlsTeoricVsRealDifference(tank, vol, tankVolumeDifferenceAlarm, hasBeenBelowPercetageSinceLastEvent);

                    actualVolume = vol.Volumen;
                }

                fechas.Add(volumes.Last().Fecha);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Controla que la dif entre real y teorico no supere el porcentaje configurado de la capacidad.
        /// Devuelve True en caso de que no la haya superado y False en caso de que sí.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="vol"></param>
        /// <param name="tankVolumeDifferenceAlarm"></param>
        /// <param name="hasBeenBelowPercentageSinceLastEvent"></param>
        private bool ControlsTeoricVsRealDifference(Tanque tank, VolumenHistorico vol, double tankVolumeDifferenceAlarm, bool hasBeenBelowPercentageSinceLastEvent)
        {
            if(tankVolumeDifferenceAlarm != 0 && tankVolumeDifferenceAlarm != HighValue) return true;

            var lastRealLevel = DaoFactory.VolumenHistoricoDAO.GetLastTankRealLevel(tank, vol.Fecha);

            if (Math.Abs((vol.Volumen - lastRealLevel.Volumen)) > tankVolumeDifferenceAlarm)
            {
                if (hasBeenBelowPercentageSinceLastEvent)
                {
					FuelMessageSaver.CreateNewEvent(null, tank.Codigo, CodeTDifRealVsTeoricoSuperada, String.Format(MsgTDifRealVsTeoricoSuperada, lastRealLevel.Volumen, vol.Volumen),
                        vol.Fecha);

					STrace.Trace(GetType().FullName, String.Format("Teorical versus real levels warning refference reached for tank: {0} (real: {1} - teorical: {2} - date: {3}).", tank.Descripcion, lastRealLevel.Volumen, vol.Volumen, vol.Fecha));
                }
                else STrace.Trace(GetType().FullName, String.Format(
@"Teorical versus real levels warning refference reached for tank: {0} (real: {1} - teorical: {2} - date: {3})
                    || A previos warning already exists (no event was raised).", tank.Descripcion, lastRealLevel.Volumen, vol.Volumen, vol.Fecha));

                return false;
            }

            return true;
        }

        /// <summary>
        /// Determina si el tanque estuvo debajo del porcentaje configurado desde el ultimo reporte.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="vol"></param>
        /// <param name="volDif"></param>
        /// <returns></returns>
        private bool HasBeenBelowPercentageSinceLastEvent(Tanque tank, VolumenHistorico vol, double volDif)
        {
            var lastMessage = DaoFactory.TanqueDAO.FindLastMessageByCode(tank.Id, CodeTDifRealVsTeoricoSuperada);

            if (lastMessage == null)
            {
				STrace.Error(GetType().FullName, "Last message date was NULL");

                return true;
            }
           
            var lastMovementBelowDifference = DaoFactory.VolumenHistoricoDAO.FindLastTeoricVolumeBelowRealAndTeoricDifference(tank.Id, vol.Fecha,lastMessage.Fecha,volDif);

            return lastMovementBelowDifference != null;
        }

        /// <summary>
        /// Controla que el volumen teorico no supere la capacidad del tanque.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="vol"></param>
        private void ControlsTeoricVolumeExceededTankCapacity(Tanque tank, VolumenHistorico vol)
        {
            if (vol.Volumen <= tank.Capacidad) return;

            if(HasBeenUnderCapacitySinceLastEvent(tank,vol))
            {
				FuelMessageSaver.CreateNewEvent(null, tank.Codigo, CodeTTeoricoSuperaCapacidad, String.Format(MsgTTeoricoSuperaCapacidad, tank.Capacidad, vol.Volumen), vol.Fecha);

                STrace.Trace(GetType().FullName, String.Format("Teorical max volume exceded for tank: {0} (volume: {1} - capacity: {2}).", tank.Descripcion, vol.Volumen, tank.Capacidad));
            }
            else STrace.Trace(GetType().FullName, String.Format("Teorical max volume exceded for tank: {0} (volume: {1} - capacity: {2}) || A previos warning already exists (no event was raised)", tank.Descripcion, vol.Volumen, tank.Capacidad));
        }

        /// <summary>
        /// Cheks if the tank teoric level has gone under its capacity since last genereted "TEORICO_SUPERA_CAPACIDAD" event.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="vol"></param>
        /// <returns></returns>
        private bool HasBeenUnderCapacitySinceLastEvent(Tanque tank, VolumenHistorico vol)
        {
            var lastMessageDate = DaoFactory.TanqueDAO.FindLastMessageByCode(tank.Id,CodeTTeoricoSuperaCapacidad);
            var lastUnderCapacityDate = DaoFactory.VolumenHistoricoDAO.FindLastTeoricVolumeUnderCapacity(tank.Id,vol.Fecha);

            return (lastMessageDate == null || lastUnderCapacityDate == null || (lastUnderCapacityDate.Fecha > lastMessageDate.Fecha)); 
        }

        /// <summary>
        /// Controla que el volumen teorico no pase de positivo a negativo.
        /// </summary>
        /// <param name="tank"></param>
        /// <param name="actualVolume"></param>
        /// <param name="vol"></param>
        private void ControlsNegativeTeoricVolume(Tanque tank, double actualVolume, VolumenHistorico vol)
        {
            if (actualVolume < 0 || vol.Volumen >= 0) return;

			FuelMessageSaver.CreateNewEvent(null, tank.Codigo, CodeTTeoricoNegativo, String.Format(MsgTTeoricoNegativo, vol.Volumen), vol.Fecha);

            STrace.Trace(GetType().FullName, String.Format("Negative volume detected for tank: {0} (volume: {1})", tank.Descripcion, vol.Volumen));
        }

        #endregion
    }
}
