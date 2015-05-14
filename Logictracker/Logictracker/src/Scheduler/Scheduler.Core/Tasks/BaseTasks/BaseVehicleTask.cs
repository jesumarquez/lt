#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DatabaseTracer.Core;

#endregion

namespace Logictracker.Scheduler.Core.Tasks.BaseTasks
{
    /// <summary>
    /// Base taks for vehicle related actions.
    /// </summary>
    public abstract class BaseVehicleTask : BaseSleepTask
    {
        #region Private Properties

        /// <summary>
        /// Configuration parameters keys.
        /// </summary>
        private const String VehiclesParameter = "Vehiculos";
        private const String InitialId = "IdInicial";

        #endregion

        #region Protected Properties

        protected List<Int32> Vehicles;

        /// <summary>
        /// Auxiliar variable for logging the amount of remaining vehicles to process.
        /// </summary>
        protected Int32 VehiclesToProcess;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs tasks main actions.
        /// </summary>
        /// <param name="timer"></param>
        protected override void OnExecute(Timer timer)
        {
            base.OnExecute(timer);

            GetVehicles();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the ordered list of vehicles ids to be processed.
        /// </summary>
        /// <returns></returns>
        private void GetVehicles()
        {
			STrace.Trace(GetType().FullName, "Retrieving vehicles to process.");

            var initialId = GetInt32(InitialId) ?? 0;

            var vehicles = GetListOfInt(VehiclesParameter) ?? DaoFactory.CocheDAO.FindAllActivos().Select(vehicle => vehicle.Id).ToList();

            Vehicles = vehicles.OrderBy(vehicle => vehicle).Where(vehicle => vehicle >= initialId).ToList();

            VehiclesToProcess = vehicles.Count();

            STrace.Trace(GetType().FullName, String.Format("Vehicles to process: {0}", VehiclesToProcess));
        }

        #endregion
    }
}