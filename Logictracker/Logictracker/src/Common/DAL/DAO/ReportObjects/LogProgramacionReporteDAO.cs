using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;

namespace Logictracker.DAL.DAO.ReportObjects
{
    //public LogProgramacionReporteDAO(ISession session) : base(session) { }

    public class LogProgramacionReporteDAO : GenericDAO<LogProgramacionReporte>
    {
        public List<LogProgramacionReporte> GetList(DateTime desde, DateTime hasta)
        {
            return Query.Where(f => f.Inicio >= desde
                                 && f.Fin <= hasta)
                        .ToList();
        }

        public void SaveNewLog(DateTime startDate, DateTime endDate, int rows, bool errors, ProgramacionReporte progReport)
        {
            var log = new LogProgramacionReporte
            {
                Inicio = startDate,
                Fin = endDate,
                Filas = rows,
                Error = errors,
                ProgramacionReporte = progReport                
            };

            SaveOrUpdate(log);
        }
    }
}

