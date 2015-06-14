#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects.Postal;
using Logictracker.Utils;
using NHibernate;
using NHibernate.Linq;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Postal
{
    /// <summary>
    /// Distributors data access class.
    /// </summary>
    public class DistribuidorDAO: GenericDAO<Distribuidor>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public DistribuidorDAO(ISession session) : base(session) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds all clients.
        /// </summary>
        /// <returns></returns>
        public override IQueryable<Distribuidor> FindAll() { return Session.Query<Distribuidor>().Where(distributor => distributor.FechaBaja == null); }

        /// <summary>
        /// Gets the distributor associated to the specified ptm distributor code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Distribuidor FindByPtmCode(String code) { return Session.Query<Distribuidor>().FirstOrDefault(distributor => distributor.Codigo == code); }

        public Distribuidor FindByUsuario(string usuario)
        {
			return Session.Query<Distribuidor>().Where(d => d.Usuario == usuario).SafeFirstOrDefault();
        }

        public Distribuidor FindByCodigo(string codigo)
        {
            return Session.Query<Distribuidor>().Where(d => d.Codigo == codigo).SafeFirstOrDefault();
        }

        /// <summary>
        /// Deletes the specified distributor.
        /// </summary>
        /// <param name="distribuidor"></param>
        public override void Delete(Distribuidor distribuidor)
        {
            if (distribuidor == null) return;

            distribuidor.FechaModificacion = DateTime.UtcNow;
            distribuidor.FechaBaja = DateTime.UtcNow;

            SaveOrUpdate(distribuidor);
        }

        #endregion
    }
}
