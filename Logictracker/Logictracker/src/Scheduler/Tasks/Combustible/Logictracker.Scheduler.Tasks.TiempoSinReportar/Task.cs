#region Usings

using System;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Scheduler.Core.Tasks.BaseTasks;

#endregion

namespace Logictracker.Scheduler.Tasks.TiempoSinReportar
{
    /// <summary>
    /// Generates no repot alarms for tanks and engines.
    /// </summary>
    public class Task : BaseTask
    {
        #region Private Constants

        /// <summary>
        /// Event constant codes.
        /// </summary>
		private const String TankNoReportEventCode = "371";
		private const String EngineNoReportEventCode = "372";

        #endregion

        #region Protected Methods

        /// <summary>
        /// Checks if any tank or engine has exceded its no report refference time.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            CheckTanksNoReportTime();

            CheckEnginesNoReportTime();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks if any engine has exceded its no report refference time.
        /// </summary>
        private void CheckEnginesNoReportTime()
        {
			STrace.Trace(GetType().FullName, "Checking if any engine hash reached its no report warning zone.");

            foreach (var motor in DaoFactory.CaudalimetroDAO.FindAll())
            {
                var lastMovement = DaoFactory.MovimientoDAO.GetLastEngineMovement(motor);

                if (motor.TiempoSinReportar.Equals(0))
                {
					STrace.Trace(GetType().FullName, String.Format("The engine {0} does not checks for no report intervals.", motor.Descripcion));

                    continue;
                }

                if (lastMovement == null || DateTime.Now.Subtract(lastMovement.Fecha).TotalMinutes > motor.TiempoSinReportar)
                {
                    var lastMessage = DaoFactory.CaudalimetroDAO.FindLastMessageByCode(motor.Id, EngineNoReportEventCode);

                    if (lastMessage == null || (lastMovement != null && lastMessage.Fecha < lastMovement.Fecha))
                    {
						STrace.Trace(GetType().FullName, String.Format("The engine {0} hash reached its no report time span ({1} minutes).", motor.Descripcion, motor.TiempoSinReportar));

						var lastReport = lastMovement != null ? String.Concat(lastMovement.Fecha.ToShortDateString(), " ", lastMovement.Fecha.ToShortTimeString())
                            : "Sin fecha de ultimo reporte";

						var text = String.Format(" - Motor: {0} - Ultimo Reporte: {1}", motor.Descripcion, lastReport);

                        FuelMessageSaver.CreateNewEvent(motor.Codigo, null, EngineNoReportEventCode, text, DateTime.Now);

                        continue;
                    }

					STrace.Trace(GetType().FullName, String.Format("The engine {0} hash reached its no report time span ({1} minutes), but a previos warning already exists.", motor.Descripcion, motor.TiempoSinReportar));

                    continue;
                }

				STrace.Trace(GetType().FullName, String.Format("The engine {0} is reporting correctly.", motor.Descripcion));
            }
        }

        /// <summary>
        /// Checks if any tank has exceded its no report refference time.
        /// </summary>
        private void CheckTanksNoReportTime()
        {
			STrace.Trace(GetType().FullName, String.Format("Checking if any tank hash reached its no report warning zone."));

            foreach (var tanque in DaoFactory.TanqueDAO.FindAll())
            {
                var lastLevel = DaoFactory.VolumenHistoricoDAO.GetLastTankRealLevel(tanque,DateTime.MaxValue);

                if (!tanque.TiempoSinReportar.HasValue || tanque.TiempoSinReportar.Value.Equals(0))
                {
					STrace.Trace(GetType().FullName, String.Format("The tank {0} does not checks for no report intervals.", tanque.Descripcion));

                    continue;
                }

                if (lastLevel == null || DateTime.Now.Subtract(lastLevel.Fecha).TotalMinutes > tanque.TiempoSinReportar.Value)
                {
                    var lastMessage = DaoFactory.TanqueDAO.FindLastMessageByCode(tanque.Id, TankNoReportEventCode);

                    if (lastMessage == null || (lastLevel != null && lastMessage.Fecha < lastLevel.Fecha))
                    {
                        STrace.Trace(GetType().FullName, String.Format("The tank {0} hash reached its no report time span ({1} minutes).", tanque.Descripcion, tanque.TiempoSinReportar.Value));

						var lastReport = lastLevel != null ? String.Concat(lastLevel.Fecha.ToShortDateString(), " ", lastLevel.Fecha.ToShortTimeString())
                            : "Sin fecha de ultimo reporte";

						var text = String.Format(" - Tanque: {0} - Ultimo Reporte: {1}", tanque.Descripcion, lastReport);

                        FuelMessageSaver.CreateNewEvent(null, tanque.Codigo, TankNoReportEventCode, text, DateTime.Now);

                        continue;
                    }

                    STrace.Trace(GetType().FullName, String.Format("The tank {0} hash reached its no report time span ({1} minutes), but a previos warning already exists.", tanque.Descripcion, tanque.TiempoSinReportar.Value));

                    continue;
                }

                STrace.Trace(GetType().FullName, String.Format("The tank {0} is reporting correctly.", tanque.Descripcion));
            }
        }

        #endregion
    }
}
