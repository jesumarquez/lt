#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Auditoria;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Auditoria
{
    public class LoginAuditDAO : GenericDAO<LoginAudit>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public LoginAuditDAO(ISession session) : base(session) { }

        #endregion

        public void CloseUserSession(int userId)
        {
            var lastLogin = Session.Query<LoginAudit>().Where(t => t.Usuario.Id == userId).Select(t=>t).OrderByDescending(t => t.FechaInicio).FirstOrDefault();

            if(lastLogin == null) return;

            lastLogin.FechaFin = DateTime.UtcNow;

            SaveOrUpdate(lastLogin);
        }

        public IEnumerable<LoginAudit> GetAuditHistory(List<int> userIds, DateTime from, DateTime to)
        {
            return Session.Query<LoginAudit>().Where(a => userIds.Contains(a.Usuario.Id) && a.FechaInicio >= from && a.FechaInicio <= to).Select(a => a).ToList();
        }
    }
}
