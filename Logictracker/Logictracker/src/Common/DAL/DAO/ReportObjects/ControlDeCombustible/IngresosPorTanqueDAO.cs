#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects.ControlDeCombustible;
using Logictracker.Types.ReportObjects.ControlDeCombustible;

#endregion

namespace Logictracker.DAL.DAO.ReportObjects.ControlDeCombustible
{
    public class IngresosPorTanqueDAO : ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public IngresosPorTanqueDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        public IEnumerable FindAllTanquesBetweenDatesByEquipo(int equipo, DateTime startDate, DateTime endDate)
        {
            var listaResultados = new List<IngresosPorEquipo>();

            if (equipo <= 0) return listaResultados;

            var nombreEquipo = DAOFactory.EquipoDAO.FindById(equipo).Descripcion;

            var movements = DAOFactory.MovimientoDAO.FindIngresosByEquipoAndDate(equipo, startDate, endDate);

            listaResultados.AddRange(from idEquipo in (from o in movements select o.Caudalimetro.Equipo.Id).Distinct().ToList()
                                     let results = (from Movimiento m in movements where m.Caudalimetro.Equipo.Id == idEquipo select m).ToList()
                                     select new IngresosPorEquipo
                                                {
                                                    CantIngresos = results.Count(),
                                                    IDEquipo = equipo,
                                                    NombreEquipo = nombreEquipo,
                                                    TotalCargado = results.Sum(mov => mov.Volumen),
                                                    UltiMedicion = DAOFactory.MovimientoDAO.FindUltimaMedicion(idEquipo, startDate, endDate).Fecha,
                                                });

            return listaResultados.OrderBy(mov => mov.NombreEquipo);
        }
    }
    
}
