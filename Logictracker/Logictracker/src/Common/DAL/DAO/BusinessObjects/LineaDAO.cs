using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.DAL.DAO.BusinessObjects.ReferenciasGeograficas;
using Logictracker.DAL.Factories;
using Logictracker.DAL.NHibernate;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Security;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Messages;
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
    public class LineaDAO: GenericDAO<Linea>
    {
//        public LineaDAO(ISession session) : base(session) { }

        #region Find Methods

        /// <summary>
        /// busca una linea por codigo
        /// </summary>
        /// <param name="empresa">empresa</param>
        /// <param name="codigo">codigo</param>
        /// <returns></returns>
        public Linea FindByCodigo(int empresa, string codigo)
        {
            var result =
                Query.FilterEmpresa(Session, new[] {empresa}, null)
                    .Where(l => !l.Baja)
                    .Where(l => l.DescripcionCorta == codigo)
                    .Cacheable()
                    .SafeFirstOrDefault();
            return result;
        }


        public IList<Linea> FindByCodigosByEmpresa(int[] empresas, string[] codigos)
        {
            var dc =
                DetachedCriteria.For<Linea>()
                    .Add(Restrictions.In(Projections.Property<Linea>(l => l.Empresa), empresas))                    
                    .SetProjection(Projections.Property<Linea>(l => l.Id));

            if (codigos != null && codigos.Length > 0)
            {
                dc.Add(Restrictions.In(Projections.Property<Linea>(l => l.DescripcionCorta), codigos));
            }

            var crit = Session.CreateCriteria<Linea>().Add(Subqueries.PropertyIn("Id", dc));
            return crit.List<Linea>();
        }

        public List<Linea> FindByCodigos(IEnumerable<int> empresa, IEnumerable<string> codigos)
        {
            return Query.FilterEmpresa(Session, empresa, null)
                        .Where(t => codigos.Contains(t.DescripcionCorta))
                        .ToList();
        }

        public Linea FindByNombre(int empresa, string nombre)
        {
            var result =
                Query.FilterEmpresa(Session, new[] {empresa}, null).Where(l => !l.Baja).Where(l => l.Descripcion == nombre).Cacheable().SafeFirstOrDefault();
            return result;
        }

        public IEnumerable<Linea> FindList(IEnumerable<int> empresas)
        {
            return FindList(empresas, null);
        }

        public IEnumerable<Linea> FindList(IEnumerable<int> empresas, Usuario user)
        {
            var result = Query.FilterEmpresa(Session, empresas, user).Where(l => !l.Baja).OrderBy(l => l.Descripcion).Cacheable().ToList();
            return result;
        }

        #endregion

        #region Get Methods

        public IEnumerable<Linea> GetLineasPermitidasPorUsuario(IEnumerable<int> empresas)
        {
            var sessionUser = WebSecurity.AuthenticatedUser;

            var userId = sessionUser != null ? sessionUser.Id : 0;

            var table = Ids2DataTable(empresas);

            var sqlQ = Session.CreateSQLQuery("exec [dbo].[sp_LineaDAO_GetLineasPermitidasPorUsuario] @empresasIds = :empresasIds, @userId = :userId;")
                    .AddEntity(typeof(Linea))
                    .SetStructured("empresasIds", table)
                    .SetInt32("userId", userId);
            var results = sqlQ.List<Linea>();
            return results;
        }

        public IEnumerable<Linea> GetList(IEnumerable<int> empresas)
        {
            var result = Query.FilterEmpresa(Session, empresas).FilterLinea(Session, empresas).Where(l => !l.Baja).OrderBy(l => l.Descripcion).Cacheable().ToList();
            return result;
        }

        #endregion

        #region Override Methods

        protected override void DeleteWithoutTransaction(Linea linea)
        {
            linea.Baja = true;
            SaveOrUpdate(linea);
            if (linea.ReferenciaGeografica != null)
                new ReferenciaGeograficaDAO().DeleteGeoRef(linea.ReferenciaGeografica.Id);            
        }
        #endregion
    }
}