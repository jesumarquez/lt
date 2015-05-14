using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    /// <summary>
    /// Mobile routes data access class.
    /// </summary>
    public class MobileRoutesDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileRoutesDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all mobile routes fragments for the specified day.
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<MobileRoutes> GetMobileRoutes(Int32 mobile, DateTime from, DateTime to)
        {
            var results = DAOFactory.DatamartDAO.GetBetweenDates(mobile, from, to);

            if (results.Count <= 0) return new List<MobileRoutes>();

            return results.Select(data => new MobileRoutes
                                    {
                                        Id = data.Vehicle.Id,
                                        Inter = data.Vehicle.Interno,
                                        VehicleType = data.Vehicle.TipoCoche.Descripcion,
                                        Driver = data.Employee != null ? data.Employee.Entidad.Descripcion : string.Empty,
                                        InitialTime = data.Begin.ToDisplayDateTime(),
                                        FinalTime = data.End.ToDisplayDateTime(),
                                        Kilometers = data.Kilometers,
                                        Duration = data.VehicleStatus.Equals("Detenido") ? data.StoppedHours : data.VehicleStatus.Equals("En Movimiento") ? data.MovementHours : data.NoReportHours,
                                        Infractions = data.Infractions,
                                        InfractionsDuration = data.InfractionMinutes,
                                        MinSpeed = data.MinSpeed,
                                        AverageSpeed = data.AverageSpeed,
                                        MaxSpeed = data.MaxSpeed,
                                        Geocerca = data.GeograficRefference != null ? data.GeograficRefference.Descripcion : string.Empty,
                                        EngineStatus = data.EngineStatus,
                                        VehicleStatus = data.VehicleStatus,
                                        Consumo = data.Consumo,
                                        HsMarcha = data.HorasMarcha
                                    })
                .ToList();
        }

        #endregion
    }
}