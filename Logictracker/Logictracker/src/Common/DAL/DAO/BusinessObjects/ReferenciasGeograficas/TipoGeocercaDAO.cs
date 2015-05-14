#region Usings

using System.Collections;
using NHibernate;
using Urbetrack.Types.BusinessObjects;

#endregion

namespace Urbetrack.DAL.DAO.BusinessObjects
{
    /// <summary>
    /// Geocerca types data access class
    /// </summary>
    public class TipoGeocercaDAO: GenericDAO<TipoGeocerca>
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new data access class using the givenn nhibernate session.
        /// </summary>
        /// <param name="sess"></param>
        public TipoGeocercaDAO(ISession sess) : base(sess) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all geocerca types for the specified location and base.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <returns></returns>
        public IList GetList(int empresa, int linea)
        {
            var hql = "from TipoGeocerca t where 1=1 ";

            if (empresa > 0) hql += " and (t.Empresa is null or t.Empresa.Id = :emp) ";
            if (linea > 0) hql += " and (t.Linea is null or t.Linea.Id = :lin) ";

            var qry = sess.CreateQuery(hql);

            if (empresa > 0) qry.SetParameter("emp", empresa);
            if (linea > 0) qry.SetParameter("lin", linea);

            return qry.List();
        }

        #endregion
    }
}
