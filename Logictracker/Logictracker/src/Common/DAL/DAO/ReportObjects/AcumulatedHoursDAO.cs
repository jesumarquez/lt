using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class AcumulatedHoursDAO: ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public AcumulatedHoursDAO(DAOFactory daoFactory) : base(daoFactory) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get mobile times report.
        /// </summary>
        /// <returns></returns>
        public List<AcumulatedHours> GetAcumulatedHours(List<int> empleados, DateTime desde, DateTime hasta)
        {
            var eventos = DAOFactory.EventoAccesoDAO.FindByEmpleadosAndFecha(empleados,desde, hasta).ToList();

            var results = new List<AcumulatedHours>();

            foreach (var empleado in empleados)
            {
                var emp = DAOFactory.EmpleadoDAO.FindById(empleado);

                if (emp == null) continue;

                var eventosEmpleado = GetEmployeeEvents(desde, empleado, eventos);

                var horas = 0.00;

                var entrada = DateTime.MinValue;

                foreach (var evento in eventosEmpleado)
                {
                    if (entrada != DateTime.MinValue && !evento.Entrada) 
                        horas = horas + evento.Fecha.Subtract(entrada).TotalHours;

                    entrada = evento.Entrada ? evento.Fecha : DateTime.MinValue;
                }

                results.Add(new AcumulatedHours(horas, empleado, emp.Entidad != null ? emp.Entidad.Descripcion : emp.Legajo));
            }

            return results.ToList();
        }

        #endregion

        #region Private Methods

        private IEnumerable<EventoAcceso> GetEmployeeEvents(DateTime desde, int empleado, List<EventoAcceso> eventos)
        {
            var lastEvent = DAOFactory.EventoAccesoDAO.FindLastEventForEmployee(empleado, desde);

            if (lastEvent != null) eventos.Add(lastEvent);

            return eventos.Where(d => d.Empleado.Id == empleado).OrderBy(e => e.Fecha).ToList();
        }

        #endregion
    }
}
