#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Messages;
using NHibernate;
using NHibernate.Mapping;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Messages
{
    /// <summary>
    /// Message type data access class.
    /// </summary>
    public class TipoMensajeDAO: GenericDAO<TipoMensaje>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public TipoMensajeDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deletes the message type associated to the specified id.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override void Delete(TipoMensaje type)
        {
            if (type == null) return;

            type.Baja = true;

            SaveOrUpdate(type);
        }

        /// <summary>
        /// Finds all message types associated to the specified location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IList FindByEmpresaLineaYUsuario(Empresa empresa, Linea linea, Usuario user)
        {
            var lin = linea != null ? linea.Id : -1;
            var emp = empresa != null ? empresa.Id : linea != null ? linea.Empresa.Id : -1;

            return FindByEmpresaLineaYUsuario(emp, lin, user);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds all message types associated to the specified location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private IList FindByEmpresaLineaYUsuario(int empresa, int linea, Usuario user)
        {
            var sql = "from TipoMensaje t where t.Baja = 0";

            if (empresa > 0) sql = string.Concat(sql, " and (t.Empresa is null or t.Empresa.Id = :emp)");

            if (linea > 0) sql = string.Concat(sql, " and (t.Linea is null or t.Linea.Id = :lin)");

            var query = Session.CreateQuery(string.Concat(sql, " order by t.Descripcion"));

            if (empresa > 0) query = query.SetParameter("emp", empresa);

            if (linea > 0) query = query.SetParameter("lin", linea);

            var tipos = new List<TipoMensaje>();

            if (user != null && user.PorTipoMensaje)
            {
                tipos = (from TipoMensaje t in query.List()
                         where user.TiposMensaje.Contains(t)
                         select t).ToList();
            }
            else
            {
                tipos = (from TipoMensaje t in query.List()
                        where user == null
                           || (t.Empresa == null && t.Linea == null)
                           || (t.Empresa != null && t.Linea == null && (user.Empresas.IsEmpty()|| user.Empresas.Contains(t.Empresa)))
                           || (t.Linea != null && (user.Lineas.IsEmpty()|| user.Lineas.Contains(t.Linea)))
                        select t).ToList();
            }
            return tipos;
        }

        #endregion
    }
}