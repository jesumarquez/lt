#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using Urbetrack.Types.BusinessObjects;

#endregion

namespace Urbetrack.DAL.DAO.BusinessObjects
{
    /// <summary>
    /// Geocercas data access class.
    /// </summary>
    public class GeocercaDAO:GenericDAO<Geocerca>
    {
        #region Constructors

        /// <summary>
        /// Instanciates a new data access class using the givenn nhibernate session.
        /// </summary>
        /// <param name="sess"></param>
        public GeocercaDAO(ISession sess) : base(sess) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns all geocercas that match the specified filter values.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <returns></returns>
        public IList GetByAreaOperacion(int empresa, int linea) { return GetByAreaOperacion(empresa, linea, -1); }

        /// <summary>
        /// Returns all geocercas that match the specified filter values.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="idTipoGeocerca"></param>
        /// <returns></returns>
        public IList GetByAreaOperacion(int empresa, int linea, int idTipoGeocerca)
        {
            return GetByAreaOperacion(empresa, linea, new List<int> {idTipoGeocerca});
        }

        /// <summary>
        /// Returns all geocercas that match the specified filter values.
        /// </summary>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="idsTipoGeocerca"></param>
        /// <returns></returns>
        public IList GetByAreaOperacion(int empresa, int linea, List<int> idsTipoGeocerca)
        {
            var hql = "from Geocerca g where (g.FechaDesde is null or g.FechaDesde <= :now) and (g.FechaHasta is null or g.FechaHasta > :now) ";

            if (empresa > 0) hql += " and (g.Empresa is null or g.Empresa.Id = :emp) ";
            if (linea > 0) hql += " and (g.Linea is null or g.Linea.Id = :lin) ";

            var evalTypes = idsTipoGeocerca != null && (idsTipoGeocerca.Count > 1 || (idsTipoGeocerca.Count.Equals(1) && (idsTipoGeocerca[0] > 0)));

            if (evalTypes) hql += " and g.TipoGeocerca.Id in (:tipos)";

            var qry = sess.CreateQuery(hql);
            qry = qry.SetParameter("now", DateTime.UtcNow);

            if (empresa > 0) qry = qry.SetParameter("emp", empresa);
            if (linea > 0) qry = qry.SetParameter("lin", linea);

            if (evalTypes) qry = qry.SetParameterList("tipos", idsTipoGeocerca);

            return qry.List();
        }
        
        /// <summary>
        /// Deletes the specified geocerca.
        /// </summary>
        /// <param name="geocerca"></param>
        /// <returns></returns>
        public int Delete(Geocerca geocerca) { return geocerca != null ? Delete(geocerca.Id) : 0; }

        /// <summary>
        /// Deletes the geocerca with the specified id.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public override int Delete(int Id)
        {
            var geocerca = FindById(Id);
            geocerca.FechaHasta = DateTime.UtcNow;

            SaveOrUpdate(geocerca);

            return 1;
        }

        #endregion
    }
}
