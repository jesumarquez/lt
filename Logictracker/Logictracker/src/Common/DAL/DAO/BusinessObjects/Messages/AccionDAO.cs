#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    /// <summary>
    /// Action data access class.
    /// </summary>
    public class AccionDAO : GenericDAO<Accion>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public AccionDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds all actions.
        /// </summary>
        /// <returns></returns>
        public new IList FindAll() { return Session.CreateCriteria(typeof (Accion)).AddOrder(Order.Asc("Descripcion")).List(); }

        /// <summary>
        /// Finds all the action associated to the givenn location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IList FindByDistritoYBase(Empresa empresa, Linea linea, Usuario user)
        {
            var emp = empresa != null ? empresa.Id : linea != null && linea.Empresa != null ? linea.Empresa.Id : -1;
            var lin = linea != null ? linea.Id : -1;

            var resultados = FindByDistritoYBase(emp, lin);

            var acciones = (from Accion a in resultados
                            where (a.Empresa == null && a.Linea == null && a.Transportista == null)
                               || (a.Empresa != null && a.Linea == null && (user.Empresas.IsEmpty() || user.Empresas.Contains(a.Empresa)))
                               || (a.Linea != null && (user.Lineas.IsEmpty() || user.Lineas.Contains(a.Linea)) && (user.Empresas.IsEmpty() || user.Empresas.Contains(a.Linea.Empresa)))
                            select a);

            if (!user.Transportistas.IsEmpty())
            {
                acciones = acciones.Where(a => a.Transportista == null || user.Transportistas.Contains(a.Transportista));
            }

            return acciones.ToList();
        }

        /// <summary>
        /// Find all actions associated to the specified message.
        /// </summary>
        /// <param name="mensaje"></param>
        /// <returns></returns>
        public IList<Accion> FindByMensaje(Mensaje mensaje)
        {
            DetachedCriteria dc = DetachedCriteria.For<Accion>("da")
                .CreateAlias("Mensaje", "m", JoinType.InnerJoin)
                .Add(Restrictions.Eq("m.Codigo", mensaje.Codigo))
                .Add(Restrictions.Eq("Baja", false))
                .SetProjection(Projections.Property("Id"));

            return Session.CreateCriteria<Accion>("a")
                .Add(Subqueries.PropertyIn("Id", dc)).List<Accion>();
        }

        /// <summary>
        /// Deletes the action associated to the givenn id.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public override void Delete(Accion action)
        {
            if (action == null) return;

            action.Baja = true;

            SaveOrUpdate(action);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Find all actions associated to the givenn location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <returns></returns>
        private IList FindByDistritoYBase(int empresa, int linea)
        {
            var sql = "from Accion a where a.Baja = 0";

            if (empresa > 0) sql = string.Concat(sql, " and (a.Empresa is null or a.Empresa.Id = :emp)");

            if (linea > 0) sql = string.Concat(sql, " and (a.Linea is null or a.Linea.Id = :lin)");

            sql = string.Concat(sql, " order by a.Descripcion");

            var query = Session.CreateQuery(sql);

            if (empresa > 0) query.SetParameter("emp", empresa);

            if (linea > 0) query.SetParameter("lin", linea);

            return query.List();
        }

        #endregion
    }
}