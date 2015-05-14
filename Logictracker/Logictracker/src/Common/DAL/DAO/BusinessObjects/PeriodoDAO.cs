using System;
using System.Collections;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using NHibernate;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    public class PeriodoDAO : GenericDAO<Periodo>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public PeriodoDAO(ISession session) : base(session) { }

        #endregion

        public IList GetByEstado(int empresa, params short[] estado)
        {
            var estados = estado.ToList();
            var q = Query.Where(p => estados.Contains(p.Estado));
            if (empresa != -1) q = q.Where(p => p.Empresa == null || p.Empresa.Id == empresa);
            return q.ToList();
        }
        public IList GetByYear(int empresa, int year)
        {
            var q = Query.Where(p => p.FechaDesde > new DateTime(year - 1, 12, 15) &&
                                     p.FechaDesde <= new DateTime(year, 12, 15));
            if (empresa != -1) q = q.Where(p => p.Empresa == null || p.Empresa.Id == empresa);
            return q.ToList();
        }
        public Periodo GetByDate(int empresa, DateTime date)
        {
            var q = Query.Where(p => p.FechaDesde <= date &&
                                     p.FechaHasta > date);
            if (empresa != -1) q = q.Where(p => p.Empresa == null || p.Empresa.Id == empresa);
            return q.FirstOrDefault();
        }
    }
}
