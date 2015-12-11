using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Utils;
using Logictracker.Utils.NHibernate;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Logictracker.DAL.DAO.BusinessObjects
{
    /// <remarks>
    /// Los métodos que empiezan con:
    ///    Find: No tienen en cuenta el usuario logueado
    ///    Get: Filtran por el usuario logueado ademas de los parametro
    /// </remarks>
    public class EmpresaDAO : GenericDAO<Empresa>
    {
//        public EmpresaDAO(ISession session) : base(session) { }

        #region Find Methods

        /// <summary>
        /// Gets the company with the given code.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public Empresa FindByCodigo(string codigo)
        {
            var dc =
                DetachedCriteria.For<Empresa>()
                    .Add(Restrictions.Eq("Baja", false))
                    .Add(Restrictions.Eq("Codigo", codigo))
                    .SetProjection(Projections.Property("Id"));

            var crit = Session.CreateCriteria<Empresa>().Add(Subqueries.PropertyIn("Id", dc)).SetMaxResults(1).SetCacheable(true);
            var result = crit.UniqueResult<Empresa>();
            return result;
        }

        public IEnumerable<Empresa> FindList()
        {
            var dc = DetachedCriteria.For<Empresa>().Add(Restrictions.Eq("Baja", false)).SetProjection(Projections.Property("Id"));

            var crit = Session.CreateCriteria<Empresa>().Add(Subqueries.In("Id", dc)).SetCacheable(true);
            var result = crit.List<Empresa>();
            return result;
        }

        #endregion

        #region Get Methods

        public IEnumerable<Empresa> GetList()
        {

            var result = Query.FilterEmpresa(Session).Where(empresa => !empresa.Baja).Cacheable().ToList();
            return result;
        }

        public Empresa GetById(int id)
        {
            return Query.FilterEmpresa(Session)
                .Where(empresa => !empresa.Baja)
                .Where(e => e.Id.Equals(id))
                .Cacheable()
                .SafeFirstOrDefault();           
        }
        
        public IEnumerable<Empresa> GetEmpresasPermitidas()
        {
            var sessionUser = WebSecurity.AuthenticatedUser;

            var userId = sessionUser != null ? sessionUser.Id : 0;

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_EmpresaDAO_GetEmpresasPermitidasPorUsuario] @userId = :userId;")
                    .AddEntity(typeof(Empresa))
                    .SetInt32("userId", userId);
            var results = sqlQ.List<Empresa>();
            return results;
        }
        
        #endregion

        #region Override Methods

        protected override void DeleteWithoutTransaction(Empresa empresa)
        {
            if (empresa == null) return;
            empresa.Baja = true;
            SaveOrUpdate(empresa);
        }

        #endregion
    }
}