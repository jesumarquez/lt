#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ReportObjects;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects
{
    /// <summary>
    /// Mobile maintenance data access class.
    /// </summary>
    public class MobileMaintenanceDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileMaintenanceDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets mobile maintenance data for the givenn location, vehicle type and date.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="vehiculos"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <returns></returns>
        public IEnumerable<MobileMaintenance> GetMobilesMaintenanceData(Usuario user, IEnumerable<Coche> vehiculos, DateTime desde, DateTime hasta)
        {
            var result = DAOFactory.DatamartDAO.GetMobileMaintenanceData(desde, hasta, vehiculos);

            foreach (var mobile in result)
            {
                var coche = vehiculos.FirstOrDefault(v => v.Id == mobile.IdVehiculo);

                var empresa = coche.Empresa != null ? coche.Empresa.Id : -1;
                var linea = coche.Linea != null ? coche.Linea.Id : -1;

                var bases = (linea > 0 ? new List<Linea> { coche.Linea } : DAOFactory.LineaDAO.GetList(new[] { empresa }))
                    .Where(l => l.ReferenciaGeografica != null)
                    .Select(l => l.ReferenciaGeografica.Id)
                    .ToList();

                var talleres = DAOFactory.TallerDAO.GetList(new[] {empresa}, new[] {linea})
                    .Where(taller => taller.ReferenciaGeografica != null)
                    .Select(taller => taller.ReferenciaGeografica.Id)
                    .ToList();

                mobile.HsPlanta = bases.Count == 0 ? 0 : DAOFactory.DatamartDAO.GetHoursInGeofence(mobile.IdVehiculo, desde, hasta, bases);
                mobile.HsTaller = talleres.Count == 0 ? 0 : DAOFactory.DatamartDAO.GetHoursInGeofence(mobile.IdVehiculo, desde, hasta, talleres);
            }

            return result.ToList();
        }

        #endregion
    }
}
