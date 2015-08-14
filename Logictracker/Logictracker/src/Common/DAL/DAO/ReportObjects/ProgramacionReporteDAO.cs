using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate.Linq;
using NHibernate;

namespace Logictracker.DAL.DAO.ReportObjects
{
    public class ProgramacionReporteDAO : GenericDAO<ProgramacionReporte>
    {
//        public ProgramacionReporteDAO(ISession session) : base(session) { }

        #region Get Methods

        public List<ProgramacionReporte> GetList(IEnumerable<int> empresas, IEnumerable<int> lineas)
        {
            return Query.FilterEmpresa(Session, empresas, null)
                        //.FilterLinea(Session, empresas, lineas)
                        .ToList();
        }

        #endregion

        #region Find Methods

        public List<ProgramacionReporte> FindByPeriodicidad(char periodicidad)
        {
            return Session.Query<ProgramacionReporte>().Where(programacion => programacion.Periodicity == periodicidad && programacion.Active).ToList();
        }

        #endregion

        public int GetReportIdByReportName(string reportName)
        {
            var firstOrDefault = Session.Query<ProgramacionReporte>().FirstOrDefault(programacion => programacion.ReportName == reportName);
            return firstOrDefault != null ? firstOrDefault.Id : 0;
        }
    }
}
