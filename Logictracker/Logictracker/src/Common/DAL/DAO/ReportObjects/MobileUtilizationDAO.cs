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
    public class MobileUtilizationDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public MobileUtilizationDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets mobile utilization report objects.
        /// </summary>
        /// <param name="centrosCosto"></param>
        /// <param name="desde"></param>
        /// <param name="hasta"></param>
        /// <param name="empresas"></param>
        /// <param name="lineas"></param>
        /// <param name="tiposVehiculo"></param>
        /// <param name="soloImproductivos"></param>
        /// <param name="gmtModifier"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IEnumerable<MobileUtilization> GetMobileUtilizations(List<int> empresas, List<int> lineas, List<int> tiposVehiculo, List<int> centrosCosto, DateTime desde, DateTime hasta,
            bool soloImproductivos, double gmtModifier, Usuario user)
        {
            var cocheDao = DAOFactory.CocheDAO;
            var datamartDao = DAOFactory.DatamartDAO;

            var coches = cocheDao.GetList(empresas, lineas, tiposVehiculo, new[] {-1}, centrosCosto)
                .Where(c => c.ControlaHs || c.ControlaKm)
                .Where(c => c.Estado < Coche.Estados.Inactivo || c.DtCambioEstado > desde)
                .ToList();
                
            var datamarts = coches.Select(d => new MobileUtilization
                                                   {
                                                       IdVehiculo = d.Id,
                                                       HsTurnoReales = datamartDao.HoursWithinShift(d.Id, desde, hasta),
                                                       HsRealesFueraTurno = datamartDao.HoursWithoutShift(d.Id, desde, hasta)
                                                   })
                                  .ToList();

            var resultados = new List<MobileUtilization>();

            if (datamarts.Count.Equals(0)) return resultados;

            var shiftDao = DAOFactory.ShiftDAO;

            var diferencia = hasta.Subtract(desde).TotalHours;
            
            foreach (var result in datamarts)
            {
                var coche = cocheDao.FindById(result.IdVehiculo);

                result.Centro = coche.CentroDeCostos != null ? coche.CentroDeCostos.Descripcion : "Sin Centro de Costos";

                result.IdCentro = coche.CentroDeCostos != null ? coche.CentroDeCostos.Id : 0;

                result.Interno = coche.Interno;

                result.PorcentajeEsperado = coche.PorcentajeProductividad;

                result.HsTurno = shiftDao.TimeByVehicleAndDate(coche, desde, hasta, gmtModifier).TotalHours;

                result.PorcentajeTurno = result.HsTurno > 0 ? result.HsTurnoReales / result.HsTurno * 100 : 0;

                result.PorcentajeProd = result.PorcentajeTurno - result.PorcentajeEsperado;

                result.HsFueraTurno = diferencia - result.HsTurno;

                result.HsEsperadas = result.HsTurno * result.PorcentajeEsperado / 100;

                result.PorcentajeFueraTurno = result.HsFueraTurno > 0 ? result.HsRealesFueraTurno / result.HsFueraTurno * 100 : 0;

                result.PorcentajeTotal = diferencia > 0 ? (result.HsTurnoReales + result.HsRealesFueraTurno) / diferencia * 100 : 0;

                resultados.Add(result);
            }

            if (!soloImproductivos) return resultados.OrderBy(res => res.Centro).ThenBy(res => res.Interno).ToList();

            return resultados.Where(res => res.PorcentajeEsperado > res.PorcentajeTurno).OrderBy(res => res.Centro).ThenBy(res => res.Interno).ToList();
        }

        #endregion
    }
}
