using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.Factories;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.ReportObjects;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class WorkedHoursDAO: ReportDAO
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new report generation class with the specified data access object.
        /// </summary>
        /// <param name="daoFactory"></param>
        public WorkedHoursDAO(DAOFactory daoFactory) : base(daoFactory) {}

        #endregion

        #region Public Methods

        /// <summary>
        /// Get mobile times report.
        /// </summary>
        /// <returns></returns>
        public List<ACWorkedHours> GetWorkedHours(int empleado, DateTime desde, DateTime hasta)
        {
            var eventos = GetEvents(empleado, desde, hasta);

            var actualDate = desde;

            var results = new List<ACWorkedHours>();

            while (actualDate <= hasta)
            {
                var eventosDia = eventos.Where(d => d.Fecha >= actualDate && d.Fecha < actualDate.AddDays(1)).OrderBy(e => e.Fecha);

                var ultimoEvento = eventos.Where(d => d.Fecha < actualDate).OrderByDescending(e => e.Fecha).FirstOrDefault();

                var horas = 0.00;

                var entrada = DateTime.MinValue;

                if (ultimoEvento != null && ultimoEvento.Entrada) entrada = actualDate;

                if (eventosDia.Count().Equals(0))
                {
                    //if (entrada != DateTime.MinValue && actualDate.DayOfYear != DateTime.UtcNow.DayOfYear) horas = 24.00;

                    results.Add(new ACWorkedHours(actualDate.ToDisplayDateTime(), horas));
                }
                else
                {
                    foreach (var evento in eventosDia)
                    {
                        if (entrada != DateTime.MinValue && !evento.Entrada) 
                            horas += evento.Fecha.Subtract(entrada).TotalHours;

                        entrada = evento.Entrada ? evento.Fecha : DateTime.MinValue;
                    }

                    //if (entrada != DateTime.MinValue && actualDate.DayOfYear != DateTime.UtcNow.DayOfYear) horas += actualDate.AddDays(1).Subtract(entrada).TotalHours;

                    results.Add(new ACWorkedHours(actualDate.ToDisplayDateTime(), horas));
                }

                actualDate = actualDate.AddDays(1);
            }

            return results.ToList();
        }

        #endregion

        #region Private Methods

        private IEnumerable<EventoAcceso> GetEvents(int empleado, DateTime desde, DateTime hasta)
        {
            var eventos = DAOFactory.EventoAccesoDAO.FindByEmpleadosAndFecha(new List<int> { empleado }, desde, hasta).ToList();

            var lastEvent = DAOFactory.EventoAccesoDAO.FindLastEventForEmployee(empleado, desde);

            if (lastEvent != null) eventos.Add(lastEvent);

            return eventos;
        }

        #endregion
    }
}
